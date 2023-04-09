using MelonLoader;
using HarmonyLib;
using Il2CppMeta;

namespace ScroogeShopExtraRefresh
{
    public class ScroogeShopExtraRefresh : MelonMod
    {
        public static int ScroogeStoreId = 20300028;

        private MelonPreferences_Category prefsCat;

        public static MelonPreferences_Entry<bool> modEnabled;
        public static MelonPreferences_Entry<int> refreshFrequency;
        public static MelonPreferences_Entry<bool> debug;

        private static List<RefreshWindow> refreshWindows;

        public override void OnInitializeMelon()
        {
            prefsCat = MelonPreferences.CreateCategory("Scrooge Shop Extra Refresh");

            modEnabled = prefsCat.CreateEntry("Enabled", true);
            refreshFrequency = prefsCat.CreateEntry("Refresh Frequency Hours", 12);
            debug = prefsCat.CreateEntry("Debug", false);

            refreshWindows = GenerateRefreshWindows();


            if (debug.Value)
            {
                LoggerInstance.Msg($"Refresh Windows:");
                foreach (var window in refreshWindows)
                    LoggerInstance.Msg($" - {window.start} - {window.end}");

                LoggerInstance.Msg("Mod loaded");
            }
        }

        [HarmonyPatch(typeof(StoreInfo), nameof(StoreInfo.CanRefreshStore))]
        public class StoreInfo_CanRefreshStore_Patch
        {
            private static bool Prefix(ref StoreInfo __instance, ref bool __result, Il2CppSystem.DateTime now)
            {
                if (!modEnabled.Value || __instance.BuildingItemID != ScroogeStoreId)
                    return true;

                Il2CppSystem.DateTime localNow = now.ToLocalTime();
                Il2CppSystem.DateTime localLastRefresh = __instance.LastRefreshDateTime.ToLocalTime();
                if (debug.Value)
                {
                    Melon<ScroogeShopExtraRefresh>.Logger.Msg($"Local Now: {localNow}");
                    Melon<ScroogeShopExtraRefresh>.Logger.Msg($"Local Last Refresh: {localLastRefresh}");
                }

                if (CanRefresh(localNow, localLastRefresh))
                {
                    if (debug.Value)
                        Melon<ScroogeShopExtraRefresh>.Logger.Msg($"Can Refresh Store: {__instance.BuildingItemID}");
                    __result = true;
                }
                else
                {
                    if (debug.Value)
                        Melon<ScroogeShopExtraRefresh>.Logger.Msg($"Not Able to Refresh Store: {__instance.BuildingItemID}");
                    __result = false;
                }
                return false;
            }
        }

        static bool CanRefresh(Il2CppSystem.DateTime now, Il2CppSystem.DateTime lastRefresh)
        {
            var diff = now - lastRefresh;
            return diff.TotalHours >= refreshFrequency.Value || refreshWindows.Any(window => now >= window.start && now < window.end && lastRefresh < window.start);
        }

        static List<RefreshWindow> GenerateRefreshWindows()
        {
            var refreshWindows = new List<RefreshWindow>();

            var midnightToday = Il2CppSystem.DateTime.Now.Date.ToLocalTime();

            for (int i = 0; i < 24 / refreshFrequency.Value; i++)
            {
                var start = midnightToday.AddHours(i * refreshFrequency.Value);
                var end = start.AddHours(refreshFrequency.Value);

                var refreshWindow = new RefreshWindow();

                refreshWindow.start = start;
                refreshWindow.end = end;

                refreshWindows.Add(refreshWindow);
            }

            return refreshWindows;
        }

        struct RefreshWindow
        {
            public Il2CppSystem.DateTime start;
            public Il2CppSystem.DateTime end;
        }
    }
}