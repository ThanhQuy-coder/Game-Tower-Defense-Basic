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

	private float _slowTimer = 0.0f;
	private bool _isSlowed = false;
	private bool _isDead = false;
	private AnimatedSprite2D _animatedSprite;
	private Tween _fadeTween;

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

			// Tắt thanh máu ngay ban đầu chỉ khi mất máu mới hiện
			HealthBar.Visible = false;

			// Tắt xử lý chuột cho thanh máu để không chắn đạn
			HealthBar.MouseFilter = Control.MouseFilterEnum.Ignore;
		}

		AddToGroup("enemy");
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleStatusEffects((float)delta);
		Move((float)delta);

		// Cập nhật vị trí thanh máu để luôn ở trên đầu quái
		if (HealthBar != null && HealthBar.Visible)
		{
			// Vector2(0, -50) là khoảng cách phía trên đầu, có thể chỉnh lại
			HealthBar.GlobalPosition = GlobalPosition + new Vector2(-HealthBar.Size.X / 2, -20);
			HealthBar.Rotation = 0; // Đảm bảo luôn nằm ngang
		}
	}

	/// <summary>
	/// Phương thức di chuyển
	/// </summary>
	/// <param name="delta">Thời gian giữa 2 frame</param>
	protected virtual void Move(float delta)
	{
		if (_isDead) return;
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
		PathFollow.SetPhysicsProcess(true); // Bật lại nếu đã tắt ở Die()

		_animatedSprite.FlipH = false;
		_animatedSprite.FlipV = false;

		// Lấy góc quay hiện tại theo độ (0 đến 360 hoặc -180 đến 180)
		float angle = Mathf.RadToDeg(PathFollow.Rotation);

		if (angle < 0) angle += 360;

		// Xác định dựa trên góc
		// 0 hoặc 360 là hướng Đông (phải), 90 là Nam (xuống), 180 là Tây (trái), 270 là Bắc (lên)
		if (angle > 60 && angle <= 135) // Xuống
		{
			_animatedSprite.Rotation = -90;
			_animatedSprite.Play("RunDown");
		}
		else if (angle > 225 && angle <= 315) // Lên
		{
			_animatedSprite.Rotation = 90;
			_animatedSprite.Play("RunUp");
		}
		else if ((angle > 0 && angle <= 60) || (angle > 315 && angle <= 360)) // phải
		{
			_animatedSprite.Rotation = 0;
			_animatedSprite.Play("RunX");
			_animatedSprite.FlipH = true;
		}
		else // trái
		{
			_animatedSprite.Rotation = 0;
			_animatedSprite.Play("RunX");
			_animatedSprite.FlipH = true;
			_animatedSprite.FlipV = true;
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

			HealthBar.Modulate = new Color(1, 1, 1, 1); // Hiện rõ (Alpha = 1)
			HealthBar.Visible = true;

			// Nếu đang có một hiệu ứng mờ dần cũ, hãy hủy nó đi
			if (_fadeTween != null) _fadeTween.Kill();

			_fadeTween = CreateTween();
			// Đợi 1.5 giây rồi bắt đầu mờ dần trong 0.5 giây
			_fadeTween.TweenInterval(1.5f);
			_fadeTween.TweenProperty(HealthBar, "modulate:a", 0.0f, 0.5f);
			_fadeTween.Finished += () => HealthBar.Visible = false;
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
	/// 			<term>Hiển thị animation die</term>
	/// 		</item>
	/// 		<item>
	/// 			<term>Xử lý việc cộng tiền cho người chơi</term>
	/// 		</item>
	/// 		<item>
	/// 			<term>Xử lý việc giải phóng node</term>
	/// 		</item>
	/// 	</list>
	/// </remarks>
	protected virtual async void Die()
	{
		_isDead = true;

		if (PathFollow != null)
		{
			PathFollow.SetProcess(false);
			PathFollow.SetPhysicsProcess(false);
		}

		// Hiển thị animation die
		// Dựa theo hướng quái để hiển thị đúng animation die
		if (_animatedSprite.Rotation == -90)
		{
			_animatedSprite.Play("DeathDown");
		}
		else if (_animatedSprite.Rotation == 90)
		{
			_animatedSprite.Play("DeathUp");
		}
		else
		{
			_animatedSprite.Play("DeathX");
		}

		// Chờ animation chạy xong
		await ToSignal(_animatedSprite, "animation_finished");

		// Cộng tiền người chơi
		if (Global.Instance != null) Global.Instance.Gold += GoldReward;

		// Giải phóng tài nguyên quái
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
