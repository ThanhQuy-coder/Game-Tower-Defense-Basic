using Godot;

/// <summary>
/// Thành phần quản lý âm thanh cho các loại đạn.
/// Giúp tách biệt logic vật lý của đạn và logic âm thanh.
/// </summary>
public partial class BulletSfxComponent : Node
{
	[Export] public AudioStream ShootSfx;   // Âm thanh khi đạn vừa bay ra
	[Export] public AudioStream ImpactSfx;  // Âm thanh khi đạn va chạm/nổ

	public override void _Ready()
	{
		// Khi đạn vừa xuất hiện (Ready), phát ngay âm thanh bắn
		PlaySfx(ShootSfx);

		// Lấy tham chiếu đến lớp đạn cha (BulletBase)
		var bullet = GetParent<BulletBase>();
		if (bullet != null)
		{
			// Kết nối với sự kiện TreeExiting (khi đạn biến mất/nổ) để phát tiếng nổ
			bullet.TreeExiting += OnBulletDestroyed;
		}
	}

	private void OnBulletDestroyed()
	{
		// Phát âm thanh va chạm trước khi Node đạn bị xóa hoàn toàn khỏi bộ nhớ
		PlaySfx(ImpactSfx);
	}

	private void PlaySfx(AudioStream stream)
	{
		if (stream == null) return;

		// Gọi đến SfxManager (Singleton) mà bạn đã xây dựng
		var sfxManager = GetNodeOrNull<SfxManager>("/root/SfxManager");

		// --- KỸ THUẬT: PITCH RANDOMIZATION ---
		// Thay đổi pitch trong khoảng 0.9 đến 1.1 giúp âm thanh không bị trùng lặp
		float randomPitch = (float)GD.RandRange(0.9, 1.1);

		sfxManager?.PlaySfx(stream);
	}
}
