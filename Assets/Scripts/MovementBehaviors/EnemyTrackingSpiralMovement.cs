using Reflex.Attributes;
using System.Collections;
using UnityEngine;

public class EnemyTrackingSpiralMovement : EnemyMovementSetting
{
    [Tooltip("How Quickly the enemy moves around the circle")]
    [SerializeField] private float m_circleSpeed = 2f;
    [Tooltip("How Quickly the radius shrinks during the spiral")]
    [SerializeField] private float m_circleApproachSpeed = 0.5f;
    [Tooltip("How far away from the player the enemy tries to rotate")]
    [SerializeField] private float m_circleRadius = 2.5f;
    
    [Tooltip("Target distance from the player before pausing")]
    [SerializeField] private float m_targetDistance = .5f;
    [Tooltip("How long the enemy pauses after reaching the player")]
    [SerializeField] private float m_pauseDuration = 2f;

    [Tooltip("If the Enemy should Start rotating clockwise or counter clockwise")]
    [SerializeField] private bool m_rotateClockwise = false;

    [Inject] readonly private PlayerManager PlayerManager;
    private PlayerController TrackedPlayer => PlayerManager.Player;

    private float CircleSpeed => m_circleSpeed * (m_circleRadius/m_currentRadius);

    //private PlayerController m_trackedPlayer;
    private float m_currentAngle;
    private float m_currentRadius;
    private Vector3 m_currentOffset;
    private Coroutine m_pauseCoroutine;

    public override void InitializeMovement()
    {
        m_currentOffset = Vector3.zero;
        RestartMovement();
    }

    public override void RestartMovement()
    {
        // Determine starting Angle relative to player
        var xDif = m_enemy.transform.position.x - TrackedPlayer.transform.position.x;
        var yDif = m_enemy.transform.position.y - TrackedPlayer.transform.position.y;
        m_currentAngle = Mathf.Atan2(yDif, xDif);
        m_currentRadius = m_circleRadius;
    }

    public override void OnUpdate()
    {
        m_enemy.SetVelocity(Vector3.zero, false);

        if (TrackedPlayer == null || m_enemy.CurrentSpeed <= 0f)
            return;

        // Rotate in a circle around the player
        m_currentOffset.x = TrackedPlayer.transform.position.x + (Mathf.Cos(m_currentAngle) * m_currentRadius);
        m_currentOffset.y = TrackedPlayer.transform.position.y + (Mathf.Sin(m_currentAngle) * m_currentRadius);
        m_currentOffset.z = TrackedPlayer.transform.position.z;

        // Lerp towards the position instead of popping
        m_enemy.transform.position = Vector3.Lerp(m_enemy.transform.position, m_currentOffset, Time.deltaTime * m_enemy.CurrentSpeed);

        if (m_enemy.transform.position.x > TrackedPlayer.transform.position.x)
            m_enemy.FaceLeft();
        else
            m_enemy.FaceRight();

        // Spiral Towards the player when within Range
        if ((m_enemy.transform.position - m_currentOffset).magnitude <= m_circleRadius)
        {
            m_currentRadius -= Time.deltaTime * m_circleApproachSpeed;
            if (m_currentRadius < m_targetDistance && m_pauseCoroutine == null)
                m_pauseCoroutine = StartCoroutine(PauseMovementRoutine());
        }

        // Advance the Angle
        if (m_rotateClockwise)
        {
            m_currentAngle -= Time.deltaTime * CircleSpeed;

            if (m_currentAngle <= 0)
                m_currentAngle += Mathf.PI * 2.0f;
        }
        else
        {
            m_currentAngle += Time.deltaTime * CircleSpeed;

            if (m_currentAngle > Mathf.PI)
                m_currentAngle -= Mathf.PI * 2.0f;
        }
            
    }

    public override void OnCollision(Collision2D collision)
    {
        m_rotateClockwise = !m_rotateClockwise;
    }

    public override void OnDealtDamage(BaseCharacterController defender)
    {
        if(m_pauseCoroutine == null)
            m_pauseCoroutine = StartCoroutine(PauseMovementRoutine());
    }

    private IEnumerator PauseMovementRoutine()
    {
        m_enemy.PauseMovement();

        yield return new WaitForSeconds(m_pauseDuration);

        if(TrackedPlayer != null)
            RestartMovement();

        m_enemy.ResumeMovement();
        m_pauseCoroutine = null;
    }
}
