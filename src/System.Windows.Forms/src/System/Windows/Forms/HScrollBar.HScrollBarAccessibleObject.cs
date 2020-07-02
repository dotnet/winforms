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
    public partial class HScrollBar
    {
        internal class HScrollBarAccessibleObject : ScrollBarAccessibleObject
        {
            internal HScrollBarAccessibleObject(HScrollBar owner) : base(owner)
            {
            }

            public override string Name
            {
                get => base.Name ?? SR.HScrollBarDefaultAccessibleName;
            }
        }
    }
}
