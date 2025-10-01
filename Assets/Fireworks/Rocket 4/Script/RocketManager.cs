using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketManager : MonoBehaviour
{

    public Rigidbody rig;
    public ConstantForce cf;
    public Transform IsKinematic;
    public ParticleSystem fireParticle;
    public ParticleSystem starParticle;
    public ParticleSystem heartParticle;
    public ParticleSystem circleParticle;
    public GameObject rocket;
    public FireworkShellManager shellMgr;

    void Start()
    {
        cf.force = Vector3.zero;
        fireParticle.Stop();
        starParticle.Stop();
        heartParticle.Stop();
        circleParticle.Stop();
    }
    public IEnumerator StartRocket()

    {
        if(shellMgr.nowstar == 1) starParticle.Play();
        if(shellMgr.nowstar == 2) heartParticle.Play();
        if(shellMgr.nowstar == 3) circleParticle.Play();

        rocket.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        cf.force = new Vector3(0f, 50f, 5f);
        fireParticle.Play();
        //Wait for 3 secs.
        yield return new WaitForSeconds(2);
        
        //Game object will turn off
        GameObject.Find("MeshRenderer4").SetActive(false);

        rig.isKinematic = true;
        cf.enabled = false;


    }
}