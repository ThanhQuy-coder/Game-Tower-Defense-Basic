using Godot;
using System;

// [Người 3]
// Script quản lý giao diện HUD. Tách biệt hoàn toàn với logic game.
// Chỉ cập nhật khi nhận được tín hiệu từ Global.
#pragma warning disable CA1050 // Declare types in namespaces

public partial class HUD : CanvasLayer
#pragma warning restore CA1050 // Declare types in namespaces

{
	[Export] private Label _lblGold;
	[Export] private Label _lblHealth;
	[Export] private Label _lblWave;
	[Export] private Button _btnPause;

	public override void _Ready()
	{
		// Đăng ký lắng nghe sự kiện từ Global (nếu Global đã sẵn sàng)
		if (Global.Instance != null)
		{
			Global.Instance.OnGoldChanged += UpdateGold;
			Global.Instance.OnHealthChanged += UpdateHealth;
			Global.Instance.OnWaveChanged += UpdateWave;
			
			// Khởi tạo giá trị ban đầu
			UpdateGold(Global.Instance.Gold);
			UpdateHealth(Global.Instance.Health);
			UpdateWave(Global.Instance.Wave);
		}
	}

	public override void _ExitTree()
	{
		// Hủy đăng ký để tránh memory leak
		if (Global.Instance != null)
		{
			Global.Instance.OnGoldChanged -= UpdateGold;
			Global.Instance.OnHealthChanged -= UpdateHealth;
			Global.Instance.OnWaveChanged -= UpdateWave;
		}
	}

	private void UpdateGold(int gold)
	{
		if (_lblGold != null) _lblGold.Text = $"Gold: {gold}";
	}

	private void UpdateHealth(int health)
	{
		if (_lblHealth != null) _lblHealth.Text = $"HP: {health}";
		
		// Hiệu ứng cảnh báo khi máu thấp
		if (health <= 5 && _lblHealth != null)
			_lblHealth.Modulate = Colors.Red;
		else if (_lblHealth != null)
			_lblHealth.Modulate = Colors.White;
	}

	private void UpdateWave(int wave)
	{
		if (_lblWave != null) _lblWave.Text = $"Wave: {wave}";
	}
}
