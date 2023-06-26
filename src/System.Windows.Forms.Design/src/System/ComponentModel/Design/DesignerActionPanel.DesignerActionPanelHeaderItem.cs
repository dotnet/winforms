// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private sealed class DesignerActionPanelHeaderItem : DesignerActionItem
    {
        public DesignerActionPanelHeaderItem(string title, string? subtitle) : base(title, null, null)
        {
            Subtitle = subtitle;
        }

        public string? Subtitle { get; }
    }
}
