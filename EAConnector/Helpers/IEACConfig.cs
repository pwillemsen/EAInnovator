using System;
using System.Configuration;

namespace EAInnovator.Helpers
{
    /// <summary>
    /// Persist application configuration information
    /// </summary>
    public class IEACConfig
    {
        private static volatile IEACConfig instance;
        private static object syncRoot = new Object();
        private Configuration configFile;
        private KeyValueConfigurationCollection appSettings;

        public static IEACConfig _
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new IEACConfig();
                    }
                }

                return instance;
            }
        }

        public IEACConfig()
        {
            configFile = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
            // Get the appSettings section
            appSettings = configFile.AppSettings.Settings;
        }

        public string this[string key]
        {
            get { return appSettings[key].Value; }
            set
            {
                if (appSettings[key] == null)
                    appSettings.Add(key, value);
                else
                    appSettings[key].Value = value;
            }
        }

        public void save()
        {
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}
