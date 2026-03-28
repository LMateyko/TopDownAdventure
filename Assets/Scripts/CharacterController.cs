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
    [SerializeField] protected Rigidbody2D m_rigidbody;

    protected bool IsAlive => m_currentHealth > 0;

    private int m_currentHealth;

    private readonly Vector3 FaceRightScale = new Vector3(1, 1, 1);
    private readonly Vector3 FaceLeftScale = new Vector3(-1, 1, 1);

    protected virtual void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    virtual protected void SetFacing(Vector2 moveValue)
    {
        if (moveValue.x > 0)
            transform.localScale = FaceRightScale;
        else if (moveValue.x < 0)
            transform.localScale = FaceLeftScale;
    }

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

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsAlive)
            return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Attack"))
            TakeDamage();
    }

    virtual protected void TakeDamage()
    {
        m_currentHealth--;

        if (m_currentHealth <= 0)
            Destroy(gameObject);
        else
            PlayAnimation("Hurt");
    }
}
