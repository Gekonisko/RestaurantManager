using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class QueueController : MonoBehaviour {
    public readonly static Vector3 POSITION_TO_ORDERS = new Vector3(7, 1, 14);

    private List<QueueData> clientsQueue = new List<QueueData>();
    private IDisposable _clientsQueueEvent;

    void Awake() {
        _clientsQueueEvent = GameEvents.GetClientsQueue().Where(data => !data.isThisFreePositionInQueue).Subscribe(data => SetPositionInQueue(data));
    }

    public Vector3 GetPositionInQueue(int clientsAfterYou) {
        float xPos = POSITION_TO_ORDERS.x - clientsAfterYou * 3;
        return new Vector3(xPos, 1, (0.125f * Mathf.Pow(xPos, 2)) - (1.75f * xPos) + 20.12f);
    }

    private void SetPositionInQueue(QueueData queueData) {
        Vector3 pos = GetPositionInQueue(clientsQueue.Count);
        GameEvents.SetClientsQueue(new QueueData(queueData.clientID, pos, true));
        clientsQueue.Add(queueData);
    }

    private void OnDestroy() {
        _clientsQueueEvent?.Dispose();
    }
}