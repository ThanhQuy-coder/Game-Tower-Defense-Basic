using System.Collections.Generic;
using Godot;

/// <summary>
/// Lưu trữ thông tin cho một wave
/// </summary>
public class WaveData
{
	public List<SpawnEntry> SpawnEntries { get; set; } = new List<SpawnEntry>();

	/// <summary>
	/// Khoảng thời gian giữa mỗi lần spawn quái (Giây)
	/// </summary>
	public float SpawnInterval { get; set; }

	// Tính tổng số lượng quái để WaveManager biết khi nào kết thúc
	public int GetTotalCount()
	{
		int total = 0;
		foreach (var entry in SpawnEntries) total += entry.Count;
		return total;
	}

	// Constructor mặc định cho hệ thống
	public WaveData() { }

	// Constructor giúp viết code ngắn gọn ở Factory
	public WaveData(float interval, params SpawnEntry[] entries)
	{
		this.SpawnInterval = interval;
		this.SpawnEntries = new List<SpawnEntry>(entries);
	}
}

/// <summary>
/// Được sử dụng để sinh quái nhiều hơn
/// </summary>
public class SpawnEntry
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
}
