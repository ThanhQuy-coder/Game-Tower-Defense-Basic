using Godot;
using System;

/// <summary>
/// Lớp cơ sở cho các loại đạn trong Game.
/// Xử lý: Di chuyển hướng mục tiêu (Homing), tự hủy khi mất mục tiêu hoặc hết thời gian.
/// </summary>
public abstract partial class BulletBase : Area2D
{
	// --- Các thuộc tính cấu hình ---
	[Export] public float Speed = 150.0f;     // Tốc độ bay
	[Export] public int Damage = 10;          // Sát thương cơ bản
	[Export] public float Lifetime = 3.0f;    // Thời gian tồn tại tối đa

	protected Vector2 Direction;              // Hướng di chuyển hiện tại
	protected bool IsInitialized = false;     // Đảm bảo đạn đã được Setup trước khi chạy
	protected EnemyBase TargetEnemy;          // Mục tiêu mà đạn đang khóa (Homing)

	public override void _Ready()
	{
		// Thiết lập bộ đếm tự hủy để tránh rác bộ nhớ
		GetTree().CreateTimer(Lifetime).Timeout += () => QueueFree();

		// Kết nối tín hiệu va chạm
		AreaEntered += OnHit;
	}

	/// <summary>
	/// Khởi tạo các thông số ban đầu khi đạn được bắn ra.
	/// </summary>
	public void Setup(Vector2 position, float rotation, int towerDamage, EnemyBase target = null)
	{
		GlobalPosition = position;
		GlobalRotation = rotation;
		TargetEnemy = target;

		// Gán sát thương của Tháp vào viên đạn này
		this.Damage = (towerDamage != 0) ? towerDamage : this.Damage;

		// Tính toán hướng ban đầu dựa trên góc xoay
		Direction = Vector2.Right.Rotated(rotation);
		IsInitialized = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsInitialized) return;

		// Xử lý logic đuổi mục tiêu (Homing Missile)
		if (TargetEnemy != null)
		{
			// Nếu mục tiêu biến mất hoặc chết, đạn tự hủy ngay để tránh bay lạc
			if (!IsInstanceValid(TargetEnemy))
			{
				QueueFree();
				return;
			}

			// Cập nhật hướng bay liên tục về phía mục tiêu
			Vector2 targetDir = (TargetEnemy.GlobalPosition - GlobalPosition).Normalized();
			Direction = targetDir; // Hard Lock: Khóa mục tiêu tuyệt đối

			// Đồng bộ góc xoay của hình ảnh đạn theo hướng bay
			GlobalRotation = Direction.Angle();
		}

		// Thực hiện di chuyển vật lý
		GlobalPosition += Direction * Speed * (float)delta;
	}

	/// <summary>
	/// Xử lý va chạm chung.
	/// </summary>
	private void OnHit(Area2D area)
	{
		if (area is EnemyBase enemy)
		{
			// Kiểm tra: Đảm bảo đạn chỉ trúng mục tiêu đã được khóa
			if (TargetEnemy != null && enemy != TargetEnemy) return;

			// Thực thi các hiệu ứng cụ thể của từng loại đạn
			ApplyEffect(enemy);
			CreateImpactEffect();

			// Xóa đạn sau khi hoàn thành nhiệm vụ
			QueueFree();
		}
	}

	/// <summary>
	/// Định nghĩa hiệu ứng cụ thể khi trúng mục tiêu (Sát thương, làm chậm, nổ lan...)
	/// </summary>
	protected abstract void ApplyEffect(EnemyBase enemy);

	/// <summary>
	/// Tạo hiệu ứng hình ảnh (VFX) khi va chạm.
	/// </summary>
	protected virtual void CreateImpactEffect()
	{
		// Sẽ được ghi đè ở lớp con để thêm hiệu ứng nổ/bụi
	}
}
