using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent<Interactable, PlayerController> m_interactionResult = new UnityEvent<Interactable, PlayerController>();

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
