using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMover unitMover;
    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;

    public UnitMover GetUnitMover()
    {
        return unitMover;
    }
    #region Client

    [Client]
    public void Select()
    {
        if(!hasAuthority){return;}
        
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if(!hasAuthority){return;}
        
        onDeselected?.Invoke();
    }

    #endregion
}
