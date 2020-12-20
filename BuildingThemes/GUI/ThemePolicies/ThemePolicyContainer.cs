using ColossalFramework;
using ColossalFramework.UI;

namespace BuildingThemes.GUI.ThemePolicies
{
    // This helper component updates the theme button/checkbox state in the policy panel every game tick.
    // It is based on the game's PolicyContainer component.
    public class ThemePolicyContainer : ToolsModifierControl
    {
        private UIButton m_Button;
        private UICheckBox m_Check;

        private void Start()
        {
            this.m_Button = base.Find<UIButton>("PolicyButton");
            this.m_Check = base.Find<UICheckBox>("Checkbox");
        }

        private void Update()
        {
            if (base.component.isVisible)
            {
                lock (m_Check)
                {
                    var districtId = ToolsModifierControl.policiesPanel.targetDistrict;
                    var theme = (Configuration.Theme)m_Check.objectUserData;
                    if (Singleton<BuildingThemesManager>.instance.GetDistrictThemes(districtId, true).Contains(theme) != this.m_Check.isChecked)
                    {
                        this.m_Check.isChecked = !this.m_Check.isChecked;

                        if (Debugger.Enabled)
                        {
                            Debugger.LogFormat("Building Themes: ThemePolicyContainer. Changing theme {0} checkbox for district {1} to state: {2}",
                                theme.name, districtId, this.m_Check.isChecked);
                        }
                    }

                    this.m_Check.isEnabled = BuildingThemesManager.instance.IsThemeManagementEnabled(districtId);

                    this.m_Button.state = (!this.m_Check.isChecked) ? UIButton.ButtonState.Normal : UIButton.ButtonState.Focused;
                
                    this.m_Button.opacity = (!this.m_Check.isEnabled) ? 0.5f : 1f;
                }
            }
        }
    }
}