// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Drawing;

public sealed unsafe partial class Graphics
{
    private delegate BOOL NativeEnumerateMetafileProc(
        EmfPlusRecordType recordType,
        uint flags,
        uint dataSize,
        byte* data,
        void* callbackData);

    private sealed class EnumerateMetafileDataAdapter : IDisposable
    {
        private readonly EnumerateMetafileProc _callback;
        private readonly NativeEnumerateMetafileProc _nativeCallback;

        public nint NativeCallback { get; }

        public EnumerateMetafileDataAdapter(EnumerateMetafileProc callback)
        {
            _callback = callback;
            _nativeCallback = CallBack;
            NativeCallback = Marshal.GetFunctionPointerForDelegate(_nativeCallback);
        }

        private BOOL CallBack(
            EmfPlusRecordType recordType,
            uint flags,
            uint dataSize,
            byte* data,
            void* callbackData)
        {
            return _callback(recordType, (int)flags, (int)dataSize, (nint)data, null);
        }

        public void Dispose()
        {
            GC.KeepAlive(_callback);
            GC.KeepAlive(_nativeCallback);
        }
    }
}
