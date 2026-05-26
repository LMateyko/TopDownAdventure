using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class PooledSfxSource : MonoBehaviour
{
    [Reflex.Attributes.Inject] readonly private PoolManager PoolManager;

    private AudioSource m_audioSource;

    public void SetSfxClip(AudioClip newClip)
    {
        m_audioSource.clip = newClip;
        m_audioSource.Play();
    }

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.loop = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if(gameObject.activeInHierarchy && !m_audioSource.isPlaying)
        {
            m_audioSource.Stop();
            PoolManager.ReleaseObject(gameObject);
        }
    }
}
