using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private DungeonData m_dungeonData;

    private DungeonMapUI m_mapUI;
    private Vector2Int m_currentPlayerRoom;

    public void MovePlayerRoomPosition(Vector2 direction)
    {
        m_mapUI.SetMapCell(m_currentPlayerRoom.x, m_currentPlayerRoom.y, DungeonData.MapCellType.Room);

        m_currentPlayerRoom.x += (int)direction.x;
        m_currentPlayerRoom.y += (int)direction.y;
        m_mapUI.SetMapCell(m_currentPlayerRoom.x, m_currentPlayerRoom.y, DungeonData.MapCellType.Player);
    }

    private void Start()
    {
        m_mapUI = FindFirstObjectByType<DungeonMapUI>();
        m_mapUI.ConfigureMapDisplay(m_dungeonData);

        m_currentPlayerRoom = m_dungeonData.PlayerStart;
    }

}
