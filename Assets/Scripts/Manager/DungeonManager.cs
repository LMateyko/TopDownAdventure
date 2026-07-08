using UnityEngine;
using Reflex.Core;
using System;
using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.SceneManagement;
using Adventure.Tools;
using Unity.Cinemachine;

public class DungeonManager : MonoBehaviour, IInstaller
{
    [SerializeField] private DungeonData m_dungeonData;
    [SerializeField] private RoomManager m_startingRoom;
    [SerializeField] private Transform m_roomTransitionRoot;

    [Tooltip("The default direction of the switches in this dungeon")]
    [SerializeField] private LinkedBarrier.ActiveDirection m_defaultSwitchSetting = LinkedBarrier.ActiveDirection.Right;

    [Space]
    [SerializeField] private Tilemap m_wallTiles;
    [SerializeField] private Tilemap m_hazardTiles;

    [Header("Debug Helpers")]
    [SerializeField] private bool m_displayFullGrid = false;

    [Inject] readonly private DungeonMapUI MapUI;
    [Inject] readonly private AudioManager AudioManager;

    public Tilemap DungeonWallTilemap => m_wallTiles;
    public (int[,], Grid, Vector3Int) DungeonTileData => (m_searchGrid, m_wallTiles.layoutGrid, m_wallTiles.origin);
    public LinkedBarrier.ActiveDirection CurrentSwitchDirection { get; private set; }
    public Action<LinkedBarrier.ActiveDirection> OnFlipSwitch { get; set; }

    private Vector2Int m_currentPlayerRoom;
    private RoomManager m_currentRoom;

    // TODO: Generate and store the search grid at editor time
    [SerializeField, HideInInspector] private int[,] m_searchGrid;

    #region Reflex IInstaller
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterValue(this);

    }
    #endregion

    public void MovePlayerRoomPosition(Vector2 direction)
    {
        m_currentRoom = m_currentRoom.LeaveRoom(direction);
        m_roomTransitionRoot.position = m_currentRoom.transform.position;

        MapUI?.SetMapCell(m_currentPlayerRoom.x, m_currentPlayerRoom.y, DungeonData.MapCellType.Room);

        m_currentPlayerRoom.x += (int)direction.x;
        m_currentPlayerRoom.y -= (int)direction.y;
        MapUI?.SetMapCell(m_currentPlayerRoom.x, m_currentPlayerRoom.y, DungeonData.MapCellType.Player);
    }

    public Queue<Vector3> GetPathBetweenPoints(Vector3 startPoint, Vector3 endPoint)
    {
        return GetPathBetweenPoints( startPoint, endPoint, out _);
    }

    public Queue<Vector3> GetPathBetweenPoints(Vector3 startPoint, Vector3 endPoint, out Dictionary<Vector2Int, double> pathReport)
    {
        var startTile = m_wallTiles.layoutGrid.WorldToCell(startPoint) - m_wallTiles.origin;
        var targetTile = m_wallTiles.layoutGrid.WorldToCell(endPoint) - m_wallTiles.origin;

        var aStarPath = AStarPathfinder.AStarSearch(m_searchGrid, startTile, targetTile, out pathReport);

        Queue<Vector3> pathPointQueue = new Queue<Vector3>();

        if(aStarPath != null)
        {
            for (int i = 0; i < aStarPath.Count; i++)
            {
                var worldPosition = m_wallTiles.layoutGrid.GetCellCenterWorld(aStarPath[i] + m_wallTiles.origin);
                pathPointQueue.Enqueue(worldPosition);
            }
        }

        return pathPointQueue;
    }

    public void FlipSwitch()
    {
        if (CurrentSwitchDirection == LinkedBarrier.ActiveDirection.Left)
            CurrentSwitchDirection = LinkedBarrier.ActiveDirection.Right;
        else
            CurrentSwitchDirection = LinkedBarrier.ActiveDirection.Left;

        OnFlipSwitch?.Invoke(CurrentSwitchDirection);
    }

    private void Awake()
    {
        CurrentSwitchDirection = m_defaultSwitchSetting;
    }

    private void Start()
    {
        AudioManager.PlayMusic(m_dungeonData.DungeonMusic);
        MapUI.ConfigureMapDisplay(m_dungeonData);

        m_currentPlayerRoom = m_dungeonData.PlayerStart;
        m_currentRoom = m_startingRoom;
        m_currentRoom.EnterRoom();

        m_roomTransitionRoot.position = m_currentRoom.transform.position;
    }

    private void OnValidate()
    {
        if (m_wallTiles != null)
            BuildPathSearchGrid();
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
        if (!m_displayFullGrid)
            return;

        if (m_wallTiles == null)
            return;

        int columns = m_searchGrid.GetLength(0);
        int rows = m_searchGrid.GetLength(1);

        Vector3Int currentPosition = Vector3Int.zero;
        var tileGrid = m_wallTiles.layoutGrid;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                currentPosition.x = x + m_wallTiles.origin.x;
                currentPosition.y = y + m_wallTiles.origin.y;

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

}
