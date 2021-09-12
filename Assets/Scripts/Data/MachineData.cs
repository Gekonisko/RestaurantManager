using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MachineData {
    public uint machineID; //  0 = nie wybrano konkretnej maszyny
    public MachineState machineState;
    public Meal cookingMeal;

    public uint cookID;
    public uint level;
    public string machineName;
    public Vector3 rotation;
    public Vector3 position;

    public MachineData(uint machineID, MachineState machineState, Meal cookingMeal, uint cookID, uint level, string name, Vector3 rotation, Vector3 position) {
        this.machineID = machineID;
        this.machineState = machineState;
        this.cookingMeal = cookingMeal;
        this.cookID = cookID;
        this.level = level;
        this.machineName = name;
        this.rotation = rotation;
        this.position = position;
    }
}