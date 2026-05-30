using Reflex.Attributes;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource m_musicAudioSource;
    [SerializeField] private PooledSfxSource m_sfxPrefab;

    [Inject] readonly private PoolManager PoolManager;

    public void PlayMusic(AudioClip newMusic)
    {
        if(m_musicAudioSource.clip != newMusic)
        {
            m_musicAudioSource.clip = newMusic;
            m_musicAudioSource.Play();
        }
    }

    public void PauseMusic()
    {
        m_musicAudioSource.Stop();
    }

    public void ResumeMusic()
    {
        m_musicAudioSource.Play();
    }

    public void PlaySfxAtLocation(AudioClip clip, Vector3 position)
    {
        PooledSfxSource sfx = PoolManager.SpawnObject(m_sfxPrefab);
        sfx.transform.position = position;
        sfx.SetSfxClip(clip);
    }

    private void Awake()
    {
        //Reflex.Injectors.GameObjectInjector.InjectObject(gameObject, Reflex.Core.Container.RootContainer);
    }

    private void Start()
    {
        Reflex.Injectors.GameObjectInjector.InjectObject(gameObject, Reflex.Core.Container.RootContainer);
    }
}
