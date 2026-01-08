using System.Collections.Generic;

/// <summary>
/// Khởi tạo cấu hình cho từng level
/// </summary>
public static class LevelFactory
{

	/// <summary>
	/// Phương thức lấy cấu hình Level
	/// </summary>
	/// <param name="level">Mức độ level.</param>
	/// <returns>Danh sách dữ liệu màn</returns>
	public static List<WaveData> GetLevelConfig(int level)
	{

		var config = new List<WaveData>();

		switch (level)
		{
			case 1:
				config.Add(new WaveData { EnemyTypeIndex = 0, Count = 5, SpawnInterval = 1.5f });
				config.Add(new WaveData { EnemyTypeIndex = 2, Count = 6, SpawnInterval = 1.2f });
				config.Add(new WaveData { EnemyTypeIndex = 1, Count = 4, SpawnInterval = 2.0f });
				break;
			case 2:
				config.Add(new WaveData { EnemyTypeIndex = 0, Count = 10, SpawnInterval = 1.0f });
				config.Add(new WaveData { EnemyTypeIndex = 2, Count = 10, SpawnInterval = 1.0f });
				config.Add(new WaveData { EnemyTypeIndex = 1, Count = 5, SpawnInterval = 1.8f });
				config.Add(new WaveData { EnemyTypeIndex = 0, Count = 15, SpawnInterval = 0.8f });
				break;
			case 3:
				config.Add(new WaveData { EnemyTypeIndex = 2, Count = 15, SpawnInterval = 0.8f });
				config.Add(new WaveData { EnemyTypeIndex = 0, Count = 20, SpawnInterval = 0.5f });
				config.Add(new WaveData { EnemyTypeIndex = 1, Count = 8, SpawnInterval = 1.5f });
				config.Add(new WaveData { EnemyTypeIndex = 2, Count = 20, SpawnInterval = 0.8f });
				config.Add(new WaveData { EnemyTypeIndex = 1, Count = 10, SpawnInterval = 1.2f });
				break;
			case 4:
				config.Add(new WaveData { EnemyTypeIndex = 0, Count = 20, SpawnInterval = 0.5f });
				config.Add(new WaveData { EnemyTypeIndex = 2, Count = 20, SpawnInterval = 0.8f });
				config.Add(new WaveData { EnemyTypeIndex = 1, Count = 15, SpawnInterval = 1.2f });
				config.Add(new WaveData { EnemyTypeIndex = 3, Count = 1, SpawnInterval = 1.0f });
				break;
		}
		return config;
	}
}