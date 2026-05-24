using Reflex.Core;
using UnityEngine;

public class PoolManagerInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        GameObject poolManagerObject = new GameObject("Pool Manager");
        DontDestroyOnLoad(poolManagerObject);

        PoolManager poolManager = poolManagerObject.AddComponent<PoolManager>();

        containerBuilder.RegisterValue(poolManager);
    }
}
