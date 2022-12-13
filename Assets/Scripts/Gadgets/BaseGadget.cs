using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGadget : BaseHandheld
{
    public List<AnimOverride> animOverrides = new List<AnimOverride>()
        {
            new AnimOverride() {clipTargetName = "PrimaryUseTrigger"},
            new AnimOverride() {clipTargetName = "SecondaryUseTrigger"},
            new AnimOverride() {clipTargetName = "HipIdle"},
            new AnimOverride() {clipTargetName = "SecondaryUseReady"},
            new AnimOverride() {clipTargetName = "Move"},
            new AnimOverride() {clipTargetName = "SecondaryUseMove"},
            new AnimOverride() {clipTargetName = "Reload"},
            new AnimOverride() {clipTargetName = "Draw"},
            new AnimOverride() {clipTargetName = "Stow"},

        };







}
