using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VisualKeyboard;

public class KeyboardActivator : MonoBehaviour, IPointerClickHandler
{
    //public TMP_InputField inputField;
    public InputField inputField;
    public GameObject visualKeyboardPrefab; // 프리팹 혹은 씬 내 키보드
    private GameObject keyboardInstance;
    private VisualKeyboard.VisualKeyboard visualKeyboard;

    public void OnPointerClick(PointerEventData eventData)
    {
        ActivateKeyboard();
    }

    private void ActivateKeyboard()
    {
        if (keyboardInstance == null)
            keyboardInstance = Instantiate(visualKeyboardPrefab);

        keyboardInstance.SetActive(true);

        visualKeyboard = keyboardInstance.GetComponent<VisualKeyboard.VisualKeyboard>();

        var vk = keyboardInstance.GetComponent<VisualKeyboard.VisualKeyboard>();
        vk.targetInputField = inputField;
    }

    private void HandleKeyClick(VisualKeyForKeyboard key)
    {
        if (key.oldKeyCode == KeyCode.Return)  // Enter 처리
        {
            SendText();
        }
    }

    private void SendText()
    {
        string message = inputField.text;
        if (string.IsNullOrEmpty(message)) return;

        // TODO: Photon Fusion RPC 호출 가능
        // networkObject.GetComponent<VRKeyboardNetwork>().RPC_SubmitText(message);

        inputField.text = "";           // 입력 초기화
        if (keyboardInstance != null)
            keyboardInstance.SetActive(false); // 키보드 숨기기

        // 이벤트 구독 해제
        if (visualKeyboard != null)
            visualKeyboard.OnKeyClick -= HandleKeyClick;
    }
}
