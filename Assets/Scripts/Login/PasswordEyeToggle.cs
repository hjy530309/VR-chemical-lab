using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordEyeToggle : MonoBehaviour
{
    public TMP_InputField passwordInput;
    public Button eyeButton;
    public Sprite eyeOpenSprite;  // 평문 보기 상태
    public Sprite eyeClosedSprite; // 비밀번호 숨김 상태
    public Image eyeImage;  // 눈 아이콘이 붙은 이미지

    private bool isPasswordVisible = false;

    void Start()
    {
        eyeButton.onClick.AddListener(TogglePasswordVisibility);
        SetEyeIcon(); // 초기 상태 아이콘 설정
    }

    void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;

        passwordInput.contentType = isPasswordVisible ?
            TMP_InputField.ContentType.Standard :
            TMP_InputField.ContentType.Password;

        passwordInput.ForceLabelUpdate(); // 즉시 반영
        SetEyeIcon();
    }

    void SetEyeIcon()
    {
        if (eyeImage != null)
        {
            eyeImage.sprite = isPasswordVisible ? eyeOpenSprite : eyeClosedSprite;
        }
    }
}
