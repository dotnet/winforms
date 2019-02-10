// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Configuration.Assemblies;

    using System.Diagnostics;

    using System;
    using System.Reflection;
        


    /// <devdoc>
    /// <para>Provides <see langword='static'/> methods for retrieving feature information from the
    ///    current system.</para>
    /// </devdoc>
    public abstract class FeatureSupport : IFeatureSupport {


        /// <devdoc>
        ///    <para>Determines whether any version of the specified feature
        ///       is installed in the system. This method is <see langword='static'/>.</para>
        /// </devdoc>
        public static bool IsPresent(string featureClassName, string featureConstName) {
            return IsPresent(featureClassName, featureConstName, new Version(0, 0, 0, 0));
        }


        /// <devdoc>
        ///    <para>Determines whether the specified or newer version of the specified feature is
        ///       installed in the system. This method is <see langword='static'/>.</para>
        /// </devdoc>
        public static bool IsPresent(string featureClassName, string featureConstName, Version minimumVersion) 
        {
            object featureId = null;
            IFeatureSupport featureSupport = null;

            //APPCOMPAT: If Type.GetType() throws, we want to return
            //null to preserve Everett behavior.
            Type c = null;
            try {
                c = Type.GetType(featureClassName);
            }
            catch (ArgumentException) {}

            if (c != null) {
                FieldInfo fi = c.GetField(featureConstName);

                if (fi != null) {
                    featureId = fi.GetValue(null);
                }
            }

            if (featureId != null && typeof(IFeatureSupport).IsAssignableFrom(c)) {

                featureSupport = (IFeatureSupport) Activator.CreateInstance(c);

                if (featureSupport != null) {
                    return featureSupport.IsPresent(featureId, minimumVersion);
                }
            }
            return false;
        }


        /// <devdoc>
        ///    <para>Gets the version of the specified feature that is available on the system.</para>
        /// </devdoc>
        public static Version GetVersionPresent(string featureClassName, string featureConstName) {
            object featureId = null;
            IFeatureSupport featureSupport = null;

            //APPCOMPAT: If Type.GetType() throws, we want to return
            //null to preserve Everett behavior.
            Type c = null;
            try {
                c = Type.GetType(featureClassName);
            }
            catch (ArgumentException) {}
            
            if (c != null) {
                FieldInfo fi = c.GetField(featureConstName);

                if (fi != null) {
                    featureId = fi.GetValue(null);
                }
            }

            if (featureId != null) 
            {
                featureSupport = (IFeatureSupport) Activator.CreateInstance(c);

                if (featureSupport != null) {
                    return featureSupport.GetVersionPresent(featureId);
                }
            }
            return null;
        }


        /// <devdoc>
        ///    <para>Determines whether any version of the specified feature
        ///       is installed in the system.</para>
        /// </devdoc>
        public virtual bool IsPresent(object feature) {
            return IsPresent(feature, new Version(0, 0, 0, 0));
        }


        /// <devdoc>
        ///    <para>Determines whether the specified or newer version of the
        ///       specified feature is installed in the system.</para>
        /// </devdoc>
        public virtual bool IsPresent(object feature, Version minimumVersion) {
            Version ver = GetVersionPresent(feature);

            if (ver != null) {
                return ver.CompareTo(minimumVersion) >= 0;
            }
            return false;
        }


        /// <devdoc>
        ///    <para>When overridden in a derived class, gets the version of the specified
        ///       feature that is available on the system.</para>
        /// </devdoc>
        public abstract Version GetVersionPresent(object feature);
    }


}
