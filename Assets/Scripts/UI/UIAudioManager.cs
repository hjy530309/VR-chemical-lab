using UnityEngine;
using UnityEngine.UI;

public class UIAudioManager : MonoBehaviour
{
    public AudioSource uiAudioSource;
    public AudioClip clickSound;

    void OnEnable()
    {
        RegisterButtons();
    }

    void RegisterButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (Button btn in buttons)
        {
            // 중복 등록 방지
            btn.onClick.RemoveListener(PlayClickSound);
            btn.onClick.AddListener(PlayClickSound);
        }
    }

    public void PlayClickSound()
    {
        if (uiAudioSource && clickSound)
            uiAudioSource.PlayOneShot(clickSound);
    }
}
