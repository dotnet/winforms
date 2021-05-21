// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms
{
    public partial class ToolStripPanel
    {
        internal partial class ToolStripPanelControlCollection : TypedControlCollection
        {
            // sort by Y, then X
            public class YXComparer : IComparer
            {
                public YXComparer() { }
                public int Compare(object first, object second)
                {
                    Control one = first as Control;
                    Control two = second as Control;

                    if (one.Bounds.Y < two.Bounds.Y)
                    {
                        return -1;
                    }

                    if (one.Bounds.Y == two.Bounds.Y)
                    {
                        if (one.Bounds.X < two.Bounds.X)
                        {
                            return -1;
                        }

                        return 1;
                    }

                    return 1;
                }
            }
        }
    }
}
