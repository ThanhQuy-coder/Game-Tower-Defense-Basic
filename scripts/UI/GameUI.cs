using Godot;
using System;

public partial class GameUI : Control
{
	public static GameUI Instance { get; private set; }

	[ExportGroup("Panels")]
	[Export] public Control BuildPanel;
	[Export] public Control ActionPanel;
	[Export] public Control ResultPanel;

	[ExportGroup("Stats Labels")]
	[Export] public Label ResultLabel;
	[Export] public Label GoldLabel;
	[Export] public Label LivesLabel;
	[Export] public Label WaveLabel;
	[Export] public Label WaveTimerLabel;
	[Export] public Label TowerInfoLabel;

	// Label thông báo giữa màn hình
	[Export] public Label CenterNotificationLabel;

	// Nút bấm
	[Export] public Button UpgradeButton;

	// [MỚI] Thêm reference tới 2 nút mới
	[Export] public Button BtnNextLevel;
	[Export] public Button BtnMenu;

	private TowerSlot _selectedSlot;

	public override void _Ready()
	{
		Instance = this;
		HideAllPanels();
		ResultPanel.Visible = false;

		// Ẩn thông báo ban đầu
		if (CenterNotificationLabel != null)
			CenterNotificationLabel.Modulate = new Color(1, 1, 1, 0);

		if (Global.Instance != null)
		{
			Global.Instance.GameOver += ShowGameOver;
			Global.Instance.Victory += ShowVictory;
		}
	}

	public override void _ExitTree()
	{
		if (Global.Instance != null)
		{
			Global.Instance.GameOver -= ShowGameOver;
			Global.Instance.Victory -= ShowVictory;
		}
	}

	public override void _Process(double delta)
	{
		if (Global.Instance != null)
		{
			GoldLabel.Text = $"💰 Gold: {Global.Instance.Gold}";
			LivesLabel.Text = $"❤️ Lives: {Global.Instance.Health}";
			WaveLabel.Text = $"🌊 Wave: {Global.Instance.Wave}";
		}

		if (WaveManager.Instance != null && WaveTimerLabel != null)
		{
			float timeLeft = WaveManager.Instance.TimeToNextWave;

			// Nếu đang đếm ngược
			if (timeLeft > 0)
			{
				WaveTimerLabel.Visible = true;
				WaveTimerLabel.Text = $"Next Wave: {Mathf.Ceil(timeLeft)}s";
				WaveTimerLabel.Modulate = Colors.Yellow;
			}
			// Nếu hết thời gian đếm ngược (Đang trong trận đấu)
			else
			{
				// Bạn có thể chọn hiện chữ COMBAT! hoặc ẩn luôn Label đi cho đỡ rối
				WaveTimerLabel.Visible = true;
				WaveTimerLabel.Text = "COMBAT!";
				WaveTimerLabel.Modulate = Colors.Red;

				// Hoặc nếu muốn ẩn đi thì dùng dòng dưới:
				// WaveTimerLabel.Visible = false;
			}
		}
	}

	// [MỚI] Hàm hiển thị thông báo Wave chuyên nghiệp
	public void ShowWaveNotification(int waveIndex)
	{
		if (CenterNotificationLabel == null) return;

		CenterNotificationLabel.Text = $"WAVE {waveIndex}";
		CenterNotificationLabel.Visible = true;

		// Tạo hiệu ứng Fade In -> Wait -> Fade Out dùng Tween
		var tween = CreateTween();
		// 1. Reset về trong suốt
		tween.TweenProperty(CenterNotificationLabel, "modulate:a", 0.0f, 0);
		// 2. Hiện lên trong 0.5s
		tween.TweenProperty(CenterNotificationLabel, "modulate:a", 1.0f, 0.5f).SetTrans(Tween.TransitionType.Cubic);
		// 3. Giữ nguyên 1s
		tween.TweenInterval(1.0f);
		// 4. Mờ đi trong 0.5s
		tween.TweenProperty(CenterNotificationLabel, "modulate:a", 0.0f, 0.5f);
	}

	private void ShowGameOver()
	{
		HideAllPanels();
		ResultPanel.Visible = true;
		ResultLabel.Text = "☠️ GAME OVER";
		ResultLabel.AddThemeColorOverride("font_color", Colors.Red);

		// [MỚI] Ẩn nút Next Level khi thua
		if (BtnNextLevel != null) BtnNextLevel.Visible = false;
	}

	private void ShowVictory()
	{
		HideAllPanels();
		ResultPanel.Visible = true;
		ResultLabel.Text = "🏆 VICTORY!";
		ResultLabel.AddThemeColorOverride("font_color", Colors.Gold);

		// [MỚI] Hiện nút Next Level khi thắng
		if (BtnNextLevel != null) BtnNextLevel.Visible = true;
	}

	public void OnBtnRestartPressed()
	{
		if (Global.Instance != null) Global.Instance.RestartGame();
	}

	// [MỚI] Xử lý nút Màn kế tiếp
	public void OnBtnNextLevelPressed()
	{
		GetTree().Paused = false;
		// Quay về màn chọn level (nơi level mới đã được unlock)
		GetTree().ChangeSceneToFile("res://scenes/ui/select_screen.tscn");
	}

	// [MỚI] Xử lý nút Về Menu
	public void OnBtnMenuPressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/ui/select_screen.tscn");
	}

	/// <summary>
	/// Phương thức cài đặt vị trí xuất hiện của panel, UX panel
	/// </summary>
	/// <param name="panel">panel đang được gọi</param>
	/// <param name="slotGlobalPos">Khu vực vị trí toàn cục</param>
	private void SetPanelPosition(Control panel, TowerSlot slot)
	{
		// 1. Lấy tọa độ thực tế của Slot trên màn hình
		Vector2 slotScreenPos = slot.GetGlobalTransformWithCanvas().Origin;

		panel.ForceUpdateTransform();
		Vector2 panelSize = panel.Size;
		Vector2 screenSize = GetViewportRect().Size; // Kích thước màn hình thực tế (ví dụ 1280x720)

		// 2. Tính toán vị trí mong muốn (Chính giữa trên đầu slot)
		float targetX = slotScreenPos.X - (panelSize.X / 2);
		float targetY = slotScreenPos.Y - panelSize.Y - 20;

		// 3. Xử lý TRÀN MÀN HÌNH (Clamping)
		// Giới hạn X: không nhỏ hơn 0, không lớn hơn (Rộng màn hình - Rộng panel)
		targetX = Mathf.Clamp(targetX, 10, screenSize.X - panelSize.X - 10);

		// Giới hạn Y: Nếu phía trên bị tràn (targetY < 0), đẩy nó xuống dưới slot thay vì hiện bên trên
		if (targetY < 10)
		{
			targetY = slotScreenPos.Y + 40; // Đặt dưới slot một khoảng 40px
		}
		// Đảm bảo không tràn mép dưới màn hình
		targetY = Mathf.Clamp(targetY, 10, screenSize.Y - panelSize.Y - 10);

		// 4. Áp dụng vị trí cuối cùng
		panel.GlobalPosition = new Vector2(targetX, targetY);

		// Thêm hiệu ứng Pop-up cho sinh động
		panel.PivotOffset = panelSize / 2;
		panel.Scale = new Vector2(0.7f, 0.7f);
		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(panel, "scale", Vector2.One, 0.2f);
	}

	/// <summary>
	/// Phương thức hiển thị thanh xây dựng trụ
	/// </summary>
	/// <param name="slot">chỗ trụ đang được chọn</param>
	/// <param name="position">tọa độ của trụ</param>
	public void ShowBuildMenu(TowerSlot slot, Vector2 position)
	{
		_selectedSlot = slot;
		BuildPanel.Visible = true;
		ActionPanel.Visible = false;

		// Cập nhật vị trí build panel theo từng trụ (Bám sát trụ)
		SetPanelPosition(BuildPanel, slot);
	}

	/// <summary>
	/// Hàm hiển thị panel hành động với trụ (nâng cấp, bán)
	/// </summary>
	/// <param name="slot">chỗ trụ đang được chọn</param>
	/// <param name="position">tọa độ của trụ</param>
	public void ShowActionMenu(TowerSlot slot, Vector2 position)
	{
		_selectedSlot = slot;
		BuildPanel.Visible = false;
		ActionPanel.Visible = true;

		UpdateActionInfo(); // Thực hiện cập nhật

		// Kiểm tra nút nâng cấp đã max chưa để thực hiện vô hiệu hóa
		if (UpgradeButton != null)
		{
			bool isMax = (_selectedSlot.CurrentTower.Level >= _selectedSlot.CurrentTower.maxLevel) ? true : false;
			UpgradeButton.Disabled = isMax; // Vô hiệu hóa nút
		}

		// Cập nhật vị trí build panel theo từng trụ (Bám sát trụ)
		SetPanelPosition(ActionPanel, slot);
	}

	private void UpdateActionInfo()
	{
		int upgradeCost = _selectedSlot.CurrentTower.GetUpgradeCost();
		int sellPrice = (_selectedSlot.CurrentTower.BaseCost + (_selectedSlot.CurrentTower.Level - 1) * 50) / 2;

		if (_selectedSlot != null && _selectedSlot.CurrentTower != null && _selectedSlot.CurrentTower.Level < _selectedSlot.CurrentTower.maxLevel)
		{
			TowerInfoLabel.Text = $"Level: {_selectedSlot.CurrentTower.Level}\nUpgrade: {upgradeCost}G\nSell: {sellPrice}G";
			UpgradeButton.FocusMode = FocusModeEnum.All;
			UpgradeButton.Modulate = Colors.White;
		}
		else
		{
			TowerInfoLabel.Text = $"Level: {_selectedSlot.CurrentTower.Level} (MAX)\nSell: {sellPrice}G";

			// Xóa bỏ Focus để mất viền khi click
			UpgradeButton.ReleaseFocus();
			UpgradeButton.FocusMode = FocusModeEnum.None;

			// Làm nút mờ đi thay vì để mặc định
			UpgradeButton.Modulate = new Color(1, 1, 1, 0.5f);
		}
	}

	public void HideAllPanels()
	{
		BuildPanel.Visible = false;
		ActionPanel.Visible = false;
		_selectedSlot = null;

		// [MỚI] Đồng bộ: Nếu có tháp nào đang được chọn, hãy bỏ chọn nó (ẩn tầm bắn)
		if (TowerBase.SelectedTower != null)
		{
			TowerBase.SelectedTower.Deselect();
		}
	}

	public void OnBtnBuildArcherPressed() => RequestBuild(0);
	public void OnBtnBuildCannonPressed() => RequestBuild(1);
	public void OnBtnBuildMagicPressed() => RequestBuild(2);

	public void OnBtnUpgradePressed()
	{
		if (_selectedSlot != null)
		{
			// 1. Thực hiện logic nâng cấp
			_selectedSlot.UpgradeTower();

			// 2. Cập nhật thông tin (Dòng này có thể giữ hoặc bỏ vì menu sắp đóng)
			UpdateActionInfo();

			// 3. THÊM DÒNG NÀY: Tắt toàn bộ menu ngay lập tức
			HideAllPanels();
		}
	}

	public void OnBtnSellPressed()
	{
		if (_selectedSlot != null)
		{
			_selectedSlot.SellTower();
			HideAllPanels();
		}
	}

	public void OnBtnClosePressed() => HideAllPanels();

	private void RequestBuild(int towerIndex)
	{
		if (_selectedSlot != null)
		{
			_selectedSlot.BuildTower(towerIndex);
			HideAllPanels();
		}
	}
}
