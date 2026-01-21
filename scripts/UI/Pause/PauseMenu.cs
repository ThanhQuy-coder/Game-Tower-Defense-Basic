using Godot;
using System;

public partial class PauseMenu : Control
{
	// Các tín hiệu để các thành phần khác (GameManager) lắng nghe
	[Signal] public delegate void RestartRequestedEventHandler();
	[Signal] public delegate void ExitToTitleRequestedEventHandler();

	public override void _Ready()
	{
		Hide(); // Mặc định ẩn popup

		// Giả sử bạn đã có Singleton Audio hoặc Global Settings
		// _musicBtn.ButtonPressed = AudioServer.IsBusMute(MusicBusIndex);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel")) // Phím Esc mặc định
		{
			TogglePause();
		}
	}

	public void TogglePause()
	{
		bool isPaused = !GetTree().Paused;
		GetTree().Paused = isPaused;
		Visible = isPaused;

		// Khi thoát menu, trả lại quyền điều khiển chuột
		if (isPaused)
			Input.MouseMode = Input.MouseModeEnum.Visible;
		else
			Input.MouseMode = Input.MouseModeEnum.Visible; // Hoặc Captured tùy game của bạn
	}

	// --- Xử lý sự kiện Button ---

	private void OnResumePressed() => TogglePause();

	private void OnRestartPressed()
	{
		EmitSignal(SignalName.RestartRequested);
	}

	private void OnExitToTitlePressed()
	{
		// Đảm bảo trả lại trạng thái chạy bình thường cho toàn bộ SceneTree
		GetTree().Paused = false;

		// Trả lại chuột về trạng thái bình thường
		Input.MouseMode = Input.MouseModeEnum.Visible;

		// Phát tín hiệu thoát
		EmitSignal(SignalName.ExitToTitleRequested);
	}
}
