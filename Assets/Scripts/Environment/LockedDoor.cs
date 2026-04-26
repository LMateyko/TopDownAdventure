using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(GameDialogRelay))]
public class LockedDoor : MonoBehaviour
{
    [SerializeField] private GameDialogRelay m_lockedText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var foundPlayer = collision.attachedRigidbody.gameObject.GetComponent<PlayerController>();
        if (foundPlayer && collision.CompareTag("Player"))
        {
            CheckLock(foundPlayer);
        }
    }

    private void CheckLock(PlayerController player)
    {
        if(player.Keys > 0)
        {
            player.Keys--;
            Destroy(gameObject);
        }
        else
        {
            m_lockedText.TriggerDialogSequence();
        }
    }
}
