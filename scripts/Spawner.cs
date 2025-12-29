using Godot;
using System;

public partial class Spawner : Node2D
{
	// Kéo thả file enemy.tscn vào ô này trong Inspector sau này
	[Export] public PackedScene EnemyScene; 
	
	private Path2D _path;
	private Timer _timer;

	public override void _Ready()
	{
		// Tìm Path2D trong Level
		_path = GetNode<Path2D>("../Path2D"); 
		_timer = GetNode<Timer>("Timer");
		
		// Kết nối sự kiện Timer
		_timer.Timeout += OnTimerTimeout;
	}

	private void OnTimerTimeout()
	{
		if (EnemyScene == null) return;

		// 1. Tạo PathFollow2D (Thứ di chuyển trên Path)
		var pathFollow = new EnemyPathFollow();
		_path.AddChild(pathFollow);
		pathFollow.Loop = false; // Không chạy lặp lại

		// 2. Tạo Enemy và nhét vào PathFollow
		var enemy = EnemyScene.Instantiate();
		pathFollow.AddChild(enemy);

		// 3. Thêm script di chuyển đơn giản cho PathFollow (Tạm thời)
		// Lưu ý: Sau này Người 1 sẽ quản lý tốc độ của Enemy
		GD.Print("Đã tạo 1 quái!");
	}
}
