using UnityEngine;

public class ChalkboardSetup : MonoBehaviour
{
    public float offsetDistance = 0.01f;

    void Start()
    {
        SetupCanvasWithRotationFix();
    }

    void SetupCanvasWithRotationFix()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
        }

        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null || transform.parent == null) return;

        // 부모의 Renderer에서 크기 가져오기
        Renderer parentRenderer = transform.parent.GetComponent<Renderer>();
        if (parentRenderer != null)
        {
            Bounds bounds = parentRenderer.bounds;
            Vector3 parentSize = bounds.size;

            rectTransform.sizeDelta = new Vector2(parentSize.x * 1000f, parentSize.y * 1000f);
            rectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            rectTransform.localPosition = new Vector3(0, 0, -offsetDistance);
        }

        // ★ 회전 보정: 부모의 Z 회전을 상쇄
        Vector3 parentRotation = transform.parent.localEulerAngles;
        rectTransform.localEulerAngles = new Vector3(0, 0, -parentRotation.z);
    }
}