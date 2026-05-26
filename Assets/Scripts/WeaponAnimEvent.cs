using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Events;

public class WeaponAnimEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent OnWeaponStart;
    [SerializeField] private UnityEvent OnWeaponFire;
    [SerializeField] private UnityEvent OnWeaponEnd;

    [Inject] readonly private PoolManager PoolManager;
    [Inject] readonly private AudioManager AudioManager;

    public void FireWeaponStart() { OnWeaponStart?.Invoke(); }
    public void FireWeaponFire() { OnWeaponFire?.Invoke(); }
    public void FireWeaponEnd() { OnWeaponEnd?.Invoke(); }

    public void OnPlayVFX(SpriteParticle particle)
    {
        SpriteParticle spawnedParticle = PoolManager.SpawnObject(particle);
        spawnedParticle.transform.position = transform.position;
    }

    public void PlaySFX(AudioClip sfx)
    {
        AudioManager.PlaySfxAtLocation(sfx, transform.position);
    }

    private void Start()
    {
        Reflex.Injectors.GameObjectInjector.InjectObject(gameObject, Reflex.Core.Container.RootContainer);
    }
}
