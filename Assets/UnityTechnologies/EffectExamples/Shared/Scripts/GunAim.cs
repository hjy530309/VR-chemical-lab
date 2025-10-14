using UnityEngine;
using System.Collections;

public class GunAim:MonoBehaviour
{
	public int borderLeft;
	public int borderRight;
	public int borderTop;
	public int borderBottom;

	private Camera parentCamera;
	private bool isOutOfBounds;

	void Start () 
	{
		parentCamera = GetComponentInParent<Camera>();
	}

	void Update()
	{
		float mouseX = Input.mousePosition.x;
		float mouseY = Input.mousePosition.y;

		if (mouseX <= borderLeft || mouseX >= Screen.width - borderRight || mouseY <= borderBottom || mouseY >= Screen.height - borderTop) 
		{
			isOutOfBounds = true;
		} 
		else 
		{
			isOutOfBounds = false;
		}

		if (!isOutOfBounds)
        {
            // 🔸 마우스의 화면 좌표를 카메라 기준 3D 방향 벡터로 변환
            Ray ray = parentCamera.ScreenPointToRay(new Vector3(mouseX, mouseY, 0f));
            
            // 🔸 총이 카메라 앞쪽 일정 거리(예: 5m) 위치를 보도록 설정
            Vector3 targetPos = ray.origin + ray.direction * 5f;

            transform.LookAt(targetPos);
        }
	}

	public bool GetIsOutOfBounds()
	{
		return isOutOfBounds;
	}
}

