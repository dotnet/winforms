// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.UITests
{
    public partial class CustomForm : Form
    {
        private ManualResetEventSlim? _manualResetEventSlim;
        internal const VIRTUAL_KEY TestKey = VIRTUAL_KEY.VK_NUMLOCK;

        public ManualResetEventSlim GetManualResetEventSlim() => _manualResetEventSlim ??= new ManualResetEventSlim();

        public CustomForm ()
        {
            Application.AddMessageFilter(new InputRedirector(this));
        }
    }
}
