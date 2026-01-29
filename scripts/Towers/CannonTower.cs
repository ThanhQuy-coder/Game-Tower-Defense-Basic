using Godot;

/// <summary>
/// Tháp pháo:
/// - Tầm bắn ngắn
/// - sát thương lớn
/// - giá trung bình
/// </summary>
public partial class CannonTower : TowerBase
{
	private readonly int[] _upgradeCosts = { 0, 120, 150 };
	private readonly int[] _damageBonus = { 0, 20, 25 };
	private readonly float[] _fireRate = { 0, 0.66f, 0.76f };


	public override void _Ready()
	{
		Range = 90.0f;
		FireRate = 0.5f;
		BaseCost = 100;
		base._Ready();
	}

	protected override void ApplyUpgradeStats()
	{
		// Lấy bonus damage từ mảng dữ liệu (Data-driven)
		if (Level <= _damageBonus.Length)
		{
			Damage += _damageBonus[Level - 1];
			FireRate = _fireRate[Level - 1];
			base.ApplyUpgradeStats();
		}

		UpdateRangeCircle();
	}

	protected override void Shoot()
	{
		if (BulletScene == null || Muzzle == null || CurrentTarget == null) return;

		var cannon = BulletScene.Instantiate<BulletBase>();
		GetTree().Root.AddChild(cannon);

		cannon.Setup(Muzzle.GlobalPosition, GetBulletRotation(), this.Damage, CurrentTarget);
	}

	public override int GetUpgradeCost()
	{
		// Nếu Level 1, trả về giá nâng lên Level 2
		if (Level < _upgradeCosts.Length)
			return _upgradeCosts[Level];
		return 999999;
	}
}
