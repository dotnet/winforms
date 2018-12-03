// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.Versioning;
namespace System.Windows.Forms
{
    /// <summary>
    /// Winforms application configuration options
    /// </summary>
    internal static class ConfigurationOptions
    {
        private static NameValueCollection applicationConfigOptions = null;
        private static Version netFrameworkVersion = null;

        // Minimum supported framework version for this feature.
        private static readonly Version featureSupportedMinimumFrameworkVersion = new Version(4, 7);

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
            if (NetFrameworkVersion.CompareTo(featureSupportedMinimumFrameworkVersion) >= 0)
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
        }

        /// <summary>
        /// Extract current targeted framework version
        /// </summary>
        public static Version NetFrameworkVersion
        {
            get
            {
                if (netFrameworkVersion == null)
                {
                    netFrameworkVersion = new Version(0,0,0,0);  // by default version set to 0.0.0.0

                    // TargetFrameworkName can be null in certain scenarios.
                    try
                    {
                        var targetFrameworkName= AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName;
                        if (!String.IsNullOrEmpty(targetFrameworkName))
                        {
                            var frameworkName = new FrameworkName(targetFrameworkName);
                            if (String.Equals(frameworkName.Identifier, ".NETFramework"))
                                netFrameworkVersion = frameworkName.Version;
                        }
                    }
                    catch (Exception e)
                    {   
                        Debug.WriteLine("Exception while reading Framework version : " + e.ToString());
                    }
                }

                return netFrameworkVersion;
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
