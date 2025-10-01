using UnityEngine;

public class ParticleColorModifier : MonoBehaviour
{
    public ParticleSystem[] particleSystems; // ps1, ps2, ps3를 배열로

    [SerializeField] private int middleKeyIndex = 1;

    public void ChangeMiddleColor(Color newColor)
    {
        foreach (var ps in particleSystems)
        {
            if (ps == null) continue;

            // ─── Color over Lifetime ───
            var col = ps.colorOverLifetime;
            col.enabled = true;

            Gradient grad = col.color.gradient;
            GradientColorKey[] colorKeys = grad.colorKeys;
            GradientAlphaKey[] alphaKeys = grad.alphaKeys;

            if (middleKeyIndex >= 0 && middleKeyIndex < colorKeys.Length)
            {
                colorKeys[middleKeyIndex].color = newColor;
            }

            grad.SetKeys(colorKeys, alphaKeys);
            col.color = grad;
        }
    }
}
