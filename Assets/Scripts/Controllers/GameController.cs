using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameController {
    private static bool _isPointOfDyingAssign = false;
    private static Vector3 _clientPointOfDying;
    public static Vector3 CLIENT_POINT_OF_DYING {
        get => _clientPointOfDying;
        set {
            if (!_isPointOfDyingAssign) {
                _clientPointOfDying = value;
                _isPointOfDyingAssign = true;
            }
        }
    }

    private static uint _freeClientID = 1;
    public static uint FREE_CLIENT_ID {
        get => _freeClientID++;
    }

    private static uint _freeMachineID = 1;
    private static Dictionary<Meal, uint> machineCount = new Dictionary<Meal, uint>();

    public static uint GetFreeMachineID(Meal machineCookingMeal) {
        if (!machineCount.ContainsKey(machineCookingMeal))
            machineCount.Add(machineCookingMeal, 0);
        machineCount[machineCookingMeal]++;
        return _freeMachineID++;
    }

    public static uint GetNumberOfMachinesByType(Meal typeOfMachine) {
        return machineCount[typeOfMachine];
    }

}