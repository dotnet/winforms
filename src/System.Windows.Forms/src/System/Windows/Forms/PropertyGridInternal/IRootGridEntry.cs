// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
 */
namespace System.Windows.Forms.PropertyGridInternal {
    using System.Runtime.InteropServices;

    using System.Diagnostics;

        using System;
        using System.Collections;
        using System.Reflection;
        using System.ComponentModel;
        using System.ComponentModel.Design;
        using System.Windows.Forms;
        using System.Drawing;
        using Microsoft.Win32;

        /// <include file='doc\IRootGridEntry.uex' path='docs/doc[@for="IRootGridEntry"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public interface IRootGridEntry{
                /// <include file='doc\IRootGridEntry.uex' path='docs/doc[@for="IRootGridEntry.BrowsableAttributes"]/*' />
                /// <devdoc>
                ///    <para>[To be supplied.]</para>
                /// </devdoc>
                AttributeCollection BrowsableAttributes {
                     get;
                     set;
                }
                /// <include file='doc\IRootGridEntry.uex' path='docs/doc[@for="IRootGridEntry.ResetBrowsableAttributes"]/*' />
                /// <devdoc>
                ///    <para>[To be supplied.]</para>
                /// </devdoc>

                void ResetBrowsableAttributes();
                /// <include file='doc\IRootGridEntry.uex' path='docs/doc[@for="IRootGridEntry.ShowCategories"]/*' />
                /// <devdoc>
                ///    <para>[To be supplied.]</para>
                /// </devdoc>
                void ShowCategories(bool showCategories);
        }
}
