using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseCharacterController
{
    [Header("Enemy Settings")]
    [SerializeField] private EnemyMovementSetting m_movementBehavior;

    private bool m_wasHurt = false;

    protected override void Start()
    {
        base.Start();
        PlayAnimation("Run");

        m_movementBehavior.InitializeMovement();
        m_movementBehavior.RestartMovement();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!IsAlive)
        {
            SetVelocity(Vector2.zero, false);
            return;
        }

        if (IsAnimPlaying("Hurt"))
        {
            m_wasHurt = true;
            return;
        }
        else if(m_wasHurt)
        {
            m_wasHurt = false;
            m_movementBehavior.RestartMovement();
        }

        m_movementBehavior.OnUpdate();
    }

    protected override void DealtDamage(BaseCharacterController defender)
    {
        base.DealtDamage(defender);
        m_movementBehavior.OnDealtDamage(defender);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsAlive)
            return;

        m_movementBehavior.OnCollision(collision);
    }
}
