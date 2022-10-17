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
    [SerializeField] private Targeter _targeter;
    [SerializeField] private float chaseRange = 10;

    private Camera _mainCamera;

    #region Server

    [ServerCallback]
    private void Update()
    {
        Targetable target = _targeter.GetTarget();
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }
            return;
        }
        if(!agent.hasPath){return;}
        if(agent.remainingDistance > agent.stoppingDistance){return;}
        
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        _targeter.ClearTarget();
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)){return;}
        
        agent.SetDestination(position);
    }

    #endregion

    #region Client
    

    #endregion
}
