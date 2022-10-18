using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();
    [SerializeField] private RectTransform unitSeleectionArea;
    
    private Camera _mainCamera;
    private Vector2 _startPosition;
    private RTSPlayer _player;

    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private void Start()
    {
        _mainCamera = Camera.main;

        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }
            
            SelectedUnits.Clear(); 
        }
        
        unitSeleectionArea.gameObject.SetActive(true);
        _startPosition = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - _startPosition.x;
        float areaHeight = mousePosition.y - _startPosition.y;

        unitSeleectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSeleectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2 , areaHeight / 2);
    }

    private void ClearSelectionArea()
    {
        unitSeleectionArea.gameObject.SetActive(false);
        if (unitSeleectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)){return;}
        
            if(!hit.collider.TryGetComponent<Unit>(out Unit unit)){return;}
        
            if(!unit.hasAuthority){return;}
        
            SelectedUnits.Add(unit);

            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Select();   
            }
            return;
        }

        Vector2 min = unitSeleectionArea.anchoredPosition - (unitSeleectionArea.sizeDelta / 2);
        Vector2 max = unitSeleectionArea.anchoredPosition + (unitSeleectionArea.sizeDelta / 2);

        foreach (Unit unit in _player.GetMyUnits())
        {
            if(SelectedUnits.Contains(unit)){continue;}
            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);
            if (screenPosition.x > min.x &&
                screenPosition.x < max.x &&
                screenPosition.y > min.y &&
                screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
    }

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }
}
