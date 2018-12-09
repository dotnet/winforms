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

    /// <include file='doc\IComPropertyBrowser.uex' path='docs/doc[@for="IComPropertyBrowser"]/*' />
    ///<internalonly/>
    public interface IComPropertyBrowser {

        /// <include file='doc\IComPropertyBrowser.uex' path='docs/doc[@for="IComPropertyBrowser.DropDownDone"]/*' />
        void DropDownDone();

        /// <include file='doc\IComPropertyBrowser.uex' path='docs/doc[@for="IComPropertyBrowser.InPropertySet"]/*' />
    
        bool InPropertySet{get;}
        /// <include file='doc\IComPropertyBrowser.uex' path='docs/doc[@for="IComPropertyBrowser.ComComponentNameChanged"]/*' />
        
        event ComponentRenameEventHandler ComComponentNameChanged;
        /// <include file='doc\IComPropertyBrowser.uex' path='docs/doc[@for="IComPropertyBrowser.EnsurePendingChangesCommitted"]/*' />
        bool EnsurePendingChangesCommitted();
        /// <include file='doc\IComPropertyBrowser.uex' path='docs/doc[@for="IComPropertyBrowser.HandleF4"]/*' />
        void HandleF4();
        /// <include file='doc\IComPropertyBrowser.uex' path='docs/doc[@for="IComPropertyBrowser.LoadState"]/*' />
        void LoadState(RegistryKey key);
        /// <include file='doc\IComPropertyBrowser.uex' path='docs/doc[@for="IComPropertyBrowser.SaveState"]/*' />
        void SaveState(RegistryKey key);
    }
}
