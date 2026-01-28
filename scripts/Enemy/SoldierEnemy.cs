using Godot;

// [LOẠI 3 MỚI] Thay thế cho Flying Enemy
// Soldier: Kẻ thù cơ bản, chỉ số cân bằng, không có gì đặc biệt
public partial class SoldierEnemy : EnemyBase
{
	public override void _Ready()
	{
		BaseSpeed = 30.0f;   // Tốc độ nhanh
		MaxHealth = 80;      // Máu khá
		GoldReward = 20;
		DamageToPlayer = 2;

		// Có thể chỉnh scale nhỏ hơn Tank nhưng to hơn Fast
		Scale = new Vector2(1.1f, 1.1f);

		// Gọi base._Ready() để thiết lập di chuyển
		base._Ready();
	}
}
