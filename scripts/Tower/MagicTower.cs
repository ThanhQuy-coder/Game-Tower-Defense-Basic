using Godot;

// [LOẠI 3] Tháp Phép: Bắn đạn làm chậm
public partial class MagicTower : TowerBase
{
	public override void _Ready()
	{
		Range = 200.0f;
		FireRate = 1.0f;
		BaseCost = 150;
		base._Ready();
	}
}
