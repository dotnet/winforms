// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies a standard interface for retrieving feature information from the current system.
    /// </summary>
    public interface IFeatureSupport
    {
        /// <summary>
        ///  Determines whether any version of the specified feature is currently available
        ///  on the system.
        /// </summary>
        bool IsPresent(object feature);

        /// <summary>
        ///  Determines whether the specified or newer version of the specified feature
        ///  is currently available on the system.
        /// </summary>
        bool IsPresent(object feature, Version minimumVersion);

        /// <summary>
        ///  Retrieves the version of the specified feature.
        /// </summary>
        Version GetVersionPresent(object feature);
    }
}
