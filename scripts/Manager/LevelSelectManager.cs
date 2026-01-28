using Godot;
using System;

public partial class LevelSelectManager : Control
{
	// Kéo thả các node từ Scene Tree vào đây hoặc dùng GetNode
	[Export] private ConfirmationDialog _confirmDialog;
	[Export] private TextureButton _resetButton;

	// Không cần export từng nút nếu đặt tên đúng chuẩn trong Scene
	private TextureButton _btnLevel2;
	private TextureButton _btnLevel3;
	private TextureButton _btnLevel4;

	public override void _Ready()
	{
		// Phần khởi tạo nút reset lại tất cả level
		_resetButton.Pressed += OnResetButtonPressed;
		_confirmDialog.Confirmed += OnResetConfirmed;

		// 1. Lấy reference tới các nút theo tên trong Scene
		// Level 1 luôn mở nên không cần xử lý khóa
		_btnLevel2 = GetNodeOrNull<TextureButton>("Level2");
		_btnLevel3 = GetNodeOrNull<TextureButton>("Level3");
		_btnLevel4 = GetNodeOrNull<TextureButton>("Level4");

		// 2. Tải dữ liệu từ ổ cứng (Đây là bước quan trọng để giữ tiến độ khi tắt máy)
		int unlockedLevel = SaveManager.LoadProgress();

		if (Global.Instance != null)
		{
			// Nếu dữ liệu trong file save cao hơn trong Global (do mới bật game), cập nhật Global
			if (unlockedLevel > Global.Instance.UnlockedLevel)
			{
				Global.Instance.UnlockedLevel = unlockedLevel;
			}
		}

		GD.Print($"[LevelSelect] Đang hiển thị menu với cấp độ mở khóa: {unlockedLevel}");

		// 3. Cập nhật giao diện
		UpdateLevelButton(_btnLevel2, 2, unlockedLevel);
		UpdateLevelButton(_btnLevel3, 3, unlockedLevel);
		UpdateLevelButton(_btnLevel4, 4, unlockedLevel);
	}

	private void UpdateLevelButton(TextureButton btn, int levelIndex, int unlockedLevel)
	{
		if (btn == null) return;

		// Tìm node con tên "LockIcon" (đã thêm trong file .tscn)
		Control lockIcon = btn.GetNodeOrNull<Control>("LockIcon");

		if (levelIndex > unlockedLevel)
		{
			// --- TRẠNG THÁI KHÓA ---
			btn.Disabled = true;

			// Làm tối nút đi (Hiệu ứng Disable)
			btn.Modulate = new Color(0.3f, 0.3f, 0.3f, 1);

			// Đổi con trỏ chuột thành cấm
			btn.MouseDefaultCursorShape = CursorShape.Forbidden;

			// Hiện ổ khóa
			if (lockIcon != null) lockIcon.Visible = true;
		}
		else
		{
			// --- TRẠNG THÁI MỞ ---
			btn.Disabled = false;

			// Màu sắc bình thường
			btn.Modulate = new Color(1, 1, 1, 1);

			// Đổi con trỏ chuột thành bàn tay
			btn.MouseDefaultCursorShape = CursorShape.PointingHand;

			// Ẩn ổ khóa
			if (lockIcon != null) lockIcon.Visible = false;
		}
	}

	private void OnResetButtonPressed()
	{
		// Hiển thị dialog ở chính giữa màn hình
		_confirmDialog.PopupCentered();
	}

	private void OnResetConfirmed()
	{
		// Gọi hàm xóa file save đã viết ở SaveManager
		SaveManager.ResetProgress();

		// Cập nhật lại giao diện ngay lập tức
		// Cách nhanh nhất là load lại scene này
		GetTree().ReloadCurrentScene();

		GD.Print("Tiến độ đã được reset!");
	}
}
