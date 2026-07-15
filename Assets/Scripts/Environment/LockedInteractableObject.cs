using UnityEngine;

/// <summary>
/// Interactable Object that must be unlocked via external trigger
/// </summary>
public class LockedInteractableObject : InteractableObject
{
    [SerializeField] private SpriteRenderer m_lockSprite;

    private bool m_locked = true;

    public void UnlockInteraction()
    {
        m_locked = false;
        m_lockSprite.sprite = null;
    }

    public override void TriggerInteraction(PlayerController player)
    {
        if(!m_locked)
            base.TriggerInteraction(player);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(!m_locked)
            base.OnTriggerEnter2D(collision);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if(!m_locked)
            base.OnTriggerExit2D(collision);
    }
}
