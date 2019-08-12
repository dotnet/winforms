// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides <see langword='static'/> methods for retrieving feature information from the
    ///  current system.
    /// </summary>
    public abstract class FeatureSupport : IFeatureSupport
    {
        /// <summary>
        ///  Determines whether any version of the specified feature is installed in the system.
        ///  This method is <see langword='static'/>.
        /// </summary>
        public static bool IsPresent(string featureClassName, string featureConstName)
        {
            return IsPresent(featureClassName, featureConstName, new Version(0, 0, 0, 0));
        }

        /// <summary>
        ///  Determines whether the specified or newer version of the specified feature is
        ///  installed in the system. This method is <see langword='static'/>.
        /// </summary>
        public static bool IsPresent(string featureClassName, string featureConstName, Version minimumVersion)
        {
            Type c = null;
            try
            {
                c = Type.GetType(featureClassName);
            }
            catch (ArgumentException)
            {
            }

            object featureId = c?.GetField(featureConstName)?.GetValue(null);
            if (featureId == null || !typeof(IFeatureSupport).IsAssignableFrom(c))
            {
                return false;
            }

            IFeatureSupport featureSupport = (IFeatureSupport)Activator.CreateInstance(c);
            return featureSupport.IsPresent(featureId, minimumVersion);
        }

        /// <summary>
        ///  Gets the version of the specified feature that is available on the system.
        /// </summary>
        public static Version GetVersionPresent(string featureClassName, string featureConstName)
        {
            Type c = null;
            try
            {
                c = Type.GetType(featureClassName);
            }
            catch (ArgumentException)
            {
            }

            object featureId = c?.GetField(featureConstName)?.GetValue(null);
            if (featureId == null || !typeof(IFeatureSupport).IsAssignableFrom(c))
            {
                return null;
            }

            IFeatureSupport featureSupport = (IFeatureSupport)Activator.CreateInstance(c);
            return featureSupport.GetVersionPresent(featureId);
        }

        /// <summary>
        ///  Determines whether any version of the specified feature is installed in the system.
        /// </summary>
        public virtual bool IsPresent(object feature) => IsPresent(feature, new Version(0, 0, 0, 0));

        /// <summary>
        ///  Determines whether the specified or newer version of the specified feature is
        ///  installed in the system.
        /// </summary>
        public virtual bool IsPresent(object feature, Version minimumVersion)
        {
            Version ver = GetVersionPresent(feature);
            if (ver == null)
            {
                return false;
            }

            return ver.CompareTo(minimumVersion) >= 0;
        }

        /// <summary>
        ///  When overridden in a derived class, gets the version of the specified feature that
        ///  is available on the system.
        /// </summary>
        public abstract Version GetVersionPresent(object feature);
    }
}
