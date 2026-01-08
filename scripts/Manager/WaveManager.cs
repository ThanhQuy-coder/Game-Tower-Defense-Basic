using Godot;
using System.Collections.Generic;

/// <summary>
/// Quản lý wave
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
	[Export] public float InitialWaitTime = 2.0f; // Thời gian chờ ban đầu

	[ExportGroup("Enemy Types")]
	[Export] public PackedScene[] EnemyPrefabs; // Kẻ thù đã được tạo sẵn

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

		// Khởi tạo các Component
		_wavesConfig = LevelFactory.GetLevelConfig(CurrentLevel);
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

		// Cập nhật trạng thái cho wave mới
		_state.EnemiesToSpawnLeft = waveData.Count; // Số lượng kẻ thù cần spawn trong wave này
		_state.TotalSpawnedInCurrentWave = 0; // reset về 0 vì wave mới
		_state.IsWaveActive = true; // wave hiện tại đang chạy

		UpdateExternalSystems(); // Gọi hệ thống bên ngoài Manager

		// Cài thời gian chờ
		_spawnTimer.WaitTime = waveData.SpawnInterval;

		_spawnTimer.Start();
	}

	/// <summary>
	/// Sinh quái khi hết giờ
	/// </summary>
	private void OnSpawnTimerTimeout()
	{
		if (_state.EnemiesToSpawnLeft <= 0)
		{
			_spawnTimer.Stop();
			return;
		}

		// Ra lệnh cho Spawner thực hiện việc tạo quái
		var currentWaveData = _wavesConfig[_state.CurrentWaveIndex];

		// ? DEBUG: kiểm tra loại quái
		// GD.Print("EnemyTypeIndex: ", currentWaveData.EnemyTypeIndex);
		// ?

		_spawner.Execute(currentWaveData.EnemyTypeIndex, _state.TotalSpawnedInCurrentWave);

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
	/// Kết thúc màn chơi
	/// </summary>
	private void FinishGame()
	{
		_state.IsGameFinished = true;
		if (Global.Instance?.Health > 0) Global.Instance.TriggerVictory();
	}
}
