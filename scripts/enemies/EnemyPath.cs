using Godot;

public partial class EnemyPath : Node2D
{
	[Export] private PackedScene _enemyScene;
	[Export] private float _spawnInterval = 2.0f;
	
	private Path2D _path;
	private Timer _spawnTimer;
	
	public override void _Ready()
	{
		_path = GetNode<Path2D>("Path2D");
		
		if (_path == null)
		{
			GD.PrintErr("Không tìm thấy Path2D!");
			return;
		}
		
		_spawnTimer = new Timer();
		_spawnTimer.WaitTime = _spawnInterval;
		_spawnTimer.Timeout += SpawnEnemy;
		AddChild(_spawnTimer);
		
		_spawnTimer.Start();
	}
	
	private void SpawnEnemy()
	{
		if (_enemyScene == null) return;
		
		var pathFollow = new PathFollow2D();
		_path.AddChild(pathFollow);
		
		// QUAN TRỌNG: Bắt đầu từ ĐẦU (progress = 0)
		pathFollow.Progress = 0;
		pathFollow.Loop = false;
		
		var enemy = _enemyScene.Instantiate<EnemyAizen>();
		pathFollow.AddChild(enemy);
		
		// Đặt đúng vị trí
		enemy.GlobalPosition = pathFollow.GlobalPosition;
		
		GD.Print($"✅ Enemy spawn tại: {pathFollow.GlobalPosition}");
	}
	
	public void StopSpawning() => _spawnTimer?.Stop();
	public void StartSpawning() => _spawnTimer?.Start();
}
