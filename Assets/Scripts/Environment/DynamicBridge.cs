using UnityEngine;

/// <summary>
/// Prevent the player from dying to pits while the bridge is active
/// </summary>
[RequireComponent (typeof(BoxCollider2D))]
public class DynamicBridge : MonoBehaviour
{
    [SerializeField] private bool m_startActive = false;

    private bool m_bridgeActive = false;

    public void ActivateBridge()
    {
        m_bridgeActive = true;
    }

    private void Awake()
    {
        m_bridgeActive = m_startActive;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_bridgeActive) return;

        var foundCharacter = collision.attachedRigidbody.gameObject.GetComponent<BaseCharacterController>();
        if (foundCharacter)
        {
            foundCharacter.IsFloating = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!m_bridgeActive) return;

        var foundCharacter = collision.attachedRigidbody.gameObject.GetComponent<BaseCharacterController>();
        if (foundCharacter)
        {
            foundCharacter.IsFloating = false;
        }
    }
}
