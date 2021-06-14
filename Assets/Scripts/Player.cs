using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField]
    private float speed;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        Moving();
        //transform.position = new Vector3(transform.position.x, 0, transform.position.y);
    }

    void Moving() {
        animator.SetBool("isRunning", true);
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