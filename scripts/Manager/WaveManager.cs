using Godot;
using System.Collections.Generic;

/// <summary>
/// Quản lý wave - [ĐÃ CHỈNH SỬA: Logic Boss Level 3 & 4 + Override Số lượng quái]
/// </summary>
public partial class WaveManager : Node
{
	public static WaveManager Instance { get; private set; }

	// Các thiết lập cấu hình cho level và hệ thống wave
	[ExportGroup("Level Settings")]
	[Export] public int CurrentLevel = 1; // Level hiện tại

	[ExportGroup("Setup")]
	[Export] public Path2D[] EnemyPaths; // Các đường đi của kẻ thù
	[Export] public float TimeBetweenWaves = 5.0f; // Thời gian giữa các wave
	
	// [ĐÃ SỬA] Tăng thời gian chờ ban đầu từ 2s lên 10s để người chơi kịp chuẩn bị
	[Export] public float InitialWaitTime = 10.0f; 

	[ExportGroup("Enemy Types")]
	// [LƯU Ý QUAN TRỌNG] Hãy sắp xếp mảng này theo độ khó: [0] Yếu nhất -> [Cuối] Boss/Mạnh nhất
	[Export] public PackedScene[] EnemyPrefabs; 

	// Thuộc tính
	private List<WaveData> _wavesConfig; // cấu hình waves
	private PathSpawner _spawner; // sinh quái
	private WaveState _state = new WaveState(); // trạng thái wave
	private Timer _spawnTimer; // Thời gian sinh
	public float TimeToNextWave { get; private set; } = 0f; // Thời gian tới wave tiếp
	private bool _isCountingDown = false; // là đếm ngược 

	/// <summary>
	/// Khởi tạo Component, cài đặt thời gian, bắt đầu đếm ngược
	/// </summary>
	public override void _Ready()
	{
		Instance = this;

		// [ĐÃ SỬA] Reset lại chỉ số Global (Vàng, Máu) khi bắt đầu màn chơi mới.
		if (Global.Instance != null)
		{
			Global.Instance.StartNewGame();
		}

		// Khởi tạo các Component
		_wavesConfig = LevelFactory.GetLevelConfig(CurrentLevel);

		// [MỚI] Xử lý cứng cho Level 3: Chỉ lấy 4 wave đầu tiên nếu config dư thừa (để loại bỏ wave 5 thừa)
		if (CurrentLevel == 3 && _wavesConfig.Count > 4)
		{
			_wavesConfig = _wavesConfig.GetRange(0, 4);
			GD.Print("WaveManager: Đã giới hạn Level 3 xuống còn 4 Wave.");
		}

		_spawner = new PathSpawner(EnemyPaths, EnemyPrefabs);

		SetupTimer();
		StartCountdown(InitialWaitTime);
	}

	/// <summary>
	/// Cài thời gian cho việc sinh quái
	/// </summary>
	private void SetupTimer()
	{
		_spawnTimer = new Timer();
		_spawnTimer.OneShot = false; // Timer lặp lại mỗi lần hết thời gian
		_spawnTimer.Timeout += OnSpawnTimerTimeout; // Thực hiện khi hết thời gian
		AddChild(_spawnTimer); // Thêm Timer vào node hiện tại để nó bắt đầu hoạt động
	}

	/// <summary>
	/// Gọi mỗi frame
	/// </summary>
	/// <param name="delta">Thời gian trôi qua giữa hai frame</param>
	public override void _Process(double delta)
	{
		if (_state.IsGameFinished) return;

		HandleCountdown((float)delta); // Cập nhật thời gian đếm được của từng frame

		CheckWaveCompletion();
	}

	/// <summary>
	/// Đếm ngược thời gian. Mục đích để 'bắt đầu wave mới'
	/// </summary>
	/// <param name="delta">Thời gian trôi qua giữa hai frame</param>
	private void HandleCountdown(float delta)
	{
		if (!_isCountingDown) return;

		TimeToNextWave -= delta;
		if (TimeToNextWave <= 0)
		{
			_isCountingDown = false;
			StartNextWave();
		}
	}

	/// <summary>
	/// Bắt đầu wave mới
	/// </summary>
	private void StartNextWave()
	{
		if (_state.CurrentWaveIndex >= _wavesConfig.Count) return;

		/// <summary>
		/// Dữ liệu wave
		/// </summary>
		var waveData = _wavesConfig[_state.CurrentWaveIndex];
		
		// [LOGIC MỚI] Xác định số lượng quái cần spawn
		int countToSpawn = waveData.Count;

		// [FIX LEVEL 4] Wave cuối: Cần đảm bảo đủ số lượng để sinh quái thường + 2 Boss
		// Nếu config mặc định quá ít (ví dụ 1), nó sẽ chỉ ra 1 Boss. Ta override lên 15.
		bool isLastWave = _state.CurrentWaveIndex == _wavesConfig.Count - 1;
		if (CurrentLevel == 4 && isLastWave)
		{
			// Nếu số lượng cấu hình nhỏ hơn 10, tăng lên 15 để đảm bảo có "Tất cả quái thường" rồi mới tới Boss
			if (countToSpawn < 10) 
			{
				countToSpawn = 15;
				GD.Print($"WaveManager: Đã tăng số lượng quái Wave cuối Level 4 lên {countToSpawn} để đảm bảo logic Boss.");
			}
		}

		// Cập nhật trạng thái cho wave mới với số lượng đã tính toán
		_state.EnemiesToSpawnLeft = countToSpawn; 
		_state.TotalSpawnedInCurrentWave = 0; // reset về 0 vì wave mới
		_state.IsWaveActive = true; // wave hiện tại đang chạy

		UpdateExternalSystems(); // Gọi hệ thống bên ngoài Manager

		// Cài thời gian chờ
		_spawnTimer.WaitTime = waveData.SpawnInterval;

		_spawnTimer.Start();
	}

	/// <summary>
	/// Sinh quái khi hết giờ - [ĐÃ SỬA: Logic Boss]
	/// </summary>
	private void OnSpawnTimerTimeout()
	{
		if (_state.EnemiesToSpawnLeft <= 0)
		{
			_spawnTimer.Stop();
			return;
		}

		int enemyIndexToSpawn = 0;
		int bossIndex = EnemyPrefabs.Length - 1; // Mặc định Boss là con cuối cùng trong mảng
		bool isLastWave = _state.CurrentWaveIndex == _wavesConfig.Count - 1;

		// --- XỬ LÝ LOGIC BOSS ---
		
		// [ĐÃ CHỈNH SỬA] LEVEL 3 - Wave cuối (Wave 4): Chỉ duy nhất 1 con Boss Mahoraga
		if (CurrentLevel == 3 && isLastWave)
		{
			enemyIndexToSpawn = bossIndex;
			
			// [FIX LỖI] Dù config wave có set số lượng bao nhiêu, ta ép buộc hệ thống 
			// chỉ spawn đúng 1 con rồi dừng lại ngay.
			_state.EnemiesToSpawnLeft = 1;
		}
		// [ĐÃ CHỈNH SỬA] LEVEL 4 - Wave cuối: Quái thường ra trước, 2 Boss ra sau cùng
		else if (CurrentLevel == 4 && isLastWave)
		{
			// Nếu chỉ còn lại 1 hoặc 2 lượt spawn cuối cùng -> Ra Boss (Tổng cộng 2 con)
			// Việc spawn liên tiếp này thường sẽ khiến Spawner phân phối vào 2 đường khác nhau (nếu cơ chế Spawner xoay vòng Path)
			if (_state.EnemiesToSpawnLeft <= 2)
			{
				enemyIndexToSpawn = bossIndex;
			}
			else
			{
				// Các lượt đầu ra tất cả loại quái thường (Random từ đầu đến sát Boss)
				if (bossIndex > 0)
				{
					enemyIndexToSpawn = GD.RandRange(0, bossIndex - 1);
				}
				else
				{
					enemyIndexToSpawn = 0; // Fallback an toàn
				}
			}
		}
		// CÁC TRƯỜNG HỢP KHÁC (Logic cũ)
		else
		{
			int maxAllowed = 0;

			if (CurrentLevel == 1) maxAllowed = 0; // Chỉ quái yếu
			else if (CurrentLevel == 2) maxAllowed = 1; // Quái tb
			else if (CurrentLevel == 3) maxAllowed = Mathf.Min(2, bossIndex - 1); // Lv3 các wave đầu
			else maxAllowed = bossIndex - 1; // Lv4 các wave đầu (trừ Boss)

			// An toàn mảng
			if (maxAllowed >= EnemyPrefabs.Length) maxAllowed = EnemyPrefabs.Length - 1;
			
			enemyIndexToSpawn = GD.RandRange(0, maxAllowed);
		}

		// Spawn
		_spawner.Execute(enemyIndexToSpawn, _state.TotalSpawnedInCurrentWave);

		_state.EnemiesToSpawnLeft--;
		_state.TotalSpawnedInCurrentWave++;
	}

	/// <summary>
	/// Kiểm tra wave đã hoàn thành chưa.
	/// </summary>
	private void CheckWaveCompletion()
	{
		// Điều kiện: Timer đã dừng (hết quái để spawn) và không còn Node quái nào trong group
		if (_spawnTimer.IsStopped() && _state.IsWaveActive)
		{
			if (GetTree().GetNodesInGroup("enemy").Count == 0)
			{
				TransitionToNextWave();
			}
		}
	}

	/// <summary>
	/// Chuyển sang wave kế tiếp
	/// </summary>
	private void TransitionToNextWave()
	{
		_state.IsWaveActive = false;
		_state.CurrentWaveIndex++;

		if (_state.CurrentWaveIndex < _wavesConfig.Count)
			StartCountdown(TimeBetweenWaves);
		else
			FinishGame();
	}

	/// <summary>
	/// Bắt đầu đếm ngược cho màn mới
	/// </summary>
	/// <param name="time">thời gian giữa hai wave</param>
	private void StartCountdown(float time)
	{
		TimeToNextWave = time;
		_isCountingDown = true;
	}

	/// <summary>
	/// Gọi các hệ thống bên ngoài Manager
	/// </summary>
	private void UpdateExternalSystems()
	{
		// Tăng wave lên 1
		Global.Instance.Wave = _state.CurrentWaveIndex + 1;

		// Hiển thị thông báo wave mới
		GameUI.Instance?.ShowWaveNotification(_state.CurrentWaveIndex + 1);
	}

	/// <summary>
	/// Kết thúc màn chơi - [ĐÃ SỬA: Mở khóa Level]
	/// </summary>
	private void FinishGame()
	{
		_state.IsGameFinished = true;
		if (Global.Instance?.Health > 0) 
		{
			Global.Instance.TriggerVictory();
			
			// [MỚI] Mở khóa level tiếp theo trong Global
			if (Global.Instance != null)
			{
				Global.Instance.UnlockLevel(CurrentLevel + 1);
			}
		}
	}
}
