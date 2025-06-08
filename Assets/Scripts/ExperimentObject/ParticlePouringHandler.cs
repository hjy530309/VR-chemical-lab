using UnityEngine;

public class ParticlePouringHandler : MonoBehaviour
{
    public float transferAmount = 0.01f;

    void OnParticleCollision(GameObject other)
{
    LiquidVolumeAnimator liquid = other.GetComponent<LiquidVolumeAnimator>();
    if (liquid != null)
    {
        liquid.level += transferAmount;
    }
}


}
