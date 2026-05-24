using Reflex.Core;
using UnityEngine;

public class PlayerManagerInstaller : MonoBehaviour, IInstaller
{
    [Tooltip("Persisting Player Prefab to spawn and track")]
    [SerializeField] private PlayerController PlayerPrefab;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        PlayerManager playerManager = new PlayerManager();

        containerBuilder.RegisterValue(playerManager);

        playerManager.SpawnPlayer(PlayerPrefab);
    }
}
