using Adventure.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarDebugger : MonoBehaviour
{
    [SerializeField] DungeonManager m_dungeonManager;

    [Header("Debug Helpers")]
    [SerializeField] private Vector2Int m_gizmoStartPos;
    [SerializeField] private Vector2Int m_gizmoEndPos;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (m_dungeonManager == null || !isActiveAndEnabled || Application.isPlaying)
            return;

        Gizmos.color = Color.magenta;
        var tileGrid = m_dungeonManager.DungeonWallTilemap.layoutGrid;
        var tilemapOrigin = m_dungeonManager.DungeonWallTilemap.origin;

        var offsetStartPos = (Vector3Int)m_gizmoStartPos + tilemapOrigin;
        var offsetEndPos = (Vector3Int)m_gizmoEndPos + tilemapOrigin;

        var startWorldPos   = tileGrid.GetCellCenterWorld(offsetStartPos);
        var endWorldPos     = tileGrid.GetCellCenterWorld(offsetEndPos);

        Gizmos.DrawSphere(startWorldPos, 0.25f);
        Gizmos.DrawSphere(endWorldPos, 0.25f);

        var dungeonPath = m_dungeonManager.GetPathBetweenPoints(startWorldPos, endWorldPos, out Dictionary<Vector2Int, double> pathReport);

        if (dungeonPath != null)
        {
            Gizmos.color = Color.blue;

            Vector3 prevPos = startWorldPos;

            foreach (var pos in dungeonPath)
            {
                var worldPosition = pos;
                Gizmos.DrawLine(prevPos, worldPosition);
                prevPos = worldPosition;
            }
        }

        if (pathReport != null)
        {
            Gizmos.color = Color.black;

            Vector3Int currentPosition = Vector3Int.zero;

            foreach (var pos in pathReport.Keys)
            {
                currentPosition = (Vector3Int)pos + tilemapOrigin;

                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;

                UnityEditor.Handles.Label(tileGrid.GetCellCenterWorld(currentPosition) - (Vector3.forward * 3), $"f:{pathReport[pos]}", style);
            }

        }
    }
#endif

}
