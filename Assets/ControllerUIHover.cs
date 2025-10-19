using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ControllerUIHover : MonoBehaviour
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
                    // 이전 버튼 Reset
                    if (lastButton != null) ResetButton(lastButton);
                    // Hover 적용
                    HighlightButton(btn);
                    lastButton = btn;
                }
            }
            else
            {
                if (lastButton != null) ResetButton(lastButton);
                lastButton = null;
            }
        }
        else
        {
            if (lastButton != null) ResetButton(lastButton);
            lastButton = null;
        }
    }

    void HighlightButton(Button btn)
    {
        var colors = btn.colors;
        colors.normalColor = colors.highlightedColor;
        btn.colors = colors;
    }

    void ResetButton(Button btn)
    {
        var colors = btn.colors;
        colors.normalColor = colors.disabledColor; // 원래 색상으로 복원
        btn.colors = colors;
    }
}
