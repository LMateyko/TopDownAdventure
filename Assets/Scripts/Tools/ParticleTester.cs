using Reflex.Attributes;
using UnityEngine;

public class ParticleTester : MonoBehaviour
{
    [SerializeField] private SpriteParticle m_particle;
    [SerializeField] private float m_spawnRate = 5f;

    [Inject] readonly private PoolManager PoolManager;

    private float m_timer;

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        if(m_timer > m_spawnRate)
        {
            SpriteParticle spawnedParticle = PoolManager.SpawnObject(m_particle);
            spawnedParticle.transform.position = transform.position;
            spawnedParticle.transform.rotation = Quaternion.identity;

            m_timer = 0f;
        }
    }
}
