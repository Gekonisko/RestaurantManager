using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BaseMachineData {
    public uint machineID; //  0 = nie wybrano konkretnej maszyny
    public MachineState machineState;

    public BaseMachineData(uint machineID, MachineState machineState) {
        this.machineID = machineID;
        this.machineState = machineState;
    }
}