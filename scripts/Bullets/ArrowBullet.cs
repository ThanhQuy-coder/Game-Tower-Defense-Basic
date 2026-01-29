using Godot;

// [LOẠI 1] Đạn tên: Bay nhanh, sát thương đơn thể
public partial class ArrowBullet : BulletBase
{
	public override void _Ready()
	{
		base._Ready();
	}

	protected override void ApplyEffect(EnemyBase enemy)
	{
		// Gây sát thương vật lý cơ bản
		enemy.TakeDamage(Damage);
	}
}
