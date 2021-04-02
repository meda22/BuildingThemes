using System;
using BuildingThemesRevisited.Utils;
using ColossalFramework;

namespace BuildingThemesRevisited
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomDistrictStyleManager : Singleton<CustomDistrictStyleManager>
    {

        public void ImportDistrictStyles()
        {
            DistrictStyle[] districtStyles = DistrictManager.instance.m_Styles;

            if (districtStyles == null)
            {
                return;
            }

            foreach (var districtStyle in districtStyles)
            {
                Logger.InfoLog($"District Style - name: {districtStyle.Name} " +
                               $"- built in: {districtStyle.BuiltIn}");

                BuildingInfo[] styleBuildingInfos = districtStyle.GetBuildingInfos();

                if (styleBuildingInfos == null)
                {
                    continue;
                }

                foreach (var buildingInfo in styleBuildingInfos)
                {
                    Logger.DebugLog($"Building: {buildingInfo.name} - " +
                                    $"Size: {buildingInfo.m_size} - Level: {buildingInfo.GetClassLevel()}");
                }
            }
            
            // TODO: Test to create new style
            // Guid myUuid = Guid.NewGuid();
            // string districtStyleUniqueId = "DS id " + myUuid;
            // DistrictStyleMetaData districtStyleMetaData = new DistrictStyleMetaData();
            // districtStyleMetaData.name = districtStyleUniqueId;
            // districtStyleMetaData.builtin = false;
            // StylesHelper.SaveStyle(districtStyleMetaData, districtStyleUniqueId, false, null);
            // TODO: it wasn't possible to load style properly in options

        }
        
    }
}