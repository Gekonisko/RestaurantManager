using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class NavMesh : MonoBehaviour {
    public Transform startPosition, endPosition;
    [Range(0.5f, 20.0f)] public float scanSquareSize = 1;

    private Vector2 _scanSize;
    private RaycastHit m_Hit;
    public NavMeshMapData walkableArea;
    private char[, ] _walkableArea = null;

    private float m_MaxDistance = 50.0f;

    private void Awake() {
        Debug.Log(typeof(string).Assembly.ImageRuntimeVersion);
        Vector3 axisDistance = AxisDistance(startPosition.position, endPosition.position);
        _scanSize = new Vector2(Mathf.Floor(axisDistance.x), Mathf.Floor(axisDistance.z));
        _walkableArea = new char[(int) (_scanSize.x / scanSquareSize), (int) (_scanSize.y / scanSquareSize)];
        walkableArea = new NavMeshMapData((int) (_scanSize.x / scanSquareSize), (int) (_scanSize.y / scanSquareSize));
        AreaScanning();
    }

    private void Start() {
        NavMeshWalkArea asset = new NavMeshWalkArea();
        asset.walkArea = GetNavMeshMap();
        asset.destinationName = "Di-de-di-da-di-de-do-do\nDi-ba-di-de-do\nDi-de-de-di-de-de-de-do-do-day-bi-di-do";
        PrintWalkableArea(asset.walkArea);
        AssetDatabase.CreateAsset(asset, "Assets/Resources/NavMesh Walk Area/c.asset");
    }

    public NavMeshMapData GetNavMeshMap() {
        return walkableArea;
    }

    public Vector3 AxisDistance(Vector3 point1, Vector3 point2) {
        return new Vector3(Mathf.Abs(point1.x - point2.x), Mathf.Abs(point1.y - point2.y), Mathf.Abs(point1.z - point2.z));
    }

    [ContextMenu("Read Saved Map")]
    private void ReadSavedMap() {
        var map = Resources.Load<NavMeshWalkArea>("NavMesh Walk Area\\NewScripableObject2").walkArea;
        Debug.Log(map);
        PrintWalkableArea(map);
    }

    private void AreaScanning() {
        for (int x = 0; x < _scanSize.x / scanSquareSize; x++) {
            for (int z = 0; z < _scanSize.y / scanSquareSize; z++) {
                walkableArea.walkArea[x].row[z] = !Physics.BoxCast(startPosition.position + new Vector3(-x * scanSquareSize, 0, z * scanSquareSize), new Vector3(scanSquareSize, 2, scanSquareSize), -transform.up, out m_Hit, transform.rotation, m_MaxDistance) == true ? 'X' : '#';
            }
        }
    }

    private void PrintWalkableArea(NavMeshMapData map) {
        StringBuilder text = new StringBuilder();
        bool isTheSameSign = false;
        for (int x = 0; x < map.dimension2Size; x++) {
            isTheSameSign = false;
            for (int z = 0; z < map.dimension1Size; z++) {
                if (map.walkArea[z].row[x] == '#') {
                    if (!isTheSameSign) {
                        isTheSameSign = true;
                        text.Append("<color=red>");
                    }
                    text.Append("#");
                } else {
                    if (isTheSameSign) {
                        isTheSameSign = false;
                        text.Append("</color>");
                    }
                    text.Append("X");
                }
            }
            if (isTheSameSign)
                text.Append("</color>");
            text.Append("\n");
        }
        Debug.Log(text);
    }

    // private void OnDrawGizmosSelected() {
    //     Gizmos.color = new Color(1, 0, 0, 0.5f);
    //     float xDistance = Mathf.Floor(Mathf.Abs(startPosition.position.x - endPosition.position.x));
    //     float yDistance = Mathf.Floor(Mathf.Abs(startPosition.position.z - endPosition.position.z));
    //     for (float x = 0; x < xDistance; x += scanSquareSize) {
    //         for (float z = 0; z < yDistance; z += scanSquareSize) {
    //             Gizmos.DrawWireCube(startPosition.position + new Vector3(-x, 0, z), new Vector3(scanSquareSize, 2, scanSquareSize));
    //         }
    //     }
    // }
}