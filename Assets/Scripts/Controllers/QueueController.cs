using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class QueueController : MonoBehaviour {
    public static readonly int NO_QUEUE = -1;

    [SerializeField] private Transform _restaurantTravelPoint, _clientPointOfDying;
    private List<QueueData> clientsQueue = new List<QueueData>();
    private IDisposable _clientsQueueEvent, _compliteOrderEvent;

    void Awake() {
        GameController.CLIENT_POINT_OF_DYING = _clientPointOfDying.position;
        _clientsQueueEvent = GameEvents.GetClientsQueue().Where(data => !data.isThisFreePositionInQueue).Subscribe(data => SetPositionInQueue(data));
        _compliteOrderEvent = GameEvents.GetComplitedOrder().Subscribe(clientID => RemoveClientFormQueue(clientID));
    }

    private void RemoveClientFormQueue(uint clientID) {
        foreach (QueueData client in clientsQueue) {
            if (client.clientID == clientID) {
                clientsQueue.Remove(client);
                break;
            }
        }
        for (int i = 0; i < clientsQueue.Count; i++)
            clientsQueue[i] = new QueueData(clientsQueue[i].clientID, clientsQueue[i].position, clientsQueue[i].isThisFreePositionInQueue, clientsQueue[i].queueNumber - 1);

        ResendNewPositionInQueue();
    }

    private void ResendNewPositionInQueue() {
        foreach (QueueData client in clientsQueue) {
            Vector3 pos = GetPositionInQueue(client.queueNumber);
            GameEvents.SetClientsQueue(new QueueData(client.clientID, pos, true, client.queueNumber));
        }
    }

    private void CreateClientDyingMap() {
        if (NavMesh.IsMapExistInResources(Client.DYING_MAP_NAME)) return;
        NavMeshMapData map = NavMesh.CreateWayMap(NavMesh.GetPositionFromWorldToMap(_clientPointOfDying.position, NavMesh.START_POSITION), NavMesh.BASE_MAP);
        NavMesh.SaveMap(map, Client.DYING_MAP_NAME, _clientPointOfDying.position);
    }

    private void CreateQueueMap(int maxPeopleInQueue) {
        if (NavMesh.IsMapExistInResources(RestaurantController.RESTAURANT_TRAVEL_MAP_NAME)) return;
        NavMeshMapData map = NavMesh.BASE_MAP;
        Vector2Int positionOnMap = NavMesh.GetPositionFromWorldToMap(GetPositionInQueue(0), NavMesh.START_POSITION);
        for (int i = 1; i < maxPeopleInQueue; i++) {
            map = NavMesh.ConnectTwoPoints(map, NavMesh.GetPositionFromWorldToMap(GetPositionInQueue(i - 1), NavMesh.START_POSITION), NavMesh.GetPositionFromWorldToMap(GetPositionInQueue(i), NavMesh.START_POSITION), (byte) i);
        }
        map = NavMesh.CreateWayMap(NavMesh.GetPositionFromWorldToMap(GetPositionInQueue(maxPeopleInQueue - 1), NavMesh.START_POSITION), map, (byte) (maxPeopleInQueue - 1));
        NavMesh.SaveMap(map, RestaurantController.RESTAURANT_TRAVEL_MAP_NAME, new Vector3(0, 0, 0));
    }

    private Vector3 GetPositionInQueue(int clientsAfterYou) {
        float muliplier = 12;
        float x = Mathf.Sqrt(clientsAfterYou);
        if (clientsAfterYou == 0)
            return _restaurantTravelPoint.position;
        return _restaurantTravelPoint.position + new Vector3(-x * muliplier + 8, 1, (0.125f * Mathf.Pow(x, 2)) * muliplier);
    }

    private void SetPositionInQueue(QueueData queueData) {
        Vector3 pos = GetPositionInQueue(clientsQueue.Count);
        GameEvents.SetClientsQueue(new QueueData(queueData.clientID, pos, true, clientsQueue.Count));
        queueData.queueNumber = clientsQueue.Count;
        clientsQueue.Add(queueData);
    }

    private void OnDestroy() {
        _clientsQueueEvent?.Dispose();
        _compliteOrderEvent?.Dispose();
    }
}