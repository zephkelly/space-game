using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChunkGenerator
{
  private WorldManager worldManager;
  private ChunkPopulator chunkPopulator;

	private const int CHUNK_DIAMETER = 50;
	internal int CHUNK_NUMBER;

  public ChunkGenerator(WorldManager _worldManager, ChunkPopulator _chunkPopulator)
  {
    worldManager = _worldManager;
    chunkPopulator = _chunkPopulator;
  }

	 //Add 5x5 to lazy chunks
  public void GenerateChunks(Vector2Int chunkCenter)
  {
    Vector2Int lazyGridKey = new Vector2Int(chunkCenter.x -3, chunkCenter.y -3);

    for (int y = 0; y < 7; y++)
    {
      for (int x = 0; x < 7; x++)
      {
        if (worldManager.InactiveChunks.ContainsKey(lazyGridKey))
        {
          Chunk inactiveChunk = worldManager.InactiveChunks[lazyGridKey];

          worldManager.InactiveChunks.Remove(lazyGridKey);
          worldManager.LazyChunks.Add(inactiveChunk.key, inactiveChunk);
        }
        else
        {
          GameObject newChunkObject = new GameObject("Chunk: " + CHUNK_NUMBER + " " + lazyGridKey);
          newChunkObject.transform.parent = worldManager.transform;
          newChunkObject.SetActive(false);
          CHUNK_NUMBER++;

          Chunk newChunk = new Chunk(lazyGridKey, newChunkObject, CHUNK_DIAMETER);
          

          chunkPopulator.PopulateChunk(newChunk);

          worldManager.LazyChunks.Add(newChunk.key, newChunk);
          worldManager.AllChunks.Add(newChunk.key, newChunk); 
        }

        lazyGridKey.x++;
      }

      lazyGridKey.y++;
      lazyGridKey.x -= 7;
    }
  }

  public void SetActiveChunks(Vector2Int chunkCenter, GameObject player)
  {
    Vector2Int activeGridKey = new Vector2Int(chunkCenter.x - 1, chunkCenter.y - 1);

    for (int y = 0; y < 3; y++)
    {
      for (int x = 0; x < 3; x++)
      {
        if (worldManager.LazyChunks.ContainsKey(activeGridKey))
        {
          Chunk lazyChunk = worldManager.LazyChunks[activeGridKey];

          worldManager.LazyChunks.Remove(activeGridKey);
          worldManager.ActiveChunks.Add(lazyChunk.key, lazyChunk);

          foreach (var asteroid in lazyChunk.asteroidSmall)
          {
            var newAsteroidObject = Object.Instantiate(worldManager.smallAsteroidPrefab, asteroid.Value.position, Quaternion.identity);
            asteroid.Value.asteroidObject = newAsteroidObject;

            lazyChunk.chunkObject.SetActive(true);
            newAsteroidObject.transform.parent = lazyChunk.chunkObject.transform;

            NetworkServer.Spawn(newAsteroidObject);
          }
        }
        else if (worldManager.ActiveChunks.ContainsKey(activeGridKey))
        {
          
        }
        else if (worldManager.AllChunks.ContainsKey(activeGridKey))
        {
          Chunk allChunk = worldManager.AllChunks[activeGridKey];

          worldManager.ActiveChunks.Add(allChunk.key, allChunk);
        }
        else
        {
          Debug.LogError("Chunk not found in lazy chunks.");
        }

        activeGridKey.x++;
      }

      activeGridKey.y++;
      activeGridKey.x -= 3;
    }
  }

  public void DeactivateActiveChunks()
  {
    //Active chunks
    if (worldManager.ActiveChunks.Count != 0)
    {
      foreach (var activeChunk in worldManager.ActiveChunks)
      {
        worldManager.InactiveChunks.Add(activeChunk.Key, activeChunk.Value);
        foreach (var asteroid in activeChunk.Value.asteroidSmall)
        {
          NetworkServer.Destroy(asteroid.Value.asteroidObject);
        }
      }

      worldManager.ActiveChunks.Clear();
    }

    //Lazy chunks
    if (worldManager.LazyChunks.Count != 0)
    {
      foreach (var lazyChunk in worldManager.LazyChunks)
      {
        worldManager.InactiveChunks.Add(lazyChunk.Key, lazyChunk.Value);
      }

      worldManager.LazyChunks.Clear();
    }
  }

	public Vector2Int GetChunkPosition(Vector2 position)
	{
		return new Vector2Int(
			Mathf.RoundToInt(position.x / CHUNK_DIAMETER),
			Mathf.RoundToInt(position.y / CHUNK_DIAMETER)
		);
	}
}
