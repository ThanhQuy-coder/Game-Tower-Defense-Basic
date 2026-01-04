using Godot;

public partial class hud : CanvasLayer
{
	// KHÔNG tạo Label mới, chỉ lấy reference từ Scene Tree
	private Label _goldLabel;
	private Label _healthLabel;
	
	public override void _Ready()
	{
		// TÌM Label CÓ SẴN trong Scene Tree của bạn
		// Giả sử đường dẫn là: "HUDContainer/GoldLabel" và "HUDContainer/HealthLabel"
		_goldLabel = GetNode<Label>("HUDContainer/GoldLabel");
		_healthLabel = GetNode<Label>("HUDContainer/HealthLabel");
		
		// HOẶC nếu Label trực tiếp dưới CanvasLayer:
		// _goldLabel = GetNode<Label>("GoldLabel");
		
		if (Global.Instance != null)
		{
			Global.Instance.GoldChanged += OnGoldChanged;
			Global.Instance.HealthChanged += OnHealthChanged;
		}
		
		UpdateUI();
	}
	
	private void UpdateUI()
	{
		if (_goldLabel != null && Global.Instance != null)
			_goldLabel.Text = "Tiền: " + Global.Instance.Gold;
		if (_healthLabel != null && Global.Instance != null)
			_healthLabel.Text = "Máu: " + Global.Instance.Health;
	}
	
	private void OnGoldChanged(int newGold)
	{
		if (_goldLabel != null)
			_goldLabel.Text = "Tiền: " + newGold;
	}
	
	private void OnHealthChanged(int newHealth)
	{
		if (_healthLabel != null)
			_healthLabel.Text = "Máu: " + newHealth;
	}
	
	// Cập nhật mỗi frame để chắc chắn
	public override void _Process(double delta)
	{
		if (_goldLabel != null && Global.Instance != null)
			_goldLabel.Text = "Tiền: " + Global.Instance.Gold;
		if (_healthLabel != null && Global.Instance != null)
			_healthLabel.Text = "Máu: " + Global.Instance.Health;
	}
}
