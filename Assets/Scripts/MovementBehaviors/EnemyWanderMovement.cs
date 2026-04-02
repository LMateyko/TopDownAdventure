using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyWanderMovement : EnemyMovementSetting
{
    [Serializable, Flags]
    private enum ResetMovementCondition
    { 
        None = 0,
        OnAnimComplete  = 1 << 0,
        OnDuration      = 1 << 1,
        OnCollision     = 1 << 2,
    }

    [Tooltip("Settings for when a new movement direction is set.")]
    [SerializeField] private ResetMovementCondition m_resetMovementConditions;
    [Tooltip("How long a direction is maintained before randomizing")]
    [SerializeField] private float m_directionDuration;

    private Vector2 m_moveDirection = Vector2.right;
    private float m_durationTimer = 0f;
    private int m_lastAnimLoop = 0;

    private ContactPoint2D[] m_contactCache = new ContactPoint2D[10];
    private readonly float WallOffset = .025f;

    public override void InitializeMovement() {}

    public override void RestartMovement()
    {
        m_enemy.GetCharacterContactPoints(ref m_contactCache);
        SetRandomDirection(m_enemy, m_contactCache);

        m_durationTimer = 0f;
    }

    public override void OnUpdate()
    {
        if(m_resetMovementConditions.HasFlag(ResetMovementCondition.OnAnimComplete))
        {
            if (m_enemy.AnimLoops() > m_lastAnimLoop)
            {
                m_enemy.GetCharacterContactPoints(ref m_contactCache);
                SetRandomDirection(m_enemy, m_contactCache);
            }

            m_lastAnimLoop = m_enemy.AnimLoops();
        }
        
        if(m_resetMovementConditions.HasFlag(ResetMovementCondition.OnDuration))
        {
            m_durationTimer += Time.deltaTime;
            if (m_durationTimer >= m_directionDuration)
            {
                m_enemy.GetCharacterContactPoints(ref m_contactCache);
                SetRandomDirection(m_enemy, m_contactCache);
                m_durationTimer = 0f;
            }
        }

        m_enemy.SetVelocity(m_moveDirection * m_enemy.CurrentSpeed, true);
    }

    public override void OnCollision(Collision2D collision)
    {
        if(m_resetMovementConditions.HasFlag(ResetMovementCondition.OnCollision))
            SetRandomDirection(m_enemy, collision.contacts);
    }

    private void SetRandomDirection(EnemyController enemy, ContactPoint2D[] currentContacts)
    {
        // Determine valid directions based on current contacts 
        List<Vector2> directions = new List<Vector2>() { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector3 cleanNormal;
        foreach (var contact in currentContacts)
        {
            cleanNormal = contact.normal;
            if (Mathf.Abs(cleanNormal.y) < 0.1f)
                cleanNormal.y = 0;
            if (Mathf.Abs(cleanNormal.x) < 0.1f)
                cleanNormal.x = 0;

            directions.Remove(cleanNormal * -1);

            // ofset position slightly by contact normals
            enemy.transform.position += cleanNormal * WallOffset;
        }

        var randomIndex = UnityEngine.Random.Range(0, directions.Count);
        m_moveDirection = directions[randomIndex];

        enemy.SetFacing(m_moveDirection);
    }
}
