using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        player.SetDisplayName($"Player {numPlayers}");
    }
}
