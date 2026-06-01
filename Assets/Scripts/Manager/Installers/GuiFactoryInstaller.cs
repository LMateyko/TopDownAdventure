using Reflex.Core;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiFactoryInstaller : MonoBehaviour, IInstaller
{
    [Tooltip("Persisting GUI Prefab to spawn")]
    [SerializeField] private GameObject GuiPrefab;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        var fullGui = Instantiate(GuiPrefab);

        PlayerInventory playerInventory = new PlayerInventory();
        containerBuilder.RegisterValue(playerInventory);

        PlayerHealthUI playerUI = fullGui.GetComponentInChildren<PlayerHealthUI>(); 
        playerUI.SetInventoryEvents(playerInventory);

        PlayerInputUI playerInputUI = fullGui.GetComponentInChildren<PlayerInputUI>();
        playerInputUI.SetInventoryEvents(playerInventory);

        DungeonMapUI mapUI = fullGui.GetComponentInChildren<DungeonMapUI>();
        containerBuilder.RegisterValue(mapUI);

        DialogUI dialogUI = fullGui.GetComponentInChildren<DialogUI>();
        containerBuilder.RegisterValue(dialogUI);

        playerInventory.LoadInitialValues();
    }

}
