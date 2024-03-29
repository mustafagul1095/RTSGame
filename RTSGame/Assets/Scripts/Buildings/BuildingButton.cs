using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private LayerMask floorMask = new LayerMask();

    private Camera _mainCamera;
    private BoxCollider _buildingCollider;
    private RTSPlayer _player;
    private GameObject _buildingPreviewInstance;
    private Renderer _buildingRendererInstance;

    private void Start()
    {
        _mainCamera = Camera.main;
        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();
        
        _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        
        _buildingCollider = building.GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        if(_buildingPreviewInstance == null){return;}
        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left){return;}
        
        if(_player.GetResources() < building.GetPrice()) {return;}

        _buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        _buildingRendererInstance = _buildingPreviewInstance.GetComponentInChildren<Renderer>();
        
        _buildingPreviewInstance.SetActive(false);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_buildingPreviewInstance == null){return;}

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            _player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }
        Destroy(_buildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)){return;}

        _buildingPreviewInstance.transform.position = hit.point;

        if (!_buildingPreviewInstance.activeSelf)
        {
            _buildingPreviewInstance.SetActive(true);
        }

        Color color = _player.CanPlaceBuilding(_buildingCollider, hit.point) ? Color.green : Color.black;
        
        _buildingRendererInstance.material.SetColor("_BaseColor", color);
    }
}
