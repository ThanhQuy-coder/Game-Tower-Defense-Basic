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
	
	// [MỚI] Label thông báo giữa màn hình
	[Export] public Label CenterNotificationLabel; 

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
	}

	private void ShowVictory() 
	{ 
		HideAllPanels(); 
		ResultPanel.Visible = true; 
		ResultLabel.Text = "🏆 VICTORY!"; 
		ResultLabel.AddThemeColorOverride("font_color", Colors.Gold); 
	}

	public void OnBtnRestartPressed() 
	{ 
		if (Global.Instance != null) Global.Instance.RestartGame(); 
	}

	public void ShowBuildMenu(TowerSlot slot, Vector2 position) 
	{ 
		_selectedSlot = slot; 
		BuildPanel.Visible = true; 
		ActionPanel.Visible = false; 
	}

	public void ShowActionMenu(TowerSlot slot, Vector2 position) 
	{ 
		_selectedSlot = slot; 
		BuildPanel.Visible = false; 
		ActionPanel.Visible = true; 
		UpdateActionInfo(); 
	}

	private void UpdateActionInfo() 
	{ 
		if (_selectedSlot != null && _selectedSlot.CurrentTower != null) 
		{ 
			int upgradeCost = _selectedSlot.CurrentTower.GetUpgradeCost(); 
			int sellPrice = (_selectedSlot.CurrentTower.BaseCost + (_selectedSlot.CurrentTower.Level - 1) * 50) / 2; 
			TowerInfoLabel.Text = $"Level: {_selectedSlot.CurrentTower.Level}\nUpgrade: {upgradeCost}G\nSell: {sellPrice}G"; 
		} 
	}

	public void HideAllPanels() 
	{ 
		BuildPanel.Visible = false; 
		ActionPanel.Visible = false; 
		_selectedSlot = null; 
	}

	public void OnBtnBuildArcherPressed() => RequestBuild(0);
	public void OnBtnBuildCannonPressed() => RequestBuild(1);
	public void OnBtnBuildMagicPressed() => RequestBuild(2);

	public void OnBtnUpgradePressed() 
	{ 
		if (_selectedSlot != null) 
		{ 
			_selectedSlot.UpgradeTower(); 
			UpdateActionInfo(); 
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
