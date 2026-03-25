using UnityEngine;

public class CameraRoomTransitioner : MonoBehaviour
{
    [Tooltip("Size of room to determine distance camera moves during transition")]
    [SerializeField] private Vector2 m_roomTileSize = new Vector2(10, 10);

    public void TransitionToNextRoom(Vector2 roomDirection)
    {
        Vector3 cameraJumpDistance = roomDirection * m_roomTileSize;

        gameObject.transform.position += cameraJumpDistance;
    }
}
