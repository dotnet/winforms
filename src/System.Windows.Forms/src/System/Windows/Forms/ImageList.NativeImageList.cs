﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public sealed partial class ImageList
    {
        internal class NativeImageList : IDisposable, IHandle
        {
#if DEBUG
            private readonly string _callStack = new StackTrace().ToString();
#endif
            private const int GrowBy = 4;
            private const int InitialCapacity = 4;

            private static readonly object s_syncLock = new object();

            public NativeImageList(Ole32.IStream pstm)
            {
                IntPtr himl;
                lock (s_syncLock)
                {
                    himl = ComCtl32.ImageList.Read(pstm);
                    Init(himl);
                }
            }

            public NativeImageList(Size imageSize, ILC flags)
            {
                IntPtr himl;
                lock (s_syncLock)
                {
                    himl = ComCtl32.ImageList.Create(imageSize.Width, imageSize.Height, flags, InitialCapacity, GrowBy);
                    Init(himl);
                }
            }

            private NativeImageList(IntPtr himl)
            {
                Handle = himl;
            }

            private void Init(IntPtr himl)
            {
                if (himl != IntPtr.Zero)
                {
                    Handle = himl;
                    return;
                }

#if DEBUG
                Debug.Fail($"{nameof(NativeImageList)} could not be created. Originating stack:\n{_callStack}");
#endif
                throw new InvalidOperationException(SR.ImageListCreateFailed);
            }

            public IntPtr Handle { get; private set; }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                lock (s_syncLock)
                {
                    if (Handle == IntPtr.Zero)
                    {
                        return;
                    }

                    ComCtl32.ImageList.Destroy(Handle);
                    Handle = IntPtr.Zero;
                }
            }

            ~NativeImageList()
            {
                // There are certain code paths where we are unable to track the lifetime of the object,
                // for example in the following scenarios:
                //
                //      this.imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream"));
                // or
                //      resources.ApplyResources(this.listView1, "listView1");
                //
                // In those cases the loose instances will be collected by the GC.
                Dispose(false);
            }

            internal NativeImageList Duplicate()
            {
                lock (s_syncLock)
                {
                    IntPtr himl = ComCtl32.ImageList.Duplicate(Handle);
                    if (himl != IntPtr.Zero)
                    {
                        return new NativeImageList(himl);
                    }
                }

#if DEBUG
                Debug.Fail($"{nameof(NativeImageList)} could not be duplicated. Originating stack:\n{_callStack}");
#endif
                throw new InvalidOperationException(SR.ImageListDuplicateFailed);
            }
        }
    }
}
