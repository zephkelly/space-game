using UnityEngine;

public class Asteroid
{
  public Vector2 position;
  public GameObject asteroidObject;

  public Asteroid(Vector2 position, GameObject asteroidObject)
  {
    this.position = position;
    this.asteroidObject = asteroidObject;
  }
}