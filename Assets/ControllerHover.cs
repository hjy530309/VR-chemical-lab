using UnityEngine;
using UnityEngine.UI;

public class ControllerHover : MonoBehaviour
{
    public Transform beamOrigin;           // 컨트롤러 Ray 시작점
    public float maxDistance = 10f;
    public LayerMask uiLayerMask;

    private Button lastButton;

    void Update()
    {
        Ray ray = new Ray(beamOrigin.position, beamOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, uiLayerMask))
        {
            Button btn = hit.collider.GetComponent<Button>();
            if (btn != null)
            {
                if (lastButton != btn)
                {
                    ResetButton(lastButton);
                    HighlightButton(btn);
                    lastButton = btn;
                }
            }
            else
            {
                ResetButton(lastButton);
                lastButton = null;
            }
        }
        else
        {
            ResetButton(lastButton);
            lastButton = null;
        }
    }

    void HighlightButton(Button btn)
    {
        if (btn == null) return;
        var colors = btn.colors;
        colors.normalColor = colors.highlightedColor; // Hover 색상
        btn.colors = colors;
    }

    void ResetButton(Button btn)
    {
        if (btn == null) return;
        var colors = btn.colors;
        colors.normalColor = Color.white; // 원래 색상
        btn.colors = colors;
    }
}
