using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ChatBehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject chatUI;
    [SerializeField] private TMP_Text chatText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private float chatDisableTime = 8f;
    
    private static event Action<string> OnMessage;
    private float _timer = 0f;
    

    // private void Update()
    // {
    //     _timer += Time.deltaTime;
    //
    //     if (_timer >= chatDisableTime)
    //     {
    //         chatUI.SetActive(false);
    //         _timer = 0;
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.Return))
    //     {
    //         chatUI.SetActive(true);
    //         _timer = 0;
    //     }
    // }

    public override void OnStartAuthority()
    {
        chatUI.SetActive(true);
        OnMessage += HandleNewMessage;
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if(!hasAuthority){return;}

        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string message)
    {
        chatText.text += message;
    }

    [Client]
    public void Send1()
    {
        Debug.Log("Sent");
        if (!Input.GetKeyDown(KeyCode.Return)){return;}
        if (string.IsNullOrWhiteSpace(inputField.text)){return;}
        CmdSendMessage(inputField.text);
        
        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        Debug.Log("Sent2");
        RpcHandleMessage($"[{connectionToClient.connectionId}]: {message}");
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
        
    }
    
}
