using UnityEngine;

public class EnemyTrackingSpiralMovement : EnemyMovementSetting
{
    [Tooltip("How Quickly the enemy moves around the circle")]
    [SerializeField] private float m_circleSpeed = 2f;
    [Tooltip("How far away from the player the enemy tries to rotate")]
    [SerializeField] private float m_circleRadius = 2.5f;

    [SerializeField] private bool m_rotateClockwise = false;

    private PlayerController m_trackedPlayer;
    private float m_currentAngle;
    private Vector3 m_currentOffset;

    public override void InitializeMovement()
    {
        m_currentOffset = Vector3.zero;
        m_trackedPlayer = FindFirstObjectByType<PlayerController>();

        // Determine starting Angle relative to player
        var xDif = m_enemy.transform.position.x - m_trackedPlayer.transform.position.x;
        var yDif = m_enemy.transform.position.y - m_trackedPlayer.transform.position.y;
        m_currentAngle = Mathf.Atan2(yDif, xDif);
    }

    public override void OnUpdate()
    {
        // Rotate in a circle around the player
        m_currentOffset.x = m_trackedPlayer.transform.position.x + (Mathf.Cos(m_currentAngle) * m_circleRadius);
        m_currentOffset.y = m_trackedPlayer.transform.position.y + (Mathf.Sin(m_currentAngle) * m_circleRadius);
        m_currentOffset.z = m_trackedPlayer.transform.position.z;

        // Lerp towards the position instead of popping
        m_enemy.transform.position = Vector3.Lerp(m_enemy.transform.position, m_currentOffset, Time.deltaTime * m_enemy.CurrentSpeed);

        if (m_enemy.transform.position.x > m_trackedPlayer.transform.position.x)
            m_enemy.FaceLeft();
        else
            m_enemy.FaceRight();

        // Advance the Angle
        if(m_rotateClockwise)
        {
            m_currentAngle -= Time.deltaTime * m_circleSpeed;

            if (m_currentAngle <= 0)
                m_currentAngle += Mathf.PI * 2.0f;
        }

        else
        {
            m_currentAngle += Time.deltaTime * m_circleSpeed;

            if (m_currentAngle > Mathf.PI)
                m_currentAngle -= Mathf.PI * 2.0f;
        }
            
    }

    public override void OnCollision(Collision2D collision)
    {
        m_rotateClockwise = !m_rotateClockwise;
    }
}
