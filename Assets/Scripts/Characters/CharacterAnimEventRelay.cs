using UnityEngine;

public class CharacterAnimEventRelay : MonoBehaviour
{
    /// <summary>
    /// Called via Anim Event to spawn a VFX at a specific time
    /// </summary>
    /// <param name="animEventParticle"></param>
    public void VfxAnimEvent(SpriteParticle animEventParticle)
    {
        // TODO: Use the pool
        Instantiate(animEventParticle, transform.position, Quaternion.identity);
    }

    public void SfxAnimEvent()
    {

    }
}
