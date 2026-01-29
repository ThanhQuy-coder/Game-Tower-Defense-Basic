using Godot;

public partial class SfxToggleButton : TextureButton
{
	[Export] public Texture2D OnTexture { get; set; }
	[Export] public Texture2D OffTexture { get; set; }

	private TextureRect _statusIcon;

	public override void _Ready()
	{
		_statusIcon = GetNodeOrNull<TextureRect>("TextureRect");

		if (_statusIcon == null)
		{
			GD.PrintErr("Lỗi: SfxToggleButton không tìm thấy TextureRect con!");
			return;
		}

		// 1. Dùng Toggled để bắt đúng trạng thái lún/nảy của nút
		this.Toggled += OnTogglePressed;

		// 2. Cập nhật giao diện ngay khi load dựa trên trạng thái nút trong Editor
		UpdateVisuals(this.ButtonPressed);
	}

	private void OnTogglePressed(bool isPressed)
	{
		// isPressed = true nghĩa là nút đang ĐANG LÚN (Thường là trạng thái OFF)
		// isPressed = false nghĩa là nút ĐANG NẢY (Thường là trạng thái ON)

		var manager = GetNodeOrNull<SfxManager>("/root/SfxManager");

		// Nếu nút lún (true) -> Tắt tiếng (false) và ngược lại
		manager?.SetSfxEnabled(!isPressed);

		UpdateVisuals(isPressed);
	}

	private void UpdateVisuals(bool isPressed)
	{
		if (_statusIcon != null)
		{
			// Nếu nút lún (OFF) -> Hiện OffTexture, ngược lại hiện OnTexture
			_statusIcon.Texture = isPressed ? OffTexture : OnTexture;
		}
	}
}
