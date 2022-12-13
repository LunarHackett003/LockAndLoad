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
        Destroy(gameObject, 0.01f);
        Collider[] cols = Physics.OverlapSphere(transform.position, explodeRange, explosionMask);
        foreach (var item in cols)
        {
            if(item.attachedRigidbody)
                item.attachedRigidbody.AddExplosionForce(explodeForce, transform.position, explodeRange, 0.5f, ForceMode.Impulse);
        }
    }
}
