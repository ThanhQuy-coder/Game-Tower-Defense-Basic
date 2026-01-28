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
	[Export] public float InitialWaitTime = 10.0f;

	[ExportGroup("Enemy Types")]
	[Export] public PackedScene[] EnemyPrefabs; // Mảng dữ liệu quái vật

	// Thuộc tính
	private int _currentEntryIndex = 0;
	private int _spawnedInEntry = 0;
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

		// Khởi tạo các Component
		_wavesConfig = LevelFactory.GetLevelConfig(CurrentLevel, Instance);

		_spawner = new PathSpawner(EnemyPaths, EnemyPrefabs);

		// Reset lại chỉ số Global (Vàng, Máu) khi bắt đầu màn chơi mới.
		if (Global.Instance != null)
		{
			Global.Instance.StartNewGame();
		}

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

		var waveData = _wavesConfig[_state.CurrentWaveIndex];

		_currentEntryIndex = 0; // Reset về nhóm quái đầu tiên của wave mới
		_spawnedInEntry = 0;    // Reset số lượng đã spawn của nhóm đó

		// Lấy dữ liệu từ factory cung cấp
		_state.EnemiesToSpawnLeft = waveData.GetTotalCount();

		_state.TotalSpawnedInCurrentWave = 0; // reset về 0 vì wave mới
		_state.IsWaveActive = true; // wave hiện tại đang chạy

		UpdateExternalSystems(); // Gọi hệ thống bên ngoài Manager

		// Cài thời gian chờ
		_spawnTimer.WaitTime = waveData.SpawnInterval;
		_spawnTimer.Start();
	}

	/// <summary>
	/// Logic sinh quái đã được tổng quát hóa, không còn check CurrentLevel.
	/// </summary>
	private void OnSpawnTimerTimeout()
	{
		if (_state.EnemiesToSpawnLeft <= 0)
		{
			_spawnTimer.Stop();
			return;
		}

		var waveData = _wavesConfig[_state.CurrentWaveIndex];

		// Bảo vệ: Nếu chẳng may danh sách rỗng
		if (waveData.SpawnEntries == null || waveData.SpawnEntries.Count == 0) return;

		var currentEntry = waveData.SpawnEntries[_currentEntryIndex];

		// Lấy ID quái từ nhóm hiện tại
		int enemyIndexToSpawn = currentEntry.EnemyTypeIndex;

		// Thực hiện sinh quái
		_spawner.Execute(enemyIndexToSpawn, _state.TotalSpawnedInCurrentWave);

		_state.EnemiesToSpawnLeft--;
		_state.TotalSpawnedInCurrentWave++;
		_spawnedInEntry++;

		// Nếu đã spawn đủ số lượng của nhóm hiện tại, chuyển sang nhóm kế tiếp
		if (_spawnedInEntry >= currentEntry.Count && _currentEntryIndex < waveData.SpawnEntries.Count - 1)
		{
			_currentEntryIndex++;
			_spawnedInEntry = 0;
		}
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
		if (Global.Instance != null)
			Global.Instance.Wave = _state.CurrentWaveIndex + 1;

		// Hiển thị thông báo wave mới
		GameUI.Instance?.ShowWaveNotification(_state.CurrentWaveIndex + 1);
	}

	/// <summary>
	/// Kết thúc màn chơi
	/// </summary>
	private void FinishGame()
	{
		_state.IsGameFinished = true;
		if (Global.Instance?.Health > 0)
		{
			Global.Instance.TriggerVictory();
			Global.Instance.UnlockLevel(CurrentLevel + 1);
		}
	}
}
