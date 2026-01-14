using Godot;

// [LOẠI 3] Tháp Phép: Bắn đạn làm chậm
public partial class MagicTower : TowerBase
{
	public override void _Ready()
	{
		Range = 120.0f;
		FireRate = 1.0f;
		BaseCost = 200;
		base._Ready();
	}
}
