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
    private IDisposable _cookTaskEvent, _machineEvent;
    private MachineData bestMachineToGo;
    private uint _freeMachineCount = 0;

    private void Awake() {
        _cookTaskEvent = GameEvents.GetCookTask().Where(data => data.cookID == cookID).Subscribe(data => CheckFreeMachines(data));
        _machineEvent = GameEvents.GetMachine().Where(data => data.cookID == cookID).Subscribe(data => SetDestination(data));
    }

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        if (!isCooking && hasCookingTask && !agent.pathPending) {
            if (agent.remainingDistance <= agent.stoppingDistance) {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                    isCooking = true;
                    GameEvents.SetCookingTime(new CookingTimeData(cookingTimeData.cookingTime, cookingTimeData.percentOfWellCookedTime, cookingTimeData.percentOfBadCookedTime, bestMachineToGo.machineID));
                    Debug.Log("Wysyłam czas gotowania do maszyny ID=" + bestMachineToGo.machineID.ToString() + bestMachineToGo.rotation);
                    //StartCoroutine("RotateToMachine");
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, bestMachineToGo.rotation.y, transform.eulerAngles.z);
                    g_cookingParticle.SetActive(true);
                    animator.SetBool("isCooking", true);
                }
            }
        }

        MoveAnimation();
    }

    IEnumerator RotateToMachine() {
        while (transform.eulerAngles.y != bestMachineToGo.rotation.z) {
            Debug.Log("OBRACAM SIE");
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
        Debug.Log("Idę do maszyny w pozycji " + bestMachineToGo.position.ToString() + " o levelu " + bestMachineToGo.level);
        GameEvents.SetMachineSate(new BaseMachineData(bestMachineToGo.machineID, MachineState.Busy));
        agent.SetDestination(bestMachineToGo.position);
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
    }
}