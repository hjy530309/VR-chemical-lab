using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chalkboard : MonoBehaviour
{
    public Vector2 boardSize = new Vector2(2f, 1.5f); // 가로, 세로 크기

    void Start()
    {
        // 칠판 크기 설정 (회전 없이)
        transform.localScale = new Vector3(boardSize.x, boardSize.y, 1f);
        transform.localRotation = Quaternion.identity; // 회전 없음
        transform.localPosition = new Vector3(0, 1.5f, 0); // 적절한 높이
    }
}
