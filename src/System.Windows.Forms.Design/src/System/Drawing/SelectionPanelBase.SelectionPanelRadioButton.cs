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

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                case Keys.Return:
                    return true;
            }

            return base.IsInputKey(keyData);
        }
    }
}
