using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeechBubbleController : MonoBehaviour
{
    public GameObject speechBubble; // 말풍선 UI 오브젝트
    public Text speechText;         // 말풍선 안의 텍스트
    public float hideDelay = 2f;    // 말 끝나고 사라지는 시간

    private Coroutine hideCoroutine;

    public void ShowBubble(string message)
    {
        // 말풍선 보여주기
        speechBubble.SetActive(true);
        speechText.text = message;

        // 이전 코루틴이 있다면 중지
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        // 일정 시간 후에 말풍선 숨기기 시작
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        speechBubble.SetActive(false);
    }
}
