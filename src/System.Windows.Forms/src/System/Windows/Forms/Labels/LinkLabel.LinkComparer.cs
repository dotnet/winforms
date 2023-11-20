// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class LinkLabel
{
    private class LinkComparer : IComparer<Link>
    {
        public int Compare(Link? link1, Link? link2)
        {
            int pos1 = link1?.Start ?? -1;
            int pos2 = link2?.Start ?? -1;

            return pos1 - pos2;
        }
    }
}
