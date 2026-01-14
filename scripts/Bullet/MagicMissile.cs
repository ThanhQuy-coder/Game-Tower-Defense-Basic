using Godot;

// [LOẠI 3] Đạn phép: Gây sát thương phép và làm chậm
public partial class MagicMissile : BulletBase
{
	[Export] public float SlowPercent = 0.5f; // Giảm 50% tốc độ
	[Export] public float SlowDuration = 2.0f;

	protected override void ApplyEffect(EnemyBase enemy)
	{
		// Gây sát thương
		enemy.TakeDamage(Damage);

		// Gọi hàm làm chậm của Enemy
		enemy.ApplySlow(SlowPercent, SlowDuration);
	}
}
