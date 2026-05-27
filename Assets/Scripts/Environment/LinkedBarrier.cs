using Reflex.Attributes;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class LinkedBarrier : MonoBehaviour
{
    public enum ActiveDirection { Left, Right }

    [SerializeField] private ActiveDirection m_activeDirection = ActiveDirection.Right;
    [SerializeField] private Sprite m_barrierUpSprite;
    [SerializeField] private Sprite m_barrierDownSprite;

    [Inject] readonly private DungeonManager DungeonManager;

    private SpriteRenderer m_spriteRenderer;
    private BoxCollider2D m_collider;

    private void OnValidate()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteRenderer.sprite = m_activeDirection == ActiveDirection.Left ? m_barrierDownSprite : m_barrierUpSprite;
    }

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        SetBarrierForDirection(DungeonManager.CurrentSwitchDirection);
        DungeonManager.OnFlipSwitch += SetBarrierForDirection;
    }

    private void SetBarrierForDirection(ActiveDirection newDirection)
    {
        if (newDirection == m_activeDirection)
        {
            SetBarrierUp();
        }
        else
        {
            SetBarrierDown();
        }
    }

    private void SetBarrierUp()
    {
        m_spriteRenderer.sprite = m_barrierUpSprite;
        m_collider.enabled = true;
    }

    private void SetBarrierDown()
    {
        m_spriteRenderer.sprite = m_barrierDownSprite;
        m_collider.enabled = false;
    }
}
