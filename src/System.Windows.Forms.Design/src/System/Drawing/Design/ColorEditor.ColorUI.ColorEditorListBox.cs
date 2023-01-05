// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace System.Drawing.Design
{
    public partial class ColorEditor
    {
        private sealed partial class ColorUI
        {
            private sealed class ColorEditorListBox : ListBox
            {
                protected override bool IsInputKey(Keys keyData)
                {
                    return keyData switch
                    {
                        Keys.Return => true,
                        _ => base.IsInputKey(keyData),
                    };
                }
            }
        }
    }
}
