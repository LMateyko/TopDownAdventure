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
}
