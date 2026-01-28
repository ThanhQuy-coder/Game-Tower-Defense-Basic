using Godot;

// [LOẠI 3] Đạn phép: Gây sát thương phép và làm chậm
public partial class MagicMissile : BulletBase
{
	[Export] public float SlowPercent = 0; // Hiệu ứng Giảm 20% tốc độ (Làm chậm kẻ địch)
	[Export] public float SlowDuration = 0; // Thời gian làm chậm

	public override void _Ready()
	{
		// Gọi Ready của lớp cha để khởi tạo Timer và Signal
		base._Ready();

		Damage = 15;
	}

	public void SetupMagic(float SlowPct, float duration)
	{
		this.SlowPercent = SlowPct;
		this.SlowDuration = duration;
	}

	protected override void ApplyEffect(EnemyBase enemy)
	{
		// Gây sát thương
		enemy.TakeDamage(Damage);

		// Gọi hàm làm chậm của Enemy
		enemy.ApplySlow(SlowPercent, SlowDuration);
	}
}
