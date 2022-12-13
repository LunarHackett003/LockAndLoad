using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public UnityEvent collideEvent;
    [SerializeField] Rigidbody rb;

    private void OnCollisionEnter(Collision collision)
    {
        collideEvent.Invoke();
    }


    private void FixedUpdate()
    {
        transform.forward = rb.velocity.normalized;
    }
}
