using UnityEngine;
using Mirror;

public class WorldManager : MonoBehaviour
{
  private WorldGenerator worldGenerator;

  private void Awake() { }

  private void Start()
  {
    worldGenerator = new WorldGenerator();
  }
}