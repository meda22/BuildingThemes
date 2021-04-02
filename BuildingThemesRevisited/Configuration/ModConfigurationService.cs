using System;
using System.IO;
using System.Xml.Serialization;
using BuildingThemesRevisited.Utils;

namespace BuildingThemesRevisited.Configuration
{
    /// <summary>
    /// Handles XML configuration file de/serialization.
    /// </summary>
    internal static class ModConfigurationService
    {
        internal static readonly string ConfigurationFile = "BuildingThemesRevisitedConfig.xml";
        
        /// <summary>
        /// Load config from XML file.
        /// </summary>
        internal static void LoadConfiguration()
        {
            try
            {
                // Check to see if configuration file exists.
                if (File.Exists(ConfigurationFile))
                {
                    // Read it.
                    using (var reader = new StreamReader(ConfigurationFile))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(XMLConfigFile));
                        if (!(xmlSerializer.Deserialize(reader) is XMLConfigFile xmlConfigFile))
                        {
                            Logger.ErrorLog("couldn't deserialize settings file");
                        }
                    }
                }
                else
                {
                    Logger.InfoLog("no settings file found");
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e, "exception reading XML settings file");
            }
        }


        /// <summary>
        /// Save config to XML file.
        /// </summary>
        internal static void SaveConfiguration()
        {
            try
            {
                // Pretty straightforward.  Serialisation is within GBRSettingsFile class.
                using (var writer = new StreamWriter(ConfigurationFile))
                {
                    var xmlSerializer = new XmlSerializer(typeof(XMLConfigFile));
                    xmlSerializer.Serialize(writer, new XMLConfigFile());
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e, "exception saving XML settings file");
            }
        }
    }
}