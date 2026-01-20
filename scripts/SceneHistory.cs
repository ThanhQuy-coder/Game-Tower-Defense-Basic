using Godot;

public partial class SceneHistory : Node
{
	public static SceneHistory Instance;   // ⚡ phải có biến tĩnh Instance

	public string LastScenePath = "";      // Lưu scene trước đó

	public override void _Ready()
	{
		Instance = this;                   // ⚡ gán Instance khi Autoload được tạo
	}
}
