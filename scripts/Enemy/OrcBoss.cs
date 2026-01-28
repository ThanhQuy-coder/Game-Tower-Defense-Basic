using Godot;

public partial class OrcBoss : EnemyBase
{
	public override void _Ready()
	{
		BaseSpeed = 10.0f;  // Đi chậm lại một chút
		MaxHealth = 1000;
		GoldReward = 500;
		DamageToPlayer = 9999;

		base._Ready();
	}
}
