using Godot;
using System;

// [TOWER SLOT]
// Đường dẫn: scripts/Tower/TowerSlot.cs
// Chức năng: Ô đất để đặt tháp. Chứa logic Xây, Nâng cấp, Bán.
public partial class TowerSlot : Node2D
{
	[ExportGroup("Tower Setup")]
	// Kéo 3 scene tháp vào đây theo thứ tự: 0=Archer, 1=Cannon, 2=Magic
	[Export] public PackedScene[] TowerScenes; 
	
	[ExportGroup("Interaction")]
	[Export] private Button _slotButton; // Nút trong suốt đè lên ô đất để bắt click

	// Tham chiếu đến tháp đang đặt trên ô này
	public TowerBase CurrentTower { get; private set; }
	public bool IsOccupied => CurrentTower != null;

	public override void _Ready()
	{
		if (_slotButton != null)
		{
			_slotButton.Pressed += OnSlotClicked;
		}
	}

	private void OnSlotClicked()
	{
		if (GameUI.Instance == null)
		{
			GD.PrintErr("Lỗi: Không tìm thấy GameUI Scene! Hãy đảm bảo Scene chứa GameUI đã được load.");
			return;
		}

		// Nếu ô trống -> Gọi UI Xây dựng
		if (!IsOccupied)
		{
			GameUI.Instance.ShowBuildMenu(this, GlobalPosition);
		}
		// Nếu đã có tháp -> Gọi UI Nâng cấp/Bán
		else
		{
			GameUI.Instance.ShowActionMenu(this, GlobalPosition);
		}
	}

	// --- CÁC HÀM LOGIC GAMEPLAY ---

	public void BuildTower(int index)
	{
		if (index < 0 || index >= TowerScenes.Length) return;
		
		// Tạo tạm instance để lấy giá tiền
		var tempTower = TowerScenes[index].Instantiate<TowerBase>();
		int cost = tempTower.BaseCost;
		
		if (Global.Instance.Gold >= cost)
		{
			Global.Instance.Gold -= cost;

			// Chính thức thêm tháp vào scene
			CurrentTower = tempTower; 
			AddChild(CurrentTower);
			CurrentTower.Position = Vector2.Zero; // Đặt vào giữa ô

			GD.Print($"Đã xây tháp loại {index}");
		}
		else
		{
			GD.Print("Không đủ tiền!");
			tempTower.QueueFree(); // Hủy nếu không đủ tiền
		}
	}

	public void UpgradeTower()
	{
		if (IsOccupied)
		{
			CurrentTower.Upgrade();
		}
	}

	public void SellTower()
	{
		if (IsOccupied)
		{
			CurrentTower.Sell();
			CurrentTower = null; // Xóa tham chiếu
		}
	}
}
