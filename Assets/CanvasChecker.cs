using UnityEngine;

public class CanvasChecker : MonoBehaviour
{
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogWarning("Canvas를 World Space로 변경하세요!");
            canvas.renderMode = RenderMode.WorldSpace;
        }

        // Canvas에 Collider 추가
        if (GetComponent<Collider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(canvas.pixelRect.width, canvas.pixelRect.height, 1f);
        }
    }
}