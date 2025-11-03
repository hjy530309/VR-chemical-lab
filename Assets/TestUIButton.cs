using UnityEngine;
using UnityEngine.UI;

public class TestUIButton : MonoBehaviour
{
    
    void Start()
    {
        Button testButton = GetComponent<Button>();
        testButton.onClick.AddListener(() => {
            Debug.Log(" 테스트 버튼 클릭 성공!");
        });
    }

    void CheckButton()
    {
        Button button = GetComponent<Button>();
        Collider collider = GetComponent<Collider>();

        if (collider == null)
        {
            Debug.Log("Collider 없음 - BoxCollider 추가 필요");
            // BoxCollider 수동 추가
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            RectTransform rect = GetComponent<RectTransform>();
            boxCollider.size = new Vector3(rect.rect.width, rect.rect.height, 1f);
        }
    }
}
