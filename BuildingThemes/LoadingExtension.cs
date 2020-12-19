using System;
using BuildingThemes.GUI;
using BuildingThemes.GUI.ThemeManager;
using BuildingThemes.GUI.ThemePolicies;
using ICities;

namespace BuildingThemes
{

    /// <summary>
    /// Main loading class: the mod runs from here
    /// </summary>
    public class LoadingExtension : LoadingExtensionBase
    {

        // mod enabled flag
        private static bool isModEnabled = true;

        public override void OnCreated(ILoading loading)
        {
            // check if we are loading to game mode
            if (loading.currentMode != AppMode.Game)
            {
                isModEnabled = false;
            }

            // run base loading
            base.OnCreated(loading);

            // we have not loaded to game mod. Unpatch everything and leave
            if (!isModEnabled)
            {
                Patcher.UnpatchAll();
                return;
            }

            Debugger.Initialize();

            Debugger.Log("ON_CREATED");
            Debugger.Log("Building Themes: Initializing Mod...");

            try
            {
                // enable policy panel from beginning
                PolicyPanelEnabler.Register();
                // set up BuildingThemesManager
                BuildingThemesManager.instance.Reset();
                // set up BuildingThemesVariationManager
                BuildingVariationManager.instance.Reset();

                // TODO: work on this method when we want to enable feature of cloning
                // UpdateConfig();

                Debugger.Log("Building Themes: Mod successfully intialized.");
                isModEnabled = true;
                // TODO: add check that Patcher has been loaded correctly
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
                // In case of any error, unpatch everything
                Patcher.UnpatchAll();
            }
        }
        
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            Debugger.Log("ON_LEVEL_LOADED");
            Debugger.OnLevelLoaded();

            try
            {
                // Don't load if it's not a game
                if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame) return;

                // TODO: add check that Patcher has been loaded correctly
                // TODO: add check that game was loaded fully
                
                BuildingThemesManager.instance.ImportThemes();

                PolicyPanelEnabler.UnlockPolicyToolbarButton();
                UIThemeManager.Initialize();
                UIStyleButtonReplacer.ReplaceStyleButton();
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            Debugger.Log("ON_LEVEL_UNLOADING");
            Debugger.OnLevelUnloading();

            BuildingThemesManager.instance.Reset();
            UIThemeManager.Destroy();
        }

        public override void OnReleased()
        {
            base.OnReleased();
            Debugger.Log("ON_RELEASED");

            BuildingThemesManager.instance.Reset();
            BuildingVariationManager.instance.Reset();
            PolicyPanelEnabler.Unregister();

            Debugger.Log("Building Themes: Reverting detoured methods...");
            try
            {
                Patcher.UnpatchAll();
                Debugger.Log("Building Themes: DistrictWorldInfoPanel Methods restored!");
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
            }

            Debugger.Log("Building Themes: Done!");

            Debugger.Deinitialize();
        }
        
        // TODO: this method is checking if building cloning is used (then enable feature). Move it to BuildingThemesMod class
        private void UpdateConfig()
        {
            // TODO: move this method to Building mod to enable cloning if used
            // If config version is 0, disable the cloning feature if it is not used in one of the themes
            // if (BuildingVariationManager.Enabled)
            // {
            //     bool cloneFeatureUsed = false;
            //
            //     if (BuildingThemesManager.instance.Configuration.version == 0)
            //     {
            //         foreach (var theme in BuildingThemesManager.instance.Configuration.themes)
            //         {
            //             foreach (var building in theme.buildings)
            //             {
            //                 if (building.baseName != null)
            //                 {
            //                     cloneFeatureUsed = true;
            //                     break;
            //                 }
            //             }
            //
            //             if (cloneFeatureUsed) break;
            //         }
            //     }
            //     else cloneFeatureUsed = true;
            //
            //     if (cloneFeatureUsed)
            //     {
            //         try { Detour.BuildingInfoDetour.Deploy(); }
            //         catch (Exception e) { Debugger.LogException(e); }
            //     }
            //     else
            //     {
            //         BuildingVariationManager.Enabled = false;
            //     }
            // }
            // BuildingThemesManager.instance.Configuration.version = 1;
            // BuildingThemesManager.instance.SaveConfig();
            
            // For now, we never enable cloning, so config will be always updated to version = 0
            BuildingThemesManager.instance.Configuration.version = 0;
            BuildingThemesManager.instance.SaveConfig();
        }
    }
}