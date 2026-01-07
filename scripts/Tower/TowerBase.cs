using Godot;
using System.Collections.Generic;

// [TOWER BASE FINAL - FIX POSITION & VISUAL]
public abstract partial class TowerBase : Node2D
{
	[ExportCategory("Config")]
	[Export] public bool IsSideView = true;

	[ExportCategory("Stats")]
	[Export] public float Range = 200.0f;
	[Export] public float FireRate = 1.0f;
	[Export] public int BaseCost = 100;
	
	[ExportCategory("Visuals")]
	[Export] public Texture2D[] BaseTextures;   // Ảnh đế tháp (Bắt buộc thay đổi)
	[Export] public Texture2D[] TurretTextures; // Ảnh lính (Tùy chọn: Để trống nếu không muốn đổi)
	
	// [MỚI] Mảng lưu vị trí đứng của lính theo từng Level
	// Ví dụ: Element 0: (0, -10), Element 1: (0, -25)...
	[Export] public Vector2[] TurretPositions;  

	[ExportCategory("Setup")]
	[Export] public PackedScene BulletScene;
	[Export] protected Marker2D Muzzle;
	[Export] protected Area2D RangeArea;
	[Export] protected CollisionShape2D RangeShape;
	[Export] protected Sprite2D BaseSprite;   
	[Export] protected Sprite2D TurretSprite; 

	protected List<EnemyBase> EnemiesInRange = new List<EnemyBase>();
	protected EnemyBase CurrentTarget;
	protected float FireCooldown = 0.0f;
	
	public int Level { get; private set; } = 1;

	public override void _Ready()
	{
		// An toàn: Đưa lính lên lớp trên cùng để không bị đế tháp che
		if (TurretSprite != null) TurretSprite.ZIndex = 1;
		if (BaseSprite != null) BaseSprite.ZIndex = 0;

		if (RangeShape != null && RangeShape.Shape is CircleShape2D circle) circle.Radius = Range;
		if (RangeArea != null)
		{
			RangeArea.AreaEntered += OnEnemyEnter;
			RangeArea.AreaExited += OnEnemyExit;
		}

		UpdateTowerVisual();
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateCooldown((float)delta);
		FindTarget();

		if (CurrentTarget != null && IsInstanceValid(CurrentTarget))
		{
			RotateTurret((float)delta);
			if (FireCooldown <= 0)
			{
				Shoot();
				FireCooldown = 1.0f / FireRate;
			}
		}
	}

	private void UpdateCooldown(float delta) { if (FireCooldown > 0) FireCooldown -= delta; }
	
	private void FindTarget()
	{
		EnemiesInRange.RemoveAll(e => e == null || !IsInstanceValid(e));
		CurrentTarget = EnemiesInRange.Count > 0 ? EnemiesInRange[0] : null;
	}

	private void RotateTurret(float delta)
	{
		if (TurretSprite != null && CurrentTarget != null)
		{
			Vector2 direction = (CurrentTarget.GlobalPosition - GlobalPosition).Normalized();

			if (IsSideView)
			{
				// Lật trái/phải
				if (direction.X < 0) TurretSprite.Scale = new Vector2(-1, 1);
				else TurretSprite.Scale = new Vector2(1, 1);
				TurretSprite.Rotation = 0; 
			}
			else
			{
				TurretSprite.GlobalRotation = direction.Angle();
			}
		}
	}

	protected virtual void Shoot()
	{
		if (BulletScene == null || Muzzle == null) return;
		var bullet = BulletScene.Instantiate<BulletBase>();
		GetTree().Root.AddChild(bullet);
		
		float bulletRotation = 0;
		if (IsSideView && CurrentTarget != null)
			bulletRotation = (CurrentTarget.GlobalPosition - Muzzle.GlobalPosition).Angle();
		else
			bulletRotation = (TurretSprite != null) ? TurretSprite.GlobalRotation : 0f;
		
		bullet.Setup(Muzzle.GlobalPosition, bulletRotation);
		bullet.Damage += (Level - 1) * 5; 
	}

	public void Upgrade()
	{
		// Kiểm tra max cấp dựa vào số lượng ảnh ĐẾ THÁP (vì lính có thể không đổi)
		int maxLevel = (BaseTextures != null) ? BaseTextures.Length : 3;
		
		if (Level >= maxLevel) return;

		int cost = GetUpgradeCost();
		if (Global.Instance.Gold >= cost)
		{
			Global.Instance.Gold -= cost;
			Level++;
			FireRate *= 1.1f;
			Range *= 1.1f;
			if (RangeShape.Shape is CircleShape2D circle) circle.Radius = Range;
			
			UpdateTowerVisual();
			GD.Print($"Upgraded to Level {Level}");
		}
	}

	private void UpdateTowerVisual()
	{
		int index = Level - 1;

		// 1. Cập nhật ĐẾ THÁP (Base)
		if (BaseSprite != null && BaseTextures != null && index < BaseTextures.Length && BaseTextures[index] != null)
		{
			BaseSprite.Texture = BaseTextures[index];
		}

		// 2. Cập nhật LÍNH (Turret) - CHỈ CẬP NHẬT NẾU CÓ ẢNH TRONG MẢNG
		// Nếu bạn để trống mảng TurretTextures, lính sẽ giữ nguyên ảnh gốc
		if (TurretSprite != null && TurretTextures != null && index < TurretTextures.Length && TurretTextures[index] != null)
		{
			TurretSprite.Texture = TurretTextures[index];
		}

		// 3. [MỚI] Cập nhật VỊ TRÍ ĐỨNG (Để không bị tháp che)
		if (TurretSprite != null && TurretPositions != null && index < TurretPositions.Length)
		{
			TurretSprite.Position = TurretPositions[index];
		}
	}

	public void Sell()
	{
		int sellPrice = (BaseCost + (Level - 1) * 50) / 2;
		Global.Instance.Gold += sellPrice;
		QueueFree();
	}

	public int GetUpgradeCost() => BaseCost / 2 * Level;

	private void OnEnemyEnter(Area2D area) { if (area is EnemyBase enemy) EnemiesInRange.Add(enemy); }
	private void OnEnemyExit(Area2D area) { if (area is EnemyBase enemy) EnemiesInRange.Remove(enemy); }
}
