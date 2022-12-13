using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;


namespace Eclipse.Firearms
{
    public class BaseFirearm : BaseHandheld
    {
        
        
        [Serializable]
        public class Ammunition
        {
            public int maxAmmo, maxReserveAmmo;
            public int currentAmmo, currentReserveAmmo;
            public bool extraChambered;
            public int GetCurrentAmmo()
            {
                return currentAmmo;
            }
            public int GetReserveAmmo()
            {
                return currentReserveAmmo;
            }
        }

        public List<AnimOverride> animOverrides = new List<AnimOverride>()
        {
            new AnimOverride() {clipTargetName = "Fire"},
            new AnimOverride() {clipTargetName = "ADSFire"},
            new AnimOverride() {clipTargetName = "HipIdle"},
            new AnimOverride() {clipTargetName = "ADS"},
            new AnimOverride() {clipTargetName = "Move"},
            new AnimOverride() {clipTargetName = "ADSMove"},
            new AnimOverride() {clipTargetName = "Reload"},
            new AnimOverride() {clipTargetName = "Draw"},
            new AnimOverride() {clipTargetName = "Stow"},

        };


        public void SetFireInput(bool input)
        {
            fireInput = input;
        }

        public float windup;
        [SerializeField] float windupDecay, windupSpeed;
        float currentWindUp;
        [SerializeField] public Ammunition ammo;
        [SerializeField] public GameObject magazine;
        [SerializeField] protected Transform fireOrigin;
        [SerializeField] protected Transform boltOrSlide;
        [SerializeField] protected Transform ejectPoint;
        [SerializeField] protected Vector2 fireVariation;
        [SerializeField] protected float fireIterations; //Useful for shotguns

        [SerializeField] protected Vector3 boltStartPosition;
        [SerializeField] protected Vector3 boltEndPosition;
        [SerializeField] protected float boltCycleTime;
        public bool singleFired;
        [Serializable]
        public class AudioClips
        {
            public AudioClip fireClip, dryfireClip, ammoInClip, ammoOutClip, boltForwardClip, boltBackClip, chargeClip;
        }
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected AudioClips clips;

        [SerializeField] protected bool fireInput, firePressed;
        protected bool canFire;
        [SerializeField] protected bool boltBack;
        
        public enum FireModes
        {
            semi = 0, auto = 1, burst = 2, single = 3
        }
        [SerializeField] protected List<FireModes> fireModes;
        [SerializeField] protected int firemodeIndex;
        [SerializeField] protected FireModes currentFiremode;
        [SerializeField] protected int burstCount;

        [SerializeField] protected bool cycleBackAfterFire;
        [SerializeField] protected bool cycleForwardsAutomatic;
        [SerializeField] protected bool lockBackWhenEmpty;
        [SerializeField] protected float recoilImpulse;
        public Vector3 magEjectAxis;
        public float magEjectForce;
        [Serializable]
        public class FiringEvents
        {
            public UnityEvent boltBackEvent, boltForwardEvent, fireEvent;
        }
        [SerializeField] protected FiringEvents firingEvents;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            ammo.currentAmmo = ammo.maxAmmo;
            ammo.currentReserveAmmo = ammo.maxReserveAmmo;
            boltOrSlide.localPosition = boltStartPosition;

            firemodeIndex %= fireModes.Count;
            currentFiremode = fireModes[firemodeIndex];
        }

        [ContextMenu("Get Bolt Start")]
        public void GetBoltStart()
        {
            if (boltOrSlide)
            {
                boltStartPosition = boltOrSlide.localPosition;
            }
        }
        [ContextMenu("Get Bolt End")]
        public void GetBoltEnd()
        {
            if (boltOrSlide)
            {
                boltEndPosition = boltOrSlide.localPosition;
            }
        }

        public void ResetSingleFire()
        {
            singleFired = false;
        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        public FireModes CurrentFiremode()
        {
            return currentFiremode;
        }

        protected virtual void FixedUpdate()
        {
            if (fireInput)
            {
                currentWindUp += Time.fixedDeltaTime * windupSpeed;
                currentWindUp = Mathf.Clamp(currentWindUp, 0, windup);
                if (windup != 0)
                {
                    if (currentWindUp == windup)
                    {
                        FireLogic();
                    }
                }
                else
                {
                    FireLogic();
                }
                firePressed = true; 
            }
            else
            {
                firePressed = false;
                currentWindUp -= Time.fixedDeltaTime * windupDecay;
                currentWindUp = Mathf.Clamp(currentWindUp, 0, windup);
            }
        }

        void FireLogic()
        {
            if (ammo.currentAmmo > 0 && !boltBack)
            {
                switch ((int)currentFiremode)
                {
                    case 0:
                        if (!firePressed)
                        {
                            Fire();
                        }
                        break;
                    case 1:
                        Fire();
                        break;
                    case 2:
                        if (!firePressed)
                        {
                            StartCoroutine(BurstCoroutine());
                        }
                        break;
                    case 3:
                        if (!singleFired)
                        {
                            Fire();
                            singleFired = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        protected IEnumerator BurstCoroutine()
        {
            for (int i = 0; i < burstCount; i++)
            {
                Fire();
                yield return new WaitUntil(() => !boltBack || ammo.currentAmmo == 0);
                Debug.Log($"fired round {i + 1} of {burstCount}");
            }
        }

        [ContextMenu("Switch Fire mode")]
        public void SwitchFireMode()
        {
            firemodeIndex++;
            firemodeIndex %= fireModes.Count;

            currentFiremode = fireModes[firemodeIndex];
        }
        protected void BeforeFire()
        {

        }
        protected virtual void Fire()
        {
            RecoilImpulse();
            if(audioSource && clips.fireClip)
            {
                audioSource.PlayOneShot(clips.fireClip);
            }
            if (fireIterations > 0)
            {
                for (int i = 0; i < fireIterations; i++)
                {
                    firingEvents.fireEvent.Invoke();

                }
            }
            else
            {
                firingEvents.fireEvent.Invoke();
            }
            ammo.currentAmmo--;
            if (cycleBackAfterFire)
            {
                SendBoltBackwards();
            }
            else
            {
                singleFired = true;
            }
        }

        void RecoilImpulse()
        {
            WeaponHandler afh = GetComponentInParent<WeaponHandler>();
            Debug.Log("Found AFH");
            afh.ReceiveRecoilImpulse(recoilImpulse);
        }

        public virtual void SendBoltBackwards()
        {
            StartCoroutine(BoltBackwards());
        }
        public virtual void SendBoltForwards()
        {
            StartCoroutine(BoltForwards());
        }

        /// <summary>
        /// Sends the bolt forwards, closing it. In open bolt guns, this is the reverse.
        /// </summary>
        /// <returns></returns>
        IEnumerator BoltForwards()
        {
            float boltProgress = 0f;
            while (boltProgress < 1)
            {
                boltProgress += Time.fixedDeltaTime / boltCycleTime;
                boltOrSlide.localPosition = Vector3.Lerp(boltEndPosition, boltStartPosition, boltProgress);
                yield return null;
            }
            firingEvents.boltForwardEvent.Invoke();
            yield return boltBack = false;
            yield return null;
        }


        /// <summary>
        /// Sends the bolt to the "backward" position.
        /// In open bolt guns, this would be the forward position where the slide or bolt appears to be closed
        /// </summary>
        /// <returns></returns>
        IEnumerator BoltBackwards()
        {
            yield return boltBack = true;
            float boltProgress = 0f;
            while (boltProgress < 1)
            {
                boltProgress += Time.fixedDeltaTime / boltCycleTime;
                boltOrSlide.localPosition = Vector3.Lerp(boltStartPosition, boltEndPosition, boltProgress);
                yield return null;
            }
            firingEvents.boltBackEvent.Invoke();
            if (cycleForwardsAutomatic)
            {
                if (ammo.currentAmmo > 0 || (ammo.currentAmmo == 0 && !lockBackWhenEmpty))
                {
                    SendBoltForwards();
                }
            }
            yield return null;
        }

        /// <summary>
        /// Calculates the ammo to be subtracted from the reserves, and refills the current ammuntiion.
        /// </summary>
        [ContextMenu("Reload debug")]
        public void Reload()
        {
            int ammoDiff = 0;
            if (ammo.currentReserveAmmo - (ammo.maxAmmo - ammo.currentAmmo) >= 0)
            {
                ammoDiff = ammo.maxAmmo - ammo.currentAmmo;
                if (ammo.extraChambered && ammo.currentAmmo > 0)
                {
                    ammoDiff++;
                }
            }
            else
            {
                ammoDiff += ammo.currentAmmo;
                Debug.Log("reserves less than max ammo");
                Debug.Log(ammoDiff + " is ammo difference");
                ammoDiff = Mathf.Clamp(ammoDiff, 0, ammo.currentReserveAmmo);
            }
            if (ammo.currentReserveAmmo >= 0)
            {
                ammo.currentReserveAmmo -= ammoDiff;
                ammo.currentAmmo += ammoDiff;
                Debug.Log("Reloaded!");
            }
            if (lockBackWhenEmpty && boltOrSlide.localPosition != boltStartPosition)
            {
                SendBoltForwards();
            }

        }



        private void OnDrawGizmosSelected()
        {
            if(boltOrSlide)
            Gizmos.DrawLine((boltOrSlide.rotation * boltStartPosition) + transform.position, boltEndPosition + transform.position);
        }

    }
}