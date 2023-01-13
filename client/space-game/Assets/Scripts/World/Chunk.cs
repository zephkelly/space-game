using UnityEngine;

public class Chunk
{
  int keyX;
  int keyY;

  public Vector2 Key => new Vector2(keyX, keyY);

  public Chunk(int keyX, int keyY)
  {
    this.keyX = keyX;
    this.keyY = keyY;
  }
}