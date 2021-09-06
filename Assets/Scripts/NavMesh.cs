using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

//GameObjekty StartPosition oraz EndPosition należy ustawić na pozycjach całkowitych by nie było żadnych niedokładności w mapie. Mapa korzysta z koordynatów X (jako X na mapie) oraz Z (jako Y na mapie)
public class NavMesh : MonoBehaviour {
    public static readonly Vector2Int POSITION_NOT_FOUND = new Vector2Int(-1, -1);
    public Transform startPosition, endPosition;
    [Range(0.5f, 20.0f)] public float scanSquareSize = 1;

    private static readonly float MAX_SCAN_DISTANCE = 50.0f;
    private static NavMeshMapData _map;
    private static Vector3 _startPosition;
    private Vector2 _scanSize;
    private RaycastHit m_Hit;

    void Awake() {
        _startPosition = startPosition.position;
        Vector3 axisDistance = GetAxisDistance(startPosition.position, endPosition.position);
        _scanSize = new Vector2(Mathf.Floor(axisDistance.x), Mathf.Floor(axisDistance.z));
        _map = new NavMeshMapData((int) (_scanSize.x / scanSquareSize), (int) (_scanSize.y / scanSquareSize));
        _map = ScanArea(_map);
    }

    public static NavMeshMapData GetMapData() {
        NavMeshMapData newMap = new NavMeshMapData(_map.dimension1Size, _map.dimension2Size);
        for (int i = 0; i < newMap.dimension1Size; i++) {
            newMap.walkArea[i] = new NavMeshMapData.NavMeshMapRowData(newMap.dimension2Size);
            newMap.walkArea[i].row = (PositionMapData[]) _map.walkArea[i].row.Clone();
        }
        return newMap;
    }

    public static Vector3 GetAxisDistance(Vector3 point1, Vector3 point2) {
        return new Vector3(Mathf.Abs(point1.x - point2.x), Mathf.Abs(point1.y - point2.y), Mathf.Abs(point1.z - point2.z));
    }

    public static Vector2 GetPositionFromMapToWorld(Vector2Int posintionOnMap) {
        return new Vector2(Mathf.Round(_startPosition.x) - Mathf.Round(posintionOnMap.x), Mathf.Round(_startPosition.z) + Mathf.Round(posintionOnMap.y));
    }

    public static Vector2Int GetPositionFromWorldToMap(Vector3 posintionInWorld) {
        return new Vector2Int((int) Mathf.Abs(Mathf.Round(_startPosition.x) - Mathf.Round(posintionInWorld.x)), (int) Mathf.Abs(Mathf.Round(_startPosition.z) - Mathf.Round(posintionInWorld.z)));
    }

    public static List<Vector2Int> GetFreePositionAroundOnMap(Vector2Int positionOnMap, NavMeshMapData map) {
        List<Vector2Int> positionAround = new List<Vector2Int>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if ((x == 0 && y == 0) || (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)) continue;

                Vector2Int position = new Vector2Int(positionOnMap.x + x, positionOnMap.y + y);
                if ((position.x >= 0 && position.x < map.dimension1Size) &&
                    (position.y >= 0 && position.y < map.dimension2Size) && (!map.walkArea[position.x].row[position.y].isWay && map.walkArea[position.x].row[position.y].isWalkable))
                    positionAround.Add(new Vector2Int(position.x, position.y));
            }
        }
        return positionAround;
    }

    public static Vector2Int GetMinCostWayAroundOnMap(NavMeshMapData map, Vector2Int positionOnMap) {
        Vector2Int minCostWay = POSITION_NOT_FOUND;
        byte minCost = map.walkArea[positionOnMap.x].row[positionOnMap.y].distance;
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue;
                Vector2Int position = new Vector2Int(positionOnMap.x + x, positionOnMap.y + y);
                if ((position.x >= 0 && position.x < map.dimension1Size) &&
                    (position.y >= 0 && position.y < map.dimension2Size) && map.walkArea[position.x].row[position.y].isWalkable && map.walkArea[position.x].row[position.y].isWay) {
                    if (map.walkArea[position.x].row[position.y].distance < minCost) {
                        minCost = map.walkArea[position.x].row[position.y].distance;
                        minCostWay = position;
                    }
                }
            }
        }
        return minCostWay;
    }

    public static bool IsVectorsEqual(Vector3 me, Vector3 other, float allowedDifference) {
        var dx = me.x - other.x;
        var dy = me.y - other.y;
        var dz = me.z - other.z;

        if (Mathf.Abs(dx) <= allowedDifference && Mathf.Abs(dy) <= allowedDifference && Mathf.Abs(dz) <= allowedDifference)
            return true;
        return false;
    }

    public static NavMeshMapData CreateWayMap(Vector2Int mapPosition) {
        NavMeshMapData newMap = GetMapData();
        newMap = SetWayToPosition(mapPosition, newMap);
        return newMap;
    }

    public static void SaveMap(NavMeshMapData map, string name) {
        NavMeshWalkArea asset = new NavMeshWalkArea();
        asset.map = map;
        AssetDatabase.CreateAsset(asset, "Assets/Resources/NavMesh Walk Area/" + name + ".asset");
    }

    public static NavMeshMapData ReadSavedMap(string name) {
        Debug.Log(Resources.Load<NavMeshWalkArea>("NavMesh Walk Area/" + name));
        try {
            return Resources.Load<NavMeshWalkArea>("NavMesh Walk Area/" + name).map;
        } catch (System.NullReferenceException) {
            throw new System.Exception("Nie intnieje Mapa o nazwie `" + name + "` w folderze Resources\\NavMesh Walk Area");
        }
    }

    private static NavMeshMapData SetWayToPosition(Vector2Int positionOnMap, NavMeshMapData map) {
        byte distance = 1;
        map.walkArea[positionOnMap.x].row[positionOnMap.y].distance = 0;
        map.walkArea[positionOnMap.x].row[positionOnMap.y].isWay = true;
        map.walkArea[positionOnMap.x].row[positionOnMap.y].isWalkable = true;
        List<Vector2Int> positionAround = new List<Vector2Int>();
        List<Vector2Int> nextPositionAround = new List<Vector2Int>();
        positionAround.AddRange(GetFreePositionAroundOnMap(positionOnMap, map));
        map = SetPostionDistanceOnMap(map, positionAround, distance);
        while (positionAround.Count > 0) {
            distance++;
            foreach (Vector2Int position in positionAround) {
                List<Vector2Int> freePositions = GetFreePositionAroundOnMap(position, map);
                map = SetPostionDistanceOnMap(map, freePositions, distance);
                nextPositionAround.AddRange(freePositions);
            }
            positionAround.Clear();
            positionAround.AddRange(nextPositionAround);
            nextPositionAround.Clear();
        }
        return map;
    }

    private static NavMeshMapData SetPostionDistanceOnMap(NavMeshMapData map, List<Vector2Int> positions, byte distance) {
        foreach (Vector2Int position in positions) {
            map.walkArea[position.x].row[position.y].distance = distance;
            map.walkArea[position.x].row[position.y].isWay = true;
        }
        return map;
    }

    private NavMeshMapData ScanArea(NavMeshMapData map) {
        for (int x = 0; x < _scanSize.x / scanSquareSize; x++) {
            for (int z = 0; z < _scanSize.y / scanSquareSize; z++) {
                map.walkArea[x].row[z].isWalkable = !Physics.BoxCast(startPosition.position + new Vector3(-x * scanSquareSize, 0, z * scanSquareSize), new Vector3(scanSquareSize, 2, scanSquareSize), -transform.up, out m_Hit, transform.rotation, MAX_SCAN_DISTANCE);
            }
        }
        return map;
    }

    public static void PrintPlayerWay(NavMeshMapData map) {
        StringBuilder text = new StringBuilder();
        bool isTheSameSign = false;
        for (int x = 0; x < map.dimension2Size; x++) {
            isTheSameSign = false;
            for (int z = 0; z < map.dimension1Size; z++) {
                if (!map.walkArea[z].row[x].isWalkable) {
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
                    if (map.walkArea[z].row[x].isWay)
                        text.Append('X');
                    else
                        text.Append('0');
                }
            }
            if (isTheSameSign)
                text.Append("</color>");
            text.Append("\n");
        }
        Debug.Log(text);
    }

    public static void PrintWalkableArea(NavMeshMapData map) {
        StringBuilder text = new StringBuilder();
        bool isTheSameSign = false;
        for (int x = 0; x < map.dimension2Size; x++) {
            isTheSameSign = false;
            for (int z = 0; z < map.dimension1Size; z++) {
                if (!map.walkArea[z].row[x].isWalkable) {
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
                    text.Append('X');
                }
            }
            if (isTheSameSign)
                text.Append("</color>");
            text.Append("\n");
        }
        Debug.Log(text);
    }

    public static void PrintDistance(NavMeshMapData map) {
        StringBuilder text = new StringBuilder();
        bool isTheSameSign = false;
        text.Append("\t<color=blue>");
        for (int x = 0; x < map.dimension1Size; x++) {
            text.Append("\t" + x);
        }
        text.Append("</color>\n");
        for (int x = 0; x < map.dimension2Size; x++) {
            isTheSameSign = false;
            text.Append("\t" + x);
            for (int z = 0; z < map.dimension1Size; z++) {
                Debug.Log(z + ", " + x);
                if (!map.walkArea[z].row[x].isWalkable) {
                    if (!isTheSameSign) {
                        isTheSameSign = true;
                        text.Append("<color=red>");
                    }
                    text.Append("\t#");
                } else {
                    if (isTheSameSign) {
                        isTheSameSign = false;
                        text.Append("</color>");
                    }
                    text.Append("\t" + map.walkArea[z].row[x].distance);
                }
            }
            if (isTheSameSign)
                text.Append("</color>");
            text.Append("\n");
        }
        Debug.Log(text);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        float xDistance = Mathf.Floor(Mathf.Abs(startPosition.position.x - endPosition.position.x));
        float yDistance = Mathf.Floor(Mathf.Abs(startPosition.position.z - endPosition.position.z));
        for (float x = 0; x < xDistance; x += scanSquareSize) {
            for (float z = 0; z < yDistance; z += scanSquareSize) {
                Gizmos.DrawWireCube(startPosition.position + new Vector3(-x, 0, z), new Vector3(scanSquareSize, 2, scanSquareSize));
            }
        }
    }
}