using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Object that allows for generic player interaction to trigger a Unity Event
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class InteractableObject : MonoBehaviour
{
    [SerializeField] private UnityEvent<InteractableObject, PlayerController> m_interactionResult = new UnityEvent<InteractableObject, PlayerController>();

    public virtual void TriggerInteraction(PlayerController player)
    {
        m_interactionResult?.Invoke(this, player);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var foundPlayer = collision.attachedRigidbody.gameObject.GetComponent<PlayerController>();
        if (foundPlayer && collision.CompareTag("Player"))
        {
            foundPlayer.PrepareInteraction(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var foundPlayer = collision.attachedRigidbody.gameObject.GetComponent<PlayerController>();
        if (foundPlayer && collision.CompareTag("Player"))
        {
            foundPlayer.ClearInteraction(this);
        }
    }
}
