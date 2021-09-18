using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;

public class RestaurantController : MonoBehaviour {
    public static readonly string RESTAURANT_TRAVEL_MAP_NAME = "RestaurantTravelPoint";

    [SerializeField] private GameObject g_ordersUI;
    [SerializeField] private GameObject g_order;
    [SerializeField] private GameObject g_cookingPanel;
    [SerializeField] private GameObject g_RestaurantTravelPoint;
    private IDisposable _orderEvent;
    private IDisposable _cookingPanelEvent;

    private void Awake() {
        _orderEvent = GameEvents.GetClientOrder().Subscribe(data => CreateOrder(data));
        _cookingPanelEvent = GameEvents.GetShowCookingPanel().Where(_ => g_cookingPanel.activeInHierarchy == false).Subscribe(data => ShowCookingPanel(data));
    }

    private void Start() {
        // NavMeshMapData map = NavMesh.CreateWayMap(NavMesh.GetPositionFromWorldToMap(g_RestaurantTravelPoint.transform.position, NavMesh.GetActualStartPosition()));

        CreateWayMapToRestaurant();
        NavMesh.PrintWalkableArea(NavMesh.ReadSavedMap(RESTAURANT_TRAVEL_MAP_NAME).map);

    }

    private void CreateWayMapToRestaurant() {
        if (NavMesh.IsMapExistInResources(RESTAURANT_TRAVEL_MAP_NAME)) return;
        NavMeshMapData map = NavMesh.CreateWayMap(NavMesh.GetPositionFromWorldToMap(g_RestaurantTravelPoint.transform.position, NavMesh.START_POSITION), NavMesh.BASE_MAP);
        NavMesh.SaveMap(map, RESTAURANT_TRAVEL_MAP_NAME, g_RestaurantTravelPoint.transform.position);
    }

    private void ShowCookingPanel(uint cookID) {
        g_cookingPanel.SetActive(true);
        g_cookingPanel.GetComponent<CookingPanelController>().cookID = cookID;
    }

    private void CreateOrder(OrderData orderData) {
        Instantiate(g_order, g_order.transform.position, g_order.transform.rotation, g_ordersUI.transform).GetComponent<OrderSetter>().SetParameters(orderData);
    }

    private void OnDestroy() {
        _orderEvent?.Dispose();
        _cookingPanelEvent?.Dispose();
    }
}