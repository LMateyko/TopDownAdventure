using UnityEngine;

public class EnemyController : BaseCharacterController, IRoomObject
{
    [Header("Enemy Settings")]
    [SerializeField] private EnemyMovementSetting m_movementBehavior;
    [SerializeField] private int m_enemyContactDamage = 1;
    [SerializeField] private float m_enemyKnockbackForce = 5f;
    [SerializeField] private bool m_alwaysSpawnInRoom = false;

    public override int Damage => m_enemyContactDamage;
    public override float KnockbackForce => m_enemyKnockbackForce;
    private bool m_wasHurt = false;
    private Vector3 m_spawnLocation;

    #region IRoomObject Implementation
    public bool IsEnabled { get; private set; }
    public bool PersistantRespawn => m_alwaysSpawnInRoom;

    public void EnableObject()
    {
        IsEnabled = true;
        transform.position = m_spawnLocation;
        PlayAnimation("Run", restart: true);
        m_movementBehavior.RestartMovement();

        m_renderer.gameObject.SetActive(true);
    }

    public void DisableObject()
    {
        IsEnabled = false;
        m_renderer.gameObject.SetActive(false);
    }
    #endregion

    protected void Awake()
    {
        m_movementBehavior.InitializeMovement();
        m_spawnLocation = transform.position;
    }

    //protected override void Start()
    //{
    //    base.Start();
    //    PlayAnimation("Run");

    //    m_movementBehavior.RestartMovement();
    //}

    // Update is called once per frame
    protected override void Update()
    {
        // Currently not in the active room
        if (!IsEnabled) return;

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
