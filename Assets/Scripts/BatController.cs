using Unity.VisualScripting;
using UnityEngine;

public class BatController : EnemyController
{
    [Tooltip("How Quickly the Bat moves around the circle")]
    [SerializeField] private float m_circleSpeed = 2f;
    [Tooltip("How far away from the player the bat tries to rotate")]
    [SerializeField] private float m_circleRadius = 2.5f;

    private float m_currentAngle;
    private Vector3 m_currentOffset;

    private PlayerController m_trackedPlayer;

    protected override void Start()
    {
        base.Start();
        m_currentOffset = Vector3.zero;
        m_trackedPlayer = FindFirstObjectByType<PlayerController>();
    }

    protected override void Update()
    {
        // TODO: Split movement behaviors into states to maintain the Controller configuration in variants
        //  Alternative Suggestion: Have enemy configuration in a scriptable object that runs the behavior in update

        if (!IsAlive)
        {
            m_rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        // Rotate in a circle around the player
        m_currentOffset.x = m_trackedPlayer.transform.position.x + (Mathf.Cos(m_currentAngle) * m_circleRadius);
        m_currentOffset.y = m_trackedPlayer.transform.position.y + (Mathf.Sin(m_currentAngle) * m_circleRadius);
        m_currentOffset.z = m_trackedPlayer.transform.position.z;

        // Lerp towards the position instead of popping
        transform.position = Vector3.Lerp(transform.position, m_currentOffset, Time.deltaTime * m_speed);

        if (transform.position.x > m_trackedPlayer.transform.position.x)
            FaceLeft();
        else
            FaceRight();

        // Advance the Angle
        m_currentAngle += Time.deltaTime * m_circleSpeed;
        if (m_currentAngle > Mathf.PI)
            m_currentAngle -= Mathf.PI * 2.0f;
    }
}
