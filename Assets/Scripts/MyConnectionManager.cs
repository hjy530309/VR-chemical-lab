using Fusion.Addons.ConnectionManagerAddon;
using Fusion;
using UnityEngine;
using System.Diagnostics;

public class MyConnectionManager : ConnectionManager
{
    //public void ChangeScene(string sceneName)
    //{
    //    int buildIndex = UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath(sceneName);
    //    if (buildIndex < 0)
    //    {
    //        Debug.LogError($"씬 {sceneName}이 빌드 설정에 없습니다.");
    //        return;
    //    }

    //    SceneRef sceneRef = SceneRef.FromIndex(buildIndex);

    //    // sceneManager가 제공하는 씬 전환 기능 사용
    //    if (sceneManager is NetworkSceneManagerDefault mgr)
    //    {
    //        mgr.StartSceneLoad(sceneRef, UnityEngine.SceneManagement.LoadSceneMode.Single);
    //    }
    //}
}
