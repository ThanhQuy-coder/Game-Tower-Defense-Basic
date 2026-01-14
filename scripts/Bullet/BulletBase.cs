using Godot;
using System;

// [BASE CLASS - MODIFIED FOR HOMING & INSTANT FAIL]
// Chịu trách nhiệm: Bay tìm mục tiêu, tự hủy nếu mất mục tiêu.
public abstract partial class BulletBase : Area2D
{
	[Export] public float Speed = 600.0f;
	[Export] public int Damage = 10;
	[Export] public float Lifetime = 3.0f;

	protected Vector2 Direction;
	protected bool IsInitialized = false;
	
	// [MỚI] Biến lưu trữ mục tiêu để khóa
	protected EnemyBase TargetEnemy;

	public override void _Ready()
	{
		// Tự hủy sau thời gian quy định (phòng hờ)
		GetTree().CreateTimer(Lifetime).Timeout += () => QueueFree();
		AreaEntered += OnHit;
	}

	// [ĐÃ SỬA] Thêm tham số target vào Setup
	public void Setup(Vector2 position, float rotation, EnemyBase target = null)
	{
		GlobalPosition = position;
		GlobalRotation = rotation;
		TargetEnemy = target; // Lưu mục tiêu

		// Tính hướng bay ban đầu
		Direction = Vector2.Right.Rotated(rotation);
		IsInitialized = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsInitialized) return;

		// [LOGIC MỚI] Khóa mục tiêu và biến mất nếu trượt
		if (TargetEnemy != null)
		{
			// Kiểm tra xem mục tiêu còn tồn tại không (chưa chết, chưa bị queueFree)
			if (!IsInstanceValid(TargetEnemy))
			{
				// Mục tiêu đã chết hoặc biến mất -> Đạn biến mất ngay lập tức (không bay lung tung)
				QueueFree();
				return;
			}

			// Cập nhật hướng bay về phía mục tiêu (Homing Missile)
			Vector2 targetDir = (TargetEnemy.GlobalPosition - GlobalPosition).Normalized();
			
			// Có thể dùng Lerp để xoay từ từ cho đẹp, nhưng ở đây gán thẳng để khóa cứng "Hard Lock"
			Direction = targetDir;
			
			// Xoay hình ảnh đạn theo hướng bay
			GlobalRotation = Direction.Angle();
		}

		// Di chuyển
		GlobalPosition += Direction * Speed * (float)delta;
	}

	// Template Method: Xử lý chung rồi gọi hàm riêng
	private void OnHit(Area2D area)
	{
		// Chỉ xử lý nếu trúng Enemy
		if (area is EnemyBase enemy)
		{
			// Nếu có TargetEnemy, chỉ cho phép trúng đúng con Target đó (tránh việc đạn xuyên qua con khác)
			// Hoặc nếu muốn đạn chắn được thì bỏ dòng check bên dưới.
			if (TargetEnemy != null && enemy != TargetEnemy) return; 

			ApplyEffect(enemy);
			CreateImpactEffect();
			QueueFree(); // Hủy đạn sau khi trúng
		}
	}

	// Các class con sẽ định nghĩa hiệu ứng cụ thể (Sát thương đơn, nổ lan, làm chậm...)
	protected abstract void ApplyEffect(EnemyBase enemy);

	protected virtual void CreateImpactEffect()
	{
		// TODO: Spawn hiệu ứng nổ/bụi tại GlobalPosition
	}
}
