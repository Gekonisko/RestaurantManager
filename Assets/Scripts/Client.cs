using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class Client : MonoBehaviour {
    public static readonly Vector3 POINT_OF_DYING = new Vector3(52, 1, 50);

    public float speed = 10;
    public OrderData order;
    private QueueData _queueData;
    private Animator _animator;
    private IDisposable _completedOrderEvent, _clientQueueEvent;
    private bool _isOrderHasBeenPlaced = false;
    private bool _isGoingToDeath = false;

    private void Awake() {
        order.clientID = GameController.FREE_CLIENT_ID;
        _completedOrderEvent = GameEvents.GetComplitedOrder().Where(clientID => clientID == order.clientID).Subscribe(_ => GetOrder());
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
            while (Vector3.Distance(transform.position, targetWorldPosition) > 0.05f) {
                RotateToPoint(targetWorldPosition);
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, speed * Time.deltaTime);
                yield return null;
            }
            mapPosition = NavMesh.GetPositionFromWorldToMap(transform.position, data.startPosition);
            data.map.walkArea[mapPosition.x].row[mapPosition.y].isWay = false;
            targetMapPosition = NavMesh.GetMinCostWayAround(data.map, mapPosition);
            if (data.map.walkArea[mapPosition.x].row[mapPosition.y].distance == _queueData.queueNumber)
                break;
        }
        while (Vector3.Distance(transform.position, _queueData.position) > 0.05f) {
            RotateToPoint(_queueData.position);
            transform.position = Vector3.MoveTowards(transform.position, _queueData.position, speed * Time.deltaTime);
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
        //transform.position = data.position;
        _queueData = data;
        StartCoroutine("GoToPosition", NavMesh.ReadSavedMap(RestaurantController.RESTAURANT_TRAVEL_MAP_NAME));
        //SetDestination(data.position);
    }

    private void GetOrder() {
        //_agent.SetDestination(POINT_OF_DYING);
        Destroy(gameObject);
        _isGoingToDeath = true;
    }

    private void OnDestroy() {
        _completedOrderEvent?.Dispose();
    }

}