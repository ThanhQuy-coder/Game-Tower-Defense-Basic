using System.Collections.Generic;

/// <summary>
/// Khởi tạo cấu hình cho từng level
/// </summary>
public static class LevelFactory
{

	/// <summary>
	/// Phương thức lấy cấu hình Level và vàng tương ứng cho từng level
	/// </summary>
	public static List<WaveData> GetLevelConfig(int level, WaveManager Instance)
	{

		var config = new List<WaveData>();

		switch (level)
		{
			case 1:
				Global.Instance.InitialGold = 120;
				config.Add(new WaveData(2, new SpawnEntry { EnemyTypeIndex = 0, Count = 5 }));
				config.Add(new WaveData(1.8f, new SpawnEntry { EnemyTypeIndex = 0, Count = 8 }));
				config.Add(new WaveData(1.5f, new SpawnEntry { EnemyTypeIndex = 0, Count = 13 }));
				config.Add(new WaveData(1, new SpawnEntry { EnemyTypeIndex = 0, Count = 21 }));
				config.Add(new WaveData(0.5f, new SpawnEntry { EnemyTypeIndex = 0, Count = 20 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 3 }));
				break;
			case 2:
				Global.Instance.InitialGold = 150;
				config.Add(new WaveData(2, new SpawnEntry { EnemyTypeIndex = 0, Count = 10 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 3 }));
				config.Add(new WaveData(2, new SpawnEntry { EnemyTypeIndex = 0, Count = 12 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 4 }));
				config.Add(new WaveData(1.5f, new SpawnEntry { EnemyTypeIndex = 0, Count = 5 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 5 }));
				config.Add(new WaveData(1.5f, new SpawnEntry { EnemyTypeIndex = 1, Count = 1 }, new SpawnEntry { EnemyTypeIndex = 0, Count = 3 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 10 }));
				config.Add(new WaveData(1, new SpawnEntry { EnemyTypeIndex = 1, Count = 2 }, new SpawnEntry { EnemyTypeIndex = 0, Count = 20 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 7 }));
				break;
			case 3:
				Global.Instance.InitialGold = 240;
				config.Add(new WaveData(3, new SpawnEntry { EnemyTypeIndex = 2, Count = 6 }));
				config.Add(new WaveData(2, new SpawnEntry { EnemyTypeIndex = 0, Count = 12 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 4 }));
				config.Add(new WaveData(2, new SpawnEntry { EnemyTypeIndex = 0, Count = 5 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 5 }));
				config.Add(new WaveData(1.5f, new SpawnEntry { EnemyTypeIndex = 1, Count = 4 }, new SpawnEntry { EnemyTypeIndex = 0, Count = 20 }));
				config.Add(new WaveData(1.5f, new SpawnEntry { EnemyTypeIndex = 1, Count = 2 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 10 }));
				break;
			case 4:
				Global.Instance.InitialGold = 240;
				config.Add(new WaveData(3, new SpawnEntry { EnemyTypeIndex = 2, Count = 6 }));
				config.Add(new WaveData(3, new SpawnEntry { EnemyTypeIndex = 0, Count = 12 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 4 }));
				config.Add(new WaveData(2, new SpawnEntry { EnemyTypeIndex = 0, Count = 5 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 5 }));
				config.Add(new WaveData(1.5f, new SpawnEntry { EnemyTypeIndex = 1, Count = 4 }, new SpawnEntry { EnemyTypeIndex = 0, Count = 20 }));
				config.Add(new WaveData(1, new SpawnEntry { EnemyTypeIndex = 1, Count = 2 }, new SpawnEntry { EnemyTypeIndex = 2, Count = 10 }));
				config.Add(new WaveData(0, new SpawnEntry { EnemyTypeIndex = 3, Count = 1 }));
				break;
		}
		return config;
	}
}