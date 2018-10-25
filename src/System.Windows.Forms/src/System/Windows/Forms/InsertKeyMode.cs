// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///     Represents the mode characters are entered in a text box.
    /// </summary>
    public enum InsertKeyMode
    {
        /// <devdoc> 
        ///     Honors the Insert key mode. 
        /// </devdoc>
        Default,

        /// <devdoc> 
        ///     Forces insertion mode to be 'on' regardless of the Insert key mode. 
        /// </devdoc>
        Insert,

        /// <devdoc> 
        ///     Forces insertion mode to be 'off' regardless of the Insert key mode. 
        /// </devdoc>
        Overwrite        
    }
}
