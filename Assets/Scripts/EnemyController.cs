using UnityEngine;

public class EnemyController : BaseCharacterController
{
    [Header("Enemy Settings")]
    [SerializeField] private EnemyMovementSetting m_movementBehavior;
    [SerializeField] private int EnemyContactDamage = 1;
    [SerializeField] private float EnemyKnockbackForce = 5f;

    public override int Damage => EnemyContactDamage;
    public override float KnockbackForce => EnemyKnockbackForce;

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

    protected override void DamageTarget(BaseCharacterController defender)
    {
        base.DamageTarget(defender);
        m_movementBehavior.OnDealtDamage(defender);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsAlive)
            return;

        m_movementBehavior.OnCollision(collision);
    }
}
