using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Cook : MonoBehaviour {

    public uint cookID = 0;
    public float speed = 10f;
    public CookingTimeData cookingTimeData;
    [SerializeField] private bool hasCookingTask = false;
    [SerializeField] private GameObject g_cookingParticle;
    private NavMeshAgent _agent;
    private Animator _animator;
    private IDisposable _cookTaskEvent, _machineEvent, _cookedMealEvent;
    private MachineData _bestMachineToGo;
    private Vector3 _startingPosition;
    private uint _freeMachineCount = 0;

    private NavMeshMapData _startPositionMap;

    private void Awake() {
        _cookTaskEvent = GameEvents.GetCookTask().Where(data => data.cookID == cookID && !hasCookingTask).Subscribe(data => CheckFreeMachines(data));
        _machineEvent = GameEvents.GetMachine().Where(data => data.cookID == cookID && !hasCookingTask).Subscribe(data => SetActualMachine(data));
        _cookedMealEvent = GameEvents.GetCookedMeal().Where(data => data.cookID == cookID).Subscribe(data => GoToStartPosition());
    }

    private void Start() {
        _startingPosition = transform.position;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        CreateWayMap();
    }

    private void CreateWayMap() {
        if (NavMesh.IsMapExistInResources("CookStartPosition")) return;
        NavMesh.SaveMap(NavMesh.CreateWayMap(NavMesh.GetPositionFromWorldToMap(transform.position, NavMesh.START_POSITION), NavMesh.BASE_MAP), "CookStartPosition", transform.position);
    }

    private void OnDestinationComplete() {
        if (hasCookingTask) {
            Debug.Log("Wysyłam czas gotowania do maszyny ID=" + _bestMachineToGo.machineID.ToString() + _bestMachineToGo.rotation);
            //StartCoroutine("RotateToMachine");
            RotateToPoint(_bestMachineToGo.position);
            g_cookingParticle.SetActive(true);
            _animator.SetBool("isCooking", true);
            GameEvents.SetCookingTime(new CookingTimeData(cookingTimeData.cookingTime, cookingTimeData.percentOfWellCookedTime, cookingTimeData.percentOfBadCookedTime, _bestMachineToGo.machineID));
        }
    }

    private void GoToStartPosition() {
        hasCookingTask = false;
        StartCoroutine("GoToPosition", NavMesh.ReadSavedMap("CookStartPosition"));
        _animator.SetBool("isCooking", false);
        g_cookingParticle.SetActive(false);
        //_agent.SetDestination(_startingPosition);
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
        }
        Vector3 newDestinationPosition = new Vector3(data.worldDestinationPosition.x, transform.position.y, data.worldDestinationPosition.z);
        while (Vector3.Distance(transform.position, newDestinationPosition) > 0.05f) {
            RotateToPoint(newDestinationPosition);
            transform.position = Vector3.MoveTowards(transform.position, newDestinationPosition, speed * Time.deltaTime);
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

    private void MoveAnimation() {
        if (_agent.velocity == Vector3.zero)
            _animator.SetBool("isRunning", false);
        else
            _animator.SetBool("isRunning", true);
    }

    private void CheckFreeMachines(CookTaskData data) {
        Debug.Log("Doszło zlecenie do kucharza o numerze " + cookID.ToString());
        GameEvents.SetFreeMachine(data);
    }

    private void SetActualMachine(MachineData data) {
        Debug.Log("Dostałem zlecenie od " + data.machineName);
        Debug.Log("GameController.GetNumberOfMachinesByType(data.cookingMeal):" + GameController.GetNumberOfMachinesByType(data.cookingMeal));
        if (_freeMachineCount == 0)
            _bestMachineToGo = data;
        _bestMachineToGo = GetBetterMachine(_bestMachineToGo, data);
        _freeMachineCount++;
        if (_freeMachineCount != GameController.GetNumberOfMachinesByType(data.cookingMeal)) return;
        _freeMachineCount = 0;
        if (_bestMachineToGo.machineState == MachineState.Busy) {
            Debug.Log("Nie ma wolnych maszyn");
            return;
        }
        hasCookingTask = true;
        Debug.Log("Idę do maszyny w pozycji " + _bestMachineToGo.machineID.ToString() + " o levelu " + _bestMachineToGo.level);
        //agent.SetDestination(bestMachineToGo.position);
        StartCoroutine("GoToPosition", NavMesh.ReadSavedMap(_bestMachineToGo.machineName));
        GameEvents.SetChosenMachine(new ChosenMachineData(cookID, _bestMachineToGo.machineID));
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