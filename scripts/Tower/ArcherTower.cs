using Godot;

/// <summary>
/// Tháp cung:
/// - Tốc độ bắn: nhanh
/// - Tầm bắn: xa
/// - giá: rẻ
/// </summary>
public partial class ArcherTower : TowerBase
{
	// Dùng mảng để tuân thủ Open/Closed: Muốn thêm Level chỉ cần thêm phần tử vào mảng
	private readonly int[] _upgradeCosts = { 0, 75, 100 }; // Giá lên Lv2, Lv3, Lv4...
	private readonly int[] _damageBonus = { 10, 15, 20 };
	private readonly float[] _fireRate = { 0, 1.15f, 1.3f };

	public override void _Ready()
	{
		Range = 100.0f;
		FireRate = 1.0f;
		BaseCost = 50;
		base._Ready();
	}

	protected override void ApplyUpgradeStats()
	{
		// Lấy bonus damage từ mảng dữ liệu (Data-driven)
		if (Level <= _damageBonus.Length)
		{
			base.ApplyUpgradeStats();
			Damage += _damageBonus[Level - 1];
			FireRate = _fireRate[Level - 1];
		}

		UpdateRangeCircle();
	}

	protected override void Shoot()
	{
		if (BulletScene == null || Muzzle == null || CurrentTarget == null) return;

		var arrow = BulletScene.Instantiate<BulletBase>();
		GetTree().Root.AddChild(arrow);

		// Gọi hàm từ lớp cha, code lớp con sẽ cực sạch
		arrow.Setup(Muzzle.GlobalPosition, GetBulletRotation(), this.Damage, CurrentTarget);
	}

	public override int GetUpgradeCost()
	{
		// Nếu Level 1, trả về giá nâng lên Level 2
		if (Level < _upgradeCosts.Length)
			return _upgradeCosts[Level];
		return 999999; // Hoặc giá trị báo hiệu Max Level
	}
}
