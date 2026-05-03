using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

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
    [Tooltip("Layers to that determine invalid movement directions")]
    [SerializeField] LayerMask m_avoidedLayers;

    private Vector2 m_moveDirection = Vector2.right;
    private float m_durationTimer = 0f;
    private int m_lastAnimLoop = 0;

    public override void InitializeMovement() {}

    public override void RestartMovement()
    {
        SetRandomDirection(m_enemy);

        m_durationTimer = 0f;
    }

    public override void OnUpdate()
    {
        if(m_resetMovementConditions.HasFlag(ResetMovementCondition.OnAnimComplete))
        {
            if (m_enemy.AnimLoops() > m_lastAnimLoop)
            {
                SetRandomDirection(m_enemy);
            }

            m_lastAnimLoop = m_enemy.AnimLoops();
        }
        
        if(m_resetMovementConditions.HasFlag(ResetMovementCondition.OnDuration))
        {
            m_durationTimer += Time.deltaTime;
            if (m_durationTimer >= m_directionDuration)
            {
                SetRandomDirection(m_enemy);
                m_durationTimer = 0f;
            }
        }

        m_enemy.SetVelocity(m_moveDirection * m_enemy.CurrentSpeed, true);
    }

    public override void OnCollision(Collision2D collision)
    {
        if(m_resetMovementConditions.HasFlag(ResetMovementCondition.OnCollision))
            SetRandomDirection(m_enemy);
    }

    public override void OnDamageTarget(IDamageable defender) {}

    private void SetRandomDirection(EnemyController enemy)
    {
        // Determine valid directions based on current contacts 
        List<Vector2> directions = new List<Vector2>() { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        // Reduce directions further looking for hazards
        for(int i = directions.Count -1; i >= 0; i--)
        {
            float castRange = 1f;

            var direction = directions[i];
            var overlap = Physics2D.OverlapPoint(transform.position + ((Vector3)direction * castRange), m_avoidedLayers);
            if (overlap != null)
                directions.RemoveAt(i);
        }

        if(directions.Count > 0)
        {
            var randomIndex = UnityEngine.Random.Range(0, directions.Count);
            m_moveDirection = directions[randomIndex];
        }
        else
        {
            m_moveDirection = Vector2.zero;
        }

        enemy.SetFacing(m_moveDirection);
    }
}
