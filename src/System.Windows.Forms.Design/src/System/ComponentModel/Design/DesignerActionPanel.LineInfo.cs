// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel
    {
        private class LineInfo
        {
            public Line Line;
            public DesignerActionItem Item;
            public DesignerActionList List;

            public LineInfo(DesignerActionList list, DesignerActionItem item, Line line)
            {
                Debug.Assert(line is not null);
                Line = line;
                Item = item;
                List = list;
            }
        }
    }
}
