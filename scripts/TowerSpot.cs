using Godot;

public partial class TowerSpot : Area2D
{
	[Export] public PackedScene TowerScene;
	[Export] public int TowerCost = 50;
	
	private bool _hasTower = false;
	private ColorRect _visual;
	private main _gameMain;
	
	public override void _Ready()
	{
		// TÌM CÁC NODE
		_visual = GetNodeOrNull<ColorRect>("ColorRect");
		if (_visual != null)
		{
			_visual.Color = new Color(1, 0, 0, 0.7f);
		}
		
		// ĐẢM BẢO NHẬN INPUT
		InputPickable = true;
		
		// TÌM MAIN NODE
		FindMainNode();
		
		// KẾT NỐI SỰ KIỆN
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		InputEvent += OnInputEvent;
		
		// KÍCH HOẠT _Input
		SetProcessInput(true);
	}
	
	private void FindMainNode()
	{
		_gameMain = GetTree().Root.GetNodeOrNull<main>("Main") ??
				   GetTree().Root.GetNodeOrNull<main>("Node2D") ??
				   GetParent() as main;
	}
	
	private void OnMouseEntered()
	{
		if (!_hasTower && _visual != null)
		{
			_visual.Color = new Color(0, 1, 0, 0.7f);
		}
	}
	
	private void OnMouseExited()
	{
		if (!_hasTower && _visual != null)
		{
			_visual.Color = new Color(1, 0, 0, 0.7f);
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && 
			mouseEvent.Pressed && 
			mouseEvent.ButtonIndex == MouseButton.Left)
		{
			// Kiểm tra click có trong area không
			var mousePos = GetGlobalMousePosition();
			var query = new PhysicsPointQueryParameters2D();
			query.Position = mousePos;
			query.CollideWithAreas = true;
			query.CollideWithBodies = false;
			
			var spaceState = GetWorld2D().DirectSpaceState;
			var results = spaceState.IntersectPoint(query);
			
			foreach (var result in results)
			{
				var collider = result["collider"].As<Node>();
				if (collider == this && !_hasTower)
				{
					PlaceTower();
					break;
				}
			}
		}
	}
	
	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && 
			mouseEvent.Pressed && 
			mouseEvent.ButtonIndex == MouseButton.Left && 
			!_hasTower)
		{
			PlaceTower();
		}
	}
	
	private void PlaceTower()
	{
		if (_hasTower) return;
		
		// KIỂM TRA TOWERSCENE
		if (TowerScene == null)
		{
			GD.Print("❌ Chưa gán TowerScene");
			return;
		}
		
		// KIỂM TRA VÀNG
		if (_gameMain != null)
		{
			if (!_gameMain.CanAffordTower(TowerCost))
			{
				GD.Print($"❌ Không đủ vàng: {TowerCost}");
				return;
			}
		}
		
		try
		{
			// TRỪ VÀNG
			if (_gameMain != null)
			{
				_gameMain.SpendGold(TowerCost);
			}
			
			// TẠO TOWER
			var tower = TowerScene.Instantiate<Node2D>();
			AddChild(tower);
			tower.Position = Vector2.Zero;
			
			// ĐÁNH DẤU VÀ ẨN
			_hasTower = true;
			if (_visual != null)
			{
				_visual.Visible = false;
			}
			
			GD.Print("✅ Đã đặt tower");
		}
		catch (System.Exception e)
		{
			GD.PrintErr($"Lỗi tạo tower: {e.Message}");
		}
	}
}
