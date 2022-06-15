// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    public partial class PropertyGrid
    {
        private record TabInfo(PropertyTab Tab, PropertyTabScope Scope, ToolStripButton Button)
        {
            public Type TabType => Tab.GetType();
        }
    }
}
