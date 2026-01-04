using Godot;

public partial class EnemyAizen : CharacterBody2D
{
	[Export] public int Health = 30;
	[Export] public int Damage = 1;
	[Export] public float Speed = 50.0f;
	[Export] public int GoldReward = 10;
	
	private PathFollow2D _pathFollow;
	private bool _reachedEnd = false;
	
	public override void _Ready()
	{
		CollisionLayer = 2;
		_pathFollow = GetParent() as PathFollow2D;
		
		if (_pathFollow != null)
		{
			// Đảm bảo bắt đầu từ đầu
			_pathFollow.Progress = 0;
			GlobalPosition = _pathFollow.GlobalPosition;
			
			GD.Print($"👾 Enemy tại: {GlobalPosition}");
			GD.Print($"   PathFollow Progress: {_pathFollow.Progress}");
		}
	}
	
	public void TakeDamage(int damageAmount)
	{
		Health -= damageAmount;
		if (Health <= 0) Die();
	}
	
	private void Die()
	{
		if (Global.Instance != null)
		{
			Global.Instance.Gold += GoldReward;
		}
		QueueFree();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (_pathFollow != null && !_reachedEnd)
		{
			// LƯU Ý: Thử cả 2 cách nếu bị ngược
			float oldProgress = _pathFollow.Progress;
			
			// CÁCH 1: Tiến lên (mặc định)
			_pathFollow.Progress += Speed * (float)delta;
			
			// CÁCH 2: Nếu enemy đi ngược, comment dòng trên, dùng dòng này:
			// _pathFollow.Progress -= Speed * (float)delta;
			
			float newProgress = _pathFollow.Progress;
			
			// Cập nhật vị trí
			GlobalPosition = _pathFollow.GlobalPosition;
			
			// DEBUG: Xem enemy đi hướng nào
			if (oldProgress < newProgress)
			{
				// GD.Print($"→ Đang đi XUÔI (+{newProgress - oldProgress:F1})");
			}
			else if (oldProgress > newProgress)
			{
				// GD.Print($"← Đang đi NGƯỢC ({oldProgress - newProgress:F1})");
			}
			
			// Kiểm tra đến cuối
			// Nếu dùng += thì kiểm tra >=
			// Nếu dùng -= thì kiểm tra <=
			if (_pathFollow.ProgressRatio >= 0.99f) // Cho cách 1
			// if (_pathFollow.ProgressRatio <= 0.01f) // Cho cách 2
			{
				ReachEnd();
			}
		}
	}
	
	private void ReachEnd()
	{
		if (_reachedEnd) return;
		_reachedEnd = true;
		
		GD.Print("💀 Enemy đến base!");
		
		if (Global.Instance != null)
		{
			Global.Instance.Health -= Damage;
		}
		QueueFree();
	}
}
