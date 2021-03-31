// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class LinkLabel
    {
        private class LinkComparer : IComparer
        {
            int IComparer.Compare(object link1, object link2)
            {
                Debug.Assert(link1 is not null && link2 is not null, "Null objects sent for comparison");

                int pos1 = ((Link)link1).Start;
                int pos2 = ((Link)link2).Start;

                return pos1 - pos2;
            }
        }
    }
}
