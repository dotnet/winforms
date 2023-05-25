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

        public void ResetManualResetEventSlim()
        {
            _manualResetEventSlim ??= new ManualResetEventSlim();
            _manualResetEventSlim.Reset();
        }

        public void SetManualResetEventSlim()
        {
            _manualResetEventSlim.OrThrowIfNull();
            if (HandleInternal == IntPtr.Zero)
            {
                throw new InvalidOperationException("window is closed/destroyed and Handle is null");
            }

            _manualResetEventSlim.Set();
        }

        public bool WaitOnManualResetEventSlim(int timeOut)
        {
            _manualResetEventSlim.OrThrowIfNull();
            if (HandleInternal == IntPtr.Zero)
            {
                throw new InvalidOperationException("window is closed/destroyed and Handle is null");
            }

            return _manualResetEventSlim.Wait(5000);
        }

        public CustomForm ()
        {
            Application.AddMessageFilter(new InputRedirector(this));
        }
    }
}
