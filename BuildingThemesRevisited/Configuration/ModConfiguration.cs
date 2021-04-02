using System.Xml.Serialization;
using BuildingThemesRevisited.Utils;

namespace BuildingThemesRevisited.Configuration
{
    /// <summary>
    /// Keeps global configuration of mod
    /// </summary>
    internal static class ModConfiguration
    {
        internal static bool UnlockPolicyPanel = true;
    }

    /// <summary>
    /// Mod Configuration XML file definition.
    /// </summary>
    [XmlRoot("ModConfiguration")]
    public class XMLConfigFile
    {

        [XmlElement("UnlockPolicyPanel")]
        public bool unlockPolicyPanel
        {
            get => ModConfiguration.UnlockPolicyPanel;
            set => ModConfiguration.UnlockPolicyPanel = value;
        }

        [XmlElement("DebugLogging")]
        public bool debugLogging
        {
            get => Logger.DebugLogging;
            set => Logger.DebugLogging = value;
        }
        
    }
}