using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChunkGenerator
{
  private WorldManager worldManager;

	private const int CHUNK_DIAMETER = 50;
	internal int CHUNK_NUMBER;

	private uint seed;

  public ChunkGenerator(WorldManager _worldManager)
  {
    worldManager = _worldManager;
    SetRandomSeed();
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
          GameObject newChunk = new GameObject("Chunk: " + CHUNK_NUMBER + " " + lazyGridKey);
          newChunk.transform.parent = worldManager.transform;
          newChunk.SetActive(false);

          Chunk newChunkInfo = new Chunk(lazyGridKey, newChunk, CHUNK_DIAMETER);
          CHUNK_NUMBER++;

          worldManager.LazyChunks.Add(newChunkInfo.key, newChunkInfo);
          worldManager.AllChunks.Add(newChunkInfo.key, newChunkInfo);      
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

    worldManager.chunksPackage.Clear();

    for (int y = 0; y < 3; y++)
    {
      for (int x = 0; x < 3; x++)
      {
        if (worldManager.LazyChunks.ContainsKey(activeGridKey))
        {
          Chunk lazyChunk = worldManager.LazyChunks[activeGridKey];

          worldManager.LazyChunks.Remove(activeGridKey);
          worldManager.ActiveChunks.Add(lazyChunk.key, lazyChunk);

          worldManager.chunksPackage.Add(lazyChunk.key);
        }
        else if (worldManager.ActiveChunks.ContainsKey(activeGridKey))
        {
          worldManager.chunksPackage.Add(activeGridKey);
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

    worldManager.PrepareActiveChunksPackage(player.GetComponent<NetworkIdentity>().connectionToClient);
  }

  public void DeactivateActiveChunks()
  {
    //Active chunks
    if (worldManager.ActiveChunks.Count != 0)
    {
      foreach (var activeChunk in worldManager.ActiveChunks)
      {
        worldManager.InactiveChunks.Add(activeChunk.Key, activeChunk.Value);
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

	private void SetRandomSeed() => seed = (uint)Random.Range(0, int.MaxValue);

	public void SetSeed(uint _seed) => seed = _seed;
}
