using UnityEngine;
using System.Linq;

public class DoorTrigger : MonoBehaviour
{
    public Animator animator;
    public float openDistance = 3f;
    private bool isOpen = false; // 이전 상태 저장

    void Update()
    {
        // 모든 플레이어 찾기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // 가까운 플레이어가 있는지 확인
        bool shouldOpen = players.Any(p => Vector3.Distance(p.transform.position, transform.position) <= openDistance);

        // 상태가 바뀔 때만 Trigger 호출
        if (shouldOpen && !isOpen)
        {
            isOpen = true;
            animator.SetTrigger("OpenDoorTrigger");
        }
        else if (!shouldOpen && isOpen)
        {
            isOpen = false;
            animator.SetTrigger("CloseDoorTrigger");
        }
    }
}
