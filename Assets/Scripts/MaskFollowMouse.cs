using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskFollowMouse : MonoBehaviour
{
    void Update()
    {
        Vector3 mouse =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mouse.z = 0;

        transform.position = mouse;
    }
}
