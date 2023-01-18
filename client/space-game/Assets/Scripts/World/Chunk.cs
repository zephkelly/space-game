using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
  public GameObject chunkObject;

  public Vector2Int key;
  public readonly Bounds chunkBounds;

  private Vector2Int chunkWorldPosition;

  public Dictionary<Vector2, Asteroid> asteroidSmall = new Dictionary<Vector2, Asteroid>();
  public Dictionary<Vector2, Asteroid> asteroidMedium = new Dictionary<Vector2, Asteroid>();
  public Dictionary<Vector2, Asteroid> asteroidLarge = new Dictionary<Vector2, Asteroid>();
  public Dictionary<Vector2, Asteroid> asteroidHuge = new Dictionary<Vector2, Asteroid>();

  int diameter;

  public Chunk(Vector2Int _key, GameObject _chunkObj, int _diameter = 50)
  {
    key = _key;
    chunkObject = _chunkObj;
    diameter = _diameter;

    chunkWorldPosition = key * diameter;
    chunkBounds =  new Bounds((Vector2)chunkWorldPosition, Vector2.one * diameter);
  }
}