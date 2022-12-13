using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Firearms {
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] int weaponIndex; //Current index in the firearm list
        [SerializeField] List<BaseFirearm> firearms; //List of all firearms the character has
        [SerializeField] int gadgetIndex;
        [SerializeField] List<BaseGadget> gadgets;
        [SerializeField] bool currentFireInput;
        [SerializeField] bool weaponEnabled; //Is the weapon equipped and enabled?
        [SerializeField] Transform magHolder; //The transform (usually the left hand) that the magazine attaches to
        [SerializeField] float recoilMultiplier; //The multiplier of the recoil angle
        [SerializeField] Vector3 impulseAddPosition; //The target position of the linear recoil impulse.
        [SerializeField] float linearRecoilCurrent;
        [SerializeField] Quaternion linearRotationInitial; 
        [SerializeField] Vector3 recoilRotationAxis; //Axis of recoil rotation
        [SerializeField] float angularRecoilMultiplier, angularRecoilDecay;
        [SerializeField] float linearTransformRotationMult;
        public float angularRecoilOut;

        [SerializeField] List<Transform> stashPoints = new List<Transform>();
        [SerializeField] Transform rightHand;

        private void Awake()
        {
            //firearms.AddRange(GetComponentsInChildren<BaseFirearm>());
            weaponIndex = 1;
        }

        public void SwitchWeapons()
        {
            GetCurrentWeapon().transform.SetParent(stashPoints[weaponIndex], false);
            weaponIndex++;
            weaponIndex %= firearms.Count;
            BroadcastMessage("WeaponChange");
            GetCurrentWeapon().transform.SetParent(rightHand, false);
        }

        public void ResetSingleFire()
        {
            GetCurrentWeapon().ResetSingleFire();
        }
        public void ToggleWeapon(int value)
        {
            bool toggle = value == 1;

            weaponEnabled = toggle;
        }

        public BaseFirearm GetCurrentWeapon()
        {
            return firearms[weaponIndex];
        }

        private void FixedUpdate()
        {
            angularRecoilOut -= Time.fixedDeltaTime * angularRecoilDecay;
            angularRecoilOut = Mathf.Clamp(angularRecoilOut, 0, 15);
            SetFireInputOnActive();   
        }

        public void ReceiveRecoilImpulse(float impulseSize)
        {
            angularRecoilOut += impulseSize * angularRecoilMultiplier;
            GetComponent<CharacterControl>().DoViewKick();
        }

        public void SetFireInput(bool input)
        {
            currentFireInput = input;
        }

        public void SetFireInputOnActive()
        {
            if (weaponEnabled)
            {
                firearms[weaponIndex].SetFireInput(currentFireInput);
            }
        }

        public void EjectRound()
        {

        }

        public void ResetBolt()
        {
            GetCurrentWeapon().SendBoltForwards();
        }

        public void AddRound()
        {
            GetCurrentWeapon().ammo.currentAmmo += 1;
        }

        public void GrabMagazine(int newHidden)
        {
            GameObject newmag = Instantiate(firearms[weaponIndex].magazine, firearms[weaponIndex].magazine.transform.position, firearms[weaponIndex].magazine.transform.rotation);
            newmag.name = "newmag";
            newmag.AddComponent<Rigidbody>();
            newmag.GetComponent<Rigidbody>().isKinematic = true;
            newmag.transform.SetParent(magHolder, true);
            firearms[weaponIndex].magazine.SetActive(newHidden != 1);
            newmag.SetActive(newHidden == 1);
        }

        public void DropMagazine()
        {
            GameObject mag = magHolder.Find("newmag").gameObject;
            GameObject mag2 = Instantiate(mag, magHolder, true);
            mag.SetActive(false);
            mag2.name = "mag2";
            Destroy(mag2, 15f);
            mag2.GetComponent<Rigidbody>().isKinematic = false;
            mag2.transform.SetParent(null, true);
        }
        public void EjectMag()
        {   
            GameObject newmag = Instantiate(firearms[weaponIndex].magazine, firearms[weaponIndex].magazine.transform.position, firearms[weaponIndex].magazine.transform.rotation);
            newmag.name = "newmag";
            Rigidbody nmrb = newmag.AddComponent<Rigidbody>();
            firearms[weaponIndex].magazine.SetActive(false);
            nmrb.AddForce(firearms[weaponIndex].magazine.transform.TransformDirection(firearms[weaponIndex].magEjectAxis) * firearms[weaponIndex].magEjectForce, ForceMode.Impulse);
            nmrb.AddTorque(firearms[weaponIndex].transform.rotation.eulerAngles);
        }

        public void GrabNewMag()
        {
            GameObject mag = magHolder.Find("newmag").gameObject;
            mag.SetActive(true);
        }

        public void InsertMagazine()
        {
            GameObject mag = magHolder.Find("newmag").gameObject;
            Destroy(mag);
            firearms[weaponIndex].magazine.SetActive(true);
            firearms[weaponIndex].Reload();
        }
    }
}