using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;
using Il2CppInterop.Runtime.Injection;

namespace SprintMod
{
    public class SprintMod : MelonMod
    {
        private MelonPreferences_Category preferencesCategory;

        public static MelonPreferences_Entry<bool> modEnabled;
        public static MelonPreferences_Entry<KeyCode> sprintKey;
        public static MelonPreferences_Entry<float> sprintMultiplier;
        public static MelonPreferences_Entry<bool> debugMode;

        public static GameObject gameObject;

        public override void OnInitializeMelon()
        {

            preferencesCategory = MelonPreferences.CreateCategory("Sprint Mod");

            modEnabled = preferencesCategory.CreateEntry("Enabled", true);
            sprintKey = preferencesCategory.CreateEntry("Sprint Key", KeyCode.LeftShift);
            sprintMultiplier = preferencesCategory.CreateEntry("Sprint Multplier", 2f);
            debugMode = preferencesCategory.CreateEntry("Debug", false);

            if (!modEnabled.Value)
                return;

            ClassInjector.RegisterTypeInIl2Cpp<SprintComponent>();

            if (debugMode.Value)
            {
                LoggerInstance.Msg("Sprint Mod Loaded.");
            }
        }

        public override void OnUpdate()
        {
            if (!modEnabled.Value)
                return;

            if (gameObject != null)
                return;

            gameObject = new GameObject();
            gameObject.AddComponent<SprintComponent>();
            Object.DontDestroyOnLoad(gameObject);

            if (debugMode.Value)
                LoggerInstance.Msg("Created Game Object");
        }
    }
}