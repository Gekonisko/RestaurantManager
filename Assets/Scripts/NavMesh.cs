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

    private float m_MaxDistance = 50.0f;

    private void Awake() {
        Vector3 axisDistance = AxisDistance(startPosition.position, endPosition.position);
        _scanSize = new Vector2(Mathf.Floor(axisDistance.x), Mathf.Floor(axisDistance.z));
        walkableArea = new NavMeshMapData((int) (_scanSize.x / scanSquareSize), (int) (_scanSize.y / scanSquareSize));
        Debug.Log(walkableArea.dimension1Size + " - " + walkableArea.dimension2Size);
        AreaScanning();
    }

    private void Start() {
        NavMeshWalkArea asset = new NavMeshWalkArea();
        asset.walkArea = GetNavMeshMap();
        asset.destinationName = "Di-de-di-da-di-de-do-do\nDi-ba-di-de-do\nDi-de-de-di-de-de-de-do-do-day-bi-di-do";
        AssetDatabase.CreateAsset(asset, "Assets\\Resources\\NavMesh Walk Area\\c.asset");

        Vector2Int cookPosition = GetPositionOnMap(new Vector2(18, -4f));
        Debug.Log(cookPosition);
        walkableArea.walkArea[cookPosition.x].col[cookPosition.y].distance = 255;

        walkableArea = SetWayToPosition(walkableArea, cookPosition);

        PrintWalkableArea(walkableArea);
    }

    public NavMeshMapData SetWayToPosition(NavMeshMapData map, Vector2Int positionOnMap) {
        byte distance = 1;
        List<Vector2Int> positionAround = new List<Vector2Int>();
        List<Vector2Int> nextPositionAround = new List<Vector2Int>();
        positionAround.AddRange(GetFreePositionAroundOnMap(map, positionOnMap));
        map = SetPostionDistanceOnMap(map, positionAround, distance);
        while (positionAround.Count > 0) {
            distance++;
            foreach (Vector2Int position in positionAround) {
                List<Vector2Int> freePositions = GetFreePositionAroundOnMap(map, position);
                map = SetPostionDistanceOnMap(map, freePositions, distance);
                nextPositionAround.AddRange(freePositions);
            }
            positionAround.Clear();
            positionAround.AddRange(nextPositionAround);
            nextPositionAround.Clear();
        }
        return map;
    }

    public Vector2Int GetPositionOnMap(Vector2 posintion) {
        return new Vector2Int((int) Mathf.Abs(Mathf.Floor(startPosition.position.x) - Mathf.Floor(posintion.x)), (int) Mathf.Abs(Mathf.Floor(startPosition.position.z) - Mathf.Floor(posintion.y)));
    }

    public NavMeshMapData GetNavMeshMap() {
        return walkableArea;
    }

    public Vector3 AxisDistance(Vector3 point1, Vector3 point2) {
        return new Vector3(Mathf.Abs(point1.x - point2.x), Mathf.Abs(point1.y - point2.y), Mathf.Abs(point1.z - point2.z));
    }

    private NavMeshMapData SetPostionDistanceOnMap(NavMeshMapData map, List<Vector2Int> positions, byte distance) {
        foreach (Vector2Int position in positions)
            map.walkArea[position.x].col[position.y].distance = distance;
        return map;
    }

    private List<Vector2Int> GetFreePositionAroundOnMap(NavMeshMapData map, Vector2Int positionOnMap) {
        List<Vector2Int> positionAround = new List<Vector2Int>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue;
                Vector2Int position = new Vector2Int(positionOnMap.x + x, positionOnMap.y + y);
                if ((position.x >= 0 && position.x < map.dimension1Size) &&
                    (position.y >= 0 && position.y < map.dimension2Size) && (map.walkArea[position.x].col[position.y].distance == 0 && map.walkArea[position.x].col[position.y].isWalkable))
                    positionAround.Add(new Vector2Int(position.x, position.y));
            }
        }
        return positionAround;
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
                walkableArea.walkArea[x].col[z].isWalkable = !Physics.BoxCast(startPosition.position + new Vector3(-x * scanSquareSize, 0, z * scanSquareSize), new Vector3(scanSquareSize, 2, scanSquareSize), -transform.up, out m_Hit, transform.rotation, m_MaxDistance);
            }
        }
    }

    private void PrintWalkableArea(NavMeshMapData map) {
        StringBuilder text = new StringBuilder();
        bool isTheSameSign = false;
        for (int x = 0; x < map.dimension2Size; x++) {
            isTheSameSign = false;
            for (int z = 0; z < map.dimension1Size; z++) {
                if (!map.walkArea[z].col[x].isWalkable) {
                    if (!isTheSameSign) {
                        isTheSameSign = true;
                        text.Append("<color=red>");
                    }
                    text.Append('#');
                } else {
                    if (isTheSameSign) {
                        isTheSameSign = false;
                        text.Append("</color>");
                    }
                    if (map.walkArea[z].col[x].distance == 255)
                        text.Append("<color=blue>K</color>");
                    else
                        text.Append('X');
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