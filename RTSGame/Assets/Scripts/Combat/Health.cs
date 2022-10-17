using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int _currentHealth;

    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdated; 

    #region Server

    public override void OnStartServer()
    {
        _currentHealth = maxHealth;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if(_currentHealth == 0){return;}

        _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);
        
        if(_currentHealth != 0){return;}
        
        ServerOnDie?.Invoke();
        
        Debug.Log("We DIED");
    }
    
    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    #endregion
}
