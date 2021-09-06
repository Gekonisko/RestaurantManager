using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class Cook : MonoBehaviour {

    public uint cookID = 0;
    public float speed = 10f;
    public CookingTimeData cookingTimeData;
    [SerializeField] private bool isCooking = false, hasCookingTask = false;
    [SerializeField] private GameObject g_cookingParticle;
    private NavMeshAgent _agent;
    private Animator _animator;
    private IDisposable _cookTaskEvent, _machineEvent, _cookedMealEvent;
    private MachineData _bestMachineToGo;
    private Vector3 _startingPosition;
    private uint _freeMachineCount = 0;

    private void Awake() {
        _cookTaskEvent = GameEvents.GetCookTask().Where(data => data.cookID == cookID && !hasCookingTask).Subscribe(data => CheckFreeMachines(data));
        _machineEvent = GameEvents.GetMachine().Where(data => data.cookID == cookID && !hasCookingTask).Subscribe(data => SetActualMachine(data));
        _cookedMealEvent = GameEvents.GetCookedMeal().Where(data => data.cookID == cookID).Subscribe(data => GoToStartPosition());
    }

    private void Start() {
        _startingPosition = transform.position;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        if (!isCooking && hasCookingTask && !_agent.pathPending) {
            if (_agent.remainingDistance <= _agent.stoppingDistance) {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f) {
                    isCooking = true;
                    Debug.Log("Wysyłam czas gotowania do maszyny ID=" + _bestMachineToGo.machineID.ToString() + _bestMachineToGo.rotation);
                    //StartCoroutine("RotateToMachine");
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, _bestMachineToGo.rotation.y, transform.eulerAngles.z);
                    g_cookingParticle.SetActive(true);
                    _animator.SetBool("isCooking", true);
                    GameEvents.SetCookingTime(new CookingTimeData(cookingTimeData.cookingTime, cookingTimeData.percentOfWellCookedTime, cookingTimeData.percentOfBadCookedTime, _bestMachineToGo.machineID));
                }
            }
        }
        //MoveAnimation();
    }

    private void GoToStartPosition() {
        hasCookingTask = false;
        isCooking = false;
        _animator.SetBool("isCooking", false);
        g_cookingParticle.SetActive(false);
        _agent.SetDestination(_startingPosition);
    }

    IEnumerator GoToPosition() {
        Vector2 worldPosition;
        Vector3 targetWorldPosition;
        NavMeshMapData map = NavMesh.ReadSavedMap(_bestMachineToGo.cookingMeal.ToString() + _bestMachineToGo.machineID);
        Vector2Int mapPosition = NavMesh.GetPositionFromWorldToMap(transform.position);
        map.walkArea[mapPosition.x].row[mapPosition.y].isWay = false;
        Vector2Int targetPositionOnMap = NavMesh.GetMinCostWayAroundOnMap(map, mapPosition);
        while (targetPositionOnMap != NavMesh.POSITION_NOT_FOUND) {
            worldPosition = NavMesh.GetPositionFromMapToWorld(targetPositionOnMap);
            targetWorldPosition = new Vector3(worldPosition.x, transform.position.y, worldPosition.y);
            while (!NavMesh.IsVectorsEqual(transform.position, targetWorldPosition, 0.05f)) {
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, speed * Time.deltaTime);
                yield return null;
            }
            mapPosition = NavMesh.GetPositionFromWorldToMap(transform.position);
            map.walkArea[mapPosition.x].row[mapPosition.y].isWay = false;
            targetPositionOnMap = NavMesh.GetMinCostWayAroundOnMap(map, mapPosition);
        }
        yield return null;
    }

    IEnumerator RotateToMachine() {
        while (transform.eulerAngles.y != _bestMachineToGo.rotation.z) {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Lerp(transform.eulerAngles.y, _bestMachineToGo.rotation.z, Time.deltaTime), transform.eulerAngles.z);
            yield return null;
        }
    }

    void MoveAnimation() {
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
        if (_freeMachineCount == 0)
            _bestMachineToGo = data;
        _bestMachineToGo = GetBetterMachine(_bestMachineToGo, data);
        _freeMachineCount++;
        if (_freeMachineCount != 3) return;
        _freeMachineCount = 0;
        if (_bestMachineToGo.machineState == MachineState.Busy) {
            Debug.Log("Nie ma wolnych maszyn");
            return;
        }
        hasCookingTask = true;
        Debug.Log("Idę do maszyny w pozycji " + _bestMachineToGo.machineID.ToString() + " o levelu " + _bestMachineToGo.level);
        //agent.SetDestination(bestMachineToGo.position);
        StartCoroutine("GoToPosition");
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