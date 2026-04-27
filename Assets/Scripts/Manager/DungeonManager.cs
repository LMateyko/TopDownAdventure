using UnityEngine;
using Reflex.Core;
using System;
using Reflex.Attributes;

public class DungeonManager : MonoBehaviour, IInstaller
{
    [SerializeField] private DungeonData m_dungeonData;
    [SerializeField] private RoomManager m_startingRoom;

    [Inject] readonly private DungeonMapUI MapUI;

    private Vector2Int m_currentPlayerRoom;
    private RoomManager m_currentRoom;

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

    private void Start()
    {
        MapUI.ConfigureMapDisplay(m_dungeonData);

        m_currentPlayerRoom = m_dungeonData.PlayerStart;
        m_currentRoom = m_startingRoom;
        m_currentRoom.EnterRoom();
    }

}
