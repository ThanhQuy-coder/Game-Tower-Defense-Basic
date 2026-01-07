using Godot;
using System.Collections.Generic;

// [WAVE MANAGER - MULTI PATH & LEVEL CONFIG]
public partial class WaveManager : Node
{
	public static WaveManager Instance { get; private set; }

	[ExportGroup("Level Settings")]
	[Export] public int CurrentLevel = 1; // 1: Dễ, 4: Có Boss
	
	[ExportGroup("Setup")]
	// [THAY ĐỔI] Dùng mảng để chứa nhiều đường đi
	[Export] public Path2D[] EnemyPaths; 
	
	[Export] public float TimeBetweenWaves = 5.0f;
	[Export] public float InitialWaitTime = 10.0f;

	[ExportGroup("Enemy Types")]
	// Thứ tự: 0: Fast, 1: Tank, 2: Soldier, 3: Boss
	[Export] public PackedScene[] EnemyPrefabs; 

	private class WaveData
	{
		public int EnemyTypeIndex;
		public int Count;
		public float SpawnInterval;
	}

	private List<WaveData> _wavesConfig = new List<WaveData>();
	private Timer _spawnTimer;
	
	private int _currentWaveIndex = 0;
	private int _enemiesToSpawnLeft = 0;
	private int _currentEnemyType = 0;
	private int _enemiesSpawnedInWave = 0; // Biến đếm để chia đường
	
	private bool _isSpawning = false;
	private bool _isWaveActive = false;
	private bool _isGameFinished = false;

	public float TimeToNextWave { get; private set; } = 0f;
	private bool _isCountingDown = false;

	public override void _Ready()
	{
		Instance = this;
		ConfigureWaves(); // Nạp cấu hình theo Level

		_spawnTimer = new Timer();
		_spawnTimer.OneShot = false;
		_spawnTimer.Timeout += SpawnEnemy;
		AddChild(_spawnTimer);

		StartCountdown(InitialWaitTime);
	}

	// --- CẤU HÌNH ĐỘ KHÓ ---
	private void ConfigureWaves()
	{
		_wavesConfig.Clear();

		switch (CurrentLevel)
		{
			case 1: // LEVEL 1: 3 Wave cơ bản, KHÔNG BOSS
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 0, Count = 5, SpawnInterval = 1.5f });  // 5 Fast
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 2, Count = 6, SpawnInterval = 1.2f });  // 6 Soldier
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 1, Count = 4, SpawnInterval = 2.0f });  // 4 Tank
				break;

			case 2: // LEVEL 2: 4 Wave, Đông hơn, 2 đường đi
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 0, Count = 10, SpawnInterval = 1.0f });
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 2, Count = 10, SpawnInterval = 1.0f });
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 1, Count = 5, SpawnInterval = 1.8f });
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 0, Count = 15, SpawnInterval = 0.8f });
				break;

			case 3: // LEVEL 3: 5 Wave, Quái trâu bò
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 2, Count = 15, SpawnInterval = 0.8f });
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 0, Count = 20, SpawnInterval = 0.5f });
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 1, Count = 8, SpawnInterval = 1.5f });
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 2, Count = 20, SpawnInterval = 0.8f });
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 1, Count = 10, SpawnInterval = 1.2f });
				break;

			case 4: // LEVEL 4: FINAL - CÓ BOSS
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 0, Count = 20, SpawnInterval = 0.5f });
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 2, Count = 20, SpawnInterval = 0.8f });
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 1, Count = 15, SpawnInterval = 1.2f });
				// WAVE CUỐI: BOSS MAHORAGA (Index 3)
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 3, Count = 1, SpawnInterval = 1.0f });
				break;

			default:
				_wavesConfig.Add(new WaveData { EnemyTypeIndex = 0, Count = 5, SpawnInterval = 1.0f });
				break;
		}
		
		GD.Print($"[WaveManager] Level {CurrentLevel} Loaded: {_wavesConfig.Count} Waves.");
	}

	public override void _Process(double delta)
	{
		if (_isCountingDown)
		{
			TimeToNextWave -= (float)delta;
			if (TimeToNextWave <= 0)
			{
				_isCountingDown = false;
				TimeToNextWave = 0;
				StartNextWave();
			}
		}

		if (!_isSpawning && _isWaveActive && !_isGameFinished)
		{
			if (GetTree().GetNodesInGroup("enemy").Count == 0)
			{
				OnWaveCleared();
			}
		}
	}

	private void StartCountdown(float time)
	{
		TimeToNextWave = time;
		_isCountingDown = true;
	}

	private void StartNextWave()
	{
		if (_currentWaveIndex >= _wavesConfig.Count) return;

		var waveData = _wavesConfig[_currentWaveIndex];
		_enemiesToSpawnLeft = waveData.Count;
		_currentEnemyType = waveData.EnemyTypeIndex;
		_enemiesSpawnedInWave = 0;
		
		if (Global.Instance != null) Global.Instance.Wave = _currentWaveIndex + 1;
		if (GameUI.Instance != null) GameUI.Instance.ShowWaveNotification(_currentWaveIndex + 1);

		_isSpawning = true;
		_isWaveActive = true;
		_spawnTimer.WaitTime = waveData.SpawnInterval;
		_spawnTimer.Start();
	}

	private void SpawnEnemy()
	{
		if (_enemiesToSpawnLeft <= 0)
		{
			_isSpawning = false; 
			_spawnTimer.Stop();
			return;
		}

		// [LOGIC MỚI] Kiểm tra mảng đường đi
		if (EnemyPaths != null && EnemyPaths.Length > 0 && _currentEnemyType < EnemyPrefabs.Length)
		{
			// Chia đều quái: Con 1 đi đường 1, Con 2 đi đường 2...
			int pathIndex = _enemiesSpawnedInWave % EnemyPaths.Length;
			Path2D selectedPath = EnemyPaths[pathIndex];

			if (selectedPath != null)
			{
				var pathFollow = new PathFollow2D { Loop = false, Rotates = false };
				selectedPath.AddChild(pathFollow);
				
				var enemy = EnemyPrefabs[_currentEnemyType].Instantiate<Node2D>();
				pathFollow.AddChild(enemy);
				
				_enemiesToSpawnLeft--;
				_enemiesSpawnedInWave++;
			}
		}
	}

	private void OnWaveCleared()
	{
		_isWaveActive = false;
		_currentWaveIndex++;

		if (_currentWaveIndex < _wavesConfig.Count)
		{
			StartCountdown(TimeBetweenWaves);
		}
		else
		{
			_isGameFinished = true;
			if (Global.Instance.Health > 0) Global.Instance.TriggerVictory();
		}
	}
}
