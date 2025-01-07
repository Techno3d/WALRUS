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
	[SerializeField]
	GameObject ShockPrefab;

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
		visualization.Visualize(maze);
		for(int i = 0; i < 10; i++) {
			int2 randCoords = int2(Random.Range(0, maze.SizeEW), Random.Range(0, maze.SizeNS));
			Vector3 coords = maze.CoordinatesToWorldPosition(randCoords, 2);
			Instantiate(ShockPrefab, coords, Quaternion.identity);
		}
		GetComponent<NavMeshSurface>().BuildNavMesh();
	}

	void Start() {
		AudioManager.instance?.ChangeClip(TypeMusic.GameBG);
	}

	void OnDestroy()
	{
		maze.Dispose();
	}
}