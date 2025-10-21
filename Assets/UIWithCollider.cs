using UnityEngine;

public class UIWithCollider : MonoBehaviour
{
    void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // BoxCollider 추가 (RayBeamer가 감지할 수 있도록)
            var collider = GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<BoxCollider>();

                // RectTransform 크기에 맞게 콜라이더 조정
                collider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.1f);
                collider.isTrigger = true; // 트리거로 설정
            }

            // Rigidbody 추가 (물리 감지를 위해)
            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true; // 물리 영향 안받도록
                rigidbody.useGravity = false;
            }
        }
    }
}