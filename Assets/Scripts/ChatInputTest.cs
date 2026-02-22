using UnityEngine;
using TMPro;
using LLMUnity;

public class ChatInputTest : MonoBehaviour
{
    [Header("LLM Components")]
    public LLMAgent agent;

    [Header("UI Elements")]
    public TMP_InputField inputField;

    void Start()
    {
        inputField.onSubmit.AddListener(OnInputFieldSubmit);
    }

    void OnInputFieldSubmit(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        _ = agent.Chat(text, HandleReply, HandleReplyCompleted);

        inputField.text = "";
        inputField.ActivateInputField();
    }

    void HandleReply(string reply)
    {
        Debug.Log(reply);
    }

    void HandleReplyCompleted()
    {
        Debug.Log("AI has finished speaking.");
    }
}