using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;


[BurstCompile]
public struct GenerateMazeJob : IJob
{
	public Maze maze;

	public int seed;

	public float pickLastProbability, openDeadEndProbability;

	public void Execute ()
	{
		var random = new Random((uint)seed);
		var scratchpad = new NativeArray<(int, MazeFlags, MazeFlags)>(
			4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
		);
		var activeIndices = new NativeArray<int>(
			maze.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory
		);
		int firstActiveIndex = 0, lastActiveIndex = 0;
		activeIndices[firstActiveIndex] = random.NextInt(maze.Length);

		while (firstActiveIndex <= lastActiveIndex)
		{
			int randomActiveIndex = random.NextInt(firstActiveIndex, lastActiveIndex + 1);
			int index = activeIndices[randomActiveIndex];
			
			int availablePassageCount = FindAvailablePassages(index, scratchpad);
			if (availablePassageCount <= 1)
			{
				activeIndices[randomActiveIndex] = activeIndices[firstActiveIndex++];
			}
			if (availablePassageCount > 0)
			{
				(int, MazeFlags, MazeFlags) passage =
					scratchpad[random.NextInt(0, availablePassageCount)];
				maze.Set(index, passage.Item2);
				maze[passage.Item1] = passage.Item3;
				activeIndices[++lastActiveIndex] = passage.Item1;
			}
		}

		if (openDeadEndProbability > 0f)
		{
			random = OpenDeadEnds(random, scratchpad);
		}
	}
	
	int FindAvailablePassages (
		int index, NativeArray<(int, MazeFlags, MazeFlags)> scratchpad
	)
	{
		int2 coordinates = maze.IndexToCoordinates(index);
		int count = 0;
		if (coordinates.x + 1 < maze.SizeEW)
		{
			int i = index + maze.StepE;
			if (maze[i] == MazeFlags.Empty)
			{
				scratchpad[count++] = (i, MazeFlags.PassageE, MazeFlags.PassageW);
			}
		}
		if (coordinates.x > 0)
		{
			int i = index + maze.StepW;
			if (maze[i] == MazeFlags.Empty)
			{
				scratchpad[count++] = (i, MazeFlags.PassageW, MazeFlags.PassageE);
			}
		}
		if (coordinates.y + 1 < maze.SizeNS)
		{
			int i = index + maze.StepN;
			if (maze[i] == MazeFlags.Empty)
			{
				scratchpad[count++] = (i, MazeFlags.PassageN, MazeFlags.PassageS);
			}
		}
		if (coordinates.y > 0)
		{
			int i = index + maze.StepS;
			if (maze[i] == MazeFlags.Empty)
			{
				scratchpad[count++] = (i, MazeFlags.PassageS, MazeFlags.PassageN);
			}
		}
		return count;
	}

	int FindClosedPassages (
		int index, NativeArray<(int, MazeFlags, MazeFlags)> scratchpad, MazeFlags exclude
	)
	{
		int2 coordinates = maze.IndexToCoordinates(index);
		int count = 0;
		if (exclude != MazeFlags.PassageE && coordinates.x + 1 < maze.SizeEW)
		{
			scratchpad[count++] = (maze.StepE, MazeFlags.PassageE, MazeFlags.PassageW);
		}
		if (exclude != MazeFlags.PassageW && coordinates.x > 0)
		{
			scratchpad[count++] = (maze.StepW, MazeFlags.PassageW, MazeFlags.PassageE);
		}
		if (exclude != MazeFlags.PassageN && coordinates.y + 1 < maze.SizeNS)
		{
			scratchpad[count++] = (maze.StepN, MazeFlags.PassageN, MazeFlags.PassageS);
		}
		if (exclude != MazeFlags.PassageS && coordinates.y > 0)
		{
			scratchpad[count++] = (maze.StepS, MazeFlags.PassageS, MazeFlags.PassageN);
		}
		return count;
	}

	Random OpenDeadEnds (
		Random random, NativeArray<(int, MazeFlags, MazeFlags)> scratchpad
	)
	{
		for (int i = 0; i < maze.Length; i++)
		{
			MazeFlags cell = maze[i];
			if (cell.HasExactlyOne() && random.NextFloat() < openDeadEndProbability)
			{
				int availablePassageCount = FindClosedPassages(i, scratchpad, cell);
				(int, MazeFlags, MazeFlags) passage =
					scratchpad[random.NextInt(0, availablePassageCount)];
				maze[i] = cell.With(passage.Item2);
				maze.Set(i + passage.Item1, passage.Item3);
			}
		}
		return random;
	}
}