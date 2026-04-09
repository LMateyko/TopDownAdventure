using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerPrefab;

    private void Awake()
    {
        // Does not need to render at runtime
        var editorRenderer = GetComponent<SpriteRenderer>();
        Destroy(editorRenderer);

        var foundPlayer = FindFirstObjectByType<PlayerController>();
        if (foundPlayer == null)
            Instantiate(m_playerPrefab, transform.position, Quaternion.identity);
        else
            foundPlayer.transform.position = transform.position;
    }
}
