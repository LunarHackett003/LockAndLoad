using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemDestroyer : MonoBehaviour
{

    [SerializeField] ParticleSystem particleSys;

    private void FixedUpdate()
    {
        if(particleSys.time > 0.2f && particleSys.isStopped)
        {
            Destroy(particleSys.gameObject, 0.1f);
        }
    }
}
