using Fusion;
using Fusion.Addons.ConnectionManagerAddon;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpaceBtn : MonoBehaviour
{
    public string targetSceneName = "Planet";
    public string rigMode = "Desktop";
    public GameObject gun; //보여야 작동이 제대로 됨

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(GoSpace);
    }

    void GoSpace()
    {
        if (!gun.activeSelf)
        {
            gun.SetActive(true); // 숨겨져 있으면 켠다
        }
        Debug.Log($"�� �̵�: {targetSceneName}");
        PlayerPrefs.SetString(Fusion.XR.Host.Desktop.RigSelection.SETTING_RIGMODE, rigMode);
        PlayerPrefs.Save();
        SceneManager.LoadScene(targetSceneName);
    }
}
