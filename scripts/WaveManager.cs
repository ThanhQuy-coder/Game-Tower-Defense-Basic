using Godot;
using System;

// [Người 2/Quản lý] - Logic sinh quái theo đợt
// Gắn script này vào một Node mới tên "WaveManager" trong Level 1
public partial class WaveManager : Node
{
    [Export]
    public PackedScene EnemyScene; // Kéo file Enemy.tscn vào đây

    [Export]
    public Path2D EnemyPath; // Kéo Node Path2D (đường đi) vào đây

    [Export]
    public float SpawnInterval = 2.0f; // Thời gian giữa các con quái (giây)

    [Export]
    public int EnemiesPerWave = 5; // Số quái mỗi đợt

    private Timer _spawnTimer;
    private int _enemiesSpawnedCount = 0;

    public override void _Ready()
    {
        // Tạo bộ đếm thời gian
        _spawnTimer = new Timer();
        _spawnTimer.WaitTime = SpawnInterval;
        _spawnTimer.OneShot = false;
        _spawnTimer.Timeout += OnSpawnTimerTimeout;
        AddChild(_spawnTimer);

        // Bắt đầu đợt 1 sau 1 giây
        GetTree().CreateTimer(1.0f).Timeout += StartNextWave;
    }

    public void StartNextWave()
    {
        if (Global.Instance != null)
        {
            Global.Instance.Wave++;
            GD.Print("Bắt đầu Wave: " + Global.Instance.Wave);
        }
        
        _enemiesSpawnedCount = 0;
        _spawnTimer.Start();
    }

    private void OnSpawnTimerTimeout()
    {
        SpawnEnemy();
        _enemiesSpawnedCount++;

        // Nếu đã sinh đủ số quái của đợt này
        if (_enemiesSpawnedCount >= EnemiesPerWave)
        {
            _spawnTimer.Stop();
            // Nghỉ 5 giây rồi qua đợt tiếp theo (tăng độ khó)
            GetTree().CreateTimer(5.0f).Timeout += () => 
            {
                EnemiesPerWave += 2; // Tăng thêm 2 con mỗi đợt
                if(SpawnInterval > 0.5f) SpawnInterval -= 0.1f; // Quái ra nhanh hơn
                _spawnTimer.WaitTime = SpawnInterval;
                StartNextWave();
            };
        }
    }

    private void SpawnEnemy()
    {
        if (EnemyScene == null || EnemyPath == null)
        {
            GD.PrintErr("Chưa gán EnemyScene hoặc EnemyPath cho WaveManager!");
            return;
        }

        // Tạo xe chở quái (PathFollow2D)
        var pathFollow = new PathFollow2D();
        pathFollow.Loop = false; // Đi hết đường thì dừng
        pathFollow.Rotates = false; // Giữ quái thẳng đứng, không xoay theo đường

        // Tạo quái
        var enemy = EnemyScene.Instantiate<Node2D>();

        // Gắn kết: Path2D -> PathFollow2D -> Enemy
        EnemyPath.AddChild(pathFollow);
        pathFollow.AddChild(enemy);
    }
}