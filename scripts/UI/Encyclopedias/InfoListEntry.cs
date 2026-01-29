using Godot;

public partial class InfoListEntry : Button
{
	// Tạo sự kiện: Khi nút này được bấm, nó sẽ thông báo và gửi dữ liệu nó đang giữ ra ngoài
	[Signal] public delegate void SelectedEventHandler(InfoResource data);

	private InfoResource _data;

	// Hàm này để InfoScreen gọi khi tạo nút
	public void Setup(InfoResource data)
	{
		_data = data;
		Text = data.Name; // Gán tên quái lên mặt nút
	}

	public override void _Ready()
	{
		// Khi nút bị bấm -> Phát tín hiệu Selected kèm theo cục data
		Pressed += () => EmitSignal(SignalName.Selected, _data);
	}
}
