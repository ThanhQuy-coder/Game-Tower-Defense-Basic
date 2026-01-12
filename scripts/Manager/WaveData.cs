using Godot;

/// <summary>
/// Lưu trữ thông tin cho một wave
/// </summary>
public class WaveData
{
	/// <summary>
	/// Chỉ số loại quái (0: Fast, 1: Tank, 2: Soldier, 3: Boss)
	/// </summary>
	/// <value>Trả về loại quái. Ví dụ: 1 (quái Tank)</value>
	public int EnemyTypeIndex { get; set; }

	/// <summary>
	/// Số lượng quái sẽ spawn
	/// </summary>
	/// <value>Trả về số lượng quái cần spawn. Ví dụ: 20 (số lượng 20)</value>
	public int Count { get; set; }

	/// <summary>
	/// Khoảng thời gian giữa mỗi lần spawn quái (Giây)
	/// </summary>
	public float SpawnInterval { get; set; }
}
