using Godot;

public partial class HueContainer : Control
{
	private Label _goldLabel;
	private Label _healthLabel;
	
	public override void _Ready()
	{
		// Tìm các Label trong Scene Tree
		_goldLabel = GetNode<Label>("GoldLabel");
		_healthLabel = GetNode<Label>("HealthLabel");
		
		// Kết nối events từ Global
		if (Global.Instance != null)
		{
			Global.Instance.GoldChanged += OnGoldChanged;
			Global.Instance.HealthChanged += OnHealthChanged;
		}
		
		// Cập nhật lần đầu
		UpdateLabels();
	}
	
	private void UpdateLabels()
	{
		if (_goldLabel != null && Global.Instance != null)
			_goldLabel.Text = "Tiền: " + Global.Instance.Gold;
		
		if (_healthLabel != null && Global.Instance != null)
			_healthLabel.Text = "Máu: " + Global.Instance.Health;
	}
	
	private void OnGoldChanged(int newGold)
	{
		if (_goldLabel != null)
			_goldLabel.Text = "Tiền: " + newGold;
	}
	
	private void OnHealthChanged(int newHealth)
	{
		if (_healthLabel != null)
			_healthLabel.Text = "Máu: " + newHealth;
	}
	
	public override void _ExitTree()
	{
		// Ngắt kết nối khi thoát
		if (Global.Instance != null)
		{
			Global.Instance.GoldChanged -= OnGoldChanged;
			Global.Instance.HealthChanged -= OnHealthChanged;
		}
	}
}
