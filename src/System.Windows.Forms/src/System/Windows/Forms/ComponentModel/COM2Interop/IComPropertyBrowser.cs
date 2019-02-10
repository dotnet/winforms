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

    public interface IComPropertyBrowser {

        void DropDownDone();

    
        bool InPropertySet{get;}
        
        event ComponentRenameEventHandler ComComponentNameChanged;
        bool EnsurePendingChangesCommitted();
        void HandleF4();
        void LoadState(RegistryKey key);
        void SaveState(RegistryKey key);
    }
}
