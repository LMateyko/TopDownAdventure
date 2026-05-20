using System.Collections;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour, IDamageable, IDamager
{
    [Header("Character Settings")]

    [Tooltip("Animation prefix for this character")]
    [SerializeField] private string m_characterPrefix;
    [SerializeField] protected float m_speed = 5f;
    [SerializeField] protected int m_maxHealth = 3;
    [SerializeField] protected bool m_grounded = true;
    [SerializeField] protected SpriteParticle m_deathVFX;

    [Space]
    [Header("Local Character References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody2D m_rigidbody;
    [SerializeField] protected SpriteRenderer m_renderer;

    public float CurrentSpeed => m_movementPaused ? 0f : m_speed;
    public Vector2 CurrentVelocity => m_rigidbody.linearVelocity;

    public bool IsFalling => IsAnimPlaying("Fall");
    public bool IsHurting => IsAnimPlaying("Hurt");
    public bool IsDying => IsAnimPlaying("Death") && !IsAnimComplete();

    protected int m_currentHealth;
    private bool m_movementPaused = false;
    private float m_totalAnimTime = 0f;

    private readonly Vector3 FaceRightScale = new Vector3(1, 1, 1);
    private readonly Vector3 FaceLeftScale = new Vector3(-1, 1, 1);

    #region IDamager Implementation

    virtual public int Damage => 1;
    virtual public float KnockbackForce => 5f;
    virtual public bool AttackEnabled => IsAlive;

    virtual public bool DamageTarget(IDamageable defender)
    {
        if (!AttackEnabled) return false;

        bool validAttack = defender.TakeDamage(Damage);
        if (!validAttack) return false;

        var contactDirection = (defender.transform.position - transform.position).normalized;
        defender.Knockback(contactDirection, force: KnockbackForce);

        return true;
    }

    #endregion

    #region IDamageable Implementation

    public bool IsAlive => m_currentHealth > 0;
    public bool IsGrounded => m_grounded;

    virtual public bool TakeDamage(int damage)
    {
        if (!IsAlive) return false;
        if (IsHurting) return false;

        m_currentHealth -= damage;

        if (m_currentHealth <= 0)
            KillCharacter();
        else
            PlayAnimation("Hurt");

        return true;
    }

    public void Knockback(Vector2 direction, float force)
    {
        SetVelocity(Vector3.zero, false);
        m_rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    #endregion

    protected virtual void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    #region Movement and Facing
    public virtual void FallIntoPit()
    {
        m_movementPaused = true;
        PlayAnimation("Fall");
        SetVelocity(Vector2.zero, false);
    }

    public void PauseMovement()
    {
        m_movementPaused = true;
    }

    public void ResumeMovement()
    {
        m_movementPaused = false;
    }

    public void SetVelocity(Vector2 velocity, bool setFacing)
    {
        m_rigidbody.linearVelocity = velocity;
        if(setFacing)
            SetFacing(m_rigidbody.linearVelocity);
    }

    public virtual void SetFacing(Vector2 moveValue)
    {
        if (moveValue.x > 0)
            FaceRight();
        else if (moveValue.x < 0)
            FaceLeft();
    }

    public void FaceLeft() { transform.localScale = FaceLeftScale; }

    public void FaceRight() {  transform.localScale = FaceRightScale; }

    public int GetCharacterContactPoints(ref ContactPoint2D[] contactPoints)
    {
        return m_rigidbody.GetContacts(contactPoints);
    }
    #endregion

    #region Animation

    public bool IsAnimComplete()
    {
        return m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    public int AnimLoops()
    {
        var animState = m_animator.GetCurrentAnimatorStateInfo(0);
        return Mathf.FloorToInt(m_totalAnimTime % animState.length);
    }

    public void PlayAnimation(string animationName, bool restart = false)
    {
        if (IsAnimPlaying(animationName) && !restart)
            return;

        //Debug.Log($"PlayAnimation for {this.gameObject}: {animationName}");

        // Play new animation and update to set the state immediately 
        m_animator.Play($"{m_characterPrefix}_{animationName}");
        m_animator.Update(0);
        m_totalAnimTime = 0f;
    }

    protected bool IsAnimPlaying(string animationName)
    {
        var fullAnimName = $"{m_characterPrefix}_{animationName}";
        return m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(fullAnimName);
    }

    #endregion

    protected virtual void Update()
    {
        m_totalAnimTime += Time.deltaTime;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // Prevent Damage if already defeated
        if (!IsAlive)
            return;

        // Prevent the attack if attacking currently isn't enabled
        if (!AttackEnabled)
            return;

        // Ignore Player v Player and Enemy v Enemy collisions
        if (collision.CompareTag(gameObject.tag))
            return;

        var defender = collision.GetComponent<IDamageable>();
        if (defender != null)
            DamageTarget(defender);
    }

    virtual public void HealCharacter(int heal)
    {
        m_currentHealth = Mathf.Min(m_currentHealth + heal, m_maxHealth);
    }

    /// <summary>
    /// Begin dying and play death animation when health reaches Zero
    /// </summary>
    virtual protected void KillCharacter()
    {
        StartCoroutine(DeathRoutine());
    }

    /// <summary>
    /// Destroy and cleanup the character. Includes respawning the character or returning to the pool
    /// </summary>
    virtual protected void DestroyCharacter()
    {
        Destroy(gameObject);
    }

    private IEnumerator DeathRoutine()
    {
        PlayAnimation("Death");

        while(IsDying)
        {
            yield return null;
        }

        DestroyCharacter();
    }
}
