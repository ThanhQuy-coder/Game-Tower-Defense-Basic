using Godot;
using System;

public partial class LevelBase : Node2D
{
	[Export] public AudioStream BackgroundMusic;
	[Export] protected GameUI _gameUI; // Chuyển sang protected để lớp con có thể truy cập nếu cần
	[Export] protected PauseMenu _pauseMenu;

	public override void _Ready()
	{
		// 1. Xử lý nhạc nền chung
		var music = GetNodeOrNull<MusicManager>("/root/MusicManager");
		if (music != null && BackgroundMusic != null)
		{
			music.PlayMusic(BackgroundMusic);
		}

		// Kiểm tra an toàn để tránh lỗi NullReference
		if (_gameUI == null || _pauseMenu == null)
		{
			GD.PrintErr($"LỖI: {Name} chưa kéo thả GameUI hoặc PauseMenu vào Inspector!");
			return;
		}

		// 2. Kết nối các Signal chung
		_gameUI.TogglePauseRequested += OnTogglePauseRequested;
		_pauseMenu.RestartRequested += OnRestartLevel;
		_pauseMenu.ExitToTitleRequested += OnExitToTitle;

		// Hàm này để các Level con có thể viết thêm logic riêng mà không cần ghi đè _Ready
		InitLevel();
	}

	// Virtual giúp Level 1,2,3 có thể viết code riêng tại đây
	protected virtual void InitLevel() { }

	private void OnTogglePauseRequested()
	{
		_pauseMenu.TogglePause();
	}

	protected virtual void OnRestartLevel()
	{
		Global.Instance.RestartGame();
	}

	protected virtual void OnExitToTitle()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/ui/select_screen.tscn");
	}
}