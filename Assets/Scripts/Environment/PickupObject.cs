using Reflex.Attributes;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PickupObject : MonoBehaviour
{
    public enum PickupType
    { 
        Heart,
        Key,
        Coin,
        Weapon_Sword,
        Weapon_Book,
        Weapon_Bow,
        Weapon_Pick
    }

    [SerializeField] private PickupType m_pickupType;
    [SerializeField] private AudioClip m_audioClip;

    [Inject] readonly private AudioManager AudioManager;
    [Inject] readonly private PlayerInventory PlayerInventory;

    protected virtual void OnCollectPickup(PlayerController player)
    {
        if(m_audioClip != null)
            AudioManager.PlaySfxAtLocation(m_audioClip, transform.position);

        switch (m_pickupType)
        {
            case PickupType.Heart:
                player.HealCharacter(1);
                break;
            case PickupType.Key:
                PlayerInventory.Keys++;
                break;
            case PickupType.Coin:
                PlayerInventory.Coins++;
                break;

            case PickupType.Weapon_Sword:
                PlayerInventory.AddWeaponToInventory(WeaponConfiguration.WeaponEnum.Sword);
                break;
            case PickupType.Weapon_Pick:
                PlayerInventory.AddWeaponToInventory(WeaponConfiguration.WeaponEnum.Pick);
                break;
            case PickupType.Weapon_Book:
                PlayerInventory.AddWeaponToInventory(WeaponConfiguration.WeaponEnum.Book);
                break;
            case PickupType.Weapon_Bow:
                PlayerInventory.AddWeaponToInventory(WeaponConfiguration.WeaponEnum.Bow);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var foundPlayer = collision.attachedRigidbody.gameObject.GetComponent<PlayerController>();
        if (foundPlayer && collision.CompareTag("Player"))
        {
            OnCollectPickup(foundPlayer);
            Destroy(gameObject);
        }
    }
}
