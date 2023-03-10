using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NavMeshMapData {
    public NavMeshMapRowData[] walkArea;
    public int dimension1Size;
    public int dimension2Size;

    public NavMeshMapData(int dimension1Size, int dimension2Size) {
        this.dimension1Size = dimension1Size;
        this.dimension2Size = dimension2Size;
        walkArea = new NavMeshMapRowData[dimension1Size];
        for (int i = 0; i < dimension1Size; i++) {
            walkArea[i] = new NavMeshMapRowData(dimension2Size);
        }
    }

    [System.Serializable]
    public struct NavMeshMapRowData {
        public PositionMapData[] row;

        public NavMeshMapRowData(int dimension1Size) {
            row = new PositionMapData[dimension1Size];
        }
    }
}

[System.Serializable]
public struct PositionMapData {
    public bool isWalkable;
    public bool isWay;
    public byte distance;
}