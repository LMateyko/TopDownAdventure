using UnityEngine;
using Reflex.Core;
using System;
using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour, IInstaller
{
    [SerializeField] private DungeonData m_dungeonData;
    [SerializeField] private RoomManager m_startingRoom;

    [Space]
    [SerializeField] private Tilemap m_wallTiles;
    [SerializeField] private Tilemap m_hazardTiles;

    [Header("Debug Helpers")]
    [SerializeField] private Vector2Int m_gizmoStartPos;
    [SerializeField] private Vector2Int m_gizmoEndPos;
    [SerializeField] private bool m_displayFullGrid = false;
    [SerializeField] private bool m_displayTestPath = false;

    [Inject] readonly private DungeonMapUI MapUI;
    
    private Vector2Int m_currentPlayerRoom;
    private RoomManager m_currentRoom;

    // TODO: Generate and store the search grid at editor time
    private int[,] m_searchGrid;

    // A structure to hold the necessary parameters
    private struct Cell
    {
        // Row and Column index of its parent
        // Note that 0 <= i <= ROW-1 & 0 <= j <= COL-1
        public int parentX, parentY;
        // f = g + h
        public double finalCost, gridCost, huristic;
    }

    #region Reflex IInstaller
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterValue(this);

    }
    #endregion

    public void MovePlayerRoomPosition(Vector2 direction)
    {
        m_currentRoom = m_currentRoom.LeaveRoom(direction);

        MapUI?.SetMapCell(m_currentPlayerRoom.x, m_currentPlayerRoom.y, DungeonData.MapCellType.Room);

        m_currentPlayerRoom.x += (int)direction.x;
        m_currentPlayerRoom.y -= (int)direction.y;
        MapUI?.SetMapCell(m_currentPlayerRoom.x, m_currentPlayerRoom.y, DungeonData.MapCellType.Player);
    }

    #region A* Grid Search
    public (Stack<Vector3>, Dictionary<Vector2Int, double>) AStarSearch(Vector3 startPos, Vector3 targetPos)
    {
        var tileGrid = m_wallTiles.layoutGrid;

        var startTile = tileGrid.WorldToCell(startPos);
        var targetTile = tileGrid.WorldToCell(targetPos);

        // Start and End tile are the same
        if(startTile == targetTile)
        {
            return (null, null);
        }

        if(!IsValidSquareInGrid(startTile.x, startTile.y) || !IsValidSquareInGrid(targetTile.x, targetTile.y))
        {
            return (null, null);
        }

        // Target is within a wall or hazard
        if (m_searchGrid[startTile.x, startTile.y] != 0
            || m_searchGrid[targetTile.x, targetTile.y] != 0)
        {
            return (null, null);
        }

        int columns = m_searchGrid.GetLength(0);
        int rows    = m_searchGrid.GetLength(1);

        bool[,] closedList = new bool[columns, rows];

        Cell[,] cellDetails = new Cell[columns, rows];
        for(int x = 0; x < columns; x++)
        {
            for(int y = 0; y < rows; y++)
            {
                cellDetails[x, y].finalCost = double.MaxValue;
                cellDetails[x, y].gridCost = double.MaxValue;
                cellDetails[x, y].huristic = double.MaxValue;
                cellDetails[x, y].parentX = -1;
                cellDetails[x, y].parentY = -1;
            }
        }

        int currentX = startTile.x;
        int currentY = startTile.y;

        cellDetails[currentX, currentY].finalCost = 0.0;
        cellDetails[currentX, currentY].gridCost = 0.0;
        cellDetails[currentX, currentY].huristic = 0.0;
        cellDetails[currentX, currentY].parentX = currentX;
        cellDetails[currentX, currentY].parentY = currentY;

        // Sorted set sorted by a double (finalCost) with the X,Y coordinates as a Vector2Int
        SortedSet<(double, Vector2Int)> openList = new SortedSet<(double, Vector2Int)> (
            Comparer<(double, Vector2Int)>.Create((a,b) => a.Item1.CompareTo(b.Item1)));

        openList.Add((0.0, new Vector2Int(currentX, currentY)));

        Dictionary<Vector2Int, double> m_trackedPositions = new Dictionary<Vector2Int, double>();

        while(openList.Count > 0)
        {
            // Removed the lowest value in the set for processing
            (double finalCost, Vector2Int position) pair = openList.Min;
            openList.Remove(pair);

            currentX = pair.position.x;
            currentY = pair.position.y;

            closedList[currentX, currentY] = true;

            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1;  j <= 1; j++)
                {
                    if ((i != 0 && j != 0) || (i == 0 && j == 0))
                        continue;

                    //if (i == 0 && j == 0)
                    //    continue;

                    int newX = currentX + i;
                    int newY = currentY + j;

                    // Check if we are still within the grid
                    if(IsValidSquareInGrid(newX, newY))
                    {
                        // Check if we have reached our target tile
                        if(targetTile.x == newX && targetTile.y == newY)
                        {
                            cellDetails[newX, newY].parentX = currentX;
                            cellDetails[newX, newY].parentY = currentY;
                            return (TracePath(tileGrid, cellDetails, targetTile), m_trackedPositions);
                        }

                        if (!closedList[newX, newY] && IsSquareOpenInGrid(newX, newY))
                        {
                            // Calculate the new values for this path
                            double newGridCost = cellDetails[currentX, currentY].gridCost + 1.0;
                            double newHeuristicValue = CalculateManhatanDistance(newX, newY, targetTile);
                            double newFinalValue = newGridCost + newHeuristicValue;

                            // If the final value for this position is lower than what we have already found, display result
                            if (cellDetails[newX, newY].finalCost == double.MaxValue || cellDetails[newX, newY].finalCost > newFinalValue)
                            {
                                var newPos = new Vector2Int(newX, newY);

                                if (m_trackedPositions.ContainsKey(newPos))
                                    m_trackedPositions[newPos] = Math.Round(newFinalValue);
                                else
                                    m_trackedPositions.Add(newPos, Math.Round(newFinalValue));

                                cellDetails[newX, newY].finalCost = newFinalValue;
                                cellDetails[newX, newY].gridCost = newGridCost;
                                cellDetails[newX, newY].huristic = newHeuristicValue;
                                cellDetails[newX, newY].parentX = currentX;
                                cellDetails[newX, newY].parentY = currentY;

                                openList.Add((newFinalValue, newPos));

                                //if (m_trackedPositions.Count == 4 + 9)
                                //    return (null, m_trackedPositions);
                            }
                        }
                    }
                }
            }
        }

        // Invalid Path
        return (null, m_trackedPositions);
    }

    private bool IsValidSquareInGrid(int x, int y)
    {
        return (x >= 0) && (y >= 0) && (x < m_searchGrid.GetLength(0)) && (y < m_searchGrid.GetLength(1));
    }

    private bool IsSquareOpenInGrid(int x, int y)
    {
        return m_searchGrid[x, y] == 0;
    }

    private double CalculateManhatanDistance(int currentX, int currentY, Vector3Int goalPos)
    {
        return Math.Abs(currentX - goalPos.x) + Math.Abs(currentY - goalPos.y);
    }

    private Stack<Vector3> TracePath(Grid tileGrid, Cell[,] cellDetails, Vector3Int targetPosition)
    {
        Stack<Vector3> results = new Stack<Vector3>();

        int columns = m_searchGrid.GetLength(0);
        int rows = m_searchGrid.GetLength(1);

        int currentX = targetPosition.x;
        int currentY = targetPosition.y;

        Stack<Vector2Int> path = new Stack<Vector2Int>();

        while (cellDetails[currentX, currentY].parentX != currentX 
            || cellDetails[currentX, currentY].parentY != currentY) 
        {
            var currentPosition = Vector3Int.zero;
            currentPosition.x = currentX + m_wallTiles.origin.x;
            currentPosition.y = currentY + m_wallTiles.origin.y;

            results.Push(tileGrid.GetCellCenterWorld(currentPosition));

            int newX = cellDetails[currentX, currentY].parentX;
            int newY = cellDetails[currentX, currentY].parentY;

            currentX = newX;
            currentY = newY;
        }

        return results;
    }
#endregion

    private void Awake()
    {
        BuildPathSearchGrid();
    }

    private void OnValidate()
    {
        if (m_wallTiles == null)
            Debug.LogError($"No Dungeon Wall Tilemap set for Dungeon Manager in Scene: {SceneManager.GetActiveScene().name}");
    }

    private void Start()
    {
        MapUI.ConfigureMapDisplay(m_dungeonData);

        m_currentPlayerRoom = m_dungeonData.PlayerStart;
        m_currentRoom = m_startingRoom;
        m_currentRoom.EnterRoom();
    }

    private void BuildPathSearchGrid()
    {
        if (m_wallTiles == null)
            return;

        m_wallTiles.CompressBounds();

        if(m_hazardTiles != null)
        {
            m_hazardTiles.origin = m_wallTiles.origin;
            m_hazardTiles.size = m_wallTiles.size;
            m_hazardTiles.ResizeBounds();
        }

        // build grid based on size of tilemap
        Vector3Int currentTilePos = Vector3Int.zero;

        m_searchGrid = new int[m_wallTiles.size.x, m_wallTiles.size.y];
        for(int x = 0; x < m_wallTiles.size.x; x++)
        {
            for(int y = 0; y < m_wallTiles.size.y; y++)
            {
                currentTilePos.x = x + m_wallTiles.origin.x;
                currentTilePos.y = y + m_wallTiles.origin.y;

                if (m_wallTiles.HasTile(currentTilePos))
                    m_searchGrid[x, y] = 2;
                else if(m_hazardTiles != null && m_hazardTiles.HasTile(currentTilePos))
                    m_searchGrid[x, y] = 1;
                else
                    m_searchGrid[x, y] = 0;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (m_wallTiles == null)
            return;

        if (Application.isEditor)
            BuildPathSearchGrid();

        int columns = m_searchGrid.GetLength(0);
        int rows    = m_searchGrid.GetLength(1);

        Vector3Int currentPosition = Vector3Int.zero;
        var tileGrid = m_wallTiles.layoutGrid;

        if(m_displayFullGrid)
        {
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    currentPosition.x = x + m_wallTiles.origin.x;
                    currentPosition.y = y + m_wallTiles.origin.y;

                    //if (m_displayTestPath && x == m_gizmoEndPos.x && y == m_gizmoEndPos.y)
                    //    Gizmos.color = Color.magenta;
                    //else if (m_displayTestPath &&  x == m_gizmoStartPos.x && y == m_gizmoStartPos.y)
                    //    Gizmos.color = Color.magenta;

                    if (m_searchGrid[x, y] == 2)
                        Gizmos.color = Color.red;
                    else if (m_searchGrid[x, y] == 1)
                        Gizmos.color = Color.yellow;
                    else
                        Gizmos.color = Color.green;

                    Gizmos.DrawSphere(tileGrid.GetCellCenterWorld(currentPosition), 0.25f);
                }
            }
        }
        
        if(m_displayTestPath)
        {
            Gizmos.color = Color.magenta;

            var offsetStartPos = (Vector3Int)m_gizmoStartPos + m_wallTiles.origin;
            var offsetEndPos = (Vector3Int)m_gizmoEndPos + m_wallTiles.origin;

            Gizmos.DrawSphere(tileGrid.GetCellCenterWorld(offsetStartPos), 0.25f);
            Gizmos.DrawSphere(tileGrid.GetCellCenterWorld(offsetEndPos), 0.25f);

            var debugStart = tileGrid.GetCellCenterWorld((Vector3Int)m_gizmoStartPos);
            var debugEnd = tileGrid.GetCellCenterWorld((Vector3Int)m_gizmoEndPos);

            var debugPath = AStarSearch(debugStart, debugEnd);

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

                foreach (var pos in debugPath.Item2.Keys)
                {
                    currentPosition.x = pos.x + m_wallTiles.origin.x;
                    currentPosition.y = pos.y + m_wallTiles.origin.y;

                    float pathSize = (float)debugPath.Item2[pos] * 0.01f;
                    //Gizmos.DrawSphere(tileGrid.GetCellCenterWorld(currentPosition) - (Vector3.forward), pathSize);

                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    style.alignment = TextAnchor.MiddleCenter;

                    Handles.Label(tileGrid.GetCellCenterWorld(currentPosition) - (Vector3.forward * 3), $"f:{debugPath.Item2[pos]}", style);
                }
            }
        }
    }

}
