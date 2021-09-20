using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class Client : MonoBehaviour {
    public static readonly string DYING_MAP_NAME = "ClientDyingMap";

    public float speed = 10;
    public OrderData order;
    private QueueData _queueData;
    private Animator _animator;
    private IDisposable _completedOrderEvent, _clientQueueEvent;
    private bool _isOrderHasBeenPlaced = false;
    private bool _isGoingToDeath = false;

    private void Awake() {
        order.clientID = GameController.FREE_CLIENT_ID;
        _completedOrderEvent = GameEvents.GetComplitedOrder().Where(clientID => clientID == order.clientID).Subscribe(_ => GetCompleteOrder());
        _clientQueueEvent = GameEvents.GetClientsQueue().Where(data => data.clientID == order.clientID && data.isThisFreePositionInQueue).Subscribe(data => SetDestinationInQueue(data));
    }

    private void Start() {
        _animator = GetComponent<Animator>();
        GameEvents.SetClientsQueue(new QueueData(order.clientID, transform.position));

    }

    private void OnDestinationComplete() {
        if (!_isOrderHasBeenPlaced && _queueData.queueNumber == 0) {
            _isOrderHasBeenPlaced = true;
            GameEvents.SetClientOrder(order);
        }
        if (_isGoingToDeath) {
            Destroy(gameObject);
        }
    }

    private IEnumerator GoToPosition(NavMeshWalkArea data) {
        _animator.SetBool("isRunning", true);
        Vector2 worldPosition;
        Vector3 targetWorldPosition;
        Vector2Int mapPosition = NavMesh.GetPositionFromWorldToMap(transform.position, data.startPosition);
        data.map.walkArea[mapPosition.x].row[mapPosition.y].isWay = false;
        Vector2Int targetMapPosition = NavMesh.GetMinCostWayAround(data.map, mapPosition);
        while (targetMapPosition != NavMesh.POSITION_NOT_FOUND) {
            worldPosition = NavMesh.GetPositionFromMapToWorld(targetMapPosition, data.startPosition);
            targetWorldPosition = new Vector3(worldPosition.x, transform.position.y, worldPosition.y);
            RotateToPoint(targetWorldPosition);
            while (Vector3.Distance(transform.position, targetWorldPosition) > 0.05f) {
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, speed * Time.deltaTime);
                yield return null;
            }
            mapPosition = NavMesh.GetPositionFromWorldToMap(transform.position, data.startPosition);
            data.map.walkArea[mapPosition.x].row[mapPosition.y].isWay = false;
            targetMapPosition = NavMesh.GetMinCostWayAround(data.map, mapPosition);
            if (data.map.walkArea[mapPosition.x].row[mapPosition.y].distance == _queueData.queueNumber)
                break;
        }
        RotateToPoint(data.worldDestinationPosition);
        while (Vector3.Distance(transform.position, data.worldDestinationPosition) > 0.05f) {
            transform.position = Vector3.MoveTowards(transform.position, data.worldDestinationPosition, speed * Time.deltaTime);
            yield return null;
        }
        _animator.SetBool("isRunning", false);
        OnDestinationComplete();
        yield return null;
    }

    private void RotateToPoint(Vector3 point) {
        Vector3 direction = point - transform.position;
        direction = Vector3.Normalize(direction);
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);
    }

    private void SetDestinationInQueue(QueueData data) {
        _queueData = data;
        NavMeshWalkArea map = NavMesh.ReadSavedMap(RestaurantController.RESTAURANT_TRAVEL_MAP_NAME);
        map.worldDestinationPosition = _queueData.position;
        StartCoroutine("GoToPosition", map);
    }

    private void GetCompleteOrder() {
        _queueData.queueNumber = QueueController.NO_QUEUE;
        _isGoingToDeath = true;
        StartCoroutine("GoToPosition", NavMesh.ReadSavedMap(DYING_MAP_NAME));
    }

    private void OnDestroy() {
        _completedOrderEvent?.Dispose();
    }

}