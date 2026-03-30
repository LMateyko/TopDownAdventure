using Unity.VisualScripting;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Character Settings")]

    [Tooltip("Animation prefix for this character")]
    [SerializeField] private string m_characterPrefix;
    [SerializeField] protected float m_speed = 5f;
    [SerializeField] private int m_maxHealth = 3;

    [Header("Local Character References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody2D m_rigidbody;

    public float CurrentSpeed => m_speed;

    protected bool IsAlive => m_currentHealth > 0;

    private int m_currentHealth;

    private readonly Vector3 FaceRightScale = new Vector3(1, 1, 1);
    private readonly Vector3 FaceLeftScale = new Vector3(-1, 1, 1);

    protected virtual void Start()
    {
        m_currentHealth = m_maxHealth;
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

    protected void PlayAnimation(string animationName)
    {
        if (IsAnimPlaying(animationName))
            return;

        m_animator.Play($"{m_characterPrefix}_{animationName}");
    }

    protected bool IsAnimPlaying(string animationName)
    {
        var fullAnimName = $"{m_characterPrefix}_{animationName}";
        return m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(fullAnimName);
    }
    
    protected bool IsAnimComplete()
    {
        return m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
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

        TakeDamage();
    }

    virtual protected void TakeDamage()
    {
        // Prevent Multiple Hits while playing the hurt animation
        if (IsAnimPlaying("Hurt"))
            return;

        m_currentHealth--;

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
