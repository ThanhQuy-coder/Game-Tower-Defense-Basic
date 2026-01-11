using Godot;
using System;

public partial class SceneButton : TextureButton
{
	// Tạo một danh sách các lựa chọn
	public enum ButtonAction { ChangeScene, QuitGame }

	[Export] public ButtonAction ActionType = ButtonAction.ChangeScene;
	[Export(PropertyHint.File, "*.tscn")] public string TargetScenePath;

	public override void _Ready()
	{
		if (this is TextureButton btn)
		{
			GD.Print("Đây là TextureButton: " + Name);
			btn.Pressed += () => GD.Print("Signal Pressed hoạt động!");
		}

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
		}
	}
}
