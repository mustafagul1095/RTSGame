using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable _target;

    public Targetable GetTarget()
    {
        return _target;
    }

    #region Server

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if(!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)){return;}

        _target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        _target = null;
    }

    #endregion

}