using Godot;

public partial class TowerBackup : Area2D
{
	[Export] private PackedScene bulletScene;
	[Export] private float fireRate = 1.0f;
	[Export] private int attackDamage = 2;
	[Export] private float attackRange = 300.0f;
	
	private Timer fireTimer;
	private EnemyAizen currentTarget;
	
	public override void _Ready()
	{
		var detectionArea = GetNode<Area2D>("Sprite2D/Area2D");
		var rangeCollider = GetNode<CollisionShape2D>("Sprite2D/Area2D/CollisionShape2D");
		fireTimer = GetNode<Timer>("Timer");
		
		if (rangeCollider.Shape is CircleShape2D circleShape)
		{
			circleShape.Radius = attackRange;
		}
		
		detectionArea.AreaEntered += OnEnemyEntered;
		detectionArea.AreaExited += OnEnemyExited;
		
		fireTimer.WaitTime = fireRate;
		fireTimer.Timeout += OnFireTimerTimeout;
		fireTimer.Start();
	}
	
	private void OnEnemyEntered(Area2D area)
	{
		if (area.GetParent() is EnemyAizen enemy && currentTarget == null)
		{
			currentTarget = enemy;
		}
	}
	
	private void OnEnemyExited(Area2D area)
	{
		if (area.GetParent() is EnemyAizen enemy && enemy == currentTarget)
		{
			currentTarget = null;
		}
	}
	
	private void OnFireTimerTimeout()
	{
		if (currentTarget != null && bulletScene != null)
		{
			FireAtTarget(currentTarget);
		}
	}
	
	private void FireAtTarget(EnemyAizen target)
	{
		var bullet = bulletScene.Instantiate<Bullet>();
		bullet.GlobalPosition = this.GlobalPosition;
		
		var direction = (target.GlobalPosition - this.GlobalPosition).Normalized();
		bullet.Rotation = direction.Angle();
		
		GetTree().Root.AddChild(bullet);
	}
}
