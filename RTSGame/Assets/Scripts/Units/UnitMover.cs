using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMover : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    private Camera _mainCamera;

    #region Server

    [Command]
    public void CmdMove(Vector3 position)
    {
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)){return;}
        
        agent.SetDestination(position);
    }

    #endregion

    #region Client
    

    #endregion
}
