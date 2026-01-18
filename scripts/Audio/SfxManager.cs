using Godot;
using System.Collections.Generic;

public partial class SfxManager : Node
{
	// Số lượng kênh phát cùng lúc tối đa
	[Export] private int _poolSize = 8;
	private List<AudioStreamPlayer> _soundPool = new();
	private int _currentIndex = 0;
	private bool _isSfxEnabled = true;

	public override void _Ready()
	{
		// Khởi tạo Pool để dùng lại các Node, tránh khởi tạo/xóa liên tục (Tối ưu hiệu năng)
		for (int i = 0; i < _poolSize; i++)
		{
			var player = new AudioStreamPlayer();
			AddChild(player);
			player.Bus = "SFX"; // Nên có một Bus riêng trong Audio Mixer
			_soundPool.Add(player);
		}
	}

	/// <summary>
	/// Phương thức duy nhất để phát hiệu ứng âm thanh
	/// </summary>
	public void PlaySfx(AudioStream stream, float pitchScale = 1.0f)
	{
		if (!_isSfxEnabled || stream == null) return;

		var player = _soundPool[_currentIndex];
		player.Stream = stream;
		player.PitchScale = pitchScale; // Thêm chút biến đổi cao độ để âm thanh bớt nhàm chán
		player.Play();

		// Xoay vòng trong Pool
		_currentIndex = (_currentIndex + 1) % _poolSize;
	}

	public void SetSfxEnabled(bool enabled)
	{
		_isSfxEnabled = enabled;
	}
}
