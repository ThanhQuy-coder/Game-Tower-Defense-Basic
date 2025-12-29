using Godot;

public partial class EnemyPathFollow : PathFollow2D
{
	[Export] public float Speed = 100f;

	public override void _Process(double delta)
	{
		Progress += Speed * (float)delta;
	}
}
