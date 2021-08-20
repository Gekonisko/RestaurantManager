using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class Client : MonoBehaviour {
    public static readonly Vector3 POINT_OF_DYING = new Vector3(52, 1, 50);

    public OrderData order;
    private NavMeshAgent _agent;
    private Animator _animator;
    private IDisposable _completedOrderEvent;
    private bool _isGoing = false;
    private bool _isOrderHasBeenPlaced = false;
    private bool _isGoingToDeath = false;

    private void Awake() {
        _completedOrderEvent = GameEvents.GetComplitedOrder().Where(clientID => clientID == order.clientID).Subscribe(_ => GetOrder());
        order.clientID = RestaurantController.FREE_CLIENT_ID;
    }

    private void Start() {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(RestaurantController.POSITION_TO_ORDERS);
        _isGoing = true;
    }

    private void Update() {
        if (_isGoing && !_agent.pathPending) {
            if (_agent.remainingDistance <= _agent.stoppingDistance) {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f) {
                    OnDestinationComplete();
                    _isGoing = false;
                }
            }
        }

        MoveAnimation();
    }

    private void MoveAnimation() {
        if (_agent.velocity == Vector3.zero)
            _animator.SetBool("isRunning", false);
        else
            _animator.SetBool("isRunning", true);
    }

    private void OnDestinationComplete() {
        if (!_isOrderHasBeenPlaced) {
            _isOrderHasBeenPlaced = true;
            GameEvents.SetClientOrder(order);
        }
        if (_isGoingToDeath) {
            Destroy(gameObject);
        }
    }

    private void GetOrder() {
        _agent.SetDestination(POINT_OF_DYING);
        _isGoing = true;
        _isGoingToDeath = true;
    }

    private void OnDestroy() {
        _completedOrderEvent?.Dispose();
    }

}