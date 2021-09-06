using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new NavMeshWalkArea", menuName = "MyScripts/NavMeshWalkArea")][System.Serializable]
public class NavMeshWalkArea : ScriptableObject {
    public NavMeshMapData map;
}