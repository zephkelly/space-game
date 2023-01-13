using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public static new MyNetworkManager singleton { get; private set; }
    
    /// Called on the server when a new client connects.
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
				base.OnServerConnect(conn);
    }

    /// Called on the server when a client is ready.
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
			base.OnServerReady(conn);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
      base.OnServerAddPlayer(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }

		public override void OnClientConnect()
    {
        base.OnClientConnect();
    }
		
		public override void OnClientDisconnect()
		{
			GameObject clientCamera = GameObject.FindGameObjectWithTag("MainCamera");
			GameObject.Destroy(clientCamera);
		}
}
