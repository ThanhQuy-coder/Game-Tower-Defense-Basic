//using Godot;
//
//public partial class main : Node2D
//{
	//public override void _Input(InputEvent @event)
	//{
		//if (@event is InputEventMouseButton mouse)
		//{
			//if (mouse.ButtonIndex == MouseButton.Left && mouse.Pressed)
			//{
				//Vector2 mousePos = GetGlobalMousePosition();
				//
				//if (Global.Instance.Gold >= 50)
				//{
					//// TRỪ VÀNG
					//Global.Instance.Gold -= 50;
					//
					//// Tạo tháp
					//CreateTower(mousePos);
				//}
			//}
		//}
	//}
	//
	//private void CreateTower(Vector2 position)
	//{
		//// Tạo tháp đơn giản (hoặc load từ scene)
		//Sprite2D tower = new Sprite2D();
		//tower.Texture = GD.Load<Texture2D>("res://icon.svg");
		//tower.Scale = new Vector2(0.4f, 0.4f);
		//tower.Modulate = Colors.Green;
		//tower.Position = position;
		//AddChild(tower);
	//}
	//
	//public override void _UnhandledInput(InputEvent @event)
	//{
		//if (@event is InputEventKey key && key.Pressed)
		//{
			//if (key.Keycode == Key.G)
			//{
				//Global.Instance.Gold += 100;
			//}
		//}
	//}
//}
