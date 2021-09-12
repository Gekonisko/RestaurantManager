using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;

public class RestaurantController : MonoBehaviour {
    [SerializeField] private GameObject g_ordersUI;
    [SerializeField] private GameObject g_order;
    [SerializeField] private GameObject g_cookingPanel;
    private IDisposable _orderEvent;
    private IDisposable _cookingPanelEvent;

    private void Awake() {
        _orderEvent = GameEvents.GetClientOrder().Subscribe(data => CreateOrder(data));
        _cookingPanelEvent = GameEvents.GetShowCookingPanel().Where(_ => g_cookingPanel.activeInHierarchy == false).Subscribe(data => ShowCookingPanel(data));
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