using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanTarget : MonoBehaviour
{
    public SpriteRenderer scannedRenderer;

    public void SetReveal(float value)
    {
        Color c = scannedRenderer.color;
        c.a = value;

        scannedRenderer.color = c;
    }
}
