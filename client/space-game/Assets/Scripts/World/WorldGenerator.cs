using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WorldGenerator
{
	private const int CHUNK_SIZE = 16;
	private const int CHUNK_SQUARED = CHUNK_SIZE * CHUNK_SIZE;

	private uint seed;

	[Server]
	public void SetSeed(uint _seed)
	{
		seed = _seed;
	}

	[Server]
	public void GenerateWorld(Vector2[] players)
	{
		// Generate the world
		// Generate the chunks
		// Generate the asteroids
		
		// Place the players
	}
}
