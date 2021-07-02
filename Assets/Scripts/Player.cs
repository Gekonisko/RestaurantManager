using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {

    [SerializeField] Camera cam;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] private float speed;
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {

        if (Input.GetMouseButtonDown(0)) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                agent.SetDestination(hit.point);
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

    void MovingByKayboard() {

        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, 180, 0);
            Debug.Log("UpArrow");
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            transform.position += new Vector3(0, 0, speed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, 0, 0);
            Debug.Log("DownArrow");
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
            transform.eulerAngles = new Vector3(0, 90, 0);
            Debug.Log("LeftArrow");
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
            transform.eulerAngles = new Vector3(0, 270, 0);
            Debug.Log("RightArrow");
        } else
            animator.SetBool("isRunning", false);

    }

}