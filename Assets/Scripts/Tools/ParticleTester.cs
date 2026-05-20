using UnityEngine;

public class ParticleTester : MonoBehaviour
{
    [SerializeField] private SpriteParticle m_particle;
    [SerializeField] private float m_spawnRate = 5f;

    private float m_timer;

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        if(m_timer > m_spawnRate)
        {
            // TODO: Spawn with Pool
            Instantiate(m_particle, transform.position, Quaternion.identity, transform);
            m_timer = 0f;
        }
    }
}
