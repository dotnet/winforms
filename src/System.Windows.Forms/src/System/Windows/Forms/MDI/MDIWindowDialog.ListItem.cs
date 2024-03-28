// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal sealed partial class MdiWindowDialog
{
    private class ListItem
    {
        public Form Form { get; }

        public ListItem(Form f)
        {
            Form = f;
        }

        public override string ToString()
        {
            return Form.Text;
        }
    }
}
