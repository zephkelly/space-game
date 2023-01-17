using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
  public new MyNetworkManager singleton { get; private set; }
  
  [SerializeField]
  private GameObject worldManagerPrefab;
  private GameObject worldManagerObject;
  private WorldManager worldManager;

  //Server ---------------------------------------------------------------
  /// Called on the server when a new client connects.
  public override void OnServerConnect(NetworkConnectionToClient conn)
  {
    base.OnServerConnect(conn);

    if (worldManagerObject == null) {
      worldManagerObject = GameObject.Instantiate(worldManagerPrefab);
      NetworkServer.Spawn(worldManagerObject);
      worldManager = worldManagerObject.GetComponent<WorldManager>();
    }
  }

  /// Called on the server when a client is ready.
  public override void OnServerReady(NetworkConnectionToClient conn)
  {
    //base.OnServerReady(conn);

    NetworkServer.SetClientReady(conn);

    if (conn.isReady) {
      OnServerAddPlayer(conn);
    }
  }

  public override void OnServerAddPlayer(NetworkConnectionToClient conn)
  {
    GameObject newPlayer = Object.Instantiate(playerPrefab);

    NetworkServer.AddPlayerForConnection(conn, newPlayer);
    newPlayer.name = "Player: " + conn.identity.netId;

    worldManager.AddPlayer(newPlayer.transform);
  }

  public override void OnServerDisconnect(NetworkConnectionToClient conn)
  {
    worldManager.RemovePlayer(conn.identity.gameObject.transform);
    
    base.OnServerDisconnect(conn);
  }

  //Client ---------------------------------------------------------------
  public override void OnClientConnect()
  {
    base.OnClientConnect();
  }
  
  public override void OnClientDisconnect()
  {
    base.OnClientDisconnect();

    RemoveCamera();
    RemovePlayer();

    //Check if we are the host
    if (!NetworkServer.active) 
    {
      RemoveWorldManager();
    }
  }

  private void RemoveCamera()
  {
    GameObject clientCamera = GameObject.FindGameObjectWithTag("MainCamera");
    GameObject.Destroy(clientCamera);
  }

  private void RemoveWorldManager()
  {
    if (worldManagerObject == null) return;
    
    GameObject.Destroy(worldManagerObject);
    worldManagerObject = null;
    worldManager = null;
  }

  private void RemovePlayer()
  {
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    GameObject.Destroy(player);

    var starfield = GameObject.FindGameObjectWithTag("StarfieldManager");
    Destroy(starfield);
  }
}
