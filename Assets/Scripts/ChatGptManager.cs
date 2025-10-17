using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ChatGptManager : MonoBehaviour
{
    // JSON 연결용 클래스
    [System.Serializable]
    public class ChatResponseWrapper
    {
        public string reply;
        public ActionData action;
    }

    [System.Serializable]
    public class ActionData
    {
        public string type;
        public string target;
    }

    public OnResponseEvent OnResponse;

    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIApi openAI = new OpenAIApi("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");

    private List<ChatMessage> messages = new List<ChatMessage>();
    public InputField inputField;
    public Animator animator;
    public TextMeshProUGUI speechText;
    public GameObject speechBubble;
    public float hideDelay = 2f;
    private bool initialized = false;

    public async void AskChatGPT(string newText)
    {
        // 최초 초기화
        if (!initialized)
        {
            ChatMessage setup = new ChatMessage
            {
                Role = "system",
                Content = @"너는 화학 실험을 도와주는 실험 조수야.  
            항상 JSON 형식으로만 대답해야 해.  
            JSON 구조는 아래와 같아:

            {
            ""reply"": ""사용자에게 보여줄 대화문"",
            ""action"": {
                ""type"": ""highlight"", 
                ""target"": ""하이라이트할 아이템 이름""
            }
            }

            만약 하이라이트할 게 없으면 ""action""은 null이거나 생략해도 돼.  
            예를 들어 사용자가 'where is beaker?'라고 물으면:
            {
            ""reply"": ""비이커는 저쪽이에요! 반짝이게 보여드릴게요 🧪"",
            ""action"": { ""type"": ""highlight"", ""target"": ""beaker"" }
            }

            사용자가 일반적인 질문을 하면:
            {
            ""reply"": ""염산은 강산이에요. 취급 시 조심하세요!""
            }
            "
            };

            messages.Add(setup);
            initialized = true;
        }

        // 사용자 메시지 등록
        ChatMessage newMessage = new ChatMessage
        {
            Role = "user",
            Content = newText
        };
        messages.Add(newMessage);

        speechText.text = "";
        speechBubble.SetActive(true);

        // OpenAI 요청
        CreateChatCompletionRequest request = new CreateChatCompletionRequest
        {
            Temperature = 0.7f,
            MaxTokens = 200,
            Messages = messages,
            Model = "gpt-3.5-turbo"
        };

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);
            Debug.Log(chatResponse.Content);

            try
            {
                var wrapper = JsonUtility.FromJson<ChatResponseWrapper>(chatResponse.Content);

                if (wrapper != null)
                {
                    // JSON에 highlight 액션이 있으면 실행
                    if (wrapper.action != null && wrapper.action.type == "highlight")
                    {
                        ObjectHighlighter.Instance.HighlightItem(wrapper.action.target);
                    }

                    // GPT 답변 표시
                    OnResponse.Invoke(wrapper.reply);
                    StartCoroutine(TypeText(wrapper.reply));
                }
                else
                {
                    // JSON 파싱은 되었지만 wrapper가 null → 일반 텍스트 처리
                    HandleNormalText(newText, chatResponse.Content);
                }
            }
            catch
            {
                // JSON 파싱 실패 → 일반 텍스트 처리
                HandleNormalText(newText, chatResponse.Content);
            }
        }

        if (inputField != null)
            inputField.text = "";

        int randomIndex = Random.Range(0, 2);
        animator.SetInteger("talkIndex", randomIndex);
        animator.SetTrigger("talking");
    }

    // 사용자 입력과 GPT 답변 기반 일반 텍스트 처리
    private void HandleNormalText(string userInput, string gptResponse)
    {
        // GPT 답변 표시
        OnResponse.Invoke(gptResponse);
        StartCoroutine(TypeText(gptResponse));

        // 사용자 입력을 기준으로 발광 효과 실행
        if (ObjectHighlighter.Instance != null)
        {
            string lowerInput = userInput.ToLower();
            
            if(lowerInput.Contains("where"))
            foreach (var item in ObjectHighlighter.Instance.ItemKeys)
            {
                if (lowerInput.ToLower().Contains(item.ToLower()))
                {
                    ObjectHighlighter.Instance.HighlightItem(item);

                    // 하이라이트 후 말하기
                    string highlightReply = $"'{item}'를 잘 보이게 해드릴게요!";
                    OnResponse.Invoke(highlightReply);
                    StartCoroutine(TypeText(highlightReply));

                    break;
                }
            }
        }
    }

    // 텍스트 하나씩 출력
    private IEnumerator TypeText(string text, int maxLineLength = 20)
    {
        speechText.text = "";
        int currentLineLength = 0;

        foreach (char letter in text)
        {
            speechText.text += letter;
            currentLineLength++;

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
        yield return new WaitForSeconds(hideDelay);
        speechBubble.SetActive(false);
    }

    void Start()
    {
        speechBubble.SetActive(false);
    }
}
