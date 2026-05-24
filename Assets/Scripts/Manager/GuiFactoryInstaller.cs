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

        PlayerHealthUI playerUI = fullGui.GetComponentInChildren<PlayerHealthUI>(); 
        containerBuilder.RegisterValue(playerUI);

        DungeonMapUI mapUI = fullGui.GetComponentInChildren<DungeonMapUI>();
        containerBuilder.RegisterValue(mapUI);

        DialogUI dialogUI = fullGui.GetComponentInChildren<DialogUI>();
        containerBuilder.RegisterValue(dialogUI);
    }

}
