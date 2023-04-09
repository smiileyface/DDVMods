using MelonLoader;
using HarmonyLib;
using Il2CppDefinitions.Grid;

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

        [HarmonyPatch(typeof(VillageObjectLimit))]
        class VillageObjectLimitPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(VillageObjectLimit.IsGoingOverLimit))]
            static bool IsGoingOverLimit(ref bool __result)
            {
                if (!modEnabled.Value)
                    return true;

                if (isDebug.Value)
                    Melon<NoObjectLimit>.Logger.Msg($"IsGoingOverLimit Before -> {__result}");
                __result = false;
                return false;
            }

            [HarmonyPrefix]
            [HarmonyPatch(nameof(VillageObjectLimit.AllCount), MethodType.Getter)]
            static bool AllCount(ref int __result)
            {
                if (!modEnabled.Value)
                    return true;

                if (isDebug.Value)
                    Melon<NoObjectLimit>.Logger.Msg($"AllCount Before -> {__result}");
                __result = 0;
                return false;
            }

            [HarmonyPrefix]
            [HarmonyPatch(nameof(VillageObjectLimit.UniqueCount), MethodType.Getter)]
            static bool UniqueCount(ref int __result)
            {
                if (!modEnabled.Value)
                    return true;

                if (isDebug.Value)
                    Melon<NoObjectLimit>.Logger.Msg($"UniqueCount Before -> {__result}");
                __result = 0;
                return false;
            }
        }
    }
}