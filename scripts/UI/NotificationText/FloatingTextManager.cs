using Godot;

public partial class FloatingTextManager : Node
{
	private PackedScene _floatingTextScene;

	public override void _Ready()
	{
		// Tự load scene khi khởi động game
		_floatingTextScene = GD.Load<PackedScene>("res://scenes/ui/components/FloatingText.tscn");

		if (_floatingTextScene == null)
			GD.PrintErr("Không load được FloatingText.tscn. Kiểm tra lại đường dẫn!");
	}

	/// <summary>
	/// Hiển thị tin nhắn đỏ nổi lên
	/// </summary>
	/// <param name="message">nội dung tin nhắn</param>
	/// <param name="globalPos">vị trí nổi lên</param>
	/// <param name="color">màu sắc</param>
	public void ShowMessage(string message, Vector2 globalPos, Color color)
	{
		if (_floatingTextScene == null)
		{
			GD.PrintErr("FloatingTextScene NULL!");
			return;
		}

		var textNode = _floatingTextScene.Instantiate<FloatingText>();
		GetTree().Root.AddChild(textNode);

		textNode.GlobalPosition = globalPos;
		textNode.SetText(message, color);
	}
}
