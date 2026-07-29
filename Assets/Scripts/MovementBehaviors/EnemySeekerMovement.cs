using Reflex.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Move towards the player's position with an A* Search path
/// </summary>
public class EnemySeekerMovement : EnemyMovementSetting
{
    [Tooltip("How long the enemy paused after successfully reaching the player")]
    [SerializeField] private float m_contactPauseDuration = 2.5f;
    [Tooltip("How many squares the enemy travel before recalculating the path")]
    [SerializeField] private int m_pathResetRate = 1;

    [Inject] readonly private PlayerManager PlayerManager;
    [Inject] readonly private DungeonManager DungeonManager;

    private PlayerController TrackedPlayer => PlayerManager.Player;
    private Vector3 NextPathPosition => m_currentPath.Count > 0 ? m_currentPath.Peek() : m_enemy.transform.position;

    private Queue<Vector3> m_currentPath = new Queue<Vector3>();
    private int m_initialPathLength = 0;
    private Coroutine m_pauseCoroutine;

    public override void InitializeMovement() {}

    public override void RestartMovement()
    {
        if (PlayerManager.Player == null)
            return;

        m_currentPath = DungeonManager.GetPathBetweenPoints(transform.position, PlayerManager.Player.transform.position);
        m_initialPathLength = m_currentPath.Count;
        m_enemy.SetFacing(NextPathPosition - transform.position);
    }

    public override void OnUpdate()
    {
        if(m_currentPath.Count == 0)
        {
            RestartMovement();
            return;
        }

        if (m_enemy.CurrentSpeed == 0)
            m_enemy.PlayAnimation("Idle", false);
        else
            m_enemy.PlayAnimation("Run", false);

        m_enemy.transform.position = Vector3.MoveTowards(m_enemy.transform.position, NextPathPosition, Time.deltaTime * m_enemy.CurrentSpeed);
        if((NextPathPosition - m_enemy.transform.position).magnitude < 0.01f)
        {
            m_currentPath.Dequeue();
            if(m_initialPathLength - m_currentPath.Count >= m_pathResetRate)
            {
                RestartMovement();
            }
            else if(m_currentPath.Count > 0)
                m_enemy.SetFacing(NextPathPosition - transform.position);
        }
    }

    public override void OnCollision(Collision2D collision) {}

    public override void OnDamageTarget(IDamageable defender)
    {
        if (m_pauseCoroutine == null)
            m_pauseCoroutine = StartCoroutine(PauseMovementRoutine());
    }

    private IEnumerator PauseMovementRoutine()
    {
        m_enemy.PauseMovement();

        yield return new WaitForSeconds(m_contactPauseDuration);

        if (TrackedPlayer != null)
            RestartMovement();

        m_enemy.ResumeMovement();
        m_pauseCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_currentPath != null)
        {
            Gizmos.color = Color.blue;

            Vector3 prevPos = transform.position;

            foreach (var pos in m_currentPath)
            {
                var worldPosition = pos;
                Gizmos.DrawLine(prevPos, worldPosition);
                prevPos = worldPosition;
            }
        }
    }
}
