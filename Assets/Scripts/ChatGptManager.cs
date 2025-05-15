using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ChatGptManager : MonoBehaviour
{
public OnResponseEvent OnResponse;

    [System.Serializable]
    public class OnResponseEvent :  UnityEvent<string> { }
    private OpenAIApi openAI = new OpenAIApi("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");
    private List<ChatMessage> messages = new List<ChatMessage>();
    public InputField inputField;
    public Animator animator;
    public TextMeshProUGUI speechText;
    public GameObject speechBubble;
    public float hideDelay = 2f;

    public async void AskChatGPT(string newText)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = newText;
        newMessage.Role = "user";

        speechText.text = "";
        messages.Add(newMessage);
        speechBubble.SetActive(true);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);

        if(response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);

            OnResponse.Invoke(chatResponse.Content);

            StartCoroutine(TypeText(chatResponse.Content));
        }

        if (inputField != null)
        {
            inputField.text = "";
        }

        int randomIndex = Random.Range(0, 2);
        animator.SetInteger("talkIndex", randomIndex);
        animator.SetTrigger("talking");

    }

    // 코루틴 함수: 텍스트를 하나씩 출력
    private IEnumerator TypeText(string text, int maxLineLength = 20)
{
    speechText.text = "";
    int currentLineLength = 0;

    foreach (char letter in text)
    {
        speechText.text += letter;
        currentLineLength++;

        // 최대 줄 길이를 넘기면 줄바꿈 추가
        if (currentLineLength >= maxLineLength && letter == ' ')
        {
            speechText.text += "\n";
            currentLineLength = 0;
        }

        yield return new WaitForSeconds(0.1f);
    }
    StartCoroutine(HideSpeechBubbleAfterDelay());
}


    private IEnumerator HideSpeechBubbleAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);  // hideDelay 시간 대기
        speechBubble.SetActive(false);  // 말풍선 숨기기
    }

    void Start() {
        speechBubble.SetActive(false);
    }
}
