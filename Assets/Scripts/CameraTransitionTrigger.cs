using UnityEngine;

public class CameraTransitionTrigger : MonoBehaviour
{
    [Tooltip("The direction of the camera shift when transitioning to a new room")]
    [SerializeField] private Vector2 m_cameraDirection = Vector2.zero;
    [Tooltip("Distance to jump the player into the next room")]
    [SerializeField] private float m_playerJumpDistance = 2f;

    private CameraRoomTransitioner m_roomTransitioner;
    private PlayerController m_playerToMove;
    private DungeonManager m_dungeonManager;

    ///<summary> Multiply by the camera direction to get the room direction </summary>
    private readonly Vector2 m_cameraFlip = new Vector2(1,-1);

    private void Awake()
    {
        // TODO: Retrieve these values via global access when needed instead of finding them on Awake
        m_roomTransitioner = FindFirstObjectByType<CameraRoomTransitioner>();
        m_playerToMove = FindFirstObjectByType<PlayerController>();
        m_dungeonManager = FindFirstObjectByType<DungeonManager>();
    }

    private void OnValidate()
    {
        if (m_cameraDirection.y < 0)
            m_cameraDirection = Vector2.down;
        else if (m_cameraDirection.y > 0)
            m_cameraDirection = Vector2.up;
        else if (m_cameraDirection.x < 0)
            m_cameraDirection = Vector2.left;
        else if (m_cameraDirection.x > 0)
            m_cameraDirection = Vector2.right;
        else
            m_cameraDirection = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != m_playerToMove.gameObject)
            return;

        m_roomTransitioner.TransitionToNextRoom(m_cameraDirection);

        Vector3 jumpDistance = m_cameraDirection * m_playerJumpDistance;
        m_playerToMove.transform.position += jumpDistance;

        m_dungeonManager.MovePlayerRoomPosition(m_cameraDirection * m_cameraFlip);
    }
}
