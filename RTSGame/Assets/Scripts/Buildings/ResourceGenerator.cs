using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float _timer;
    private RTSPlayer _player;

    public override void OnStartServer()
    {
        _timer = interval;
        _player = connectionToClient.identity.GetComponent<RTSPlayer>();

        _health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        _health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _player.SetResources(_player.GetResources() + resourcesPerInterval);
            
            _timer += interval;
        }
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }
}
