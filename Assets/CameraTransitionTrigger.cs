using UnityEngine;

public class CameraTransitionTrigger : MonoBehaviour
{
    [Tooltip("The direction of the camera shift when transitioning to a new room")]
    [SerializeField] private Vector2 m_cameraDirection = Vector2.zero;
    [Tooltip("Distance to jump the player into the next room")]
    [SerializeField] private float m_playerJumpDistance = 2f;

    private CameraRoomTransitioner m_roomTransitioner;
    private PlayerController m_playerToMove;

    private void Awake()
    {
        // TODO: Retrieve these values via global access when needed instead of finding them on Awake
        m_roomTransitioner = FindFirstObjectByType<CameraRoomTransitioner>();
        m_playerToMove = FindFirstObjectByType<PlayerController>();
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
        if (collision.gameObject != m_playerToMove)
            return;

        m_roomTransitioner.TransitionToNextRoom(m_cameraDirection);

        Vector3 jumpDistance = m_cameraDirection * m_playerJumpDistance;
        m_playerToMove.transform.position += jumpDistance;
    }
}
