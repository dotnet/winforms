// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    
    /// <devdoc>
    /// Possible values for property grid sorting mode 
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum PropertySort{
            
            /// <devdoc>
            /// Properties will not be sorted, rather they
            /// will be displayed in the order that they are retrieved
            /// from the TypeDescriptor.
            /// </devdoc>
            NoSort = 0,
            
            /// <devdoc>
            /// Properties are sorted as a flat, alphabetical list.
            /// </devdoc>
            Alphabetical = 1,
            
            /// <devdoc>
            /// Properties are under category groups, which are defined 
            /// by the properties themselves.
            /// </devdoc>
            Categorized = 2, 
            
            /// <devdoc>
            /// Properties are under category groups, which are defined 
            /// by the properties themselves, and are alphabetical
            /// within those groups.
            /// </devdoc>
            
            CategorizedAlphabetical = Alphabetical | Categorized,
            
    }
}
