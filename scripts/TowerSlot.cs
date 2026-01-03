using Godot;
using System;

// [Người 1] - Logic ô đặt tháp (Dùng Button)
public partial class TowerSlot : Node2D
{
	[Export] public int TowerCost = 50; 
	[Export] public PackedScene TowerScene;

	private bool _isOccupied = false;
	private Sprite2D _sprite;
	private Button _clickButton;

	public override void _Ready()
	{
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		
		// Tìm cái nút Button mà bạn vừa thêm vào
		_clickButton = GetNodeOrNull<Button>("Button");

		if (_clickButton != null)
		{
			// Kết nối sự kiện bấm nút với hàm OnButtonPressed
			_clickButton.Pressed += OnButtonPressed;
			GD.Print($"[SYSTEM] Ô {Name} đã kết nối với Nút bấm thành công!");
		}
		else
		{
			GD.PrintErr($"[LỖI] Không tìm thấy node 'Button' trong {Name}. Bạn đã thêm nó vào Scene chưa?");
		}
	}

	// Hàm này chạy khi Nút bị bấm
	private void OnButtonPressed()
	{
		GD.Print($"[CLICK] Đã bấm vào nút của ô: {Name}");
		BuildTower();
	}

	private void BuildTower()
	{
		if (Global.Instance == null) { GD.PrintErr("Lỗi Global chưa chạy!"); return; }
		
		if (_isOccupied) 
		{
			GD.Print("Ô này đã có người ở!");
			return;
		}

		if (Global.Instance.Gold >= TowerCost)
		{
			if (TowerScene != null)
			{
				Global.Instance.Gold -= TowerCost;
				
				// Tạo tháp
				var tower = TowerScene.Instantiate<Node2D>();
				AddChild(tower);
				
				// Tắt nút bấm đi để không bấm lại được nữa (tránh lỗi xây chồng)
				_isOccupied = true;
				if (_clickButton != null) _clickButton.Visible = false; // Ẩn nút đi
				
				GD.Print("Xây xong!");
			}
			else GD.PrintErr($"Chưa gán TowerScene cho {Name}");
		}
		else 
		{
			GD.Print($"Không đủ tiền! Có: {Global.Instance.Gold}, Cần: {TowerCost}");
		}
	}
}
