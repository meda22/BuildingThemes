﻿using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace BuildingThemes.GUI.ThemeManager
{
    public class UINewThemeModal : UIPanel
    {
        private UITitleBar m_title;
        private UITextField m_name;
        private UIButton m_ok;
        private UIButton m_cancel;

        private static UINewThemeModal _instance;

        public static UINewThemeModal instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UIView.GetAView().AddUIComponent(typeof(UINewThemeModal)) as UINewThemeModal;
                }
                return _instance;
            }
        }

        public override void Start()
        {
            base.Start();

            backgroundSprite = "UnlockingPanel2";
            isVisible = false;
            canFocus = true;
            isInteractive = true;
            width = 250;

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            m_title.title = "Create New Theme";
            m_title.iconSprite = "ToolbarIconZoomOutCity";
            m_title.isModal = true;

            // Name
            UILabel name = AddUIComponent<UILabel>();
            name.height = 30;
            name.text = "Theme name:";
            name.relativePosition = new Vector3(5, m_title.height);

            m_name = UIUtils.CreateTextField(this);
            m_name.width = width - 10;
            m_name.height = 30;
            m_name.padding = new RectOffset(6, 6, 6, 6);
            m_name.relativePosition = new Vector3(5, name.relativePosition.y + name.height + 5);

            m_name.Focus();
            m_name.eventTextChanged += (c, s) =>
            {
                m_ok.isEnabled = !s.IsNullOrWhiteSpace() && BuildingThemesManager.instance.GetThemeByName(s) == null;
            };

            m_name.eventTextSubmitted += (c, s) =>
            {
                if (m_ok.isEnabled && Input.GetKey(KeyCode.Return)) m_ok.SimulateClick();
            };

            // Ok
            m_ok = UIUtils.CreateButton(this);
            m_ok.text = "Create";
            m_ok.isEnabled = false;
            m_ok.relativePosition = new Vector3(5, m_name.relativePosition.y + m_name.height + 5);

            m_ok.eventClick += (c, p) =>
            {
                UIThemeManager.instance.CreateTheme(m_name.text);
                UIView.PopModal();
                Hide();
            };

            // Cancel
            m_cancel = UIUtils.CreateButton(this);
            m_cancel.text = "Cancel";
            m_cancel.relativePosition = new Vector3(width - m_cancel.width - 5, m_ok.relativePosition.y);

            m_cancel.eventClick += (c, p) =>
            {
                UIView.PopModal();
                Hide();
            };

            height = m_cancel.relativePosition.y + m_cancel.height + 5;
            relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));

            isVisible = true;
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();

            UIComponent modalEffect = GetUIView().panelsLibraryModalEffect;

            if (isVisible)
            {
                m_name.text = "";
                m_name.Focus();

                if (modalEffect != null)
                {
                    modalEffect.Show(false);
                    ValueAnimator.Animate("NewThemeModalEffect", delegate(float val)
                    {
                        modalEffect.opacity = val;
                    }, new AnimatedFloat(0f, 1f, 0.7f, EasingType.CubicEaseOut));
                }
            }
            else if (modalEffect != null)
            {
                ValueAnimator.Animate("NewThemeModalEffect", delegate(float val)
                {
                    modalEffect.opacity = val;
                }, new AnimatedFloat(1f, 0f, 0.7f, EasingType.CubicEaseOut), delegate
                {
                    modalEffect.Hide();
                });
            }
        }

        protected override void OnKeyDown(UIKeyEventParameter p)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                p.Use();
                UIView.PopModal();
                Hide();
            }

            base.OnKeyDown(p);
        }
    }
}
