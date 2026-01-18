using Godot;

/// <summary>
/// Loại đạn pháo (Cannon Ball): Tốc độ bay chậm nhưng có khả năng gây sát thương nổ lan (AOE).
/// </summary>
public partial class CannonBall : BulletBase
{
	// --- Cấu hình chỉ số đặc trưng ---
	[Export] public float ExplosionRadius = 100.0f; // Bán kính vụ nổ

	public override void _Ready()
	{
		// Gọi Ready của lớp cha để khởi tạo Timer và Signal
		base._Ready();

		// Ghi đè tốc độ mặc định của đạn cơ bản
		Speed = 200.0f;
	}

	/// <summary>
	/// Triển khai hiệu ứng nổ lan khi đạn va chạm với mục tiêu.
	/// </summary>
	/// <param name="target">Kẻ địch trực tiếp trúng đạn</param>
	protected override void ApplyEffect(EnemyBase target)
	{
		// 1. Gây sát thương trực tiếp cho mục tiêu chính
		target.TakeDamage(Damage);

		// 2. Xử lý sát thương nổ lan (AOE) cho các kẻ địch xung quanh
		// Lấy danh sách tất cả kẻ địch thuộc nhóm "enemy"
		var allEnemies = GetTree().GetNodesInGroup("enemy");

		foreach (Node node in allEnemies)
		{
			// Kiểm tra: Phải là EnemyBase và không phải là mục tiêu chính đã nhận sát thương
			if (node is EnemyBase enemy && enemy != target)
			{
				// Tính khoảng cách giữa vị trí nổ và kẻ địch trong danh sách
				float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);

				if (distance <= ExplosionRadius)
				{
					// Gây sát thương lan (giảm còn khoảng 33% so với sát thương gốc)
					enemy.TakeDamage(Damage / 3);
				}
			}
		}

		GD.Print($"Bùm! Đã gây nổ lan trong bán kính {ExplosionRadius}px.");
	}
}
