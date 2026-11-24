// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Drawing;

public sealed unsafe partial class Graphics
{
    // GDI+ ImageAbort returns a 4-byte BOOL. DrawImageAbort returns bool, which has no runtime marshalling
    // in this assembly and can't be used for a reverse P/Invoke (NativeAOT IL1005).
    private delegate BOOL NativeDrawImageAbort(void* callbackData);

    private sealed class DrawImageAbortAdapter
    {
        private readonly DrawImageAbort _callback;
        private readonly NativeDrawImageAbort _nativeCallback;

        public nint NativeCallback { get; }

        public DrawImageAbortAdapter(DrawImageAbort callback)
        {
            _callback = callback;
            _nativeCallback = CallBack;
            NativeCallback = Marshal.GetFunctionPointerForDelegate(_nativeCallback);
        }

        private BOOL CallBack(void* callbackData) => _callback((nint)callbackData);
    }
}
