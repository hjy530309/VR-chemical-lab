using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChatGptManager : MonoBehaviour
{
public OnResponseEvent OnResponse;

    [System.Serializable]
    public class OnResponseEvent :  UnityEvent<string> { }
    private OpenAIApi openAI = new OpenAIApi("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");
    private List<ChatMessage> messages = new List<ChatMessage>();
    public InputField inputField;

    public async void AskChatGPT(string newText)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = newText;
        newMessage.Role = "user";

        messages.Add(newMessage);

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
        }
        if (inputField != null)
        {
            inputField.text = "";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
