using Godot;
using System;
using System.Collections.Generic;

// Tower logic: Tìm mục tiêu -> Xoay nòng -> Bắn
public partial class Tower : Node2D
{
	[Export]
	private PackedScene bulletScene; // Kéo file Bullet.tscn vào đây trên Inspector

	private List<Node2D> targetsInRange = new List<Node2D>(); // Danh sách quái trong tầm
	private Node2D currentTarget;
	private Marker2D muzzle;
	private Timer reloadTimer;

	public override void _Ready()
	{
		muzzle = GetNode<Marker2D>("Muzzle");
		reloadTimer = GetNode<Timer>("ReloadTimer");

		// Setup Observer cho vùng tầm bắn (Range)
		var rangeArea = GetNode<Area2D>("Range");
		rangeArea.AreaEntered += OnEnemyEnterRange;
		rangeArea.AreaExited += OnEnemyExitRange;
		
		// Setup Timer bắn súng
		reloadTimer.Timeout += OnShootTimerTimeout;
	}

	public override void _Process(double delta)
	{
		UpdateTarget();

		if (currentTarget != null && IsInstanceValid(currentTarget))
		{
			// OOP: Tower điều khiển việc xoay của chính nó
			LookAt(currentTarget.GlobalPosition);
		}
	}

	private void OnEnemyEnterRange(Area2D area)
	{
		if (area.IsInGroup("enemy"))
		{
			targetsInRange.Add(area);
		}
	}

	private void OnEnemyExitRange(Area2D area)
	{
		if (targetsInRange.Contains(area))
		{
			targetsInRange.Remove(area);
		}
	}

	// Logic chọn mục tiêu (Strategy Pattern đơn giản: Chọn con đầu tiên vào list)
	private void UpdateTarget()
	{
		// Làm sạch list nếu có quái đã chết (null)
		targetsInRange.RemoveAll(x => x == null || !IsInstanceValid(x));

		if (targetsInRange.Count > 0)
		{
			currentTarget = targetsInRange[0]; // Chọn con đầu tiên
		}
		else
		{
			currentTarget = null;
		}
	}

	// Factory Method (sơ khai): Tạo ra instance của Bullet
	private void OnShootTimerTimeout()
	{
		if (currentTarget != null && IsInstanceValid(currentTarget))
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		if (bulletScene == null)
		{
			GD.PrintErr("Chưa gắn Bullet Scene vào Tower!");
			return;
		}

		// Instantiate đạn
		var bullet = bulletScene.Instantiate<Area2D>();
		
		// Đặt vị trí đạn tại nòng súng (Muzzle)
		// Lưu ý: Phải thêm vào Root của Scene chính chứ không phải thêm vào Tower
		// Nếu thêm vào Tower, đạn sẽ xoay theo Tower -> sai vật lý
		GetTree().Root.AddChild(bullet);
		
		bullet.GlobalPosition = muzzle.GlobalPosition;
		bullet.GlobalRotation = GlobalRotation; // Đạn bay theo hướng súng đang quay
	}
}
