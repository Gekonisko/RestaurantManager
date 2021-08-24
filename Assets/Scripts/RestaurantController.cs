using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class RestaurantController : MonoBehaviour {
    private static uint _freeClientID = 1;
    public static uint FREE_CLIENT_ID {
        get {
            return _freeClientID++;
        }
    }

    [SerializeField] private GameObject g_ordersUI;
    [SerializeField] private GameObject g_order;
    [SerializeField] private GameObject g_cookingPanel;
    private IDisposable _orderEvent;
    private IDisposable _cookingPanelEvent;

    IEnumerable das() {
        yield return null;
    }

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