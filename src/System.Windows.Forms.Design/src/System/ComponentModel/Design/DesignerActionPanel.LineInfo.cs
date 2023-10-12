// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private abstract class LineInfo
    {
        public abstract DesignerActionItem? Item { get; }

        public abstract Line CreateLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel);
        public abstract Type LineType { get; }
    }

    private abstract class StandardLineInfo(DesignerActionList list) : LineInfo
    {
        public abstract override DesignerActionItem Item { get; }
        public DesignerActionList List { get; } = list;
    }
}
