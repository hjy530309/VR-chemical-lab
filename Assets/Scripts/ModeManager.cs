using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode { VR, Desktop }


/*   VR / PC 모드 전환    */
public class ModeManager : MonoBehaviour
{
    public GameMode currentMode;
    public GameObject vrPlayer;
    public GameObject pcPlayer;

    void Start()
    {
        if (currentMode == GameMode.VR)
        {
            vrPlayer.SetActive(true);
            pcPlayer.SetActive(false);
        }
        else
        {
            vrPlayer.SetActive(false);
            pcPlayer.SetActive(true);
        }
    }
}
