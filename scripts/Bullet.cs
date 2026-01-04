//using Godot;
//using System;
//
//// OOP: Bullet chỉ biết bay và gây sát thương. Nó không cần biết ai bắn nó.
//public partial class Bullet : Area2D
//{
	//[Export]
	//private int speed = 500;
	//
	//[Export]
	//private int damage = 2;
//
	//public override void _Ready()
	//{
		//// Design Pattern: Observer (lắng nghe sự kiện va chạm)
		//// Kết nối tín hiệu AreaEntered với hàm OnAreaEntered
		//AreaEntered += OnAreaEntered;
		//
		//// Kết nối tín hiệu rời màn hình để tự hủy (tránh rác bộ nhớ)
		//var notifier = GetNodeOrNull<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		//if (notifier != null)
		//{
			//notifier.ScreenExited += () => QueueFree();
		//}
	//}
//
	//public override void _Process(double delta)
	//{
		//// Logic: Bay thẳng theo hướng bên phải (Transform.X) của viên đạn
		//// Người 1 thống nhất: Trục bắn là trục X.
		//Position += Transform.X * speed * (float)delta;
	//}
//
	//// Hàm xử lý va chạm
	//private void OnAreaEntered(Area2D area)
	//{
		//// Kiểm tra xem vật va chạm có phải là kẻ thù không
		//if (area.IsInGroup("enemy"))
		//{
			//// Kiểm tra xem object đó có script Enemy không để gọi hàm TakeDamage
			//// Đây là tính đa hình (Polymorphism) sơ khai: Tương tác với object qua interface/method public
			//if (area is Enemy enemy)
			//{
				//enemy.TakeDamage(damage);
				//
				//// Đạn trúng thì biến mất
				//QueueFree(); 
			//}
		//}
	//}
//}
using Godot;

public partial class Bullet : Area2D
{
	[Export] private int speed = 500;
	[Export] private int damage = 50; // Tăng damage để test
	
	public override void _Ready()
	{
		CollisionLayer = 3;
		CollisionMask = 2;
		
		AreaEntered += OnAreaEntered;
	}
	
	public override void _Process(double delta)
	{
		Position += Transform.X * speed * (float)delta;
	}
	
	private void OnAreaEntered(Area2D area)
	{
		if (area.GetParent() is EnemyAizen enemy)
		{
			enemy.TakeDamage(damage);
			QueueFree();
		}
	}
}
