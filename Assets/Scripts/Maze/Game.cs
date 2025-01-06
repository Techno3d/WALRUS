using TMPro;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.AI.Navigation;
using static Unity.Mathematics.math;

public class Game : MonoBehaviour
{
	[SerializeField]
	MazeVisualization visualization;

	[SerializeField]
	int2 mazeSize = int2(20, 20);

	[SerializeField]
	float pickLastProbability = 0.5f;
	[SerializeField]
	float openDeadEndProbability = 0.5f;

	[SerializeField, Tooltip("Use zero for random seed.")]
	int seed;

	Maze maze;

	void Awake()
	{
		maze = new Maze(mazeSize);
		new GenerateMazeJob
		{
			maze = maze,
			seed = seed != 0 ? seed : Random.Range(1, int.MaxValue),
			pickLastProbability = pickLastProbability,
			openDeadEndProbability = openDeadEndProbability
		}.Schedule().Complete();
		maze[maze.Length / 2] = MazeFlags.PassageAll;
		maze[maze.Length / 2 + 1] = MazeFlags.PassageAll;
		visualization.Visualize(maze);
		GetComponent<NavMeshSurface>().BuildNavMesh();
	}

	void OnDestroy()
	{
		maze.Dispose();
	}
}