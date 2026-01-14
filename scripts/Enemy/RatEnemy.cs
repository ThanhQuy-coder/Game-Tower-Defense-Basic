using Godot;

// [LOẠI 1] Quái nhanh: Máu ít, chạy cực nhanh
public partial class RatEnemy : EnemyBase
{
	public override void _Ready()
	{
		BaseSpeed = 40.0f;
		MaxHealth = 50;     // Máu giấy
		GoldReward = 5;
		base._Ready();
	}
}
