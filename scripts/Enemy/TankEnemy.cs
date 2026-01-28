using Godot;

// [LOẠI 2] Quái Tank: Chậm, máu trâu
public partial class TankEnemy : EnemyBase
{
	public override void _Ready()
	{
		BaseSpeed = 20.0f;
		MaxHealth = 150;
		GoldReward = 50;
		DamageToPlayer = 4;
		base._Ready();
	}
}
