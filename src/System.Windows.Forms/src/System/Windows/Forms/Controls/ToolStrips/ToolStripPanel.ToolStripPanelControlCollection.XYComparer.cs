// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public partial class ToolStripPanel
{
    internal partial class ToolStripPanelControlCollection : TypedControlCollection
    {
        // sort by X, then Y
        public class XYComparer : IComparer<IArrangedElement>
        {
            public XYComparer() { }
            public int Compare(IArrangedElement? first, IArrangedElement? second)
            {
                Control? one = first as Control;
                Control? two = second as Control;

                if (IComparerHelpers.CompareReturnIfNull(one, two, out int? returnValue))
                {
                    return (int)returnValue;
                }

                if (one.Bounds.X < two.Bounds.X)
                {
                    return -1;
                }

                if (one.Bounds.X == two.Bounds.X)
                {
                    return one.Bounds.Y < two.Bounds.Y ? -1 : 1;
                }

                return 1;
            }
        }
    }
}
