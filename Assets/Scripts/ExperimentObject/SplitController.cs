using UnityEngine;

public class SplitController : MonoBehaviour
{
    public Renderer rend;

    [Range(0f, 1f)]
    public float split = 0f; // 초기값
    

    void Start()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();

        // 초기 _Split 값 설정
        rend.material.SetFloat("_Split", split);
    }

    public void SetSplit(float value)
    {
        split = Mathf.Clamp01(value); // 0~1 범위 제한
        rend.material.SetFloat("_Split", split);
    }
    public void SetSplitDelta(float delta)
    {
        split = Mathf.Clamp01(split + delta); // 0~1 범위 유지
        rend.material.SetFloat("_Split", split);
    }

    public void ChangePowderColor(Color colorBottom, Color colorTop)
    {
        rend.material.SetColor("_ColorBottom", colorBottom);
        rend.material.SetColor("_ColorTop", colorTop);
    }
}
