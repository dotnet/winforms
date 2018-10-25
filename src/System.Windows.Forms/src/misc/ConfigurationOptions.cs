// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Diagnostics;
    using System.Runtime.Versioning;

    /// <summary>
    /// Winforms application configuration options
    /// </summary>
    internal static class ConfigurationOptions
    {
        private static NameValueCollection applicationConfigOptions = null;

        // Current OS version
        internal static Version OSVersion = Environment.OSVersion.Version;
           
        // RS2 build number - we may need to change once we know RTM version. below is pre-RTM RS2 version.
        internal static readonly Version RS2Version = new Version(10, 0, 14933, 0); 

        static ConfigurationOptions()
        {
            PopulateWinformsSection();
        }

        private static void PopulateWinformsSection()
        {
            try
            {
                applicationConfigOptions = ConfigurationManager.GetSection(ConfigurationStringConstants.WinformsApplicationConfigurationSectionName) as NameValueCollection;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception while reading" + ConfigurationStringConstants.WinformsApplicationConfigurationSectionName + " from app.config file " + " " + ex.ToString());
            }
        }

        /// <summary>
        /// Extract value of the key specified from collection read from app.config file's winforms section
        /// </summary>
        /// <param name="settingName"> setting key name</param>
        /// <returns>value of key</returns>
        public static string GetConfigSettingValue(string settingName)
        {
            if (applicationConfigOptions != null && !string.IsNullOrEmpty(settingName))
            {
                return applicationConfigOptions.Get(settingName);
            }

            return null;
        }

    }
}
