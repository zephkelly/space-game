using UnityEngine;

public class Asteroid
{
  int positionX;
  int positionY;

  public Vector2 Position => new Vector2(positionX, positionY);

  public Asteroid(int positionX, int positionY)
  {
    this.positionX = positionX;
    this.positionY = positionY;
  }
}