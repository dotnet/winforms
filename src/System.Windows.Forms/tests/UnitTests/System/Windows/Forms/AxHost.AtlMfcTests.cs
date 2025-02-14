// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32.System.ApplicationInstallationAndServicing;

namespace System.Windows.Forms.Tests;

public class AxHostAtlTests
{
    #region Utilities

    // taken from \src\System.Windows.Forms\tests\InteropTests\PropertyGridTests.cs

    [DllImport("kernel32", SetLastError = true)]
    private static extern void ReleaseActCtx(IntPtr hActCtx);

    private unsafe void ExecuteWithActivationContext(string applicationManifest, Action action)
    {
        ACTCTXW context = new();
        HANDLE handle;
        fixed (char* p = applicationManifest)
        {
            context.cbSize = (uint)sizeof(ACTCTXW);
            context.lpSource = p;

            handle = PInvoke.CreateActCtx(&context);
        }

        if (handle == IntPtr.Zero)
        {
            throw new Win32Exception();
        }

        try
        {
            nuint cookie;
            if (!PInvoke.ActivateActCtx(handle, &cookie))
            {
                throw new Win32Exception();
            }

            try
            {
                action();
            }
            finally
            {
                if (!PInvoke.DeactivateActCtx(0, cookie))
                {
                    throw new Win32Exception();
                }
            }
        }
        finally
        {
            ReleaseActCtx(handle);
        }
    }

    #endregion

    [WinFormsFact]
    public void AxHost_AtlControl_CreateAndSetText()
    {
        if (RuntimeInformation.ProcessArchitecture != Architecture.X86)
        {
            return;
        }

        ExecuteWithActivationContext("App.manifest", () =>
        {
            using Form form = new();
            using AxNativeAtlControl control = new();

            int textChangedEventCount = 0;
            string textChangedEventArg = null;
            const string testText = "Hello World";

            control.AxTextChanged += (string text) =>
            {
                textChangedEventCount++;
                textChangedEventArg = text;
            };

            form.Shown += (object sender, EventArgs e) =>
            {
                control.AxText = testText;
                form.Close();
            };

            ((ISupportInitialize)control).BeginInit();
            form.Controls.Add(control);
            ((ISupportInitialize)control).EndInit();
            form.ShowDialog();

            Assert.Equal(1, textChangedEventCount);
            Assert.Equal(testText, textChangedEventArg);
        });
    }

    private unsafe class AxNativeAtlControl : AxHost
    {
        private const int DISPID_TEXT = -517;
        private const int DISPID_CLICK = -600;

        [ComImport]
        [Guid("424ddab6-a4b1-422f-93b9-7e254b28dbf0")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        private interface IAxTestControl
        {
            [DispId(DISPID_TEXT)] string Text { set; get; }
        }

        [ComImport]
        [Guid("726D447D-8F66-4330-9531-F591011DC7B7")]
        [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        private interface IAxTestControlEvents
        {
            [DispId(1)] void OnTextChanged([MarshalAs(UnmanagedType.BStr)] string text);
            [DispId(2)] void OnButtonClick(int count);
            [DispId(DISPID_CLICK)] void OnClick();
        }

        private sealed class AxNativeAtlControlEventHandler(AxNativeAtlControl control) : IAxTestControlEvents
        {
            private readonly AxNativeAtlControl _control = control;
            void IAxTestControlEvents.OnTextChanged(string text) => _control.AxTextChanged?.Invoke(text);
            void IAxTestControlEvents.OnButtonClick(int count) => _control.AxButtonClick?.Invoke(count);
            void IAxTestControlEvents.OnClick() => _control.AxClick?.Invoke();
        }

        public event Action<string> AxTextChanged;
        public event Action<int> AxButtonClick;
        public event Action AxClick;

        private ConnectionPointCookie cookie;
        public AxNativeAtlControl() : base("4EABA135-7C8A-4DB8-B732-2A6AE511D7EB") { }
        protected override void CreateSink() => this.cookie = new(GetOcx(), new AxNativeAtlControlEventHandler(this), typeof(IAxTestControlEvents));
        protected override void DetachSink() => this.cookie.Disconnect();

        public string AxText
        {
            get => ((IAxTestControl)GetOcx()).Text;
            set => ((IAxTestControl)GetOcx()).Text = value;
        }
    }
}
