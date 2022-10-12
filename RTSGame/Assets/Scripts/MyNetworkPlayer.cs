using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar] [SerializeField] private string displayName = "Missing Name";

    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }
}
