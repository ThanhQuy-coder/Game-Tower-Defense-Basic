using Godot;
using System;

// [BASE CLASS UPDATE]
public abstract partial class EnemyBase : Area2D
{
	[ExportCategory("Stats")]
	[Export] public float BaseSpeed = 100.0f;
	[Export] public int MaxHealth = 50;
	[Export] public int GoldReward = 10;
	[Export] public int DamageToPlayer = 1;

	protected float CurrentSpeed;
	protected int CurrentHealth;
	protected PathFollow2D PathFollow;
	protected ProgressBar HealthBar;
	protected float AnimationRotationOffset = -90;

	private float _slowTimer = 0.0f;
	private bool _isSlowed = false;
	private AnimatedSprite2D _animatedSprite;

	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
		CurrentSpeed = BaseSpeed;
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (GetParent() is PathFollow2D path)
		{
			PathFollow = path;
			PathFollow.Loop = false;
			PathFollow.Rotates = true;
		}

		// Tìm thanh máu và cài đặt
		HealthBar = GetNodeOrNull<ProgressBar>("HealthBar");
		if (HealthBar != null)
		{
			HealthBar.MaxValue = MaxHealth;
			HealthBar.Value = CurrentHealth;

			// [THAY ĐỔI Ở ĐÂY] Luôn hiển thị thanh máu
			HealthBar.Visible = true;

			// Tắt xử lý chuột cho thanh máu để không chắn đạn
			HealthBar.MouseFilter = Control.MouseFilterEnum.Ignore;
		}

		AddToGroup("enemy");
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleStatusEffects((float)delta);
		Move((float)delta);
	}

	/// <summary>
	/// Phương thức di chuyển
	/// </summary>
	/// <param name="delta">Thời gian giữa 2 frame</param>
	protected virtual void Move(float delta)
	{
		if (PathFollow == null) return;

		PathFollow.Progress += CurrentSpeed * delta;

		UpdateAnimation();

		if (PathFollow.ProgressRatio >= 1.0f) ReachDestination();

	}

	/// <summary>
	/// Phương thức xử lý animation
	/// </summary>
	protected void UpdateAnimation()
	{
		// Lấy góc quay hiện tại theo độ (0 đến 360 hoặc -180 đến 180)
		float angle = Mathf.RadToDeg(PathFollow.Rotation);

		if (angle < 0) angle += 360;

		// Xác định dựa trên góc
		// 0 hoặc 360 là hướng Đông (phải), 90 là Nam (xuống), 180 là Tây (trái), 270 là Bắc (lên)
		if (angle > 45 && angle <= 135)
		{
			_animatedSprite.Rotation = AnimationRotationOffset;
			_animatedSprite.Play("RunY");
			_animatedSprite.FlipH = false; // Xuống
		}
		else if (angle > 225 && angle <= 315)
		{
			_animatedSprite.Rotation = AnimationRotationOffset + 180;
			_animatedSprite.Play("RunY");
			_animatedSprite.FlipH = false; // Lên
		}
		else if ((angle > 0 && angle <= 45) || (angle > 315 && angle <= 360))
		{
			_animatedSprite.Rotation = AnimationRotationOffset + 90;
			_animatedSprite.Play("RunX");
			_animatedSprite.FlipH = true;
		}
		else // Các hướng còn lại là đi ngang (trái hoặc phải)
		{
			_animatedSprite.Rotation = AnimationRotationOffset + 90;
			_animatedSprite.Play("RunX");
			_animatedSprite.FlipH = false;
		}
	}

	/// <summary>
	/// Phương thức bị gây sát thương
	/// </summary>
	/// <remarks>
	/// 	<example>
	/// 		Quái bị trụ tấn công hoặc bị các nguồn sát thương khác tấn công
	/// 	</example>
	/// </remarks>
	/// <param name="amount">Số máu trừ</param>
	public virtual void TakeDamage(int amount)
	{
		CurrentHealth -= amount;
		UpdateHealthBar();

		if (CurrentHealth <= 0)
		{
			Die();
		}
		else
		{
			// Hiệu ứng nháy đỏ khi trúng đạn
			Modulate = Colors.Red;
			var tween = CreateTween();
			tween.TweenProperty(this, "modulate", Colors.White, 0.1f);
		}
	}

	/// <summary>
	/// Phương thức cập nhật thanh máu của quái
	/// </summary>
	protected virtual void UpdateHealthBar()
	{
		if (HealthBar != null)
		{
			HealthBar.Value = CurrentHealth;
		}
	}

	/// <summary>
	/// Phương thức thêm hiệu ứng làm chậm
	/// </summary>
	/// <param name="percent">phần trăm làm chậm</param>
	/// <param name="duration">khoảng thời gian bị chậm</param>
	public void ApplySlow(float percent, float duration)
	{
		CurrentSpeed = BaseSpeed * (1.0f - percent);
		_slowTimer = duration;
		_isSlowed = true;
		Modulate = Colors.Blue;
	}

	/// <summary>
	/// Phương thức xử lý thời gian trạng thái hiệu ứng của quái
	/// </summary>
	/// <remarks>
	/// <example>
	/// Quái bị làm chậm hoặc các hiệu ứng có lợi/bất lợi khác trong khoảng thời gian nhất định
	/// </example>
	/// </remarks>
	/// <param name="delta">Thời gian giữa 2 frame</param>
	private void HandleStatusEffects(float delta)
	{
		if (_isSlowed)
		{
			_slowTimer -= delta;
			if (_slowTimer <= 0)
			{
				_isSlowed = false;
				CurrentSpeed = BaseSpeed;
				Modulate = Colors.White;
			}
		}
	}

	/// <summary>
	/// Phương thức xử lý việc quái bị tiêu diệt
	/// </summary>
	/// <remarks>
	/// 	<list type="number">
	/// 		<item>
	/// 			<term>Xử lý việc cộng tiền cho người chơi</term>
	/// 		</item>
	/// 		<item>
	/// 			<term>Xử lý việc giải phóng node</term>
	/// 		</item>
	/// 	</list>
	/// </remarks>
	protected virtual void Die()
	{
		if (Global.Instance != null) Global.Instance.Gold += GoldReward;
		if (GetParent() is PathFollow2D) GetParent().QueueFree();
		else QueueFree();
	}

	/// <summary>
	/// Phương thức xử lý việc quái di chuyển đến đích
	/// </summary>
	/// <remarks>
	/// 	<list type="number">
	/// 		<item>
	/// 			<term>Xử lý việc trừ máu người chơi</term>
	/// 		</item>
	/// 		<item>
	/// 			<term>Xử lý việc giải phóng node</term>
	/// 		</item>
	/// 	</list>
	/// </remarks>
	protected void ReachDestination()
	{
		if (Global.Instance != null) Global.Instance.Health -= DamageToPlayer;
		if (GetParent() is PathFollow2D) GetParent().QueueFree();
		else QueueFree();
	}
}
