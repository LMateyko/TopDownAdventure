
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] List<EnemyController> m_roomEnemies = new List<EnemyController>();

    [Header("Room Connections")]
    [SerializeField] RoomManager m_northRoom;
    [SerializeField] RoomManager m_eastRoom;
    [SerializeField] RoomManager m_southRoom;
    [SerializeField] RoomManager m_westRoom;

    public void EnterRoom()
    {
        // Enable Enemies within room
        foreach (var enemy in m_roomEnemies)
            if(enemy != null)
                enemy.EnableObject();

        // Set Camera to room position
        var cameraPosition = Camera.main.transform.position;
        cameraPosition.x = transform.position.x;
        cameraPosition.y = transform.position.y;

        Camera.main.transform.position = cameraPosition;
    }

    /// <summary>Leave the current room and enter the connected room in a direction. </summary>
    /// <param name="exitDirection">The direction of the connected room to load.</param>
    /// <returns>Returns resulting room in desired direction.</returns>
    public RoomManager LeaveRoom(Vector2 exitDirection)
    {
        // Disable Enemies within the room
        foreach (var enemy in m_roomEnemies)
            if (enemy != null)
                enemy?.DisableObject();

        // Enter Connected Room
        if (exitDirection == Vector2.up)
        {
            m_northRoom.EnterRoom();
            return m_northRoom;
        }
        else if (exitDirection == Vector2.right)
        {
            m_eastRoom.EnterRoom();
            return m_eastRoom;
        }
        else if (exitDirection == Vector2.down)
        {
            m_southRoom.EnterRoom();
            return m_southRoom;
        }
        else // West Room
        {
            m_westRoom.EnterRoom();
            return m_westRoom;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw Red Lines when connected to other rooms
        Gizmos.color = Color.red;
        if (m_northRoom != null)
            Gizmos.DrawLine(transform.position, m_northRoom.transform.position);
        if (m_eastRoom != null)
            Gizmos.DrawLine(transform.position, m_eastRoom.transform.position);
        if (m_southRoom != null)
            Gizmos.DrawLine(transform.position, m_southRoom.transform.position);
        if (m_westRoom != null)
            Gizmos.DrawLine(transform.position, m_westRoom.transform.position);

        // Draw Shorter blue lines to help show empty connections
        Gizmos.color = Color.blue;
        if (m_northRoom == null)
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * 2f));
        if (m_eastRoom == null)
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.right * 2f));
        if (m_southRoom == null)
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * 2f));
        if (m_westRoom == null)
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.left * 2f));
    }
}
