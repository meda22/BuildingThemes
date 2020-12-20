using HarmonyLib;

namespace BuildingThemes.Patches
{
   
    
    [HarmonyPatch(typeof(DistrictManager))]
    static class DistrictManagerPatch
    {

        [HarmonyPrefix]
        [HarmonyPatch("ReleaseDistrictImplementation")]
        static bool ReleaseDistrictImplementationPrefix(byte district, ref District data)
        {
            if (data.m_flags != 0)
            {
                BuildingThemesManager.instance.ToggleThemeManagement(district, false);
            }
            
            return true; // run vanilla method afterwards
        }
        
    }

}