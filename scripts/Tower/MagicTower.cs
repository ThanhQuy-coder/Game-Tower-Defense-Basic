using Godot;

// Tháp Phép: Bắn đạn làm chậm
public partial class MagicTower : TowerBase
{
	private readonly int[] _upgradeCosts = { 0, 150, 200 };
	private readonly int[] _damageBonus = { 0, 22, 28 };
	private readonly float[] _effectSlow = { 0.2f, 0.25f, 0.3f };
	private readonly float[] _duration = { 1, 1.5f, 2 };
	private readonly float[] _fireRate = { 0, 0.88f, 0.968f };


	public override void _Ready()
	{
		Range = 100.0f;
		FireRate = 0.8f;
		BaseCost = 120;
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

		var magic = BulletScene.Instantiate<BulletBase>();
		GetTree().Root.AddChild(magic);

		if (magic is MagicMissile Magic)
		{
			// Tính toán hiệu ứng dựa theo Level của Tower
			float currentSlow = _effectSlow[Level - 1];
			float currentDuration = _duration[Level - 1];

			Magic.SetupMagic(currentSlow, currentDuration);
		}

		magic.Setup(Muzzle.GlobalPosition, GetBulletRotation(), this.Damage, CurrentTarget);
	}

	public override int GetUpgradeCost()
	{
		// Nếu Level 1, trả về giá nâng lên Level 2
		if (Level < _upgradeCosts.Length)
			return _upgradeCosts[Level];
		return 999999;
	}
}
