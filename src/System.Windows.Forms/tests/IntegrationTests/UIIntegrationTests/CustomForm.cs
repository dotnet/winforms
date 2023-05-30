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
        internal const int Timeout = 5000;

        public bool DoNotSendTestInput { get; internal set; }

        private readonly InputRedirector _messageFilter;
        internal bool ParentClosed;

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
            return _manualResetEventSlim.Wait(timeOut);
        }

        public CustomForm (bool doNotSendTestInput = false)
        {
            DoNotSendTestInput = doNotSendTestInput;
            _messageFilter = new InputRedirector(this);
            Application.AddMessageFilter(_messageFilter);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ParentClosed = true;
            _manualResetEventSlim?.Set();
            Application.RemoveMessageFilter(_messageFilter);
        }
    }
}
