using UnityEngine;
using TMPro;

public class PasswordChecker : MonoBehaviour
{
    public TMP_InputField inputUser_id;
    public TMP_InputField inputName;
    public TMP_InputField inputEmail;
    public TMP_InputField inputPwd;
    public TMP_InputField inputConfirmPassword;
    public TMP_Text warningText;

    public DatabaseManager dbManager;

    public void CheckPasswordMatch()
    {
        if(dbManager.isIdChecked == false){
            warningText.text = "중복 확인을 해주세요.";
            warningText.color = Color.red;
        }
        else if(inputUser_id.text == "" || inputName.text == "" || inputEmail.text == "" || inputPwd.text == "" || inputConfirmPassword.text == "" ){
            warningText.text = "빈칸을 채워주세요.";
            warningText.color = Color.red;
        }
        else if (inputPwd.text != inputConfirmPassword.text)
        {
            warningText.text = "비밀번호가 일치하지 않습니다.";
            warningText.color = Color.red;
        }
        else
        {
            warningText.text = "";
            dbManager.RegisterUser(inputUser_id.text, inputName.text, inputEmail.text, inputPwd.text);
        }
    }
}
