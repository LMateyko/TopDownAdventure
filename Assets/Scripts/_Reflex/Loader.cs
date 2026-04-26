using Reflex.Core;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private void Start()
    {
        void InstallExtra(UnityEngine.SceneManagement.Scene scene, ContainerBuilder builder)
        {
            builder.RegisterValue("of Developers");
        }

        // This way you can access ContainerBuilder of the scene that is currently building
        ContainerScope.OnSceneContainerBuilding += InstallExtra;

        // If you are loading scenes without addressables
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Greet").completed += operation =>
        {
            ContainerScope.OnSceneContainerBuilding -= InstallExtra;
        };

        //// If you are loading scenes with addressables
        //UnityEngine.AddressableAssets.Addressables.LoadSceneAsync("Greet").Completed += operation =>
        //{
        //    ContainerScope.OnSceneContainerBuilding -= InstallExtra;
        //};
    }
}