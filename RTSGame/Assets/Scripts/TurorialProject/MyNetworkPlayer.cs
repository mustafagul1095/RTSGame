using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private Renderer displayColoRenderer;
    
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] 
    [SerializeField] private string displayName = "Missing Name";
    
    [SyncVar(hook = nameof(HandleDisplayColourUpdated))] 
    [SerializeField] private Color displayColor = Color.black;

    #region Server
    
    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }
    
    [Server]
    public void SetDisplayColor(Color newDisplayColor)
    {
        displayColor = newDisplayColor;
    }

    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        RpcLogNewName(newDisplayName);
        SetDisplayName(newDisplayName);
    }
    #endregion

    #region Client

    private void HandleDisplayColourUpdated(Color oldColor, Color newColor)
    {
        displayColoRenderer.material.SetColor("_BaseColor", newColor);
    }

    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        displayNameText.SetText(newName);
    }

    [ContextMenu("Set My Name")] 
    private void SetMyName()
    {
        CmdSetDisplayName("My New Name");
    }

    [ClientRpc]
    private void RpcLogNewName(string newDisplayName)
    {
        Debug.Log(newDisplayName);
    }
    #endregion
}
