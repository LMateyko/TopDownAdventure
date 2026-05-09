using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_speed = 5f;
    [SerializeField] private SpriteRenderer m_renderer;

    private Vector3 TravelVector => transform.localScale.x * transform.right * m_speed * Time.deltaTime;

    // Update is called once per frame
    private void Update()
    {
        transform.position += TravelVector;

        if (!IsOnScreen())
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }

    private bool IsOnScreen()
    {
        var camera = Camera.main;
        var maxViewportPoint = camera.WorldToViewportPoint(m_renderer.bounds.max);
        var minViewportPoint = camera.WorldToViewportPoint(m_renderer.bounds.min);

        bool renderOnScreen = (maxViewportPoint.x > 0f && minViewportPoint.x < 1f &&
                                    maxViewportPoint.y > 0f && minViewportPoint.y < 1f);
        return renderOnScreen;
    }
}
