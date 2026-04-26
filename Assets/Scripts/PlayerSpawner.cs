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

        if (PlayerManager.Player == null)
            PlayerManager.Player = Instantiate(m_playerPrefab, transform.position, Quaternion.identity);
        else
            PlayerManager.Player.transform.position = transform.position;
    }
}
