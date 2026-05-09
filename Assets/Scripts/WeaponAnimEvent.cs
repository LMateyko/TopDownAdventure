using UnityEngine;
using UnityEngine.Events;

public class WeaponAnimEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent OnWeaponStart;
    [SerializeField] private UnityEvent OnWeaponFire;
    [SerializeField] private UnityEvent OnWeaponEnd;

    public void FireWeaponStart()   { OnWeaponStart?.Invoke();  }
    public void FireWeaponFire()    { OnWeaponFire?.Invoke();   }
    public void FireWeaponEnd()     { OnWeaponEnd?.Invoke();    }
}
