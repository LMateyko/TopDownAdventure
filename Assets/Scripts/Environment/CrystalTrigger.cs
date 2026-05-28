using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class CrystalTrigger : MonoBehaviour, IDamageable
{
    [SerializeField] private Sprite m_resetSprite;
    [SerializeField] private Sprite m_activeSprite;
    [SerializeField] private AudioClip m_triggerAudio;

    [Space]
    [SerializeField] private UnityEvent m_activationEvent;

    [Inject] readonly private AudioManager AudioManager;

    private SpriteRenderer m_spriteRenderer;

    public bool IsAlive { get; private set; }

    public bool IsGrounded => true;

    public bool IsValidTarget()
    {
        return IsAlive;
    }

    public void Knockback(Vector2 direction, float force) {}

    public void TakeDamage(IDamager source, int damage)
    {
        if (source is EnemyController)
            return;

        AudioManager.PlaySfxAtLocation(m_triggerAudio, transform.position);
        m_spriteRenderer.sprite = m_activeSprite;
        m_activationEvent?.Invoke();

        IsAlive = false;
    }

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        IsAlive = true;
    }
}
