using Godot;
using System;

public partial class LevelSelectManager : Control
{
	// Không cần export từng nút nếu đặt tên đúng chuẩn trong Scene
	private TextureButton _btnLevel2;
	private TextureButton _btnLevel3;
	private TextureButton _btnLevel4;

	public override void _Ready()
	{
		// Lấy reference tới các nút theo tên trong Scene
		// Level 1 luôn mở nên không cần xử lý khóa
		_btnLevel2 = GetNodeOrNull<TextureButton>("Level2");
		_btnLevel3 = GetNodeOrNull<TextureButton>("Level3");
		_btnLevel4 = GetNodeOrNull<TextureButton>("Level4");

		// Lấy dữ liệu từ Global (Level cao nhất đã mở)
		int unlockedLevel = 1;
		if (Global.Instance != null)
		{
			unlockedLevel = Global.Instance.UnlockedLevel;
		}

		// Cập nhật trạng thái cho từng nút
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
}
