using UnityEngine;

public class RoomTransitionTrigger : MonoBehaviour
{
    [Tooltip("The direction of the camera shift when transitioning to a new room")]
    [SerializeField] private Vector2 m_transitionDirection = Vector2.zero;
    [Tooltip("Distance to jump the player into the next room")]
    [SerializeField] private float m_playerJumpDistance = 2f;

    private PlayerController m_playerToMove;
    private DungeonManager m_dungeonManager;

    private void Start()
    {
        // TODO: Retrieve these values via global access when needed instead of finding them on Awake
        m_playerToMove = FindFirstObjectByType<PlayerController>();
        m_dungeonManager = FindFirstObjectByType<DungeonManager>();
    }

    private void OnValidate()
    {
        if (m_transitionDirection.y < 0)
            m_transitionDirection = Vector2.down;
        else if (m_transitionDirection.y > 0)
            m_transitionDirection = Vector2.up;
        else if (m_transitionDirection.x < 0)
            m_transitionDirection = Vector2.left;
        else if (m_transitionDirection.x > 0)
            m_transitionDirection = Vector2.right;
        else
            m_transitionDirection = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != m_playerToMove.gameObject)
            return;

        Vector3 jumpDistance = m_transitionDirection * m_playerJumpDistance;
        m_playerToMove.transform.position += jumpDistance;

        m_dungeonManager.MovePlayerRoomPosition(m_transitionDirection);
    }
}
