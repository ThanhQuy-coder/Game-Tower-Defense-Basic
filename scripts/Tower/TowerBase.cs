using Godot;
using System.Collections.Generic;
using System.Linq; // Cần dùng để sắp xếp List

// [TOWER BASE FINAL - SYNC UI & RANGE]
public abstract partial class TowerBase : Node2D
{
	[ExportCategory("Config")]
	[Export] public bool IsSideView = true;

	[ExportCategory("Stats")]
	[Export] public float Range = 200.0f;
	[Export] public float FireRate = 1.0f;
	[Export] public int BaseCost = 100;

	[ExportCategory("Visuals")]
	[Export] public Texture2D[] BaseTextures;   // Ảnh đế tháp
	[Export] public Texture2D[] TurretTextures; // Ảnh lính
	[Export] public Vector2[] TurretPositions;  // Vị trí lính

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

	// [MỚI] Biến static để lưu tháp đang được chọn duy nhất trên toàn bản đồ
	public static TowerBase SelectedTower { get; private set; }
	private bool _isSelected = false;

	public int Level { get; private set; } = 1;
	public int maxLevel { get; private set; } = 3;

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

	/// <summary>
	/// [ĐÃ SỬA] Dùng _Input để bắt sự kiện chọn tháp (Ưu tiên cao nhất)
	/// </summary>
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			// Nếu click đúng vào vị trí tháp (khoảng cách < 40)
			if (GetGlobalMousePosition().DistanceTo(GlobalPosition) < 40)
			{
				// 1. Nếu đang có tháp khác được chọn -> Bỏ chọn nó trước
				if (SelectedTower != null && SelectedTower != this)
				{
					SelectedTower.Deselect();
				}

				// 2. Chọn tháp này (Hiện tầm bắn)
				if (!_isSelected)
				{
					_isSelected = true;
					SelectedTower = this; // Gán static
					QueueRedraw();
				}

				// LƯU Ý: Không dùng SetInputAsHandled() ở đây để sự kiện click vẫn truyền
				// xuống được TowerSlot (hoặc Button) bên dưới để mở UI Nâng Cấp.
			}
		}
	}

	/// <summary>
	/// [ĐÃ SỬA] Dùng _UnhandledInput để xử lý khi click vào VÙNG TRỐNG
	/// </summary>
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			// Sự kiện chạy vào đây nghĩa là không click vào UI hay Button nào (Click đất trống)

			// Nếu tháp này đang được chọn -> Bỏ chọn và đóng luôn UI
			if (_isSelected)
			{
				Deselect(); // Tắt tầm bắn
				GameUI.Instance?.HideAllPanels(); // Tắt bảng nâng cấp
			}
		}
	}

	/// <summary>
	/// Hàm tắt chọn tháp (Ẩn tầm bắn).
	/// </summary>
	public void Deselect()
	{
		if (_isSelected)
		{
			_isSelected = false;
			// Nếu mình đang là tháp được chọn static thì xóa đi
			if (SelectedTower == this) SelectedTower = null;
			QueueRedraw(); // Vẽ lại để ẩn vòng tròn
		}
	}

	/// <summary>
	/// Vẽ vòng tròn tầm bắn
	/// </summary>
	public override void _Draw()
	{
		if (_isSelected)
		{
			// Vẽ vòng tròn màu đỏ nhạt, bán kính bằng Range
			DrawCircle(Vector2.Zero, Range, new Color(1, 0, 0, 0.2f));
			DrawArc(Vector2.Zero, Range, 0, Mathf.Tau, 64, new Color(1, 0, 0, 0.5f), 2.0f);
		}
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

		if (EnemiesInRange.Count == 0)
		{
			CurrentTarget = null;
			return;
		}

		CurrentTarget = EnemiesInRange
			.OrderBy(e => e.GlobalPosition.DistanceSquaredTo(GlobalPosition))
			.FirstOrDefault();
	}

	private void RotateTurret(float delta)
	{
		if (TurretSprite != null && CurrentTarget != null)
		{
			Vector2 direction = (CurrentTarget.GlobalPosition - GlobalPosition).Normalized();

			if (IsSideView)
			{
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

		bullet.Setup(Muzzle.GlobalPosition, bulletRotation, CurrentTarget);
		bullet.Damage += (Level - 1) * 5;
	}

	public void Upgrade()
	{
		maxLevel = (BaseTextures != null) ? BaseTextures.Length : 3;

		if (Level >= maxLevel) return;

		int cost = GetUpgradeCost();
		if (Global.Instance.Gold >= cost)
		{
			Global.Instance.Gold -= cost;
			Level++;
			FireRate *= 1.1f;
			Range *= 1.1f;
			if (RangeShape.Shape is CircleShape2D circle) circle.Radius = Range;

			if (_isSelected) QueueRedraw();

			UpdateTowerVisual();
			GD.Print($"Upgraded to Level {Level}");
		}
		else
		{
			// GỌI THÔNG BÁO TẠI ĐÂY
			var textManager = GetNode<FloatingTextManager>("/root/FloatingTextManager");

			// "this.GlobalPosition" chính là vị trí của trụ hiện tại
			textManager.ShowMessage("Không đủ vàng!", GlobalPosition, Colors.Red);

			// Có thể kết hợp phát âm thanh "Error" đã làm ở bước trước
			// SfxManager.Instance.PlaySfx(ErrorSfx);
		}
	}

	private void UpdateTowerVisual()
	{
		int index = Level - 1;

		if (BaseSprite != null && BaseTextures != null && index < BaseTextures.Length && BaseTextures[index] != null)
		{
			BaseSprite.Texture = BaseTextures[index];
		}

		if (TurretSprite != null && TurretTextures != null && index < TurretTextures.Length && TurretTextures[index] != null)
		{
			TurretSprite.Texture = TurretTextures[index];
		}

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
