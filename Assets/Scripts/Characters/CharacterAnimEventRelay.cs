using Reflex.Attributes;
using UnityEngine;

public class CharacterAnimEventRelay : MonoBehaviour
{
    [Inject] readonly private PoolManager PoolManager;

    /// <summary>
    /// Called via Anim Event to spawn a VFX at a specific time
    /// </summary>
    /// <param name="animEventParticle"></param>
    public void VfxAnimEvent(SpriteParticle animEventParticle)
    {
        var spawnedParticle = PoolManager.SpawnObject<SpriteParticle>(animEventParticle);
        spawnedParticle.transform.position = transform.position;
        spawnedParticle.transform.rotation = Quaternion.identity;
    }

    public void SfxAnimEvent()
    {

    }
}
