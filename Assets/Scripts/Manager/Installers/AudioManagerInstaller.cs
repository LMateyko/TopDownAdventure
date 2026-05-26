using Reflex.Core;
using UnityEngine;

public class AudioManagerInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] AudioManager m_managerPrefab;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        AudioManager spawnedManager = Instantiate(m_managerPrefab);
        containerBuilder.RegisterValue(spawnedManager);
    }
}
