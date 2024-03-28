// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal partial class StyleCollectionEditor
{
    protected class NavigationalTableLayoutPanel : TableLayoutPanel
    {
        private List<RadioButton> RadioButtons
        {
            get
            {
                List<RadioButton> radioButtons = [];
                foreach (Control control in Controls)
                {
                    RadioButton radioButton = (RadioButton)control;
                    if (radioButton is not null)
                    {
                        radioButtons.Add(radioButton);
                    }
                }

                return radioButtons;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool down = keyData == Keys.Down;
            bool up = keyData == Keys.Up;

            if (down || up)
            {
                List<RadioButton> radioButtons = RadioButtons;

                for (int i = 0; i < radioButtons.Count; i++)
                {
                    RadioButton radioButton = radioButtons[i];
                    if (radioButton.Focused)
                    {
                        int focusIndex;
                        if (down)
                        {
                            focusIndex = i == RadioButtons.Count - 1 ? 0 : i + 1;
                        }
                        else
                        {
                            focusIndex = i == 0 ? RadioButtons.Count - 1 : i - 1;
                        }

                        radioButtons[focusIndex].Focus();
                        return true;
                    }
                }
            }

            return base.ProcessDialogKey(keyData);
        }
    }
}
