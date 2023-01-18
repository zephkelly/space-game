using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;

public class WorldManager : NetworkBehaviour
{
  private static ChunkGenerator chunkGenerator;

  [SerializeField]
  private Transform clientPlayer;  //Always active
  private NetworkIdentity clientNetIdentity;

  [SerializeField]
  private List<Transform> players = new List<Transform>();

  // Chunks -----------------------------------------------

  private Dictionary<Transform, Vector2Int> playerChunkPositions = new Dictionary<Transform, Vector2Int>();

  private readonly SyncDictionary<Vector2Int, Chunk> activeChunks = new SyncDictionary<Vector2Int, Chunk>();

  private readonly SyncDictionary<Vector2Int, Chunk> lazyChunks = new SyncDictionary<Vector2Int, Chunk>();

  private Dictionary<Vector2Int, Chunk> inactiveChunks = new Dictionary<Vector2Int, Chunk>();

  private Dictionary<Vector2Int, Chunk> allChunks = new Dictionary<Vector2Int, Chunk>();

  private float chunkFetchTimer = 0;
  private float fetchInterval = 0.2f;

  // Getters -----------------------------------------------

  public Dictionary<Vector2Int, Chunk> AllChunks => allChunks;

  public Dictionary<Vector2Int, Chunk> InactiveChunks => inactiveChunks;

  public SyncDictionary<Vector2Int, Chunk> ActiveChunks => activeChunks;

  public SyncDictionary<Vector2Int, Chunk> LazyChunks => lazyChunks;

  private void Start()
  {
    chunkGenerator = new ChunkGenerator(this);

    if (isServer) 
    {
      GenerateInitialChunks();
    }
  }
  
  [Server]
  public void AddPlayer(Transform player)
  {
    players.Add(player);

    if (playerChunkPositions.ContainsKey(player)) return;
    playerChunkPositions.Add(player, Vector2Int.zero);
  }

  [Server]
  public void RemovePlayer(Transform player) 
  {
    players.Remove(player);

    if (playerChunkPositions.ContainsKey(player)) {
      playerChunkPositions.Remove(player);
    }
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
      return;
    }

    //If we are a client
    GetClientChunkPosition();
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

  public List<Vector2Int> chunksPackage = new List<Vector2Int>();
  private List<Vector2Int> lastChunkPackage = new List<Vector2Int>();

  [Server]
  public void PrepareActiveChunksPackage(NetworkConnection target)
  {
    var deltaChunks = new List<Vector2Int>();

    //Find identical chunks
    foreach (var chunk in chunksPackage) {
      if (lastChunkPackage.Contains(chunk)) continue;

      deltaChunks.Add(chunk);
    }

    lastChunkPackage.Clear();

    // Bundle package
    Vector2Int[] activeChunksBundle = new Vector2Int[deltaChunks.Count];

    for (int i = 0; i < activeChunksBundle.Length; i++) {
      activeChunksBundle[i] = deltaChunks[i];
    }

    TargetSendActiveChunksPackage(target, activeChunksBundle);
    chunksPackage.CopyTo(lastChunkPackage);
    chunksPackage.Clear();
  }

  [TargetRpc]
  public void TargetSendActiveChunksPackage(NetworkConnection target, Vector2Int[] chunkPackage)
  {
    if (target.identity.isClient && isServer) {
      Debug.Log("We are hosting!");
      return;
    }
    
    Debug.Log("Spawning " + chunkPackage.Length + " chunks");
  }

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