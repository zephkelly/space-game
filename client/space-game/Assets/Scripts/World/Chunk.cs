using UnityEngine;
using Mirror;

public class Chunk
{
  [SyncVar]
  public Vector2Int Key;

  [SyncVar]
  int diameter;

  public Chunk() { }

  public Chunk(Vector2Int _key, int _diameter)
  {
    Key = _key;
    diameter = _diameter;
  }
}