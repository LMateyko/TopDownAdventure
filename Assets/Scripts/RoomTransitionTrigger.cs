using UnityEngine;
using Reflex.Attributes;
using System.Collections.Generic;

public class RoomTransitionTrigger : MonoBehaviour
{
    [Tooltip("The direction of the camera shift when transitioning to a new room")]
    [SerializeField] private Vector2 m_transitionDirection = Vector2.zero;
    [Tooltip("Distance to jump the player into the next room")]
    [SerializeField] private float m_playerJumpDistance = 2f;

    [Inject] private readonly DungeonManager m_dungeonManager;
    [Inject] readonly private PlayerManager PlayerManager;

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
        if (collision.gameObject != PlayerManager.Player.gameObject)
            return;

        Vector3 jumpDistance = m_transitionDirection * m_playerJumpDistance;
        PlayerManager.Player.transform.position += jumpDistance;

        m_dungeonManager.MovePlayerRoomPosition(m_transitionDirection);
    }
}
