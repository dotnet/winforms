// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Collections;
    using Microsoft.Win32;


    public interface ICom2PropertyPageDisplayService {

        void ShowPropertyPage(string title, object component, int dispid, Guid pageGuid, IntPtr parentHandle);
    }

}
