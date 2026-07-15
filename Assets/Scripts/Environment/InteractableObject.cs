using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Object that allows for generic player interaction to trigger a Unity Event
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class InteractableObject : MonoBehaviour
{
    [SerializeField] private bool m_singleInteraction = false;
    [SerializeField] private UnityEvent<InteractableObject, PlayerController> m_interactionResult = new UnityEvent<InteractableObject, PlayerController>();

    private bool m_disabledInteraction = false;

    public virtual void TriggerInteraction(PlayerController player)
    {
        m_interactionResult?.Invoke(this, player);
        if (m_singleInteraction)
        {
            m_disabledInteraction = true;
            player.ClearInteraction(this);
        } 
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_disabledInteraction)
            return;

        var foundPlayer = collision.attachedRigidbody.gameObject.GetComponent<PlayerController>();
        if (foundPlayer && collision.CompareTag("Player"))
        {
            foundPlayer.PrepareInteraction(this);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (m_disabledInteraction)
            return;

        var foundPlayer = collision.attachedRigidbody.gameObject.GetComponent<PlayerController>();
        if (foundPlayer && collision.CompareTag("Player"))
        {
            foundPlayer.ClearInteraction(this);
        }
    }
}
