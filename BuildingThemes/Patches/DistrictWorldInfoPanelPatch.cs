using ColossalFramework.UI;
using HarmonyLib;

namespace BuildingThemes.Patches
{

    [HarmonyPatch(typeof(DistrictWorldInfoPanel), "OnPoliciesClick")]
    static class OnPoliciesClickPatch
    {
        static void Postfix()
        {
            UIView.Find<UIPanel>("PoliciesPanel").Find<UITabstrip>("Tabstrip").selectedIndex = 0;
        }
    }
}