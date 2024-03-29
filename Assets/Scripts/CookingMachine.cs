using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;

public class CookingMachine : MonoBehaviour {
    public readonly Color GREEN = new Color(0, 1, 0.12f), RED = new Color(0.86f, 0.11f, 0.09f);
    public string machineName;
    public uint machineID = 0;
    [SerializeField] private float machineWarmUpTime;
    [SerializeField] private MachineState _machineSatate;
    [SerializeField] private Meal _cookingMeal;
    [SerializeField] private uint _level;
    [SerializeField] private GameObject travelPoint;

    private CookingTimeData _cookingTime;
    private Animator _animator;
    private Renderer _render;
    private IDisposable _freeMachineEvent, _machineStateEvent, _cookingTimeEvent;
    private IEnumerator _cooking;
    private uint _actualCook;
    private bool _isMealFinished = false;

    private void Awake() {
        machineID = GameController.GetFreeMachineID(_cookingMeal);
        _freeMachineEvent = GameEvents.GetFreeMachine().Where(data => data.meal == _cookingMeal).Subscribe(data => SetCookToMachine(data));
        _machineStateEvent = GameEvents.GetChosenMachine().Where(data => data.machineID == machineID).Subscribe(data => SetChosenMachine(data));
        _cookingTimeEvent = GameEvents.GetCookingTime().Where(data => data.machineID == machineID).Subscribe(data => SetCooking(data));
    }

    private void Start() {
        _animator = GetComponent<Animator>();
        _render = GetComponent<Renderer>();
        CreateWayMap();
    }

    private void OnMouseDown() {
        if (_isMealFinished) {
            _isMealFinished = false;
            StopCoroutine(_cooking);
            _render.material.SetFloat("Boolean_isEmission", 0);
            GameEvents.SetCookedMeal(new CookedMealData(_actualCook, new MealData(_cookingMeal, 1), false));
            _animator.SetBool("isCooking", false);
            _machineSatate = MachineState.Free;
        }
    }

    private void CreateWayMap() {
        if (NavMesh.IsMapExistInResources(machineName)) return;
        NavMesh.SaveMap(NavMesh.CreateWayMap(NavMesh.GetPositionFromWorldToMap(travelPoint.transform.position, NavMesh.START_POSITION), NavMesh.BASE_MAP), machineName, travelPoint.transform.position);
    }

    private IEnumerator Cooking() {
        //Debug.Log(_cookingTime.cookingTime * ((100 - (_cookingTime.percentOfBadCookedTime + _cookingTime.percentOfWellCookedTime)) / 100.0f) + machineWarmUpTime);
        yield return new WaitForSeconds(_cookingTime.cookingTime * ((100 - (_cookingTime.percentOfBadCookedTime + _cookingTime.percentOfWellCookedTime)) / 100.0f));
        _isMealFinished = true;
        _render.material.SetFloat("Boolean_isEmission", 1);
        _render.material.SetFloat("Vector1_EmissionFrequency", 5);
        _render.material.SetColor("Color_EmissionColor", GREEN);
        //Debug.Log(_cookingTime.cookingTime * (_cookingTime.percentOfWellCookedTime / 100.0f));
        yield return new WaitForSeconds(_cookingTime.cookingTime * (_cookingTime.percentOfWellCookedTime / 100.0f));
        _render.material.SetFloat("Vector1_EmissionFrequency", 10);
        _render.material.SetColor("Color_EmissionColor", RED);
        //Debug.Log(_cookingTime.cookingTime * (_cookingTime.percentOfBadCookedTime / 100.0f));
        yield return new WaitForSeconds(_cookingTime.cookingTime * (_cookingTime.percentOfBadCookedTime / 100.0f));
        _render.material.SetFloat("Boolean_isEmission", 0);
        yield return null;
    }

    private void SetCooking(CookingTimeData data) {
        _cookingTime = data;
        _animator.SetBool("isCooking", true);
        _cooking = Cooking();
        StartCoroutine(_cooking);
    }

    private void SetCookToMachine(CookTaskData data) {
        Debug.Log("Zlecenie Doszło do " + machineName);
        GameEvents.SetMachine(new MachineData(machineID, _machineSatate, _cookingMeal, data.cookID, _level, machineName, transform.eulerAngles, transform.position));
    }

    private void SetChosenMachine(ChosenMachineData data) {
        _machineSatate = MachineState.Busy;
        _actualCook = data.cookID;
    }

    private void OnDestroy() {
        _freeMachineEvent?.Dispose();
        _machineStateEvent?.Dispose();
        _cookingTimeEvent?.Dispose();
    }
}