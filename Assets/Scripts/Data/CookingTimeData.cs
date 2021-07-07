using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CookingTimeData {
    public float cookingTime;
    public byte percentOfWellCookedTime;
    public byte percentOfBadCookedTime;
    [HideInInspector] public uint machineID;

    public CookingTimeData(float cookingTime, byte percentOfWellCookedTime, byte percentOfBadCookedTime, uint machineID) {
        this.cookingTime = cookingTime;
        this.percentOfWellCookedTime = percentOfWellCookedTime;
        this.percentOfBadCookedTime = percentOfBadCookedTime;
        this.machineID = machineID;
    }
}