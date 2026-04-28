using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScanner : MonoBehaviour
{
    public float radius = 2f;
    public LayerMask foodLayer;

    void Update()
    {
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                transform.position,
                radius,
                foodLayer);

        foreach (Collider2D hit in hits)
        {
            ScanTarget target =
                hit.GetComponent<ScanTarget>();

            if (target != null)
            {
                float dist =
                    Vector2.Distance(
                        transform.position,
                        hit.transform.position);

                float reveal =
                    1 - (dist / radius);

                reveal = Mathf.Clamp01(reveal);

                target.SetReveal(reveal);
            }
        }
    }
}