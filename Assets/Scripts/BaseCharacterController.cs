using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
    [Header("Character Settings")]

    [Tooltip("Animation prefix for this character")]
    [SerializeField] private string m_characterPrefix;
    [SerializeField] protected float m_speed = 5f;
    [SerializeField] protected int m_maxHealth = 3;

    [Header("Local Character References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody2D m_rigidbody;

    public float CurrentSpeed => m_movementPaused ? 0f : m_speed;
    virtual public int Damage => 1;
    virtual public float KnockbackForce => 5f;

    protected bool IsAlive => m_currentHealth > 0;

    protected int m_currentHealth;
    private bool m_movementPaused = false;
    private float m_totalAnimTime = 0f;

    private readonly Vector3 FaceRightScale = new Vector3(1, 1, 1);
    private readonly Vector3 FaceLeftScale = new Vector3(-1, 1, 1);

    protected virtual void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    #region Movement and Facing
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

    public void PlayAnimation(string animationName)
    {
        if (IsAnimPlaying(animationName))
            return;

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

        // Only process triggers that are attacks
        if (collision.gameObject.layer != LayerMask.NameToLayer("Attack"))
            return;

        // Ignore Player v Player and Enemy v Enemy collisions
        if (collision.attachedRigidbody.gameObject.CompareTag(gameObject.tag))
            return;

        if (!IsAnimPlaying("Hurt"))
        {
            BaseCharacterController attacker = collision.attachedRigidbody.GetComponent<BaseCharacterController>();
            if (attacker != null)
                attacker.DamageTarget(this);
        }
    }

    virtual protected void DamageTarget(BaseCharacterController defender)
    {
        defender.TakeDamage(Damage);

        var contactDirection = (defender.transform.position - transform.position).normalized;
        defender.Knockback(contactDirection, force: KnockbackForce);
    }

    protected void Knockback(Vector2 direction, float force)
    {
        m_rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    virtual protected void TakeDamage(int damage)
    {
        m_currentHealth -= damage;

        if (m_currentHealth <= 0)
            KillCharacter();
        else
            PlayAnimation("Hurt");
    }

    virtual protected void KillCharacter()
    {
        Destroy(gameObject);
    }
}
