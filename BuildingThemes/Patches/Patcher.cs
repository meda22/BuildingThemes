using HarmonyLib;
using CitiesHarmony.API;

namespace BuildingThemes
{
    // Class to manage Harmony patches in this mod
    public static class Patcher
    {

        // Unique harmony id
        private const string harmonyId = "com.github.meda22.csl.buildingthemes";
        private static bool patched = false;

        public static void PatchAll()
        {
            // already patched -> nothing to do
            if (patched) return;

            if (HarmonyHelper.IsHarmonyInstalled)
            {
                // TODO: add logging message

                // Apply all annotated patches and update the flag
                Harmony harmony = new Harmony(harmonyId);
                harmony.PatchAll();
                patched = true;
            }
            else 
            { 
                // TODO: add logging message that harmony is not ready
            }
        }

        public static void UnpatchAll()
        {
            // if patched, unpatch all with our harmony id
            if (patched)
            {
                Harmony harmony = new Harmony(harmonyId);
                harmony.UnpatchAll(harmonyId);
                patched = false;
            }

        }

    }
}
