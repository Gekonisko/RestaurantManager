using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CookingMachine : MonoBehaviour {
    public uint machineID = 0;
    [SerializeField] private MachineState _machineSatate;
    [SerializeField] private Meal _cookingMeal;
    [SerializeField] private uint _level;
    [SerializeField] private GameObject travelPoint;

    private IDisposable _freeMachineEvent, _machineStateEvent;

    private void Awake() {
        _freeMachineEvent = GameEvents.GetFreeMachine().Where(data => data.meal == _cookingMeal).Subscribe(data => SetCookToMachine(data));
        _machineStateEvent = GameEvents.GetMachineSate().Where(data => data.machineID == machineID).Subscribe(data => SetMachineState(data));
        Debug.Log(gameObject.name + ":" + transform.rotation.z);
    }

    private void SetCookToMachine(CookTaskData data) {
        if (machineID == 0)
            throw new System.Exception("Maszyna o nazwie `" + gameObject.name + "` nie ma przydzielonego ID");
        GameEvents.SetMachine(new MachineData(machineID, _machineSatate, data.cookID, _level, travelPoint == null ? gameObject.transform.position : travelPoint.transform.position));
    }

    private void SetMachineState(BaseMachineData data) {
        _machineSatate = data.machineState;
    }

    private void OnDestroy() {
        _freeMachineEvent?.Dispose();
        _machineStateEvent?.Dispose();
    }
}