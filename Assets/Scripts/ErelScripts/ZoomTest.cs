using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomTest : MonoBehaviour
{
    [SerializeField] private float zoomOutMin;
    [SerializeField] private float zoomOutMax;
    bool iszooming = false;
    float Clampx, Clampy;
    private void Update()
    {
        float MouseWheelAxis = Input.GetAxis("Mouse ScrollWheel");
        if (MouseWheelAxis != 0)
        {
            zoom(MouseWheelAxis);

        }
        if (iszooming)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(Clampx, Clampy), 0.5f);
        }
    }

    public void zoom(float increment)
    {
        Clampx = Mathf.Clamp(transform.localScale.x + increment, zoomOutMin, zoomOutMax);
        Clampy = Mathf.Clamp(transform.localScale.y + increment, zoomOutMin, zoomOutMax);
        iszooming = true;

    }
}
