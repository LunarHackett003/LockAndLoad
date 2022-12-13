using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RayFire;
public class DestructibleSurface : MonoBehaviour
{
    float currentIntegrity = 100;
    float depletedIntegrityOnHit;

    private void Start()
    {

    }

    public static void ReceiveHit(Vector3 position, Vector3 direction, float radius, float damage)
    {

    }


}
