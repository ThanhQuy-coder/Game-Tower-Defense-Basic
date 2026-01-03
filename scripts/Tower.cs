using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// [Người 1]
// Tower Logic nâng cao:
// 1. Sử dụng danh sách động để quản lý mục tiêu trong tầm bắn.
// 2. Hỗ trợ các chế độ bắn: FIRST (đầu tiên), CLOSE (gần nhất), STRONG (máu nhiều nhất).
// 3. Xoay nòng súng mượt mà (Lerp).
public partial class Tower : Node2D
{
	public enum TargetMode { First, Close, Strong }

	[ExportCategory("Tower Config")]
	[Export] public float Range = 250.0f;
	[Export] public float FireRate = 1.0f; // Số viên đạn mỗi giây
	[Export] public TargetMode CurrentTargetMode = TargetMode.First;
	[Export] public PackedScene BulletScene;
	[Export] public float RotationSpeed = 10.0f;

	[ExportCategory("Setup")]
	[Export] private Marker2D _muzzle; // Vị trí nòng súng
	[Export] private Sprite2D _turretSprite; // Phần xoay được
	[Export] private Area2D _rangeArea;
	[Export] private CollisionShape2D _rangeShape;

	private List<Enemy> _enemiesInRange = new List<Enemy>();
	private Enemy _currentTarget;
	private float _fireCooldown = 0.0f;

	public override void _Ready()
	{
		// Cập nhật bán kính vùng phát hiện địch
		if (_rangeShape != null && _rangeShape.Shape is CircleShape2D circle)
		{
			circle.Radius = Range;
		}

		// Kết nối tín hiệu Area2D
		if (_rangeArea != null)
		{
			_rangeArea.AreaEntered += OnAreaEntered;
			_rangeArea.AreaExited += OnAreaExited;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateCooldown((float)delta);
		SelectTarget();

		if (_currentTarget != null && IsInstanceValid(_currentTarget))
		{
			RotateTurret((float)delta);
			if (_fireCooldown <= 0)
			{
				Shoot();
				_fireCooldown = 1.0f / FireRate;
			}
		}
	}

	private void UpdateCooldown(float delta)
	{
		if (_fireCooldown > 0) _fireCooldown -= delta;
	}

	// Logic chọn mục tiêu thông minh
	private void SelectTarget()
	{
		// Loại bỏ các mục tiêu đã chết hoặc không hợp lệ khỏi danh sách
		_enemiesInRange.RemoveAll(e => e == null || !IsInstanceValid(e));

		if (_enemiesInRange.Count == 0)
		{
			_currentTarget = null;
			return;
		}

		switch (CurrentTargetMode)
		{
			case TargetMode.First:
				// Giả định con nào có GlobalPosition xa nhất trên đường đi (hoặc đơn giản là vào trước)
				// Ở đây ta lấy phần tử đầu tiên (vào tầm trước)
				_currentTarget = _enemiesInRange[0]; 
				break;
			
			case TargetMode.Close:
				_currentTarget = _enemiesInRange.OrderBy(e => e.GlobalPosition.DistanceSquaredTo(GlobalPosition)).FirstOrDefault();
				break;
				
			case TargetMode.Strong:
				// Cần thêm thuộc tính MaxHealth vào Enemy để sort, tạm thời lấy First
				_currentTarget = _enemiesInRange[0];
				break;
		}
	}

	private void RotateTurret(float delta)
	{
		if (_turretSprite == null) return;

		Vector2 direction = (_currentTarget.GlobalPosition - GlobalPosition).Normalized();
		float targetAngle = direction.Angle();
		// Xoay mượt mà (Interpolation)
		float currentAngle = _turretSprite.GlobalRotation;
		_turretSprite.GlobalRotation = (float)Mathf.LerpAngle(currentAngle, targetAngle, RotationSpeed * delta);
		// Lưu ý: Sprite cần hướng sang phải (0 độ) mặc định
	}

	private void Shoot()
	{
		if (BulletScene == null || _muzzle == null) return;

		var bullet = BulletScene.Instantiate<Bullet>();
		// Sử dụng Global.Instance để spawn đạn vào root, tránh bị scale/rotate theo tháp
		if (Global.Instance != null)
		{
			Global.Instance.SpawnActor(bullet, _muzzle.GlobalPosition);
			bullet.Setup(_currentTarget, _turretSprite.GlobalRotation);
		}
	}

	// Event Handlers
	private void OnAreaEntered(Area2D area)
	{
		if (area is Enemy enemy)
		{
			_enemiesInRange.Add(enemy);
		}
	}

	private void OnAreaExited(Area2D area)
	{
		if (area is Enemy enemy)
		{
			_enemiesInRange.Remove(enemy);
		}
	}
}
