using HarmonyLib;
using UnityEngine;

namespace BuildingThemes.Patches
{
    // This patch catches the position of abandoned buildings which are replaced by new level 1 buildings
    [HarmonyPatch(typeof(ImmaterialResourceManager))]
    static class ImmaterialResourceManagerPatch
    {
        private static int debugCounter = 0;
        
        [HarmonyPrefix]
        [HarmonyPatch("AddResource")]
        [HarmonyPatch(new [] {typeof(ImmaterialResourceManager.Resource), typeof(int), typeof(Vector3), typeof(float)},
            new [] {ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal})]
        static bool AddResourcePrefix(ImmaterialResourceManager.Resource resource, int rate, Vector3 position, float radius)
        {
            if (Debugger.Enabled && debugCounter < 10)
            {
                debugCounter++;
                Debugger.Log("Building Themes: Patched ImmaterialResourceManager.AddResource was called.");
            }

            // Catch the position of the abandoned building
            if (resource == ImmaterialResourceManager.Resource.Abandonment)
            {
                BuildingThemesMod.position = position;
            }

            return true; // run vanilla method now
        }
        
    }
    
}
