using Godot;
using System;

// [Người 1]
// Bullet logic:
// 1. Hỗ trợ Homing (đuổi theo mục tiêu) nếu target vẫn còn sống.
// 2. Nếu target chết, tiếp tục bay thẳng hướng cuối cùng.
// 3. Tự hủy khi ra khỏi màn hình.
public partial class Bullet : Area2D
{
	[Export] public int Damage = 5;
	[Export] public float Speed = 600.0f;
	[Export] public float Lifetime = 3.0f;
	[Export] public bool IsHoming = true; // Đạn đuổi?
	[Export] public float SteerForce = 5.0f; // Độ bẻ lái

	private Enemy _target;
	private Vector2 _velocity;

	public override void _Ready()
	{
		// Tự hủy sau thời gian Lifetime để tránh lag game
		GetTree().CreateTimer(Lifetime).Timeout += () => QueueFree();
		
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
	}

	public void Setup(Enemy target, float rotation)
	{
		_target = target;
		GlobalRotation = rotation;
		_velocity = Vector2.Right.Rotated(rotation) * Speed;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsHoming && _target != null && IsInstanceValid(_target))
		{
			// Logic tên lửa đuổi (Steering behavior)
			Vector2 desiredVelocity = (_target.GlobalPosition - GlobalPosition).Normalized() * Speed;
			Vector2 steering = (desiredVelocity - _velocity) * SteerForce * (float)delta;
			_velocity += steering;
			
			// Giới hạn tốc độ và cập nhật góc quay
			_velocity = _velocity.Normalized() * Speed;
			GlobalRotation = _velocity.Angle();
		}

		GlobalPosition += _velocity * (float)delta;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is Enemy enemy)
		{
			enemy.TakeDamage(Damage);
			CreateImpactEffect();
			QueueFree();
		}
	}
	
	// Dự phòng nếu Enemy dùng CharacterBody2D
	private void OnBodyEntered(Node2D body)
	{
		if (body is Enemy enemy) // Cần sửa Enemy class để kế thừa đúng nếu dùng Body
		{
			// Logic tương tự
		}
	}

	private void CreateImpactEffect()
	{
		// TODO: Instantiate particle effect
	}
}
