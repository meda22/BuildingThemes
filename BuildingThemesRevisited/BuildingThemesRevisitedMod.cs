using BuildingThemesRevisited.Configuration;
using BuildingThemesRevisited.GUI;
using BuildingThemesRevisited.Patches;
using BuildingThemesRevisited.Utils;
using CitiesHarmony.API;
using ICities;

namespace BuildingThemesRevisited
{
    public class BuildingThemesRevisitedMod : IUserMod
    {
        internal static string ModName = "Building Themes Revisited";
        
        public string Name => "Building Themes Revisited WIP";

        public string Description => "Create building themes and apply them to cities and districts. Ver. Alpha 0.0.1 :)";
        
        /// <summary>
        /// When mod is enabled
        /// </summary>
        public void OnEnabled() 
        {
            HarmonyHelper.DoOnHarmonyReady(Patcher.PatchAll);
            
            // load configuration
            ModConfigurationService.LoadConfiguration();
        }
        
        /// <summary>
        /// When mod is disabled
        /// </summary>
        public void OnDisabled() 
        {
            if (HarmonyHelper.IsHarmonyInstalled) Patcher.UnpatchAll();
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            var group = helper.AddGroup("Building Themes Revisited");
            
            group.AddCheckbox(
                "Generate Debug Output", 
                Logger.DebugLogging,
                isChecked =>
                {
                    Logger.DebugLogging = isChecked;
                    ModConfigurationService.SaveConfiguration();
                });
            
            group.AddCheckbox(
                "Unlock Policies Panel from start", 
                PolicyPanelEnabler.unlock,
                isChecked =>
                { 
                    PolicyPanelEnabler.unlock = isChecked;
                    ModConfigurationService.SaveConfiguration();
                });

            // TODO: set up mod settings window
            /*
             *  group.AddCheckbox("Unlock Policies Panel From Start", PolicyPanelEnabler.Unlock,
                delegate (bool c) { PolicyPanelEnabler.Unlock = c; });
             * 
             *  Enable Prefab Cloning - checkbox
             *
             *  Warning message when selecting an invalid theme - checkbox
             *
             */
            
        }
    }
}