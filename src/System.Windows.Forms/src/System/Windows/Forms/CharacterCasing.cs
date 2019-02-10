// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;




    /// <devdoc>
    ///    <para>
    ///       Specifies the case of characters in a Textbox control.
    ///    </para>
    /// </devdoc>
    public enum CharacterCasing {


        /// <devdoc>
        ///    <para>
        ///       The case of
        ///       characters is left unchanged.
        ///    </para>
        /// </devdoc>
        Normal = 0,


        /// <devdoc>
        ///    <para>
        ///       Converts all characters to uppercase.
        ///    </para>
        /// </devdoc>
        Upper = 1,


        /// <devdoc>
        ///    <para>
        ///       Converts all characters to lowercase.
        ///    </para>
        /// </devdoc>
        Lower = 2,

    }
}
