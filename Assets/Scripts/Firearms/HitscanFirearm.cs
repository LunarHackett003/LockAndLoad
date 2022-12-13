using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Firearms
{
    public class HitscanFirearm : BaseFirearm
    {
        public class Bullet
        {
            public float time;
            public Vector3 initialPosition;
            public Vector3 initalVelocity;
            public TrailRenderer tracer;
            public float bulletRadius;
        }


        [SerializeField] LayerMask bulletMask;
        [SerializeField] float bulletDrop;
        [SerializeField] float bulletSpeed;
        [SerializeField] float bulletLifetime;
        [SerializeField] float bulletRadius;
        [SerializeField] TrailRenderer tracer;
        [SerializeField] AnimationCurve damageFalloff;
        [SerializeField] float baseDamage;
        [SerializeField] bool destroyObjects;
        Vector3 GetPosition(Bullet bullet)
        {
            //p + v*t + 0.5 * g * t * t
            Vector3 gravity = bulletDrop * Vector3.down;
            return bullet.initialPosition + bullet.initalVelocity * bullet.time +
                (0.5f * gravity * bullet.time * bullet.time);
        }
        List<Bullet> bullets = new List<Bullet>();
        Bullet CreateBullet(Vector3 position, Vector3 velocity)
        {
            Bullet bullet = new Bullet();
            bullet.initialPosition = position;
            bullet.initalVelocity = velocity;
            bullet.time = 0.0f;
            bullet.bulletRadius = bulletRadius;
            if (tracer)
            {
                bullet.tracer = Instantiate(tracer, position, Quaternion.identity).GetComponent<TrailRenderer>();
                bullet.tracer.AddPosition(position);
                Destroy(bullet.tracer.gameObject, bulletLifetime + (bulletLifetime / 5));
            }
            return bullet;
        }

        protected override void Fire()
        {
            base.Fire();
                FireBullet();
        }
        public void UpdateBullets()
        {
            SimulateBullets();
            DestroyBullets();
        }
        void SimulateBullets()
        {
            bullets.ForEach(bullet =>
            {
                Vector3 p0 = GetPosition(bullet);
                bullet.time += Time.fixedDeltaTime;
                Vector3 p1 = GetPosition(bullet);
                RaycastSegment(p0, p1, bullet);
            });
        }
        void RaycastSegment(Vector3 startPoint, Vector3 endPoint, Bullet bullet)
        {
            Vector3 direction = endPoint - startPoint;
            float distance = (direction).magnitude;

            if (Physics.SphereCast(startPoint, 0.05f, direction, out RaycastHit hitInfo, distance, bulletMask))
            {
                Debug.DrawLine(fireOrigin.position, hitInfo.point, Color.red, 1f);

                //hitEffect.transform.position = hitInfo.point;
                //hitEffect.transform.up = hitInfo.normal;
                //hitEffect.Play();
                if(bullet.tracer)
                    bullet.tracer.transform.position = hitInfo.point;

                if (hitInfo.rigidbody)
                {
                    hitInfo.rigidbody.AddForceAtPosition(EvaluateDamageCurve(bullet) * -hitInfo.normal, hitInfo.point);
                }
                    if(hitInfo.collider && hitInfo.collider.GetComponent<DestructibleSurface>() && destroyObjects)
                    {
                        DestructibleSurface.ReceiveHit(hitInfo.point, direction, bullet.bulletRadius, EvaluateDamageCurve(bullet));
                    }
                bullet.time = bulletLifetime;
            }
            else
            {
                if (bullet.tracer)
                {
                    bullet.tracer.transform.position = endPoint;
                }
                Debug.DrawRay(startPoint, endPoint - startPoint, Color.red, 1f);
            }
        }
            void DestroyBullets()
        {
            bullets.RemoveAll(bullet => bullet.time >= bulletLifetime);
        }
        public float EvaluateDamageCurve(Bullet b)
        {
            float dmg = 0f;
            dmg = damageFalloff.Evaluate(b.time);
            return dmg * baseDamage;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdateBullets();
        }
        void FireBullet()
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            
            if (fireIterations > 0)
            {
                for (int i = 0; i < fireIterations; i++)
                {
                    BulletMaker();
                }
            }
            else
            {
                BulletMaker();
            }
        }

        void BulletMaker()
        {
            float randomX = 0, randomY = 0;
            randomX = Random.Range(-1f, 1f);
            randomY = Random.Range(-1f, 1f);
            Vector3 bulletVelocity = (fireOrigin.forward * bulletSpeed) + (fireVariation.x * fireOrigin.right * randomX) + (fireVariation.y * fireOrigin.up * randomY);
            var bt = CreateBullet(fireOrigin.position, bulletVelocity);
            bullets.Add(bt);
        }
    }
}