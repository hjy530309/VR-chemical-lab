using UnityEngine;

public class ParticleLiquidCollision : MonoBehaviour
{
    public Progress progress;
    public LiquidType liquidType;

    void OnParticleCollision(GameObject other)
    {
        var target = other.GetComponentInChildren<LiquidVolumeAnimator>();
        if (target != null && target.level >= 0.1f)
        {
            progress.OnLiquidTouch(liquidType, other);
        }
    }
}
