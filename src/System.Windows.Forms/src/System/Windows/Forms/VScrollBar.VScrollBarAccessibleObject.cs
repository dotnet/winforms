// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Forms
{
    public partial class VScrollBar
    {
        internal class VScrollBarAccessibleObject : ScrollBarAccessibleObject
        {
            internal VScrollBarAccessibleObject(VScrollBar owner) : base(owner)
            {
            }

            public override string Name
            {
                get => base.Name ?? SR.VScrollBarDefaultAccessibleName;
            }
        }
    }
}
