using Godot;

public partial class MusicManager : Node
{
	private AudioStreamPlayer _audioPlayer;

	public override void _Ready()
	{
		// Khởi tạo Node âm thanh duy nhất cho toàn game
		_audioPlayer = new AudioStreamPlayer();
		AddChild(_audioPlayer);

		// Đảm bảo nhạc vẫn phát ngay cả khi game bị Pause
		ProcessMode = ProcessModeEnum.Always;

		// Cấu hình cơ bản (nên dùng Bus riêng để dễ chỉnh âm lượng sau này)
		_audioPlayer.Bus = "Music";
	}

	/// <summary>
	/// Hàm duy nhất để thay đổi nhạc nền. 
	/// Nếu bài nhạc đang phát trùng với yêu cầu, nó sẽ không phát lại từ đầu.
	/// </summary>
	public void PlayMusic(AudioStream stream)
	{
		if (stream == null) return;

		// Kiểm tra nếu đang phát đúng bài đó rồi thì không chạy lại
		if (_audioPlayer.Stream == stream && _audioPlayer.Playing)
			return;

		_audioPlayer.Stream = stream;
		_audioPlayer.Play();
	}

	public void StopMusic()
	{
		_audioPlayer.Stop();
	}

	/// <summary>
	/// Hàm sử dụng để tạm dùng phát nhạc
	/// </summary>
	/// <param name="enabled">Biến kích hoạt</param>
	public void SetMusicEnabled(bool enabled)
	{
		_audioPlayer.StreamPaused = !enabled;
	}
}
