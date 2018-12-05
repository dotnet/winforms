// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Collections;    
    using System.ComponentModel.Design;
    using Microsoft.Win32;

    ///<internalonly/>
    public interface IComPropertyBrowser {

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        void DropDownDone();

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
    
        bool InPropertySet{get;}
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        
        event ComponentRenameEventHandler ComComponentNameChanged;
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        bool EnsurePendingChangesCommitted();
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        void HandleF4();
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        void LoadState(RegistryKey key);
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        void SaveState(RegistryKey key);
    }
}
