using Reflex.Attributes;
using System;
using UnityEngine;

public class TriggeredDoor : MonoBehaviour
{
    [SerializeField] private AudioClip m_unlockAudio;
    [Inject] readonly private AudioManager AudioManager;

    public void UnlockDoor()
    {
        if (m_unlockAudio != null)
            AudioManager.PlaySfxAtLocation(m_unlockAudio, transform.position);

        Destroy(gameObject);
    }
}
