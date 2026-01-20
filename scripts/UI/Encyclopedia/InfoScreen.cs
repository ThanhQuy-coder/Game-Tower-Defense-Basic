using Godot;
using Godot.Collections; // Để dùng Array

/// <summary>
/// Lớp phụ trách việc hiển thị thông tin tháp/kẻ thù trong game
/// </summary>
public partial class InfoScreen : Control
{
	// --- PHẦN 1: Dữ liệu đầu vào (Kéo thả trong Inspector) ---
	[ExportGroup("Data Lists")]
	[Export] public Array<InfoResource> EnemyList; // Kéo các file .tres Orc, Slime vào đây
	[Export] public Array<InfoResource> TowerList; // Kéo các file .tres Archer, Cannon vào đây

	// --- PHẦN 2: Tham chiếu UI (Kéo thả các Node vào đây) ---
	[ExportGroup("UI References")]
	[Export] private Control _categoryPanel;
	[Export] private Control _contentPanel;
	[Export] private VBoxContainer _listContainer; // Node ListContainer (Trang trái)

	[Export] private TextureRect _iconRect;        // Node IconRect (Trang phải)
	[Export] private Label _nameLabel;
	[Export] private Label _descLabel;
	[Export] private Label _statsLabel;

	[Export] private PackedScene _entryPrefab;     // Kéo file InfoListEntry.tscn vào đây

	public override void _Ready()
	{
		// Trạng thái ban đầu: Hiện bảng chọn, ẩn nội dung
		ShowCategorySelection();
	}

	// Hàm gọi khi bấm nút "Quái Vật" (Kết nối signal Pressed của BtnSelectEnemy vào đây)
	public void OnBtnEnemyPressed() => LoadList(EnemyList);

	// Hàm gọi khi bấm nút "Tháp Canh" (Kết nối signal Pressed của BtnSelectTower vào đây)
	public void OnBtnTowerPressed() => LoadList(TowerList);

	private void ShowCategorySelection()
	{
		_categoryPanel.Visible = true;
		_contentPanel.Visible = false;
	}

	// Logic chính: Tạo danh sách nút
	private void LoadList(Array<InfoResource> dataList)
	{
		_categoryPanel.Visible = false;
		_contentPanel.Visible = true;

		// 1. Xóa sạch danh sách cũ (nếu có)
		foreach (Node child in _listContainer.GetChildren())
		{
			child.QueueFree();
		}

		// 2. Tạo nút mới từ danh sách Data
		foreach (var data in dataList)
		{
			// Tạo ra một cái nút từ khuôn đúc
			var newButton = _entryPrefab.Instantiate<InfoListEntry>();
			_listContainer.AddChild(newButton);

			// Nạp dữ liệu vào nút
			newButton.Setup(data);

			// Lắng nghe: Khi nút này được chọn -> Cập nhật trang phải
			newButton.Selected += (selectedData) => ShowDetail(selectedData);
		}

		// Tự động hiển thị cái đầu tiên cho đẹp (nếu danh sách không rỗng)
		if (dataList.Count > 0)
		{
			ShowDetail(dataList[0]);
		}
	}

	// Logic hiển thị chi tiết sang trang phải
	private void ShowDetail(InfoResource data)
	{
		_nameLabel.Text = data.Name.ToUpper();
		_iconRect.Texture = data.Icon;
		_descLabel.Text = data.Description;
		_statsLabel.Text = $"❤️ HP: {data.Health}\n⚔️ DMG: {data.Damage}\n🏃SPD: {data.Speed}";
	}

	public void OnBtnBackPressed()
	{
		ShowCategorySelection();
	}
}
