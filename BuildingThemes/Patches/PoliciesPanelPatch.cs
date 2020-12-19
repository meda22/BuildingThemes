using BuildingThemes.GUI.ThemePolicies;
using ColossalFramework.UI;
using HarmonyLib;

namespace BuildingThemes.Patches
{
    
    
    /// <summary>
    /// This Patch hooks into the Policy Panel to display the 'Themes' tab
    /// </summary>
    // TODO: there is a harmless error during patches, but we should investigate how to avoid them anyway
    [HarmonyPatch(typeof(PoliciesPanel), "SetParentButton")]
    static class SetParentButtonPatch
    {
        static void Prefix(UIButton button)
        {
            if (button == null) return;
            
            // We have to remove the custom tab before the original SetParentButton method is called
            // SetParentButton() is searching for a TutorialUITag component which our tab does not have
            ThemePolicyTab.RemoveThemesTab();
        }

        static void Postfix()
        {
            ThemePolicyTab.AddThemesTab();
        }
    }
    
    [HarmonyPatch(typeof(PoliciesPanel), "RefreshPanel")]
    static class RefreshPanelPatch
    {
        static void Prefix()
        {
            // We have to remove the custom tab before the original SetParentButton method is called
            // SetParentButton() is searching for a TutorialUITag component which our tab does not have
            ThemePolicyTab.RemoveThemesTab();
        }

        static void Postfix()
        {
            ThemePolicyTab.AddThemesTab();
        }
    }
}
