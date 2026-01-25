using Godot;
using System;
using System.Threading.Tasks; // Cần thêm thư viện này để dùng Task.Delay

/// <summary>
/// Class Global quản lý trạng thái toàn cục của trò chơi (Singleton).
/// Chịu trách nhiệm lưu trữ máu, tiền, wave, và điều phối Game Over / Victory.
/// </summary>
public partial class Global : Node
{
	// Biến tĩnh Instance để truy cập Global từ bất kỳ đâu (Pattern Singleton)
	public static Global Instance { get; private set; }

	// --- CÁC TÍN HIỆU (SIGNALS) ---
	// Được phát ra để UI hoặc các hệ thống khác lắng nghe và cập nhật
	[Signal] public delegate void StatsChangedEventHandler(); // Khi máu/tiền/wave thay đổi
	[Signal] public delegate void GameOverEventHandler();     // Khi thua
	[Signal] public delegate void VictoryEventHandler();      // Khi thắng

	// --- BIẾN PRIVATE ---
	private int _gold;
	private int _health;
	private int _wave;

	// --- CẤU HÌNH BAN ĐẦU (EXPORT ĐỂ CHỈNH TRONG EDITOR) ---
	[Export] public int InitialGold = 550;   // Tiền khởi điểm
	[Export] public int InitialHealth = 20;  // Máu khởi điểm

	// [LƯU TRỮ] Biến lưu level cao nhất đã mở khóa. Mặc định là 1.
	[Export] public int UnlockedLevel = 1;

	// --- CÁC PROPERTIES (GETTER/SETTER) ---
	
	/// <summary>
	/// Số vàng hiện tại. Khi thay đổi sẽ phát tín hiệu StatsChanged.
	/// </summary>
	public int Gold
	{
		get => _gold;
		set { _gold = value; EmitSignal(SignalName.StatsChanged); }
	}

	/// <summary>
	/// Máu hiện tại của người chơi.
	/// Khi máu về 0 hoặc thấp hơn, sẽ kích hoạt Game Over.
	/// </summary>
	public int Health
	{
		get => _health;
		set
		{
			_health = value;
			EmitSignal(SignalName.StatsChanged); // Cập nhật UI ngay lập tức

			// Kiểm tra điều kiện thua
			if (_health <= 0)
			{
				// Gọi hàm xử lý thua với độ trễ (để tránh lỗi xung đột luồng hoặc UI cập nhật quá nhanh)
				CallDeferred(nameof(TriggerGameOverDelayed));
			}
		}
	}

	/// <summary>
	/// Wave (đợt quái) hiện tại.
	/// </summary>
	public int Wave
	{
		get => _wave;
		set { _wave = value; EmitSignal(SignalName.StatsChanged); }
	}

	// --- HÀM KHỞI TẠO ---

	public override void _Ready()
	{
		// Thiết lập Singleton
		Instance = this;
		
		// Đảm bảo Global luôn chạy kể cả khi game đang Pause (để xử lý menu, restart...)
		ProcessMode = ProcessModeEnum.Always;

		// [QUAN TRỌNG] Tải dữ liệu từ ổ cứng ngay khi game vừa bật
		// Việc này đảm bảo biến UnlockedLevel luôn đúng với lần chơi trước
		UnlockedLevel = SaveManager.LoadProgress();
		GD.Print($"[Global] Game khởi động. Level đã mở: {UnlockedLevel}");

		// Khởi tạo các chỉ số ban đầu
		StartNewGame();
	}

	/// <summary>
	/// Đặt lại các chỉ số về mặc định để bắt đầu màn chơi mới.
	/// </summary>
	public void StartNewGame()
	{
		GetTree().Paused = false; // Bỏ pause nếu đang pause

		// Gán lại giá trị gốc
		Gold = InitialGold;
		Health = InitialHealth;
		Wave = 1;

		// Cập nhật UI ngay lập tức
		EmitSignal(SignalName.StatsChanged);
	}

	/// <summary>
	/// Hàm mở khóa level tiếp theo và lưu vào ổ cứng.
	/// </summary>
	/// <param name="levelToUnlock">Số thứ tự level muốn mở khóa</param>
	public void UnlockLevel(int levelToUnlock)
	{
		// Chỉ mở khóa và LƯU nếu level mới cao hơn level hiện tại
		// (Tránh trường hợp chơi lại level 1 xong bị ghi đè dữ liệu cũ)
		if (levelToUnlock > UnlockedLevel)
		{
			UnlockedLevel = levelToUnlock;
			
			// [QUAN TRỌNG] Gọi SaveManager để ghi xuống ổ cứng ngay lập tức
			SaveManager.SaveProgress(UnlockedLevel);
			
			GD.Print($"[Global] New Level Unlocked & Saved -> {UnlockedLevel}");
		}
	}

	/// <summary>
	/// Xử lý sự kiện thua cuộc có độ trễ nhỏ.
	/// </summary>
	private async void TriggerGameOverDelayed()
	{
		// Chờ 0.1s để UI kịp vẽ số máu về 0 trước khi hiện bảng thua
		await ToSignal(GetTree().CreateTimer(0.1f), "timeout");

		if (GetTree().Paused) return; // Nếu game đã pause rồi thì thôi
		
		GD.Print("GAME OVER!");
		EmitSignal(SignalName.GameOver); // Phát tín hiệu để hiện UI Game Over
		GetTree().Paused = true;         // Dừng game lại
	}

	/// <summary>
	/// Xử lý sự kiện chiến thắng có độ trễ.
	/// </summary>
	public async void TriggerVictory()
	{
		// Thêm độ trễ cho chiến thắng để cảm giác mượt hơn, người chơi kịp nhìn thấy quái cuối chết
		await ToSignal(GetTree().CreateTimer(0.2f), "timeout");

		if (GetTree().Paused) return;
		
		GD.Print("VICTORY!");
		EmitSignal(SignalName.Victory); // Phát tín hiệu để hiện UI Victory
		GetTree().Paused = true;        // Dừng game lại
	}

	/// <summary>
	/// Chơi lại màn chơi hiện tại.
	/// </summary>
	public void RestartGame()
	{
		GetTree().Paused = false;      // Bỏ pause trước khi reload
		StartNewGame();                // Reset chỉ số
		GetTree().ReloadCurrentScene();// Load lại scene hiện tại
	}
}
