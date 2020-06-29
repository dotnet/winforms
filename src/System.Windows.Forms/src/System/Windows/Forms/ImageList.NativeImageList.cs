// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms
{
    public sealed partial class ImageList
    {
        internal class NativeImageList : IDisposable, IHandle
        {
#if DEBUG
            private readonly string _callStack;
#endif

            internal NativeImageList(IntPtr himl)
            {
                Handle = himl;
#if DEBUG
                _callStack = Environment.StackTrace;
#endif
            }

            public IntPtr Handle { get; private set; }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void Dispose(bool disposing)
            {
                if (Handle != IntPtr.Zero)
                {
                    ComCtl32.ImageList.Destroy(Handle);
                    Handle = IntPtr.Zero;
                }

#if DEBUG
                GC.SuppressFinalize(this);
#endif
            }

            ~NativeImageList()
            {
#if DEBUG
                Debug.Fail($"{nameof(NativeImageList)} was not disposed properly. Originating stack:\n{_callStack}");
#endif

                Dispose(false);
            }
        }
    }
}
