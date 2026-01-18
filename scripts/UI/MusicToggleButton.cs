using Godot;

public partial class MusicToggleButton : TextureButton
{
	// Sử dụng Texture2D để chấp nhận mọi loại texture (Atlas, Image, v.v.)
	[Export] public Texture2D OnTexture { get; set; }
	[Export] public Texture2D OffTexture { get; set; }

	private TextureRect _statusIcon;
	private bool _isMusicOn = true;

	public override void _Ready()
	{
		// Kiểm tra Node con để tránh crash (Robustness)
		_statusIcon = GetNodeOrNull<TextureRect>("TextureRect");

		if (_statusIcon == null)
		{
			GD.PrintErr("Lỗi: Không tìm thấy TextureRect con!");
			return;
		}

		this.Pressed += OnTogglePressed;
		UpdateVisuals();
	}

	private void OnTogglePressed()
	{
		_isMusicOn = !_isMusicOn;

		// Gọi đến Singleton MusicManager đã tạo ở bước trước
		var musicManager = GetNodeOrNull<MusicManager>("/root/MusicManager");
		musicManager?.SetMusicEnabled(_isMusicOn);

		UpdateVisuals();
	}

	private void UpdateVisuals()
	{
		if (_statusIcon != null)
		{
			_statusIcon.Texture = _isMusicOn ? OnTexture : OffTexture;
		}
	}
}
