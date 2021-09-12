using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameController {
    private static uint _freeClientID = 1;
    public static uint FREE_CLIENT_ID {
        get {
            return _freeClientID++;
        }
    }

    private static Dictionary<Meal, uint> machineCount = new Dictionary<Meal, uint>();
    private static uint _freeMachineID = 1;

    // public static void InitGameController() {
    //     foreach (Meal meal in Enum.GetValues(typeof(Meal))) {
    //         if (!machineCount.ContainsKey(meal))
    //             machineCount.Add(meal, 0);
    //     }
    // }

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