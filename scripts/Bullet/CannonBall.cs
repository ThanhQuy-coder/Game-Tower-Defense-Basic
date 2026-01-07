using Godot;

// [LOẠI 2] Đạn pháo: Bay chậm, nổ lan (AOE)
public partial class CannonBall : BulletBase
{
	[Export] public float ExplosionRadius = 100.0f;

	public override void _Ready()
	{
		base._Ready();
		Speed = 400.0f; // Pháo bay chậm
	}

	protected override void ApplyEffect(EnemyBase target)
	{
		// 1. Gây sát thương cho mục tiêu chính
		target.TakeDamage(Damage);

		// 2. Tìm các kẻ địch khác xung quanh để gây sát thương nổ (AOE)
		var allEnemies = GetTree().GetNodesInGroup("enemy");
		foreach (Node node in allEnemies)
		{
			if (node is EnemyBase enemy && enemy != target)
			{
				if (GlobalPosition.DistanceTo(enemy.GlobalPosition) <= ExplosionRadius)
				{
					enemy.TakeDamage(Damage / 2); // Sát thương nổ lan bằng 50% gốc
				}
			}
		}
		
		GD.Print("Bùm! Nổ lan.");
	}
}
