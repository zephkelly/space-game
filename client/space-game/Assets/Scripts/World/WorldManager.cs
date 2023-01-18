using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;

public class WorldManager : NetworkBehaviour
{
  private static ChunkGenerator chunkGenerator;
  private static ChunkPopulator chunkPopulator;

  [SerializeField]
  private Transform clientPlayer;  //Always active
  private NetworkIdentity clientNetIdentity;

  [SerializeField]
  private List<Transform> players = new List<Transform>();

  [SerializeField]
  public GameObject smallAsteroidPrefab;

  // Chunks -----------------------------------------------

  private Dictionary<Transform, Vector2Int> playerChunkPositions = new Dictionary<Transform, Vector2Int>();

  private Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();

  private Dictionary<Vector2Int, Chunk> lazyChunks = new Dictionary<Vector2Int, Chunk>();

  private Dictionary<Vector2Int, Chunk> inactiveChunks = new Dictionary<Vector2Int, Chunk>();

  private Dictionary<Vector2Int, Chunk> allChunks = new Dictionary<Vector2Int, Chunk>();

  private float chunkFetchTimer = 0;
  private float fetchInterval = 0.2f;

  // Getters -----------------------------------------------

  public Dictionary<Vector2Int, Chunk> AllChunks => allChunks;
  public Dictionary<Vector2Int, Chunk> InactiveChunks => inactiveChunks;
  public Dictionary<Vector2Int, Chunk> LazyChunks => lazyChunks;
  public Dictionary<Vector2Int, Chunk> ActiveChunks => activeChunks;

  private void Start()
  {
    chunkPopulator = new ChunkPopulator(this);
    chunkGenerator = new ChunkGenerator(this, chunkPopulator);

    if (isServer) 
    {
      GenerateInitialChunks();
    }
  }
  
  [Server]
  public void AddPlayer(Transform player)
  {
    players.Add(player);

    playerChunkPositions.Add(player, Vector2Int.zero);

    chunkGenerator.SetActiveChunks(Vector2Int.zero, player.gameObject);
  }

  [Server]
  public void RemovePlayer(Transform player) 
  {
    players.Remove(player);

    playerChunkPositions.Remove(player);
  }

  [Server]
  private void GenerateInitialChunks()
  {
    chunkGenerator.GenerateChunks(Vector2Int.zero);
  }
  

  private void FixedUpdate()
  {
    FetchChunkPositions();
  }

  private void FetchChunkPositions()
  {
    if (chunkFetchTimer > 0) {
      chunkFetchTimer -= Time.fixedDeltaTime;
      return;
    }

    chunkFetchTimer = fetchInterval;

    //If we are purely a server
    if (clientPlayer == null) {
      if (players.Count <= 0) return;
      GeneratePlayerChunks();
      return;
    }
    
    //If we are a host
    if (clientPlayer != null && isServer) {
      if (players.Count <= 0) return;

      GeneratePlayerChunks();
      //GetClientChunkPosition();
      return;
    }

    //If we are a client
    //GetClientChunkPosition();
  }

  [Server]
  private void GeneratePlayerChunks()
  {
    for (int i = 0; i < players.Count; i++)
    {
      Vector2Int chunkPosition = chunkGenerator.GetChunkPosition(players[i].position);

      if (playerChunkPositions[players[i]] == chunkPosition) continue;

      playerChunkPositions[players[i]] = chunkPosition;

      chunkGenerator.DeactivateActiveChunks();
      chunkGenerator.GenerateChunks(chunkPosition);

      chunkGenerator.SetActiveChunks(chunkPosition, players[i].gameObject);
    }
  }

  /*
  public List<Vector2Int> chunksPackage = new List<Vector2Int>();

  private Dictionary<NetworkConnection, List<Vector2Int>> lastChunkPackages = new Dictionary<NetworkConnection, List<Vector2Int>>();

  [Server]
  public void PrepareActiveChunksPackage(NetworkConnection target)
  {
    //Find identical chunks -------------------------------------
    var deltaChunks = new List<Vector2Int>();

    foreach (var chunk in chunksPackage) 
    {
      if (lastChunkPackages[target].Contains(chunk)) continue;

      deltaChunks.Add(chunk);
    }

    // Bundle package -------------------------------------------
    Vector2Int[] activeChunksBundle = new Vector2Int[deltaChunks.Count];

    for (int i = 0; i < activeChunksBundle.Length; i++) {
      activeChunksBundle[i] = deltaChunks[i];
    }

    //Send package to client ------------------------------------
    if (target.identity != NetworkServer.localConnection.identity) {
      TargetSendActiveChunksPackage(target, activeChunksBundle);
    } else {
      foreach (var chunk in activeChunksBundle) {
        allChunks[chunk].chunkObject.SetActive(true);
      }
    }

    //Update last package dictionary ----------------------------
    if (lastChunkPackages.ContainsKey(target)) 
    {
      lastChunkPackages[target].Clear();
      chunksPackage.CopyTo(lastChunkPackages[target]);
    } 
    else {
      lastChunkPackages.Add(target, chunksPackage);
    }

    chunksPackage.Clear();
  }

  internal int CHUNK_NUMBER;
  [TargetRpc]
  public void TargetSendActiveChunksPackage(NetworkConnection target, Vector2Int[] chunkPackage)
  {
    foreach (var chunk in chunkPackage) {
      GameObject newChunk = new GameObject("Chunk: " + CHUNK_NUMBER + " " + chunk);
      newChunk.transform.parent = this.transform;

      Chunk newChunkInfo = new Chunk(chunk, newChunk);

      //TEMP ---
      if (!activeChunks.ContainsKey(chunk)){
        activeChunks.Add(chunk, newChunkInfo);
        CHUNK_NUMBER++;
      }
    }

    //Do a check for any chunks now lazy and send a command to the server updating the state of the chunk
  }
  */

  [Client]
  private void GetClientChunkPosition()
  {
    Vector2Int chunkPosition = chunkGenerator.GetChunkPosition(clientPlayer.position);

    if (playerChunkPositions[clientPlayer] == chunkPosition) return;

    playerChunkPositions[clientPlayer] = chunkPosition;
  }
 
  [Client]
  public void AddClientPlayer(Transform player) 
  {
    clientPlayer = player;
    clientNetIdentity = clientPlayer.GetComponent<NetworkIdentity>();

    if (playerChunkPositions.ContainsKey(clientPlayer)) return;
    playerChunkPositions.Add(clientPlayer, Vector2Int.zero);
  }
}