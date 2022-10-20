using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private float buildingRangeLimit = 5f;
    
    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;

    public event Action<int> ClientOnResourcesUpdated;

    private Color _teamColor = new Color();
    private List<Unit> _myUnits = new List<Unit>();
    private List<Building> _myBuildings = new List<Building>();

    public Color GetTeamColor()
    {
        return _teamColor;
    }
        
    public int GetResources()
    {
        return resources;
    }
    public List<Unit> GetMyUnits()
    {
        return _myUnits;
    }
    
    public List<Building> GetMyBuildings()
    {
        return _myBuildings;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2,
                Quaternion.identity,
                buildingBlockLayer))
        {
            return false;
        }
        
        foreach (Building building in _myBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude <= 
                buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }
    

    #region Server
    

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }
    
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    [Server]
    public void SetTeamColor(Color teamColor)
    {
        _teamColor = teamColor;
    }
    
    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach (Building building in buildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }
        if(buildingToPlace == null){return;}
        
        if(resources < buildingToPlace.GetPrice()){return;}

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        
        if(!CanPlaceBuilding(buildingCollider, point)){return;}

        GameObject buildingInstance = 
            Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
        
        NetworkServer.Spawn(buildingInstance, connectionToClient);
        SetResources(resources - buildingToPlace.GetPrice());
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId){return;}
        _myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId){return;}
        _myUnits.Remove(unit);
    }
    
    private void ServerHandleBuildingSpawned(Building building)
    {
        if(building.connectionToClient.connectionId != connectionToClient.connectionId){return;}
        _myBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if(building.connectionToClient.connectionId != connectionToClient.connectionId){return;}
        _myBuildings.Remove(building);
    }  

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        if(NetworkServer.active){return;}
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        if(!isClientOnly || !hasAuthority){return;}
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }
    
    private void AuthorityHandleBuildingSpawned(Building building)
    {
        _myBuildings.Add(building);
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        _myBuildings.Remove(building);
    } 
    
    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        _myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        _myUnits.Remove(unit);
    } 

    #endregion
}
