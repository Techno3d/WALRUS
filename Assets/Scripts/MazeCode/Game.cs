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
	int2 mazeSize = int2(15, 15);

	[SerializeField, Tooltip("Use zero for random seed.")]
	int seed;

	Maze maze;

	void Awake ()
	{
		maze = new Maze(mazeSize);
		new GenerateMazeJob
		{
			maze = maze,
			seed = seed != 0 ? seed : Random.Range(1, int.MaxValue)
		}.Schedule().Complete();
		visualization.Visualize(maze);
	}

	void OnDestroy ()
	{
		maze.Dispose();
	}
}