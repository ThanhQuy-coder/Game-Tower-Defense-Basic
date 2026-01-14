using Godot;

// [LOẠI 2] Quái Tank: Chậm, máu trâu
public partial class TankEnemy : EnemyBase
{
	public override void _Ready()
	{
		BaseSpeed = 30.0f;  // Rất chậm
		MaxHealth = 150;    // Máu trâu
		GoldReward = 20;
		DamageToPlayer = 2; // Đấm đau hơn
		base._Ready();
	}
}
