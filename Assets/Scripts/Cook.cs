using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class Cook : MonoBehaviour {

    public uint cookID = 0;
    public CookingTimeData cookingTimeData;
    [SerializeField] private bool isCooking = false, hasCookingTask = false;
    //[SerializeField] private float rotationSmooth;
    [SerializeField] private GameObject g_cookingParticle;
    private NavMeshAgent agent;
    private Animator animator;
    private IDisposable _cookTaskEvent, _machineEvent, _cookedMealEvent;
    private MachineData bestMachineToGo;
    private Vector3 startingPosition;
    private uint _freeMachineCount = 0;

    private void Awake() {
        _cookTaskEvent = GameEvents.GetCookTask().Where(data => data.cookID == cookID && !hasCookingTask).Subscribe(data => CheckFreeMachines(data));
        _machineEvent = GameEvents.GetMachine().Where(data => data.cookID == cookID && !hasCookingTask).Subscribe(data => SetDestination(data));
        _cookedMealEvent = GameEvents.GetCookedMeal().Where(data => data.cookID == cookID).Subscribe(data => GoToStartPosition());
    }

    private void Start() {
        startingPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        if (!isCooking && hasCookingTask && !agent.pathPending) {
            if (agent.remainingDistance <= agent.stoppingDistance) {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                    isCooking = true;
                    Debug.Log("Wysyłam czas gotowania do maszyny ID=" + bestMachineToGo.machineID.ToString() + bestMachineToGo.rotation);
                    //StartCoroutine("RotateToMachine");
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, bestMachineToGo.rotation.y, transform.eulerAngles.z);
                    g_cookingParticle.SetActive(true);
                    animator.SetBool("isCooking", true);
                    GameEvents.SetCookingTime(new CookingTimeData(cookingTimeData.cookingTime, cookingTimeData.percentOfWellCookedTime, cookingTimeData.percentOfBadCookedTime, bestMachineToGo.machineID));
                }
            }
        }
        MoveAnimation();
    }

    private void GoToStartPosition() {
        hasCookingTask = false;
        isCooking = false;
        animator.SetBool("isCooking", false);
        g_cookingParticle.SetActive(false);
        agent.SetDestination(startingPosition);
    }

    IEnumerator RotateToMachine() {
        while (transform.eulerAngles.y != bestMachineToGo.rotation.z) {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Lerp(transform.eulerAngles.y, bestMachineToGo.rotation.z, Time.deltaTime), transform.eulerAngles.z);
            yield return null;
        }
    }

    void MoveAnimation() {
        if (agent.velocity == Vector3.zero)
            animator.SetBool("isRunning", false);
        else
            animator.SetBool("isRunning", true);
    }

    private void CheckFreeMachines(CookTaskData data) {
        Debug.Log("Doszło zlecenie do kucharza o numerze " + cookID.ToString());
        GameEvents.SetFreeMachine(data);
    }

    private void SetDestination(MachineData data) {
        if (_freeMachineCount == 0)
            bestMachineToGo = data;
        bestMachineToGo = GetBetterMachine(bestMachineToGo, data);
        _freeMachineCount++;
        if (_freeMachineCount != 3) return;
        _freeMachineCount = 0;
        if (bestMachineToGo.machineState == MachineState.Busy) {
            Debug.Log("Nie ma wolnych maszyn");
            return;
        }
        hasCookingTask = true;
        Debug.Log("Idę do maszyny w pozycji " + bestMachineToGo.machineID.ToString() + " o levelu " + bestMachineToGo.level);
        agent.SetDestination(bestMachineToGo.position);
        GameEvents.SetChosenMachine(new ChosenMachineData(cookID, bestMachineToGo.machineID));
    }

    private MachineData GetBetterMachine(MachineData machine1, MachineData machine2) {
        if (machine1.machineState == MachineState.Busy && machine2.machineState == MachineState.Free)
            return machine2;
        if (machine2.level > machine1.level && machine2.machineState == MachineState.Free)
            return machine2;
        return machine1;
    }

    private void OnDestroy() {
        _cookTaskEvent?.Dispose();
        _machineEvent?.Dispose();
        _cookedMealEvent?.Dispose();
    }
}