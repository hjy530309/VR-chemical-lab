using UnityEngine;

public class ControllerLine : MonoBehaviour
{
    public Transform beamOrigin;
    public LineRenderer lineRenderer;
    public float maxDistance = 10f;
    public bool showLine = false;

    void Update()
    {
        lineRenderer.enabled = showLine;

        if (showLine)
        {
            lineRenderer.SetPosition(0, beamOrigin.position);
            lineRenderer.SetPosition(1, beamOrigin.position + beamOrigin.forward * maxDistance);
        }
    }
}
