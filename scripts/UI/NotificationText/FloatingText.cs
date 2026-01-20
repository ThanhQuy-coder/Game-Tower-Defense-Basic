using Godot;

public partial class FloatingText : Marker2D
{
	public override void _Ready()
	{
		var label = GetNode<Label>("Label");

		var tween = CreateTween().SetParallel(true);

		tween.TweenProperty(this, "position:y", Position.Y - 50, 1.0f);
		tween.TweenProperty(this, "modulate:a", 0.0f, 1.0f);

		tween.Finished += () => QueueFree();
	}

	public void SetText(string text, Color color)
	{
		var label = GetNode<Label>("Label");
		label.Text = text;
		label.Modulate = color;
	}
}
