using Reflex.Core;
using UnityEngine;

public class RootInstaller : MonoBehaviour, IInstaller
{
    //[SerializeField] private GameObject[] ManagersToRegister;

    public void InstallBindings(ContainerBuilder builder)
    {
        //builder.RegisterValue("Hello"); // Note that values are always registered as singletons
        //builder.RegisterType(typeof(PlayerManager), Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Eager);

        //foreach(var manager in ManagersToRegister)
        //{
        //    builder.RegisterValue(manager);
        //}
    }
}