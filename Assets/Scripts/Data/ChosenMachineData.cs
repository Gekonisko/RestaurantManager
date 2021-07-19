using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChosenMachineData {
    public uint cookID;
    public uint machineID;

    public ChosenMachineData(uint cookID, uint machineID) {
        this.cookID = cookID;
        this.machineID = machineID;
    }
}