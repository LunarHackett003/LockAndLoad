using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eclipse.Firearms;
using UnityEngine.Animations;
using JetBrains.Annotations;

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get
        {
            return this.Find(x => x.Key.name.Equals(name)).Value;
        }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));

            if (index != -1)
            {
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }
}

namespace Eclipse.Animation
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] WeaponHandler wh;
        [SerializeField] CharacterControl characterControl;
        [SerializeField] RuntimeAnimatorController animControllerBase;
        [SerializeField] AnimatorOverrideController animOverrideController;

        [SerializeField] List<BaseHandheld.AnimOverride> runtimeOverrides;
        public void WeaponChange()
        {
            SetWeaponAnimOverrides();
        }

        private void Start()
        {
            //animControllerBase = animator.runtimeAnimatorController;
            animOverrideController = new AnimatorOverrideController(animControllerBase);
            animator.runtimeAnimatorController = animOverrideController;

            //Invoke("SetAnimationOverrides", 0.1f);
        }

        protected AnimationClipOverrides clipOverrides;
        void SetWeaponAnimOverrides()
        {
            clipOverrides = new AnimationClipOverrides(animOverrideController.overridesCount);
            animOverrideController.GetOverrides(clipOverrides);
            runtimeOverrides = new List<BaseHandheld.AnimOverride>( wh.GetCurrentWeapon().animOverrides);
            Debug.Log(runtimeOverrides.Count + " overrides found");
            //Collect all of the weapon animation clips and assign them to local variables. This reduces having to write "GetCurrentWeapon" lots of times.
            //Also utilises the baseHandeld animation getter.        
            //  clipOverrides["Reload"] = BaseHandheld.GetOverrideByName("Reload", runtimeOverrides).animationClip;
            //  clipOverrides["Move"] = BaseHandheld.GetOverrideByName("Move", runtimeOverrides).animationClip;//  clipOverrides["ADS"] = BaseHandheld.GetOverrideByName("ADS", runtimeOverrides).animationClip;
            //  clipOverrides["HipIdle"] = BaseHandheld.GetOverrideByName("HipIdle", runtimeOverrides).animationClip;//  clipOverrides["ADSMove"] = BaseHandheld.GetOverrideByName("ADSMove", runtimeOverrides).animationClip;
            //  clipOverrides["Fire"] = BaseHandheld.GetOverrideByName("Fire", runtimeOverrides).animationClip;//  clipOverrides["ADSFire"] = BaseHandheld.GetOverrideByName("ADSFire", runtimeOverrides).animationClip;
            //  clipOverrides["Draw"] = BaseHandheld.GetOverrideByName("Draw", runtimeOverrides).animationClip;//  clipOverrides["Stow"] = BaseHandheld.GetOverrideByName("Stow", runtimeOverrides).animationClip;
            //Welp, redid it again. I was getting NullRefs.
            clipOverrides["Reload"] = wh.GetCurrentWeapon().animOverrides.Find(x => x.clipTargetName == "Reload").animationClip;
            clipOverrides["Move"] = wh.GetCurrentWeapon().animOverrides.Find(x => x.clipTargetName == "Move").animationClip;
            clipOverrides["ADSMove"] = wh.GetCurrentWeapon().animOverrides.Find(x => x.clipTargetName == "ADSMove").animationClip;
            clipOverrides["ADS"] = wh.GetCurrentWeapon().animOverrides.Find(x => x.clipTargetName == "ADS").animationClip;
            clipOverrides["HipIdle"] = wh.GetCurrentWeapon().animOverrides.Find(x => x.clipTargetName == "HipIdle").animationClip;
            clipOverrides["Fire"] = wh.GetCurrentWeapon().animOverrides.Find(x => x.clipTargetName == "Fire").animationClip;
            clipOverrides["ADSFire"] = wh.GetCurrentWeapon().animOverrides.Find(x => x.clipTargetName == "ADSFire").animationClip;
            clipOverrides["Draw"] = wh.GetCurrentWeapon().animOverrides.Find(x => x.clipTargetName == "Draw").animationClip;
            clipOverrides["Stow"] = wh.GetCurrentWeapon().animOverrides.Find(x => x.clipTargetName == "Stow").animationClip;
            animOverrideController.ApplyOverrides(clipOverrides);
        }


        public void GetAndSetAnimatorParameters()
        {
            if(characterControl.crouched)
                animator.SetFloat("MoveMultiplier", 0.5f, 0.2f, Time.fixedDeltaTime);
            else
                animator.SetFloat("MoveMultiplier", 1f, 0.2f, Time.fixedDeltaTime);


            animator.SetFloat("HorizontalMove", characterControl.inputs.moveInput.x, 0.1f, Time.fixedDeltaTime);
            animator.SetFloat("VerticalMove", characterControl.inputs.moveInput.y, 0.1f, Time.fixedDeltaTime);
            animator.SetBool("Moving", Vector2.Distance(Vector2.zero, characterControl.inputs.moveInput) >= 0.1f);
            animator.SetBool("Crouched", characterControl.crouched);
            animator.SetBool("AimDownSights", characterControl.inputs.aimInput);
            if(wh.GetCurrentWeapon().CurrentFiremode() == BaseFirearm.FireModes.single)
            {
                animator.SetBool("Fire", wh.GetCurrentWeapon().singleFired);
            }
            else
                animator.SetBool("Fire", characterControl.inputs.fireInput && wh.GetCurrentWeapon().ammo.currentAmmo > 0);
            animator.SetBool("Switch", characterControl.inputs.switchInput);
        }
        private void FixedUpdate()
        {
            GetAndSetAnimatorParameters();
        }

        public void SetTrigger(string paramname, float time)
        {
            StartCoroutine(TriggerToggle(paramname, time));
        }
        IEnumerator TriggerToggle(string paramname, float time)
        {
            animator.SetTrigger(paramname);
            yield return new WaitForSeconds(time);
            animator.ResetTrigger(paramname);
            yield return null;
        }
    }
}
