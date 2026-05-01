using UnityEngine;

public class UVFoodScanner : MonoBehaviour
{
    [Header("Scanner Settings")]
    public float radius = 2.5f;
    public LayerMask foodLayer;

    [Header("Input")]
    public bool requireInput = true;
    public int mouseButton = 1; // 0 = left click, 1 = right click

    [Header("Reveal")]
    public float revealStrength = 1.5f;

    private readonly Collider2D[] hits = new Collider2D[32];

    private void Update()
    {
        if (requireInput && !Input.GetMouseButton(mouseButton))
            return;

        int hitCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            radius,
            hits,
            foodLayer
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = hits[i];

            if (hit == null)
                continue;

            ScenTarget target = hit.GetComponent<ScenTarget>();

            if (target == null)
                continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            float reveal = revealStrength - distance / radius;
            reveal = Mathf.Clamp01(reveal);

            target.SetReveal(reveal);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}