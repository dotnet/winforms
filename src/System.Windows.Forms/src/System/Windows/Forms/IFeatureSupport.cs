// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Configuration.Assemblies;

    using System.Diagnostics;

    using System;
    
    /// <include file='doc\IFeatureSupport.uex' path='docs/doc[@for="IFeatureSupport"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies a standard
    ///       interface for retrieving feature information from the current system.
    ///    </para>
    /// </devdoc>

    public interface IFeatureSupport {
    
        /// <include file='doc\IFeatureSupport.uex' path='docs/doc[@for="IFeatureSupport.IsPresent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Determines whether any version of the specified feature
        ///       is currently available
        ///       on the system.
        ///    </para>
        /// </devdoc>
        bool IsPresent(object feature);
        
        /// <include file='doc\IFeatureSupport.uex' path='docs/doc[@for="IFeatureSupport.IsPresent1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Determines whether the specified or newer version of the
        ///       specified feature
        ///       is currently available on the system.
        ///    </para>
        /// </devdoc>
        bool IsPresent(object feature, Version minimumVersion);
    
        /// <include file='doc\IFeatureSupport.uex' path='docs/doc[@for="IFeatureSupport.GetVersionPresent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves the version
        ///       of the specified feature.
        ///    </para>
        /// </devdoc>
        Version GetVersionPresent(object feature);
    }

}
