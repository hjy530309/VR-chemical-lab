using Fusion;
using Fusion.Addons.ConnectionManagerAddon;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpaceBtn : MonoBehaviour
{
    public string targetSceneName = "Planet";

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(GoSpace);
    }

    void GoSpace()
    {
        Debug.Log($"æ¿ ¿Ãµø: {targetSceneName}");
        SceneManager.LoadScene(targetSceneName);
    }
}
