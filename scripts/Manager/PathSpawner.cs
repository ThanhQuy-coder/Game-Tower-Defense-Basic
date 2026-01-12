using Godot;

/// <summary>
/// Gắn con quái vào đúng đường đi, không quan tâm khi nào cần spawn.
/// </summary>
public class PathSpawner
{
	private readonly Path2D[] _paths;
	private readonly PackedScene[] _prefabs;

	/// <summary>
	/// constructor
	/// </summary>
	/// <param name="paths">Những đường đi của quái</param>
	/// <param name="prefabs">Những enemy đã được tạo</param>
	public PathSpawner(Path2D[] paths, PackedScene[] prefabs)
	{
		_paths = paths;
		_prefabs = prefabs;
	}

	/// <summary>
	/// Thực hiện việc gắn quái vào đúng đường đi
	/// </summary>
	/// <example>
	/// 	Nếu có 3 đường thì thực hiện như sau:
	/// 	enemy 0 -> path 0
	/// 	enemy 1 -> path 1
	/// 	enemy 2 -> path 2
	/// 	enemy 3 -> path 0
	/// </example>
	/// <param name="enemyTypeIndex">Loại kẻ thù</param>
	/// <param name="totalSpawnedSoFar">Tổng số quái đã được sinh ra</param>
	public void Execute(int enemyTypeIndex, int totalSpawnedSoFar)
	{
		if (_paths == null || _paths.Length == 0 || enemyTypeIndex >= _prefabs.Length) return;

		// Logic chia đường
		int pathIndex = totalSpawnedSoFar % _paths.Length;
		Path2D selectedPath = _paths[pathIndex];

		if (selectedPath != null)
		{
			var pathFollow = new PathFollow2D
			{
				Loop = false,
				Rotates = false
			};
			selectedPath.AddChild(pathFollow);

			var enemy = _prefabs[enemyTypeIndex].Instantiate<Node2D>();
			pathFollow.AddChild(enemy);
		}
	}
}