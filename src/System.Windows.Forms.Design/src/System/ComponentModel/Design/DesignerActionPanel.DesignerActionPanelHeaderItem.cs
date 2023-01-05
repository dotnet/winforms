// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel
    {
        private sealed class DesignerActionPanelHeaderItem : DesignerActionItem
        {
            private readonly string _subtitle;

            public DesignerActionPanelHeaderItem(string title, string subtitle) : base(title, null, null)
            {
                _subtitle = subtitle;
            }

            public string Subtitle
            {
                get => _subtitle;
            }
        }
    }
}
