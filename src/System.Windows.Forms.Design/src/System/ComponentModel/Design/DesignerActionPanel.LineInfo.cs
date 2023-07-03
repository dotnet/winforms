// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private abstract class LineInfo
    {
        public readonly DesignerActionItem? Item;
        public readonly DesignerActionList? List;

        protected LineInfo(DesignerActionList? list, DesignerActionItem? item)
        {
            Item = item;
            List = list;
        }

        public abstract Line CreateLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel);
        public abstract Type LineType { get; }
    }
}
