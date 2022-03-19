// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    internal sealed partial class MdiWindowDialog
    {
        private class ListItem
        {
            public Form form;

            public ListItem(Form f)
            {
                form = f;
            }

            public override string ToString()
            {
                return form.Text;
            }
        }
    }
}
