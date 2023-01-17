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
          worldManager.LazyChunks.Add(inactiveChunk.Key, inactiveChunk);
        }
        else
        {
          GameObject newChunk = new GameObject("Chunk " + CHUNK_NUMBER);
          newChunk.transform.parent = worldManager.transform;
          newChunk.SetActive(false);

          Chunk newChunkInfo = new Chunk(lazyGridKey, CHUNK_DIAMETER);
          CHUNK_NUMBER++;

          worldManager.LazyChunks.Add(newChunkInfo.Key, newChunkInfo);
          worldManager.AllChunks.Add(newChunkInfo.Key, newChunkInfo);      
        }

        lazyGridKey.x++;
      }

      lazyGridKey.y++;
      lazyGridKey.x -= 7;
    }

    Debug.Log("Total chunks: " + worldManager.AllChunks.Count);
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
          worldManager.ActiveChunks.Add(lazyChunk.Key, lazyChunk);

          worldManager.TargetGetChunkFromServer(player.GetComponent<NetworkIdentity>().connectionToClient, lazyChunk.Key);
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