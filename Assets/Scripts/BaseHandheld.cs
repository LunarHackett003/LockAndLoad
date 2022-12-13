using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHandheld : MonoBehaviour
{
    /*This class exists to supply animations to all usable items.
     * - If the item is a firearm, then it will override the weapon animations (firing, reloading, etc).
     * - if the item is a gadget, then it will override the gadget animations (obviously equipping, deploying, etc)
     * - if a consumable, then it will override and play the consumption animation.
    */

    [System.Serializable]
    public class AnimOverride
    {
        public string clipTargetName;
        public AnimationClip animationClip;
    }
    public static AnimOverride GetOverrideByName(string name, List<AnimOverride> overrides)
    {
        foreach (var item in overrides)
        {
            if(item.clipTargetName == name)
            {
                return item;
            }
            else
            {
                return null;
            }
        }
        
        return null;
    }

}
