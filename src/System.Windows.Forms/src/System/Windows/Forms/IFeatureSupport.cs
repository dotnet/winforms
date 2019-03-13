﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies a standard interface for retrieving feature information from the current system.
    /// </devdoc>
    public interface IFeatureSupport
    {
        /// <devdoc>
        /// Determines whether any version of the specified feature is currently available
        /// on the system.
        /// </devdoc>
        bool IsPresent(object feature);

        /// <devdoc>
        /// Determines whether the specified or newer version of the specified feature
        /// is currently available on the system.
        /// </devdoc>
        bool IsPresent(object feature, Version minimumVersion);

        /// <devdoc>
        /// Retrieves the version of the specified feature.
        /// </devdoc>
        Version GetVersionPresent(object feature);
    }
}
