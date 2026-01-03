using Godot;
using System;

// [Người 1]
// Enemy chuyên nghiệp:
// 1. Kế thừa CharacterBody2D hoặc Area2D (Ở đây dùng Area2D để tối ưu va chạm đạn).
// 2. Tự động xử lý di chuyển theo PathFollow2D (Node cha).
// 3. Có sự kiện chết, thanh máu, và hiệu ứng nhận damage.
public partial class Enemy : Area2D
{
	[ExportCategory("Stats")]
	[Export] public float Speed = 1.0f;
	[Export] public int MaxHealth = 20;
	[Export] public int GoldReward = 15;
	[Export] public int DamageToPlayer = 1;

	private int _currentHealth;
	private PathFollow2D _pathFollow; // Tham chiếu đến node cha để di chuyển
	private ProgressBar _healthBar;
	private Sprite2D _sprite;

	public override void _Ready()
	{
		_currentHealth = MaxHealth;
		
		// Setup Visuals
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		_healthBar = GetNodeOrNull<ProgressBar>("HealthBar");
		
		if (_healthBar != null)
		{
			_healthBar.MaxValue = MaxHealth;
			_healthBar.Value = _currentHealth;
			_healthBar.Visible = false; // Chỉ hiện khi mất máu
		}

		// Kiểm tra cấu trúc Node
		if (GetParent() is PathFollow2D pathFollow)
		{
			_pathFollow = pathFollow;
			_pathFollow.Loop = false; // Quái đi hết đường thì thôi
		}
		
		AddToGroup("enemy");
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAlongPath((float)delta);
	}

	private void MoveAlongPath(float delta)
	{
		if (_pathFollow != null)
		{
			// Di chuyển node cha (PathFollow2D)
			_pathFollow.Progress += Speed * delta;

			// Kiểm tra xem đã đi hết đường chưa (ProgressRatio >= 1.0)
			if (_pathFollow.ProgressRatio >= 1.0f)
			{
				ReachBase();
			}
		}
	}

	// Hàm nhận sát thương (Gọi bởi Bullet)
	public void TakeDamage(int amount)
	{
		_currentHealth -= amount;
		
		// Visual Feedback
		if (_healthBar != null)
		{
			_healthBar.Visible = true;
			_healthBar.Value = _currentHealth;
		}
		
		// Flash effect
		if (_sprite != null)
		{
			_sprite.Modulate = new Color(10, 10, 10); // Flash trắng sáng
			var tween = CreateTween();
			tween.TweenProperty(_sprite, "modulate", Colors.White, 0.1f);
		}

		if (_currentHealth <= 0)
		{
			Die();
		}
	}

	private void ReachBase()
	{
		// Trừ máu người chơi
		if (Global.Instance != null)
		{
			Global.Instance.Health -= DamageToPlayer;
		}
		QueueFreeParent(); // Xóa cả PathFollow2D
	}

	private void Die()
	{
		// Cộng tiền
		if (Global.Instance != null)
		{
			Global.Instance.Gold += GoldReward;
		}
		
		// TODO: Spawn death particle effect here
		
		QueueFreeParent();
	}

	private void QueueFreeParent()
	{
		// Nếu cha là PathFollow2D, xóa cha để dọn dẹp sạch sẽ
		if (_pathFollow != null)
			_pathFollow.QueueFree();
		else
			QueueFree();
	}
}
