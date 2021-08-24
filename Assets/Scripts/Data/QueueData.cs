using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QueueData {
    public uint clientID;
    public Vector3 position;
    public bool isThisFreePositionInQueue;

    public QueueData(uint clientID, Vector3 position, bool isThisFreePositionInQueue) {
        this.clientID = clientID;
        this.position = position;
        this.isThisFreePositionInQueue = isThisFreePositionInQueue;
    }
}