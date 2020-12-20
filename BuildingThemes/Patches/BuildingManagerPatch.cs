using ColossalFramework.Math;
using HarmonyLib;

namespace BuildingThemes.Patches
{

    [HarmonyPatch(typeof(BuildingManager))]
    static class BuildingManagerPatch
    {
        
        private static int debugCounter = 0;
        
        // The original GetRandomBuildingInfo method. 
        // The only method that still points here is the "Downgrade" method which resets abandoned buildings to L1
        [HarmonyPrefix]
        [HarmonyPatch("GetRandomBuildingInfo")]
        static bool GetRandomBuildingInfoPrefix(ref BuildingInfo __result, ref Randomizer r, ItemClass.Service service,
            ItemClass.SubService subService, ItemClass.Level level, int width, int length,
            BuildingInfo.ZoningMode zoningMode, int style)
        {
            
            if (Debugger.Enabled && debugCounter < 10)
            {
                debugCounter++;
                Debugger.Log("Building Themes: Patched BuildingManager.GetRandomBuildingInfo was called on abandoned building.");
            }
            
            __result = RandomBuildings.GetRandomBuildingInfo_Spawn(BuildingThemesMod.position, ref r, service, 
                subService, level, width, length, zoningMode, style);

            return false;
        }
        
    }
}
