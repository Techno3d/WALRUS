using TMPro;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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

	void Awake ()
	{
		maze = new Maze(mazeSize);
		new GenerateMazeJob
		{
			maze = maze,
			seed = seed != 0 ? seed : Random.Range(1, int.MaxValue),
			pickLastProbability = pickLastProbability,
			openDeadEndProbability = openDeadEndProbability
		}.Schedule().Complete();
		visualization.Visualize(maze);
	}

	void OnDestroy ()
	{
		maze.Dispose();
	}
}