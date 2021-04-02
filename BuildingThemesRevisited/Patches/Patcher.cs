using BuildingThemesRevisited.Utils;
using CitiesHarmony.API;
using HarmonyLib;

namespace BuildingThemesRevisited.Patches
{
    /// <summary>
    /// Class to handle patches in this mod.
    /// </summary>
    public static class Patcher
    {
        // unique harmony id
        private const string HarmonyId = "com.github.meda22.csl.buildingthemesrevisited";
        
        /// <summary>
        /// Flag if patches has been applied
        /// </summary>
        internal static bool patched { get; private set; }
        
        /// <summary>
        /// Apply all patches from the mod
        /// </summary>
        public static void PatchAll()
        {
            if (patched)
            {
                // already patched -> nothing to do
                return;
            }

            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Logger.InfoLog("Deploying Harmony patches...");
                var harmony = new Harmony(HarmonyId);
                harmony.PatchAll();
                patched = true;
            }
            else
            {
                Logger.ErrorLog("Harmony is NOT ready!");
            }
        }
        
        /// <summary>
        /// Revert all patches from the mod
        /// </summary>
        public static void UnpatchAll()
        {
            // not patched -> do nothing
            if (!patched) return;
            
            // if patched, unpatch all with our harmony id   
            Logger.InfoLog("Reverting all Harmony patches...");
            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);
            patched = false;
        }
    }
}