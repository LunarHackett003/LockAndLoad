using Eclipse.Firearms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Eclipse {
    public class CharacterControl : MonoBehaviour
    {
        public WeaponHandler wh;
        public Animation.CharacterAnimator characterAnimator;
        [SerializeField] float horizontalLookSpeed, verticalLookSpeed;
        [SerializeField] float currentVerticalAim;
        [SerializeField] float minAimAngle, maxAimAngle;
        [SerializeField] Transform aimTransform;
        [SerializeField] Transform secondaryRecoilTransform;
        [SerializeField] Vector3 aimAxis;
        [SerializeField] float moveSpeed;
        float thisrecoilangle;
        float thisrecoilvelocity;
        [SerializeField] Rigidbody rb;
        float recoilDamped, recoilVelocity;

        [SerializeField] float leanAngle, leanSpeed;
        float currentLean, targetLean;
        [System.Serializable]
        public class RecoilViewKick
        {
            public Vector3 startPos, endPos;
            public float time;
        }
        [SerializeField] RecoilViewKick rvk;
        [SerializeField] float recoilDampTime;



        public bool crouched;

        IEnumerator ViewKick()
        {
            float coTime = 0;
            while (coTime < rvk.time)
            {
                secondaryRecoilTransform.localPosition = Vector3.Lerp(rvk.startPos, rvk.endPos, coTime);
                coTime += Time.fixedDeltaTime;
                yield return null;
            }
            yield return null;
            while (coTime > 0)
            {
                secondaryRecoilTransform.localPosition = Vector3.Lerp(rvk.startPos, rvk.endPos, coTime);
                coTime -= Time.fixedDeltaTime;
                yield return null;
            }

            yield return null;
        }
        
        public void DoViewKick()
        {
            StartCoroutine(ViewKick());
        }

        [System.Serializable]
        public class InputVariables
        {
            public InputActionAsset inputAsset;

            public Vector2 moveInput, lookInput;
            public bool fireInput, aimInput;
            public bool reloadInput, jumpInput, switchInput;
            public bool crouchInput;
            public float leanInput;

        }
        public InputVariables inputs;

        private void Awake()
        {
            inputs.inputAsset.Enable();
            
            inputs.inputAsset.FindAction("Jump").performed += Jump;
            inputs.inputAsset.FindAction("Reload").performed += Reload;
            inputs.inputAsset.FindAction("Crouch").performed += Crouch;
            inputs.inputAsset.FindAction("Lean").performed += Lean;
        }

        private void Lean(InputAction.CallbackContext context)
        {
            float leanInput = context.ReadValue<float>();
            if(leanInput == targetLean)
            {
                targetLean = 0;
            }
            else
            {
                targetLean = leanInput;
            }
        }

        private void Start()
        {
            currentVerticalAim = 0.5f;
            secondaryRecoilTransform.localPosition = rvk.startPos;
        }

        void Jump(InputAction.CallbackContext context)
        {
            
        }
        void Reload(InputAction.CallbackContext context)
        {
            BaseFirearm bf = wh.GetCurrentWeapon();

            if(bf.ammo.currentReserveAmmo > bf.ammo.maxAmmo)
            {
                if(bf.ammo.currentAmmo < bf.ammo.maxAmmo || (bf.ammo.currentAmmo < bf.ammo.maxAmmo + 1 && bf.ammo.extraChambered))
                {
                    Debug.Log("Triggering Reload");
                    characterAnimator.SetTrigger("Reload", 0.1f);
                }
                else
                {
                    //Mag is full, do nothing
                }
            }
            else
            {
                if(bf.ammo.currentAmmo != bf.ammo.currentReserveAmmo)
                {
                    Debug.Log("Triggering Reload");
                    characterAnimator.SetTrigger("Reload", 0.1f);
                }
            }

        }
        private void FixedUpdate()
        {
            GetInputs();

            currentLean = Mathf.Lerp(currentLean, targetLean, Time.fixedDeltaTime * leanSpeed);

            AimRotate();

            MoveCharacter();

            wh.SetFireInput(inputs.fireInput);
        }

        void MoveCharacter()
        {

            if (!crouched)
                rb.MovePosition(transform.position + (transform.rotation * new Vector3(inputs.moveInput.x * moveSpeed * Time.fixedDeltaTime, 0, inputs.moveInput.y * moveSpeed * Time.fixedDeltaTime)));
            else
                rb.MovePosition(transform.position + (transform.rotation * new Vector3(inputs.moveInput.x * (moveSpeed / 2) * Time.fixedDeltaTime, 0, inputs.moveInput.y * (moveSpeed / 2) * Time.fixedDeltaTime)));

        }

        public void Crouch(InputAction.CallbackContext context)
        {
            crouched = !crouched;
            Debug.Log("toggling crouch");
        }

        void GetInputs()
        {
            InputActionAsset ia = inputs.inputAsset;
            inputs.moveInput = ia.FindAction("Move").ReadValue<Vector2>();
            inputs.lookInput = ia.FindAction("Look").ReadValue<Vector2>();
            inputs.fireInput = ia.FindAction("Fire").ReadValue<float>() >= 0.3f;
            inputs.aimInput = ia.FindAction("Aim").ReadValue<float>() >= 0.3f;
            inputs.switchInput = ia.FindAction("Switch").ReadValue<float>() >= 0.3f;
        }

        void AimRotate()
        {

            currentVerticalAim += Time.fixedDeltaTime * -inputs.lookInput.y * verticalLookSpeed;
            currentVerticalAim = Mathf.Clamp(currentVerticalAim, minAimAngle, maxAimAngle);
            transform.rotation *= Quaternion.Euler(0, Time.fixedDeltaTime * inputs.lookInput.x * horizontalLookSpeed, 0);

            //secondaryRecoilTransform.localRotation = Quaternion.Euler(Mathf.Lerp(secondaryRecoilTransform.localEulerAngles.x, -afh.angularRecoilCurrent, Time.fixedDeltaTime * 10), 0, 0);
            recoilDamped = Mathf.SmoothDamp(recoilDamped, wh.angularRecoilOut, ref recoilVelocity, recoilDampTime);
            aimTransform.localRotation = Quaternion.Euler(aimAxis * (currentVerticalAim + -recoilDamped) + new Vector3(0, 0, leanAngle * currentLean));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(secondaryRecoilTransform.position + secondaryRecoilTransform.rotation * rvk.startPos, secondaryRecoilTransform.position + secondaryRecoilTransform.rotation * rvk.endPos);
        }

    }
}