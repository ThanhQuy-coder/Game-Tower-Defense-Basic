using Godot;

public partial class BossMahoraga : EnemyBase
{
	[Export] public int AdaptationThreshold = 50; // Mất 50 máu sẽ thích nghi
	private int _damageTakenCycle = 0;
	private int _adaptationLevel = 0;

	public override void _Ready()
	{
		BaseSpeed = 20.0f;  // Đi chậm lại một chút
		MaxHealth = 1000;    
		GoldReward = 500;
		DamageToPlayer = 5; // [ĐÃ SỬA] Giảm xuống 5 theo yêu cầu Nerf
		
		// Scale ban đầu to gấp đôi quái thường (giữ nguyên, nhưng không to thêm nữa)
		Scale = new Vector2(2.0f, 2.0f);
		
		base._Ready();
	}

	// [FIX LỖI NULL REFERENCE - UpdateAnimation]
	// Ngăn code base gọi animation di chuyển bị thiếu
	protected override void UpdateAnimation()
	{
		// Để trống: Boss tạm thời không có animation di chuyển
	}

	// [FIX LỖI CS0507 - Access Modifier]
	// Đã đổi từ 'public' sang 'protected' để khớp với EnemyBase.Die()
	protected override void Die()
	{
		// Tự xử lý cộng tiền (Logic lấy từ EnemyBase nhưng bỏ phần Animation)
		if (Global.Instance != null)
		{
			Global.Instance.Gold += GoldReward;
		}

		// Hủy Boss ngay lập tức thay vì chờ Animation
		QueueFree();
	}

	public override void TakeDamage(int amount)
	{
		// Giảm sát thương nhận vào dựa trên cấp độ thích nghi
		// Mỗi cấp thích nghi giảm 1 damage nhận vào
		int reducedDamage = amount - _adaptationLevel; 
		if (reducedDamage < 1) reducedDamage = 1; // Luôn nhận ít nhất 1 dmg

		base.TakeDamage(reducedDamage);
		
		_damageTakenCycle += reducedDamage;

		// Logic "Bánh xe quay" thích nghi
		if (_damageTakenCycle >= AdaptationThreshold)
		{
			Adapt();
			_damageTakenCycle = 0;
		}
	}

	private void Adapt()
	{
		_adaptationLevel++;
		GD.Print($"[BOSS] Mahoraga thích nghi cấp độ {_adaptationLevel}! (Giảm {_adaptationLevel} sát thương nhận vào)");

		// 1. Hồi một chút máu
		CurrentHealth += 20; 
		if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
		UpdateHealthBar();

		// 2. Tăng tốc độ nhẹ
		CurrentSpeed += 5.0f;
		
		// 3. Hiệu ứng Visual : đổi màu Vàng
		var tween = CreateTween();
		tween.TweenProperty(this, "modulate", Colors.Gold, 0.5f); // Hóa vàng báo hiệu thích nghi
		tween.TweenCallback(Callable.From(() => Modulate = Colors.White)).SetDelay(0.5f);
	}
}
