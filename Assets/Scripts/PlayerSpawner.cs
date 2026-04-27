using Reflex.Attributes;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerPrefab;

    [Inject] readonly private PlayerManager PlayerManager;

    private void Awake()
    {
        // Does not need to render at runtime
        var editorRenderer = GetComponent<SpriteRenderer>();
        Destroy(editorRenderer);

        PlayerManager.Player.transform.position = transform.position;
    }
}
