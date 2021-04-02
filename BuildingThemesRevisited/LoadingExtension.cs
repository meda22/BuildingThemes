using System;
using BuildingThemesRevisited.GUI;
using BuildingThemesRevisited.Patches;
using BuildingThemesRevisited.Utils;
using ICities;

namespace BuildingThemesRevisited
{
    public class LoadingExtension : LoadingExtensionBase
    {
        // flags
        private static bool isModEnabled = false;
        private static bool isPatchWorking = false;
        
        /// <summary>
        /// Called by the game when the mod is initialised at the start of the loading process.
        /// </summary>
        /// <param name="loading">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            
            // check if we are loading to game mode
            if (loading.currentMode != AppMode.Game)
            {
                isModEnabled = false;
                Logger.InfoLog("Not loading into the game mode, skipping mod activation.");
                
                Patcher.UnpatchAll();
                return;
            }
            
            Logger.InfoLog("Initializing Mod (OnCreated)...");

            // check that Harmony patches has been applied
            if (!Patcher.patched)
            {
                isModEnabled = false;
                Logger.ErrorLog("Harmony Patches not applied. Aborting.");
                return;
            }

            try
            {
                
                
                PolicyPanelEnabler.Register();
                // TODO: all checks passed - we can set up following:
                // BuildingThemesManager.instance.Reset();
                // 3) BuildingVariationManager (probably only for cloning?)
                
                isModEnabled = true;
                isPatchWorking = true;
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Exception occured during mod initialization (OnCreated)!");
                Patcher.UnpatchAll();
                isModEnabled = false;
                isPatchWorking = false;
            }
        }

        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            
            // Do nothing if it not loaded to game mode
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame) return;
            
            Logger.InfoLog("Finishing loading of Building Themes config...");

            if (!isPatchWorking)
            {
                isModEnabled = false;
                Logger.ErrorLog("Mod has not been loaded properly. Abort.");
                return;
            }

            try
            {
                PolicyPanelEnabler.UnlockPolicyToolbarButton();
                CustomDistrictStyleManager.instance.ImportDistrictStyles();
                // Load building themes UI 
                // TODO: load following:
                // BuildingThemesManager.instance.ImportThemes();
                // 
                // UIThemeManager.Initialize();
                // UIStyleButtonReplacer.ReplaceStyleButton();
                // PolicyPanelEnabler.UnlockPolicyToolbarButton();
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Exception occured during mod loading (OnLevelLoaded)!");
                isModEnabled = false;
                isPatchWorking = false;
            }
        }
        
        // TODO: should I implement OnLevelUnloading? or OnReleased?
    }
}