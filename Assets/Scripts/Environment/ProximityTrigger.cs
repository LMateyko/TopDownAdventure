using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class ProximityTrigger : MonoBehaviour
{
    [SerializeField] private Sprite m_activeSprite;
    [SerializeField] private Sprite m_inactiveSprite;

    [SerializeField] private UnityEvent OnTrigger;

    private SpriteRenderer m_renderer;

    private void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_renderer.sprite = m_activeSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var foundPlayer = collision.GetComponent<PlayerController>();
        if (foundPlayer != null && foundPlayer.IsGrounded)
        {
            OnTrigger?.Invoke();
            m_renderer.sprite = m_inactiveSprite;
            this.enabled = false;
        }
    }

}
