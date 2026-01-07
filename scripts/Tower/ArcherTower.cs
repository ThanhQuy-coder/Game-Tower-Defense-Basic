using Godot;

// [LOẠI 1] Tháp Cung: Bắn tên, tốc độ khá, tầm xa trung bình
public partial class ArcherTower : TowerBase
{
	public override void _Ready()
	{
		Range = 100.0f;
		FireRate = 1.5f;
		BaseCost = 50;
		base._Ready();
	}
	
	// Sử dụng logic Shoot mặc định của TowerBase (Instantiate BulletScene)
}
