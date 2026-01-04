	using Godot;

	public partial class main : Node2D
	{
		[Export] public int StartingGold = 200;
		public int CurrentGold;
		
		public override void _Ready()
		{
			GD.Print("=== MAIN SCENE READY ===");
			CurrentGold = StartingGold;
			UpdateGoldDisplay();
		}
		
		private void UpdateGoldDisplay()
		{
			// Tìm node hiển thị vàng trong HUD
			var goldLabel = GetNodeOrNull<Label>("hud/GoldLabel");
			if (goldLabel != null)
			{
				goldLabel.Text = $"Gold: {CurrentGold}";
			}
		}
		
		public override void _Input(InputEvent @event)
		{
			// Chỉ xử lý phím để test
			if (@event is InputEventKey key && key.Pressed)
			{
				// PHÍM G: +100 VÀNG (TEST)
				if (key.Keycode == Key.G)
				{
					CurrentGold += 100;
					UpdateGoldDisplay();
					GD.Print($"+100 vàng! Tổng: {CurrentGold}");
				}
				
				// PHÍM T: TEST TẠO TOWER TRỰC TIẾP (bỏ qua TowerSpot)
				if (key.Keycode == Key.T)
				{
					var towerScene = GD.Load<PackedScene>("res://scenes/actors/tower-backup.tscn");
					if (towerScene != null)
					{
						var tower = towerScene.Instantiate<Node2D>();
						tower.Position = GetGlobalMousePosition();
						AddChild(tower);
						GD.Print("✓ Đã tạo tower tại vị trí chuột");
					}
				}
			}
		}
		
		public bool CanAffordTower(int cost)
		{
			return CurrentGold >= cost;
		}
		
		public void SpendGold(int amount)
		{
			if (amount <= CurrentGold)
			{
				CurrentGold -= amount;
				UpdateGoldDisplay();
			}
		}
		
		public void AddGold(int amount)
		{
			CurrentGold += amount;
			UpdateGoldDisplay();
		}
	}
