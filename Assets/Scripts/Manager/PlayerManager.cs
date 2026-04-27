using Reflex.Core;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IInstaller
{
    [Tooltip("Persisting Player Prefab to spawn and track")]
    [SerializeField] private PlayerController PlayerPrefab;

    public PlayerController Player { get; private set; } = null;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterValue(this);
        Player = Instantiate(PlayerPrefab);
    }
}
