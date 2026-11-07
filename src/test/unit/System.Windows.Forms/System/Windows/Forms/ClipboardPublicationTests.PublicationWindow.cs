// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public partial class ClipboardPublicationTests
{
    /// <summary>
    ///  Keeps queued input observable without involving an interactive window.
    /// </summary>
    private sealed class PublicationWindow : NativeWindow, IDisposable
    {
        private const int Message = 0x8000;

        public PublicationWindow() => CreateHandle(new CreateParams());

        public int Delivered { get; private set; }

        public void QueueMessage() => Check(PostMessage(Handle, Message, 0, 0));

        public unsafe void VerifyMessagePreserved()
        {
            global::Windows.Win32.UI.WindowsAndMessaging.MSG message;
            ((bool)global::Windows.Win32.PInvokeCore.PeekMessage(
                &message,
                (global::Windows.Win32.Foundation.HWND)Handle,
                Message,
                Message,
                global::Windows.Win32.UI.WindowsAndMessaging.PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE))
                .Should().BeTrue("the posted message must remain queued after publication");
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == Message)
            {
                Delivered++;
            }

            base.WndProc(ref message);
        }

        public void Dispose() => DestroyHandle();
    }
}
