using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{

    [SerializeField] float health;
    [SerializeField] GameObject explosionPrefab;


    [SerializeField] bool forceOnExplode, damageOnExplode;
    [SerializeField] float explodeForce, explodeRange, explodeDamage;
    [SerializeField] LayerMask explosionMask;
    [ContextMenu("Boom")]
    public void Explode()
    {
        
        Instantiate(explosionPrefab, transform.position, transform.rotation);

        Collider[] cols = Physics.OverlapSphere(transform.position, explodeRange, explosionMask);
        foreach (var item in cols)
        {
            if(item.attachedRigidbody)
                item.attachedRigidbody.AddExplosionForce(explodeForce * (item.attachedRigidbody.mass / 2), transform.position, explodeRange, 0.5f, ForceMode.Impulse);
        }
        
        
        Destroy(gameObject, 0.01f);
    }
}
