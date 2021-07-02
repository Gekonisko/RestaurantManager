using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class Cook : MonoBehaviour {

    public uint cookID = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private IDisposable _cookTaskEvent, _machineEvent;
    private MachineData bestMachineToGo;
    private uint _freeMachineCount = 0;

    private void Awake() {
        _cookTaskEvent = GameEvents.GetCookTask().Where(data => data.cookID == cookID).Subscribe(data => SetTask(data));
        _machineEvent = GameEvents.GetMachine().Where(data => data.cookID == cookID).Subscribe(data => SetDestination(data));
    }

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        MoveAnimation();
    }

    void MoveAnimation() {
        if (agent.velocity == Vector3.zero)
            animator.SetBool("isRunning", false);
        else
            animator.SetBool("isRunning", true);
    }

    private void SetTask(CookTaskData data) {
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