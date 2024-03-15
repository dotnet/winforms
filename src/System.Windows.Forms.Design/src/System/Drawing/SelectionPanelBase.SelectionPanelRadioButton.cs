// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace System.Drawing.Design;

internal abstract partial class SelectionPanelBase
{
    protected class SelectionPanelRadioButton : RadioButton
    {
        public SelectionPanelRadioButton()
        {
            AutoCheck = false;
        }

        protected override bool ShowFocusCues => true;

        protected override bool IsInputKey(Keys keyData) => keyData switch
        {
            Keys.Left or Keys.Right or Keys.Up or Keys.Down or Keys.Return => true,
            _ => base.IsInputKey(keyData),
        };
    }
}
