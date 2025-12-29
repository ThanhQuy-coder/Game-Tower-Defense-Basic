using Godot;

public partial class Global : Node
{
	private static Global _instance;
	public static Global Instance { get { return _instance; } }
	
	private int _gold = 100;
	private int _health = 10;
	
	public event System.Action<int> GoldChanged;
	public event System.Action<int> HealthChanged;
	
	public int Gold
	{
		get => _gold;
		set
		{
			_gold = value;
			GoldChanged?.Invoke(_gold);
		}
	}
	
	public int Health
	{
		get => _health;
		set
		{
			_health = value;
			HealthChanged?.Invoke(_health);
		}
	}
	
	public override void _EnterTree()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			QueueFree();
		}
	}
}
