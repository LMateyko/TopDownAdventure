using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class BaseCharacterController : MonoBehaviour, IDamageable, IDamager
{
    [Header("Character Settings")]

    [Tooltip("Animation prefix for this character")]
    [SerializeField, PrefabEditOnly] private string m_characterPrefix;
    [SerializeField, PrefabEditOnly] protected float m_speed = 5f;
    [SerializeField, PrefabEditOnly] protected int m_maxHealth = 3;
    [SerializeField, PrefabEditOnly] protected bool m_grounded = true;

    [Space]
    [Header("Local Character References")]
    [SerializeField, PrefabEditOnly] private Animator m_animator;
    [SerializeField, PrefabEditOnly] private Rigidbody2D m_rigidbody;
    [SerializeField, PrefabEditOnly] protected SpriteRenderer m_renderer;

    [Space]
    [Header("Character Events")]
    [Tooltip("Event that fires when the character is hit.")]
    [SerializeField] private UnityEvent<IDamager, IDamageable> m_onHitEvent;
    [Tooltip("Event that fires when the character is initial killed and starts dying")]
    [SerializeField] private UnityEvent<IDamager, IDamageable> m_onKillEvent;
    [Tooltip("Event that fires when the character is cleaned up and removed")]
    [SerializeField] private UnityEvent<IDamager, IDamageable> m_onDestroyEvent;

    public Action<BaseCharacterController> OnKillCharacter { get; set; }
    public Action<BaseCharacterController> OnDestroyCharacter { get; set; }

    public float CurrentSpeed => m_movementPaused ? 0f : m_speed;
    public Vector2 CurrentVelocity => m_rigidbody.linearVelocity;

    public bool IsFalling => IsAnimPlaying("Fall");
    public bool IsHurting => IsAnimPlaying("Hurt") && !IsAnimComplete();
    public bool IsDying => (IsAnimPlaying("Death") || IsFalling) && !IsAnimComplete();

    protected int m_currentHealth;
    private bool m_movementPaused = false;
    private float m_totalAnimTime = 0f;

    private readonly Vector3 FaceRightScale = new Vector3(1, 1, 1);
    private readonly Vector3 FaceLeftScale = new Vector3(-1, 1, 1);
    private const float DeathKnockbackMultiplier = 1.15f;

    #region IDamager Implementation

    virtual public int Damage => 1;
    virtual public float KnockbackForce => 5f;
    virtual public bool AttackEnabled => IsAlive;

    virtual public bool DamageTarget(IDamageable defender)
    {
        if (!AttackEnabled) return false;
        if (!defender.IsValidTarget()) return false;

        var contactDirection = (defender.transform.position - transform.position).normalized;
        defender.Knockback(contactDirection, force: KnockbackForce);
        defender.TakeDamage(this, Damage);

        return true;
    }

    #endregion

    #region IDamageable Implementation

    public bool IsAlive => m_currentHealth > 0;
    public bool IsGrounded => m_grounded;

    public bool IsFloating { get; set; }

    public bool IsValidTarget()
    {
        if (!IsAlive) return false;
        if (IsHurting) return false;

        return true;
    }

    virtual public void TakeDamage(IDamager source, int damage)
    {
        m_currentHealth -= damage;

        if (m_currentHealth <= 0)
        {
            KillCharacter(source);
        }
        else
        {
            m_onHitEvent?.Invoke(source, this);
            PlayAnimation("Hurt");
        }
    }

    public void Knockback(Vector2 direction, float force)
    {
        SetVelocity(direction * force, false);
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
        return Mathf.FloorToInt(m_totalAnimTime / animState.length);
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

    public void RestoreCharacter()
    {
        HealCharacter(m_maxHealth);
        PlayAnimation("Idle");
    }

    virtual public void HealCharacter(int heal)
    {
        m_currentHealth = Mathf.Min(m_currentHealth + heal, m_maxHealth);
    }

    /// <summary>
    /// Begin dying and play death animation when health reaches Zero
    /// </summary>
    virtual protected void KillCharacter(IDamager source, bool instant = false)
    {
        m_onKillEvent?.Invoke(source, this);
        OnKillCharacter?.Invoke(this);

        if(!instant)
            StartCoroutine(DeathRoutine(source));
    }

    /// <summary>
    /// Destroy and cleanup the character. Includes respawning the character or returning to the pool
    /// </summary>
    virtual protected void DestroyCharacter()
    {
        OnDestroyCharacter?.Invoke(this);
        Destroy(gameObject);
    }

    private IEnumerator DeathRoutine(IDamager source)
    {
        PlayAnimation("Death");
        m_rigidbody.linearVelocity *= DeathKnockbackMultiplier;

        while(IsDying)
        {
            yield return null;
        }

        m_onDestroyEvent?.Invoke(source, this);
        DestroyCharacter();
    }
}
