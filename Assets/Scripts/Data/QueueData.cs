using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QueueData {
    public uint clientID;
    public Vector3 position;
    public bool isThisFreePositionInQueue;
    public int queueNumber;

    public QueueData(uint clientID, Vector3 position, bool isThisFreePositionInQueue = false, int queueNumber = -1) {
        this.clientID = clientID;
        this.position = position;
        this.isThisFreePositionInQueue = isThisFreePositionInQueue;
        this.queueNumber = queueNumber;
    }
}