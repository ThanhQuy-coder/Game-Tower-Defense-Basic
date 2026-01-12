using Godot;

// [LOẠI 1] Quái nhanh: Máu ít, chạy cực nhanh
public partial class FastEnemy : EnemyBase
{
	public override void _Ready()
	{
		BaseSpeed = 40.0f; // Nhanh gấp đôi
		MaxHealth = 15;     // Máu giấy
		GoldReward = 5;
		base._Ready();
	}
}
