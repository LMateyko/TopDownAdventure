using Reflex.Attributes;
using System;
using UnityEngine;

public class CharacterAnimEventRelay : MonoBehaviour
{
    [Inject] readonly private PoolManager PoolManager;
    [Inject] readonly private AudioManager AudioManager;

    /// <summary>
    /// Called via Anim Event to spawn a VFX at a specific time
    /// </summary>
    /// <param name="animEventParticle"></param>
    public void VfxAnimEvent(VFXAnimData animEventParticle)
    {
        var spawnedParticle = PoolManager.SpawnObject(animEventParticle.animEventParticle);
        spawnedParticle.transform.position = transform.position + animEventParticle.Offset;
        spawnedParticle.transform.rotation = Quaternion.identity;
    }

    public void SfxAnimEvent(AudioClip audioClip)
    {
        AudioManager.PlaySfxAtLocation(audioClip, transform.position);
    }
}


