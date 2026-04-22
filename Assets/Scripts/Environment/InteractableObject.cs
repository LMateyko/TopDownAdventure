using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent<Interactable> m_interactionResult = new UnityEvent<Interactable>();

    public virtual void TriggerInteraction()
    {
        m_interactionResult?.Invoke(this);
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
