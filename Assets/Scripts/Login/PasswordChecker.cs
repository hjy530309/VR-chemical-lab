using UnityEngine;
using TMPro;

public class PasswordChecker : MonoBehaviour
{
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_Text warningText;

    public void CheckPasswordMatch()
    {
        if (passwordInput.text != confirmPasswordInput.text)
        {
            warningText.text = "비밀번호가 일치하지 않습니다.";
            warningText.color = Color.red;
        }
        else
        {
            warningText.text = ""; // 또는 "확인되었습니다" 등
        }
    }
}
