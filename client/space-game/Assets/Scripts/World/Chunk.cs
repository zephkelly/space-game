using UnityEngine;

public class Chunk
{
  public GameObject chunkObject;

  public Vector2Int key;

  int diameter;

  public Chunk() { }

  public Chunk(Vector2Int _key, GameObject _chunkObj, int _diameter = 50)
  {
    key = _key;
    chunkObject = _chunkObj;
    diameter = _diameter;
  }
}