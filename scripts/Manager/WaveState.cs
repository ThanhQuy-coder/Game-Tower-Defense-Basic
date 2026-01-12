/// <summary>
/// Xử lý đém ngược và trạng thái
/// </summary>
public class WaveState
{
	/// <summary>
	/// Số Wave hiện tại
	/// </summary>
	public int CurrentWaveIndex { get; set; } = 0;

	/// <summary>
	/// Kẻ thù còn lại
	/// </summary>
	public int EnemiesToSpawnLeft { get; set; } = 0;

	/// <summary>
	/// Tổng số quái trong wave hiện tại
	/// </summary>
	public int TotalSpawnedInCurrentWave { get; set; } = 0;

	/// <summary>
	/// Wave đang hoạt động
	/// </summary>
	public bool IsWaveActive { get; set; } = false;

	/// <summary>
	/// Trạng thái game đã hoàn thành
	/// </summary>
	public bool IsGameFinished { get; set; } = false;
}
