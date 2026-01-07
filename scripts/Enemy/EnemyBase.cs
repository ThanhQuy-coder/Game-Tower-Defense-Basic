using Godot;
using System;

// [BASE CLASS UPDATE]
// Thay đổi: Thanh máu luôn hiển thị (Visible = true) ngay từ đầu
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

	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
		CurrentSpeed = BaseSpeed;
		
		if (GetParent() is PathFollow2D path)
		{
			PathFollow = path;
			PathFollow.Loop = false;
			PathFollow.Rotates = false; 
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

	protected virtual void Move(float delta)
	{
		if (PathFollow != null)
		{
			PathFollow.Progress += CurrentSpeed * delta;
			if (PathFollow.ProgressRatio >= 1.0f) ReachDestination();
		}
	}

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

	protected virtual void UpdateHealthBar() 
	{ 
		if (HealthBar != null)
		{
			HealthBar.Value = CurrentHealth;
		}
	}

	public void ApplySlow(float percent, float duration)
	{
		CurrentSpeed = BaseSpeed * (1.0f - percent);
		_slowTimer = duration;
		_isSlowed = true;
		Modulate = Colors.Blue; 
	}

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

	protected virtual void Die()
	{
		if (Global.Instance != null) Global.Instance.Gold += GoldReward;
		if (GetParent() is PathFollow2D) GetParent().QueueFree();
		else QueueFree();
	}

	protected void ReachDestination()
	{
		if (Global.Instance != null) Global.Instance.Health -= DamageToPlayer;
		if (GetParent() is PathFollow2D) GetParent().QueueFree();
		else QueueFree();
	}
}
