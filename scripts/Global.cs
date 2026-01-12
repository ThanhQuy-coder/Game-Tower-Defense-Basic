using Godot;
using System;
using System.Threading.Tasks; // Cần thêm thư viện này để dùng Task.Delay

public partial class Global : Node
{
	public static Global Instance { get; private set; }

	[Signal] public delegate void StatsChangedEventHandler();
	[Signal] public delegate void GameOverEventHandler();
	[Signal] public delegate void VictoryEventHandler();

	private int _gold;
	private int _health;
	private int _wave;

	[Export] public int InitialGold = 550;
	[Export] public int InitialHealth = 20;

	public int Gold
	{
		get => _gold;
		set { _gold = value; EmitSignal(SignalName.StatsChanged); }
	}

	public int Health
	{
		get => _health;
		set
		{
			_health = value;
			EmitSignal(SignalName.StatsChanged); // Cập nhật UI ngay lập tức
			
			if (_health <= 0)
			{
				// Gọi hàm xử lý thua với độ trễ
				CallDeferred(nameof(TriggerGameOverDelayed));
			}
		}
	}

	// ... (Giữ nguyên Wave và _Ready) ...
	public int Wave
	{
		get => _wave;
		set { _wave = value; EmitSignal(SignalName.StatsChanged); }
	}

	public override void _Ready()
	{
		Instance = this;
		ProcessMode = ProcessModeEnum.Always;
		StartNewGame();
	}

	public void StartNewGame()
	{
		GetTree().Paused = false; 
		Gold = InitialGold;
		Health = InitialHealth;
		Wave = 1;
	}

	// Hàm xử lý Game Over có độ trễ
	private async void TriggerGameOverDelayed()
	{
		// Chờ 1 khung hình hoặc 0.1s để UI kịp vẽ số 0
		await ToSignal(GetTree().CreateTimer(0.1f), "timeout");

		if (GetTree().Paused) return;
		GD.Print("GAME OVER!");
		EmitSignal(SignalName.GameOver);
		GetTree().Paused = true; 
	}

	public async void TriggerVictory()
	{
		// Cũng thêm độ trễ cho chiến thắng để cảm giác mượt hơn
		await ToSignal(GetTree().CreateTimer(0.2f), "timeout");

		if (GetTree().Paused) return;
		GD.Print("VICTORY!");
		EmitSignal(SignalName.Victory);
		GetTree().Paused = true;
	}

	public void RestartGame()
	{
		GetTree().Paused = false;
		StartNewGame();
		GetTree().ReloadCurrentScene();
	}
}
