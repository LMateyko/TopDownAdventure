using Reflex.Core;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiFactoryInstaller : MonoBehaviour, IInstaller
{
    [Tooltip("Persisting GUI Prefab to spawn")]
    [SerializeField] private GameObject GuiPrefab;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        PlayerInventory playerInventory = new PlayerInventory();
        containerBuilder.RegisterValue(playerInventory);

        var fullGui = Instantiate(GuiPrefab);

        DungeonMapUI mapUI = fullGui.GetComponentInChildren<DungeonMapUI>();
        containerBuilder.RegisterValue(mapUI);

        DialogUI dialogUI = fullGui.GetComponentInChildren<DialogUI>();
        containerBuilder.RegisterValue(dialogUI);
    }

}
