// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        private class PictureWrapper : Ole32.IPicture, Ole32.IPersistStream, IDisposable
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

            public int Handle
            {
                get
                {
                    int retVal;
                    int errorCode = ((delegate* unmanaged<IntPtr, int*, int>)(*(*(void***)_wrappedInstance + 3 /* IPicture.get_Handle slot */)))
                        (_wrappedInstance, &retVal);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    return retVal;
                }
            }

            public int hPal
            {
                get
                {
                    int retVal;
                    int errorCode = ((delegate* unmanaged<IntPtr, int*, int>)(*(*(void***)_wrappedInstance + 4 /* IPicture.get_hPal slot */)))
                        (_wrappedInstance, &retVal);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    return retVal;
                }
            }

            public short Type
            {
                get
                {
                    short retVal;
                    int errorCode = ((delegate* unmanaged<IntPtr, short*, int>)(*(*(void***)_wrappedInstance + 5 /* IPicture.get_Type slot */)))
                        (_wrappedInstance, &retVal);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    return retVal;
                }
            }

            public int Width
            {
                get
                {
                    int retVal;
                    int errorCode = ((delegate* unmanaged<IntPtr, int*, int>)(*(*(void***)_wrappedInstance + 6 /* IPicture.get_Width slot */)))
                        (_wrappedInstance, &retVal);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    return retVal;
                }
            }

            public int Height
            {
                get
                {
                    int retVal;
                    int errorCode = ((delegate* unmanaged<IntPtr, int*, int>)(*(*(void***)_wrappedInstance + 7 /* IPicture.get_Height slot */)))
                        (_wrappedInstance, &retVal);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    return retVal;
                }
            }

            public HRESULT GetClassID(
                Guid* pClassID)
            {
                throw new NotImplementedException();
            }

            public HRESULT IsDirty()
            {
                throw new NotImplementedException();
            }

            public void Load(Interop.Ole32.IStream pstm)
            {
                Guid persistedStreamIID = IID.IPersistStream;
                Guid streamIID = IID.IStream;
                IntPtr streamUnkPtr = IntPtr.Zero;
                IntPtr streamPtr = IntPtr.Zero;
                IntPtr persistedStreamPtr = IntPtr.Zero;

                try
                {
                    int errorCode = Marshal.QueryInterface(_wrappedInstance, ref persistedStreamIID, out persistedStreamPtr);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    streamUnkPtr = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(pstm, CreateComInterfaceFlags.None);
                    errorCode = Marshal.QueryInterface(streamUnkPtr, ref streamIID, out streamPtr);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    errorCode = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)persistedStreamPtr + 5 /* IPersistStream.Load slot */)))
                        (persistedStreamPtr, streamPtr);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }
                }
                finally
                {
                    if (streamPtr != IntPtr.Zero)
                    {
                        Marshal.Release(streamPtr);
                    }

                    if (streamUnkPtr != IntPtr.Zero)
                    {
                        int count = Marshal.Release(streamUnkPtr);
                        Debug.Assert(count == 0, $"streamUnkPtr = {count}");
                    }

                    if (persistedStreamPtr != IntPtr.Zero)
                    {
                        Marshal.Release(persistedStreamPtr);
                    }
                }
            }

            public void Save(Interop.Ole32.IStream pstm, Interop.BOOL fClearDirty)
            {
                throw new NotImplementedException();
            }

            public long GetSizeMax()
            {
                throw new NotImplementedException();
            }
        }
    }
}
