// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public sealed partial class ImageList
    {
        internal class NativeImageList : IDisposable, IHandle
        {
            private IntPtr himl;
#if DEBUG
            private readonly string callStack;
#endif

            internal NativeImageList(IntPtr himl)
            {
                this.himl = himl;
#if DEBUG
                callStack = Environment.StackTrace;
#endif
            }

            public IntPtr Handle
            {
                get
                {
                    return himl;
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void Dispose(bool disposing)
            {
                if (himl != IntPtr.Zero)
                {
                    ComCtl32.ImageList.Destroy(himl);
                    himl = IntPtr.Zero;
                }
            }

            ~NativeImageList()
            {
                Dispose(false);
            }
        }
    }
}
