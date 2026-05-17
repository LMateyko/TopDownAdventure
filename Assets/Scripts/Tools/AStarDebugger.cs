using Adventure.Tools;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarDebugger : MonoBehaviour
{
    [SerializeField] DungeonManager m_dungeonManager;

    [Header("Debug Helpers")]
    [SerializeField] private Vector2Int m_gizmoStartPos;
    [SerializeField] private Vector2Int m_gizmoEndPos;

    private void OnDrawGizmosSelected()
    {
        if (m_dungeonManager == null || !isActiveAndEnabled)
            return;

        Gizmos.color = Color.magenta;
        var tileGrid = m_dungeonManager.DungeonTileData.Item2;
        var tilemapOrigin = m_dungeonManager.DungeonTileData.Item3;

        var offsetStartPos = (Vector3Int)m_gizmoStartPos + tilemapOrigin;
        var offsetEndPos = (Vector3Int)m_gizmoEndPos + tilemapOrigin;

        Gizmos.DrawSphere(tileGrid.GetCellCenterWorld(offsetStartPos), 0.25f);
        Gizmos.DrawSphere(tileGrid.GetCellCenterWorld(offsetEndPos), 0.25f);

        var debugStart = tileGrid.GetCellCenterWorld((Vector3Int)m_gizmoStartPos);
        var debugEnd = tileGrid.GetCellCenterWorld((Vector3Int)m_gizmoEndPos);

        var debugPath = AStarPathfinder.AStarSearch(m_dungeonManager.DungeonTileData.Item1, tileGrid, tilemapOrigin, debugStart, debugEnd);

        if (debugPath.Item1 != null)
        {
            Gizmos.color = Color.blue;

            Vector3 prevPos = tileGrid.GetCellCenterWorld(offsetStartPos);

            foreach (var pos in debugPath.Item1)
            {
                Gizmos.DrawLine(prevPos, pos);
                prevPos = pos;
            }
        }

        if (debugPath.Item2 != null)
        {
            Gizmos.color = Color.black;

            Vector3Int currentPosition = Vector3Int.zero;

            foreach (var pos in debugPath.Item2.Keys)
            {
                currentPosition = (Vector3Int)pos + tilemapOrigin;

                float pathSize = (float)debugPath.Item2[pos] * 0.01f;
                //Gizmos.DrawSphere(tileGrid.GetCellCenterWorld(currentPosition) - (Vector3.forward), pathSize);

                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;

                UnityEditor.Handles.Label(tileGrid.GetCellCenterWorld(currentPosition) - (Vector3.forward * 3), $"f:{debugPath.Item2[pos]}", style);
            }
        }
    }
}
