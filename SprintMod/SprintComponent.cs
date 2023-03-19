using Il2CppMdl.Avatar;
using MelonLoader;
using UnityEngine;

namespace SprintMod
{
    public class SprintComponent : MonoBehaviour
    {
        private PlayerAvatar avatar;
        
        public void Update()
        {
            if (!SprintMod.modEnabled.Value)
                return;

            if (avatar is null)
            {
                avatar = FindObjectOfType<PlayerAvatar>();
            }
            if (avatar is null)
                return;

            try
            {
                var animator = avatar.GetComponent<Animator>();
                if (animator is null)
                    return;

                var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
                if (currentClipInfo is null)
                    return;

                var clipName = currentClipInfo[0].clip.name;
                if (clipName is null)
                    return;

                if (clipName.StartsWith("locomotion_run") && Input.GetKey(SprintMod.sprintKey.Value))
                {
                    if (SprintMod.debugMode.Value && avatar.RunSpeedMultiplier < SprintMod.sprintMultiplier.Value)
                        Melon<SprintMod>.Logger.Msg("Now Sprinting");
                    avatar.RunSpeedMultiplier = SprintMod.sprintMultiplier.Value;
                    animator.speed = 1 / SprintMod.sprintMultiplier.Value;
                }
                else
                {
                    if (SprintMod.debugMode.Value && avatar.RunSpeedMultiplier > 1)
                        Melon<SprintMod>.Logger.Msg("No longer Sprinting");
                    avatar.RunSpeedMultiplier = 1;
                    animator.speed = 1;
                }
            }
            catch { }
        }
    }
}
