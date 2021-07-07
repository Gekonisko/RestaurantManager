using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Client : MonoBehaviour {

    public OrderData order;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isOrderHasBeenPlaced = false;

    private void Start() {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(RestaurantController.POSITION_TO_ORDERS);
    }

    private void Update() {
        if (!isOrderHasBeenPlaced && !agent.pathPending) {
            if (agent.remainingDistance <= agent.stoppingDistance) {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                    GameEvents.SetClientOrder(order);
                    isOrderHasBeenPlaced = true;
                }
            }
        }

        MoveAnimation();
    }

    void MoveAnimation() {
        if (agent.velocity == Vector3.zero)
            animator.SetBool("isRunning", false);
        else
            animator.SetBool("isRunning", true);
    }

}