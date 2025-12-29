using Godot;
using System;

// Áp dụng OOP: Class Enemy chịu trách nhiệm quản lý máu và cái chết của chính nó.
// Không xử lý di chuyển ở đây vì Người 2 sẽ dùng PathFollow2D để di chuyển Node cha của Enemy.
public partial class Enemy : Area2D
{
	// Encapsulation: Dùng [Export] để Designer (Người 2) có thể chỉnh chỉ số trực tiếp trên Editor
	[Export]
	private int maxHp = 10;
	
	private int currentHp;

	[Export]
	private int goldReward = 10; // Tiền rơi ra khi chết

	// Signal để báo cho hệ thống khác (UI, Game Manager) biết quái đã chết
	[Signal]
	public delegate void OnEnemyDiedEventHandler(int gold);

	public override void _Ready()
	{
		currentHp = maxHp;
		
		// Đảm bảo Node này luôn nằm trong group "enemy" để đạn có thể nhận diện
		if (!IsInGroup("enemy"))
		{
			AddToGroup("enemy");
		}
	}

	// Hàm Public: Cho phép các đối tượng khác (như Bullet) gây sát thương
	public void TakeDamage(int damageAmount)
	{
		currentHp -= damageAmount;
		
		// Hiệu ứng nhấp nháy khi trúng đạn (Optional - Visual feedback)
		Modulate = Colors.Red;
		GetTree().CreateTimer(0.1f).Timeout += () => Modulate = Colors.White;

		if (currentHp <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		// Logic: Khi chết thì làm gì?
		// 1. Cộng tiền (Gọi Global - nhưng để đảm bảo decouple, ta nên dùng Signal hoặc gọi an toàn)
		// Cách gọi Global an toàn (Singleton pattern của Người 3)
		var globalNode = GetNodeOrNull<Node>("/root/Global");
		if (globalNode != null)
		{
			// Dùng Reflection hoặc dynamic để gọi biến Gold nếu không muốn phụ thuộc cứng vào file Global.cs chưa tồn tại
			// Tuy nhiên, vì đã thống nhất biến, ta có thể giả định Global có biến Gold.
			// Ở đây mình in ra log để test trước.
			GD.Print($"Enemy died! +{goldReward} Gold");
			
			// Nếu đã có script Global của Người 3, bạn có thể uncomment dòng dưới:
			// Global.Instance.Gold += goldReward; 
		}

		// 2. Xóa khỏi game
		QueueFree();
		
		// Lưu ý: Nếu Enemy là con của PathFollow2D (do Người 2 làm), 
		// việc QueueFree Enemy sẽ chỉ xóa hình ảnh quái, PathFollow2D vẫn chạy rỗng.
		// Tốt nhất là xóa luôn node cha nếu cha là PathFollow2D.
		if (GetParent() is PathFollow2D)
		{
			GetParent().QueueFree();
		}
	}
}