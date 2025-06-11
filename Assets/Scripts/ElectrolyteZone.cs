using UnityEngine;
using Fusion;

public class ElectrolyteZone : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        var metal = other.GetComponentInParent<MetalObject>();
        if (metal != null)
        {
            Debug.Log("금속이 용액에 들어감");
            metal.InElectrolyte = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        var metal = other.GetComponentInParent<MetalObject>();
        if (metal != null)
        {
            Debug.Log("금속이 용액에서 나옴");
            metal.InElectrolyte = false;
        }
    }
}
