using Godot;

/// <summary>
/// Thành phần xử lý âm thanh cho Button. 
/// Gắn vào dưới bất kỳ Node nào kế thừa từ BaseButton để tự động kích hoạt âm thanh.
/// </summary>
public partial class ButtonSfxComponent : Node
{
	// --- Cấu hình Resources ---
	[Export] public AudioStream ClickSfx;   // Âm thanh khi nhấn nút
	[Export] public AudioStream HoverSfx;   // Âm thanh khi di chuột qua

	// --- Cấu hình Logic ---
	[Export] public bool UseRandomPitch = true; // Bật/Tắt biến đổi cao độ ngẫu nhiên

	private BaseButton _parentButton;

	public override void _Ready()
	{
		// Lấy Node cha và ép kiểu sang BaseButton (như Button, TextureButton, v.v.)
		_parentButton = GetParent<BaseButton>();

		if (_parentButton != null)
		{
			// Đăng ký sự kiện từ Button vào các hàm xử lý nội bộ
			_parentButton.Pressed += OnButtonPressed;
			_parentButton.MouseEntered += OnMouseEntered;
		}
	}

	/// <summary>
	/// Handler xử lý khi nút được nhấn
	/// </summary>
	private void OnButtonPressed()
	{
		Play(ClickSfx);
	}

	/// <summary>
	/// Handler xử lý khi chuột đi vào vùng diện tích của nút
	/// </summary>
	private void OnMouseEntered()
	{
		Play(HoverSfx);
	}

	/// <summary>
	/// Logic trung tâm để gửi yêu cầu phát âm thanh tới SfxManager
	/// </summary>
	/// <param name="stream">File âm thanh cần phát</param>
	private void Play(AudioStream stream)
	{
		// Kiểm tra an toàn: Nếu không có file âm thanh thì thoát
		if (stream == null) return;

		// Truy cập Singleton quản lý âm thanh hiệu ứng qua đường dẫn Autoload
		var sfxSystem = GetNode<SfxManager>("/root/SfxManager");

		// Tính toán cao độ (Pitch): Giúp âm thanh tự nhiên hơn khi lặp lại nhiều lần
		float pitch = UseRandomPitch ? (float)GD.RandRange(0.9, 1.1) : 1.0f;

		// Giao việc phát âm thanh cho SfxManager xử lý (SOLID: Single Responsibility)
		sfxSystem.PlaySfx(stream, pitch);
	}
}
