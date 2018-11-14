// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    
    /// <include file='doc\PropertySort.uex' path='docs/doc[@for="PropertySort"]/*' />
    /// <devdoc>
    /// Possible values for property grid sorting mode 
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum PropertySort{
            
            /// <include file='doc\PropertySort.uex' path='docs/doc[@for="PropertySort.NoSort"]/*' />
            /// <devdoc>
            /// Properties will not be sorted, rather they
            /// will be displayed in the order that they are retrieved
            /// from the TypeDescriptor.
            /// </devdoc>
            NoSort = 0,
            
            /// <include file='doc\PropertySort.uex' path='docs/doc[@for="PropertySort.Alphabetical"]/*' />
            /// <devdoc>
            /// Properties are sorted as a flat, alphabetical list.
            /// </devdoc>
            Alphabetical = 1,
            
            /// <include file='doc\PropertySort.uex' path='docs/doc[@for="PropertySort.Categorized"]/*' />
            /// <devdoc>
            /// Properties are under category groups, which are defined 
            /// by the properties themselves.
            /// </devdoc>
            Categorized = 2, 
            
            /// <include file='doc\PropertySort.uex' path='docs/doc[@for="PropertySort.CategorizedAlphabetical"]/*' />
            /// <devdoc>
            /// Properties are under category groups, which are defined 
            /// by the properties themselves, and are alphabetical
            /// within those groups.
            /// </devdoc>
            
            CategorizedAlphabetical = Alphabetical | Categorized,
            
    }
}
