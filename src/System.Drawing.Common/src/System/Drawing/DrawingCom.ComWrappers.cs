// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Runtime.InteropServices;

namespace System.Drawing;

/// <summary>
///  The ComWrappers implementation for System.Drawing.Common's COM interop usages.
/// </summary>
/// <remarks>
///  <para>
///   Supports IStream and IPicture COM interfaces.
///  </para>
/// </remarks>
internal sealed unsafe partial class DrawingCom : ComWrappers
{
    private static Guid IID_IStream { get; } = new(0x0000000C, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

    internal static DrawingCom Instance { get; } = new();

    private DrawingCom() { }

    protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
    {
        count = 0;
        return null;
    }

    protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
    {
        Debug.Assert(flags == CreateObjectFlags.UniqueInstance);

        Guid pictureIID = IPicture.IID;
        int hr = Marshal.QueryInterface(externalComObject, in pictureIID, out IntPtr comObject);
        if (hr == HRESULT.S_OK)
        {
            return new PictureWrapper(comObject);
        }

        throw new NotImplementedException();
    }

    protected override void ReleaseObjects(IEnumerable objects)
    {
        throw new NotImplementedException();
    }

    internal static void ThrowExceptionForHR(int errorCode)
    {
        // Pass -1 for errorInfo to indicate that Windows' GetErrorInfo shouldn't be called, and only
        // throw the Exception corresponding to the specified errorCode.
        Marshal.ThrowExceptionForHR(errorCode, errorInfo: new IntPtr(-1));
    }

    internal interface IPicture : IDisposable
    {
#pragma warning disable IDE1006 // Naming Styles
        internal static Guid IID = new(0x7BF80980, 0xBF32, 0x101A, 0x8B, 0xBB, 0, 0xAA, 0x00, 0x30, 0x0C, 0xAB);
#pragma warning restore IDE1006 // Naming Styles

        // NOTE: Only SaveAsFile is invoked. The other methods on IPicture are not necessary

        int SaveAsFile(IntPtr pstm, int fSaveMemCopy, int* pcbSize);
    }

    private sealed class PictureWrapper : IPicture
    {
        private readonly IntPtr _wrappedInstance;

        public PictureWrapper(IntPtr wrappedInstance)
        {
            _wrappedInstance = wrappedInstance;
        }

        public void Dispose()
        {
            Marshal.Release(_wrappedInstance);
        }

        public unsafe int SaveAsFile(IntPtr pstm, int fSaveMemCopy, int* pcbSize)
        {
            // Get the IStream implementation, since the ComWrappers runtime returns a pointer to the IUnknown interface implementation
            Guid streamIID = IID_IStream;

            ThrowExceptionForHR(Marshal.QueryInterface(pstm, in streamIID, out IntPtr pstmImpl));

            try
            {
                return ((delegate* unmanaged<IntPtr, IntPtr, int, int*, int>)(*(*(void***)_wrappedInstance + 15 /* IPicture.SaveAsFile slot */)))
                    (_wrappedInstance, pstmImpl, fSaveMemCopy, pcbSize);
            }
            finally
            {
                Marshal.Release(pstmImpl);
            }
        }
    }
}
