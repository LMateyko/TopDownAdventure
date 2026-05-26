using Reflex.Attributes;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(GameDialogRelay))]
public class LockedDoor : MonoBehaviour
{
    [SerializeField] private AudioClip m_lockedAudio;
    [SerializeField] private AudioClip m_unlockAudio;
    [SerializeField] private GameDialogRelay m_lockedText;

    [Inject] readonly private AudioManager AudioManager;

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
            AudioManager.PlaySfxAtLocation(m_unlockAudio, transform.position);
            player.Keys--;
            Destroy(gameObject);
        }
        else
        {
            AudioManager.PlaySfxAtLocation(m_lockedAudio, transform.position);
            m_lockedText.TriggerDialogSequence();
        }
    }
}
