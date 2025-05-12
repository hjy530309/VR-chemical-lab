using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetId : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string userId = UserInfoManager.Instance.userId;
        Debug.Log("로그인한 아이디: " + userId);
    }
}
