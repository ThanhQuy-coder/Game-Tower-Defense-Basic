using Godot;

// [LOẠI 3 MỚI] Thay thế cho Flying Enemy
// Soldier: Kẻ thù cơ bản, chỉ số cân bằng, không có gì đặc biệt
public partial class SoldierEnemy : EnemyBase
{
	public override void _Ready()
	{
		BaseSpeed = 80.0f;   // Tốc độ trung bình
		MaxHealth = 30;      // Máu khá
		GoldReward = 12;
		
		// Có thể chỉnh scale nhỏ hơn Tank nhưng to hơn Fast
		Scale = new Vector2(1.1f, 1.1f); 
		
		// Gọi base._Ready() để thiết lập di chuyển
		base._Ready();
		
		// Đổi màu để nhận diện (Ví dụ: Màu xám thép)
		Modulate = new Color(0.6f, 0.7f, 0.8f); 
	}
}
