using UnityEngine;

public class CharacterProximityAnimator : MonoBehaviour
{
    public float detectDistance = 2f; // 감지 거리
    public Animator animator;         // Animator 연결

    void Update()
    {
        if (animator == null) return;

        Transform cameraTransform = Camera.main?.transform;
        if (cameraTransform == null) return;

        // 카메라와 오브젝트 거리 계산
        float distance = Vector3.Distance(transform.position, cameraTransform.position);

        // Animator의 "near" 파라미터 업데이트
        animator.SetBool("character_nearby", distance <= detectDistance);
    }
}
