using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PickupObject : MonoBehaviour
{
    public enum PickupType
    { 
        Heart,
        Key,
        Coin
    }

    [SerializeField] private PickupType m_pickupType;

    protected virtual void OnCollectPickup(PlayerController player)
    {
        switch (m_pickupType)
        {
            case PickupType.Heart:
                player.HealCharacter(1);
                break;
            case PickupType.Key:
                player.Keys++;
                break;
            case PickupType.Coin:
                player.Coins++;
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
