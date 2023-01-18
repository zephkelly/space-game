using UnityEngine;
using Mirror;

public class ChunkPopulator
{
  private WorldManager worldManager;

  // Properties -----------------------------------------------

  private static int asteroidSmallCount = 20;
  private static int asteroidMediumCount = 10;
  private static int asteroidLargeCount = 5;
  private static int asteroidHugeCount = 1;

  private uint seed;

  public ChunkPopulator(WorldManager _worldManager) {
    worldManager = _worldManager;
    SetRandomSeed();
  }

  public void PopulateChunk(Chunk chunk)
  {
    GenerateAsteroids(chunk);
  }

  private void GenerateAsteroids(Chunk chunk)
  {
    int minimum = Random.Range(asteroidSmallCount - 10, asteroidSmallCount + 10);
    int maximum = Random.Range(asteroidSmallCount - 5, asteroidSmallCount + 15);
    int asteroidCount = Random.Range(minimum, maximum);

    var spawnPoint = Vector2.zero;

    for (int i = 0; i < asteroidCount; i++)
    {
      spawnPoint = GetRandomChunkPosition(chunk.chunkBounds);
      Debug.Log("Asteroid at: " + spawnPoint);

      var asteroid = new Asteroid(spawnPoint, null);
      chunk.asteroidSmall.Add(spawnPoint, asteroid);
    }
  }

  Vector2 GetRandomChunkPosition(Bounds chunkBounds)
  {
    return new Vector2(
      Random.Range(chunkBounds.min.x, chunkBounds.max.x),
      Random.Range(chunkBounds.min.y, chunkBounds.max.y)
    );
  }

  private void SetRandomSeed() => seed = (uint)Random.Range(0, int.MaxValue);

	public void SetSeed(uint _seed) => seed = _seed;
}