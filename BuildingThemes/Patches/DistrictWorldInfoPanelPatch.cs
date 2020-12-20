using ColossalFramework.UI;
using HarmonyLib;

namespace BuildingThemes.Patches
{

    [HarmonyPatch(typeof(DistrictWorldInfoPanel))]
    static class DistrictWorldInfoPanelPatch
    {

        [HarmonyPostfix]
        [HarmonyPatch("OnPoliciesClick")]
        static void OnPoliciesClickPostfix()
        {
            UIView.Find<UIPanel>("PoliciesPanel").Find<UITabstrip>("Tabstrip").selectedIndex = 0;
        }

    }

}