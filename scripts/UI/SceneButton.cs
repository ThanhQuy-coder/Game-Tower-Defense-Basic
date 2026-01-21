using Godot;
using System;

/// <summary>
/// Lớp phụ trách việc chuyển các scene
/// </summary>
public partial class SceneButton : TextureButton
{
	// Tạo một danh sách các lựa chọn
	public enum ButtonAction { ChangeScene, QuitGame, Back }

	[Export] public ButtonAction ActionType = ButtonAction.ChangeScene;
	[Export(PropertyHint.File, "*.tscn")] public string TargetScenePath;

	public override void _Ready()
	{
		// Kết sự kiện pressed của chính nó với hàm xử lý
		this.Pressed += OnButtonPressed;

		// Kiểm tra xem chuột có bị chặn không
		if (this.MouseFilter == MouseFilterEnum.Ignore)
		{
			GD.PrintErr("CẢNH BÁO: Node " + Name + " đang để Mouse Filter là Ignore, sẽ không bấm được!");
		}
	}

	private void OnButtonPressed()
	{
		switch (ActionType)
		{
			case ButtonAction.ChangeScene:
				if (!string.IsNullOrEmpty(TargetScenePath))
				{
					GetTree().Paused = false;
					// Lưu scene hiện tại lại
					var current = GetTree().CurrentScene.SceneFilePath;
					SceneHistory.Instance.LastScenePath = current;

					GetTree().ChangeSceneToFile(TargetScenePath);
				}
				else
				{
					GD.PrintErr("Chưa gán đường dẫn Scene cho nút: " + Name);
				}
				break;

			case ButtonAction.QuitGame:
				GetTree().Quit();
				break;

			case ButtonAction.Back:
				if (!string.IsNullOrEmpty(SceneHistory.Instance.LastScenePath))
				{
					GetTree().ChangeSceneToFile(SceneHistory.Instance.LastScenePath);
				}
				else
				{
					GD.PrintErr("Không có scene trước đó để quay lại!");
				}
				break;
		}
	}

}
