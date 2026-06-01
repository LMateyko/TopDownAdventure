using Reflex.Attributes;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(GameDialogRelay))]
public class LockedDoor : MonoBehaviour
{
    [SerializeField] private AudioClip m_lockedAudio;
    [SerializeField] private AudioClip m_unlockAudio;
    [SerializeField] private GameDialogRelay m_lockedText;

    [Inject] readonly private AudioManager AudioManager;
    [Inject] readonly private PlayerInventory PlayerInventory;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var foundPlayer = collision.rigidbody.gameObject.GetComponent<PlayerController>();
        if (foundPlayer && collision.transform.CompareTag("Player"))
        {
            CheckLock(foundPlayer);
        }
    }

    private void CheckLock(PlayerController player)
    {
        if(PlayerInventory.Keys > 0)
        {
            AudioManager.PlaySfxAtLocation(m_unlockAudio, transform.position);
            PlayerInventory.Keys--;
            Destroy(gameObject);
        }
        else
        {
            AudioManager.PlaySfxAtLocation(m_lockedAudio, transform.position);
            m_lockedText.TriggerDialogSequence();
        }
    }
}
