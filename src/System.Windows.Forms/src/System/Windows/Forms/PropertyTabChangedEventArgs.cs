// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    [ComVisible(true)]
    public class PropertyTabChangedEventArgs : EventArgs
    {
        public PropertyTabChangedEventArgs(PropertyTab oldTab, PropertyTab newTab)
        {
            OldTab = oldTab;
            NewTab = newTab;
        }

        public PropertyTab OldTab { get; }

        public PropertyTab NewTab { get; }
    }
}
