using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MachineData {
    public uint machineID; //  0 = nie wybrano konkretnej maszyny
    public MachineState machineState;

    public uint cookID;
    public uint level;
    public Meal cookingMeal;
    public Vector3 rotation;

    public MachineData(uint machineID, MachineState machineState, uint cookID, uint level, Meal cookingMeal, Vector3 rotation) {
        this.machineID = machineID;
        this.machineState = machineState;
        this.cookID = cookID;
        this.level = level;
        this.cookingMeal = cookingMeal;
        this.rotation = rotation;
    }
}