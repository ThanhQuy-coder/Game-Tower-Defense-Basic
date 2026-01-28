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
	[Export] public int Damage = 0;

	[ExportCategory("Visuals")]
	[Export] public Texture2D[] BaseTextures;   // Ảnh tháp
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

	public static TowerBase SelectedTower { get; private set; } // Biến static để lưu tháp đang được chọn duy nhất trên toàn bản đồ
	private bool _isSelected = false; // biến lính canh xem tháp được chọn hay không
	public int Level { get; private set; } = 1; // Cấp độ của tháp
	public int maxLevel { get; private set; } = 3;

	public override void _Ready()
	{
		// Đưa lính lên lớp trên cùng để không bị đế tháp che
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
	/// Dùng _Input để bắt sự kiện chọn tháp (Ưu tiên cao nhất)
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

				// 2. Chọn tháp (Hiện tầm bắn)
				if (!_isSelected)
				{
					_isSelected = true;
					SelectedTower = this; // Gán static
					QueueRedraw();
				}
			}
		}
	}

	/// <summary>
	/// Dùng _UnhandledInput để xử lý khi click vào VÙNG TRỐNG
	/// </summary>
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
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

		// Chỉ tìm mục tiêu mới nếu mục tiêu hiện tại không còn hợp lệ
		if (CurrentTarget == null || !IsInstanceValid(CurrentTarget) || !EnemiesInRange.Contains(CurrentTarget))
		{
			FindTarget();
		}

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

		// CurrentTarget = EnemiesInRange
		// 	.OrderBy(e => e.GlobalPosition.DistanceSquaredTo(GlobalPosition))
		// 	.FirstOrDefault();

		CurrentTarget = EnemiesInRange
		.OrderByDescending(e => e.DistanceTravelled)
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

	/// <summary>
	/// Chức năng bắn - kích hoạt việc tạo đạn bắn tới kẻ địch
	/// </summary>
	protected virtual void Shoot()
	{
		//
		// Để tháp kế thừa tự viết sát thương, hiệu ứng riêng
		//
	}

	/// <summary>
	/// Hàm xử lý việc nâng cấp trụ
	/// </summary>
	public void Upgrade()
	{
		var textManager = GetNode<FloatingTextManager>("/root/FloatingTextManager");
		maxLevel = (BaseTextures != null) ? BaseTextures.Length : 3;

		if (Level >= maxLevel)
		{
			textManager.ShowMessage("Trụ đã nâng cấp tối đa", GlobalPosition, Colors.Red);
			return;
		}

		int cost = GetUpgradeCost();
		if (Global.Instance.Gold >= cost)
		{
			Global.Instance.Gold -= cost;
			Level++;

			// Từng loại trụ nâng cấp từng thuộc tính khác nhau
			ApplyUpgradeStats();

			if (_isSelected) QueueRedraw();

			UpdateTowerVisual();
			GD.Print($"Upgraded to Level {Level}");
		}
		else
		{
			// Gọi thông báo không đủ vàng
			textManager.ShowMessage("Không đủ vàng!", GlobalPosition, Colors.Red);
		}
	}

	protected float GetBulletRotation()
	{
		if (IsSideView && CurrentTarget != null)
			return (CurrentTarget.GlobalPosition - Muzzle.GlobalPosition).Angle();
		return (TurretSprite != null) ? TurretSprite.GlobalRotation : 0f;
	}

	/// <summary>
	/// Hàm sử dụng để cho tháp kế thừa tự điều chỉnh phần thuộc tính riêng khi lên cấp
	/// </summary>
	/// <param name="levelCurrent">Level hiện tại của tháp dùng để linh hoạt thay đổi thuộc tính nâng cấp</param>
	protected virtual void ApplyUpgradeStats()
	{
		// Công thức tăng trưởng chung (Tăng 10% mỗi cấp)
		Range *= 1.1f;
	}

	protected void UpdateRangeCircle()
	{
		if (RangeShape.Shape is CircleShape2D circle) circle.Radius = Range;
	}

	/// <summary>
	/// Cập nhật giao diện của trụ
	/// </summary>
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

	/// <summary>
	/// Hàm lấy giá tiền cần để nâng cấp
	/// </summary>
	/// <returns>giá tiền</returns>
	public virtual int GetUpgradeCost() => BaseCost / 2 * Level;

	private void OnEnemyEnter(Area2D area) { if (area is EnemyBase enemy) EnemiesInRange.Add(enemy); }
	private void OnEnemyExit(Area2D area) { if (area is EnemyBase enemy) EnemiesInRange.Remove(enemy); }
}
