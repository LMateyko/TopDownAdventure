using Reflex.Core;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiFactoryInstaller : MonoBehaviour, IInstaller
{
    [Tooltip("Persisting GUI Prefab to spawn")]
    [SerializeField] private GameObject GuiPrefab;
    [Tooltip("Player Manager for associating the player events with GUI changes")]
    [SerializeField] private PlayerManager PlayerManager;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        var fullGui = Instantiate(GuiPrefab);

        PlayerHealthUI playerUI = fullGui.GetComponentInChildren<PlayerHealthUI>(); 
        containerBuilder.RegisterValue(playerUI);

        playerUI.SetPlayerEvents(PlayerManager.Player);

        DungeonMapUI mapUI = fullGui.GetComponentInChildren<DungeonMapUI>();
        containerBuilder.RegisterValue(mapUI);

        DialogUI dialogUI = fullGui.GetComponentInChildren<DialogUI>();
        containerBuilder.RegisterValue(dialogUI);
    }

}
