using Reflex.Attributes;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LinkedBarrierLever : MonoBehaviour, IDamageable
{
    [SerializeField] private Sprite m_leftSwitchSprite;
    [SerializeField] private Sprite m_rightSwitchSprite;
    [SerializeField] private AudioClip m_switchFlipAudio;

    public bool IsAlive => true;
    public bool IsGrounded => false;

    [Inject] readonly private DungeonManager DungeonManager;
    [Inject] readonly private AudioManager AudioManager;

    private SpriteRenderer m_spriteRenderer;

    public bool IsValidTarget()
    {
        return true;
    }

    public void Knockback(Vector2 direction, float force) {}

    public void TakeDamage(IDamager source, int damage)
    {
        AudioManager.PlaySfxAtLocation(m_switchFlipAudio, transform.position);
        DungeonManager.FlipSwitch();
    }

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetSwitchForDirection(DungeonManager.CurrentSwitchDirection);
        DungeonManager.OnFlipSwitch += SetSwitchForDirection;
    }

    private void SetSwitchForDirection(LinkedBarrier.ActiveDirection newDirection)
    {
        m_spriteRenderer.sprite = newDirection == LinkedBarrier.ActiveDirection.Left ? m_leftSwitchSprite : m_rightSwitchSprite;
    }
}
