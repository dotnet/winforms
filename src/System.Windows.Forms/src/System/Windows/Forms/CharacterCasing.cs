// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;



    /// <include file='doc\CharacterCasing.uex' path='docs/doc[@for="CharacterCasing"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the case of characters in a Textbox control.
    ///    </para>
    /// </devdoc>
    public enum CharacterCasing {

        /// <include file='doc\CharacterCasing.uex' path='docs/doc[@for="CharacterCasing.Normal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The case of
        ///       characters is left unchanged.
        ///    </para>
        /// </devdoc>
        Normal = 0,

        /// <include file='doc\CharacterCasing.uex' path='docs/doc[@for="CharacterCasing.Upper"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Converts all characters to uppercase.
        ///    </para>
        /// </devdoc>
        Upper = 1,

        /// <include file='doc\CharacterCasing.uex' path='docs/doc[@for="CharacterCasing.Lower"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Converts all characters to lowercase.
        ///    </para>
        /// </devdoc>
        Lower = 2,

    }
}
