using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Firearms
{
    public class ProjectileFirearm : BaseFirearm
    {

        [SerializeField] GameObject projectile;
        [SerializeField] float launchImpulse;

        protected override void Fire()
        {
            base.Fire();

            GameObject proj = Instantiate(projectile, fireOrigin.position, Quaternion.identity);
            projectile.transform.forward = fireOrigin.forward;

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            rb.AddForce(proj.transform.forward * launchImpulse, ForceMode.Impulse);
        }
    }
}