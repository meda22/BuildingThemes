using System.Timers;
using BuildingThemesRevisited.Configuration;
using BuildingThemesRevisited.Utils;
using ColossalFramework.UI;

namespace BuildingThemesRevisited.GUI
{
    public static class PolicyPanelEnabler
    {
        
        private static bool _registered = false;

        public static bool unlock
        {
            get => ModConfiguration.UnlockPolicyPanel;
            set
            {
                ModConfiguration.UnlockPolicyPanel = value;
                if (_registered) UnlockPolicyToolbarButton();
            }
        }

        public static void Register()
        {
            if (_registered) return;
            
            UnlockManager.instance.m_milestonesUpdated += TimedUnlock;
            _registered = true;

        }

        public static void Unregister()
        {
            if (!_registered) return;
            
            UnlockManager.instance.m_milestonesUpdated -= TimedUnlock;
            _registered = false;
        }

        public static void TimedUnlock()
        {
            if (!unlock) return;
            
            // Hook up the Elapsed event for the timer. 
            var timer = new Timer(300);
            
            timer.Elapsed += delegate
            {
                timer.Enabled = false;
                timer.Dispose();
                UnlockPolicyToolbarButton();
            };
            
            timer.Enabled = true;
        }
        
        public static void UnlockPolicyToolbarButton()
        {
            if (!unlock) return;
            
            Logger.InfoLog("Unlock Policy Toolbar Button");
            var policiesButtonTransform = ToolsModifierControl.mainToolbar.gameObject.transform.Find("Policies");
            if (policiesButtonTransform == null) return;
            Logger.InfoLog("Unlocking Policy Toolbar now...");
            policiesButtonTransform.gameObject.GetComponent<UIButton>().isEnabled = true;
        }
    }
}