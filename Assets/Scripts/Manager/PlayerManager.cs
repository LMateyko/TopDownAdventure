using Reflex.Core;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IInstaller
{
    public PlayerController Player { get; set; } = null;

    public ChestRewardData TestData;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        // TODO: Turn into a Factory and spawning the player here. 
        containerBuilder.RegisterValue(this);
    }
}
