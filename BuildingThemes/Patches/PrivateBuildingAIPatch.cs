using ColossalFramework;
using ColossalFramework.Math;
using HarmonyLib;

namespace BuildingThemes.Patches
{

    [HarmonyPatch(typeof(PrivateBuildingAI))]
    static class PrivateBuildingAIPatch
    {
        
        [HarmonyPrefix]
        [HarmonyPatch("GetUpgradeInfo")]
        [HarmonyPatch(new [] { typeof(ushort), typeof(Building) },
            new [] {ArgumentType.Normal, ArgumentType.Ref})]
        static bool GetUpgradeInfoPrefix(ref BuildingInfo __result, ref BuildingInfo ___m_info, ushort buildingID, ref Building data)
        {
            // vanilla start
            if (data.m_level == 4)
            {
                __result = null;
                return false; // do not run vanilla method
            }
            Randomizer r = new Randomizer(buildingID);
            for (int i = 0; i <= data.m_level; i++)
            {
                r.Int32(1000u);
            }
            ItemClass.Level level = (ItemClass.Level)(data.m_level + 1);
            DistrictManager instance = Singleton<DistrictManager>.instance;
            byte district = instance.GetDistrict(data.m_position);
            ushort style = instance.m_districts.m_buffer[district].m_Style;
            // vanilla end
            
            __result = RandomBuildings.GetRandomBuildingInfo_Upgrade(data.m_position, data.m_infoIndex,
                ref r, ___m_info.m_class.m_service, ___m_info.m_class.m_subService, level, data.Width, data.Length, ___m_info.m_zoningMode, style);
            
            return false; // do not run vanilla method
        }
    }
    
}
