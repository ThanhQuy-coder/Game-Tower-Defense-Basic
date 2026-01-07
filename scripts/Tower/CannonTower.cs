using Godot;

// [LOẠI 2] Tháp Pháo: Bắn chậm, tầm ngắn, nhưng đạn nổ lan (do BulletScene là CannonBall)
public partial class CannonTower : TowerBase
{
	public override void _Ready()
	{
		Range = 100.0f;
		FireRate = 0.5f; // 2 giây bắn 1 phát
		BaseCost = 120;
		base._Ready();
	}
}
