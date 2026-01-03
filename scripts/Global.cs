using Godot;
using System;

// [Người 3]
// Singleton quản lý trạng thái toàn cục của game.
// Sử dụng C# Events để các thành phần khác (UI, Game Logic) đăng ký lắng nghe thay vì gọi trực tiếp.
public partial class Global : Node
{
	// Thread-safe Singleton
	private static Global _instance;
	public static Global Instance => _instance;

	// Game Settings & State
	private int _gold = 100;
	private int _health = 20;
	private int _wave = 1;
	private bool _isGameOver = false;

	// Events - Observer Pattern
	public event Action<int> OnGoldChanged;
	public event Action<int> OnHealthChanged;
	public event Action<int> OnWaveChanged;
	public event Action OnGameOver;

	public int Gold
	{
		get => _gold;
		set
		{
			_gold = value;
			OnGoldChanged?.Invoke(_gold); // Bắn tín hiệu khi tiền thay đổi
		}
	}

	public int Health
	{
		get => _health;
		set
		{
			_health = value;
			OnHealthChanged?.Invoke(_health);
			
			if (_health <= 0 && !_isGameOver)
			{
				_health = 0;
				_isGameOver = true;
				OnGameOver?.Invoke();
				GD.Print("GAME OVER");
				// Có thể gọi GetTree().Paused = true; tại đây
			}
		}
	}

	public int Wave
	{
		get => _wave;
		set
		{
			_wave = value;
			OnWaveChanged?.Invoke(_wave);
		}
	}

	public override void _EnterTree()
	{
		if (_instance != null)
		{
			QueueFree();
			return;
		}
		_instance = this;
	}

	// Helper để spawn object tại root scene (tránh bị xoay theo cha)
	public void SpawnActor(Node actor, Vector2 globalPosition)
	{
		GetTree().Root.AddChild(actor);
		if (actor is Node2D node2d)
		{
			node2d.GlobalPosition = globalPosition;
		}
	}
}
