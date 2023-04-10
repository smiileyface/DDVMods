using MelonLoader;
using HarmonyLib;
using Il2CppMdl.Grid;
using Il2CppDefinitions.Grid;
using Il2CppMdl.Ui;
using UnityEngine;

namespace NoObjectLimit
{
    public class NoObjectLimit : MelonMod
    {
        private MelonPreferences_Category _category;

        private static MelonPreferences_Entry<bool> modEnabled;
        private static MelonPreferences_Entry<bool> isDebug;

        public override void OnInitializeMelon()
        {
            _category = MelonPreferences.CreateCategory("No Object Limit");

            modEnabled = _category.CreateEntry("Enabled", true);
            isDebug = _category.CreateEntry("Debug", false);

            if (isDebug.Value)
                LoggerInstance.Msg("Mod Loaded");
        }

        [HarmonyPatch(typeof(GridEditMode))]
        class GridEditModePatches
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(GridEditMode.CanAdd))]
            static void CanAdd(ref Il2CppSystem.Threading.Tasks.Task<bool> __result)
            {
                if (!modEnabled.Value)
                    return;

                if (isDebug.Value)
                    Melon<NoObjectLimit>.Logger.Msg($"CanAdd Before -> {__result.Result}");
                __result = new Il2CppSystem.Threading.Tasks.Task<bool>(true);
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(GridEditMode.CanAddObjectFromBackpack))]
            static void CanAddObjectFromBackpack(ref bool __result)
            {
                if (!modEnabled.Value)
                    return;

                if (isDebug.Value)
                    Melon<NoObjectLimit>.Logger.Msg($"CanAddObjectFrombackpack Before -> {__result}");
                __result = true;
            }
        }

        [HarmonyPatch(typeof(VillageObjectLimit))]
        class VillageObjectLimitPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(VillageObjectLimit.IsGoingOverLimit))]
            static void IsGoingOverLimit(ref bool __result)
            {
                if (!modEnabled.Value)
                    return;

                if (isDebug.Value)
                    Melon<NoObjectLimit>.Logger.Msg($"IsGoingOverLimit Before -> {__result}");
                __result = false;
            }
        }

        [HarmonyPatch(typeof(HudLimit))]
        class HudLimitPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(HudLimit._disableColor), MethodType.Getter)]
            static void DisableColor(ref Color32 __result)
            {
                if (!modEnabled.Value)
                    return;

                if (isDebug.Value)
                    Melon<NoObjectLimit>.Logger.Msg($"disableColor Before -> {__result}");
                __result = Color.white;
            }
        }
    }
}