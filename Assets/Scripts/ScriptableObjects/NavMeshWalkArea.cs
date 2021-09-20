using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new NavMeshWalkArea", menuName = "Scriptables/NavMeshWalkArea")][System.Serializable]
public class NavMeshWalkArea : ScriptableObject {
    public NavMeshMapData map;
    public Vector3 startPosition;
    public Vector3 worldDestinationPosition;
}