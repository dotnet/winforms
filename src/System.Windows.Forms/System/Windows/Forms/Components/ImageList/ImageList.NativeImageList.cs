// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Com;

namespace System.Windows.Forms;

public sealed partial class ImageList
{
    internal sealed class NativeImageList : IDisposable, IHandle<HIMAGELIST>
    {
#if DEBUG
        private readonly string _callStack = new StackTrace().ToString();
#endif
        private const int GrowBy = 4;
        private const int InitialCapacity = 4;

        private static readonly Lock s_syncLock = new();

        public unsafe NativeImageList(IStream.Interface pstm)
        {
            HIMAGELIST himl;
            lock (s_syncLock)
            {
                using var stream = ComHelpers.TryGetComScope<IStream>(pstm, out HRESULT hr);
                Debug.Assert(hr.Succeeded);
                himl = PInvoke.ImageList_Read(stream);
                Init(himl);
            }
        }

        public NativeImageList(Size imageSize, IMAGELIST_CREATION_FLAGS flags)
        {
            HIMAGELIST himl;
            lock (s_syncLock)
            {
                himl = PInvoke.ImageList_Create(imageSize.Width, imageSize.Height, flags, InitialCapacity, GrowBy);
                Init(himl);
            }
        }

        private NativeImageList(HIMAGELIST himl)
        {
            HIMAGELIST = himl;
        }

        private void Init(HIMAGELIST himl)
        {
            if (!himl.IsNull)
            {
                HIMAGELIST = himl;
                return;
            }

#if DEBUG
            Debug.Fail($"{nameof(NativeImageList)} could not be created. Originating stack:\n{_callStack}");
#endif
            throw new InvalidOperationException(SR.ImageListCreateFailed);
        }

        HIMAGELIST IHandle<HIMAGELIST>.Handle => HIMAGELIST;

        internal HIMAGELIST HIMAGELIST { get; private set; }

        public void Dispose()
        {
            lock (s_syncLock)
            {
                if (HIMAGELIST.IsNull)
                {
                    return;
                }

                PInvoke.ImageList.Destroy(this);
                HIMAGELIST = HIMAGELIST.Null;
            }

            GC.SuppressFinalize(this);
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
            Dispose();
        }

        internal NativeImageList Duplicate()
        {
            lock (s_syncLock)
            {
                HIMAGELIST himl = PInvoke.ImageList_Duplicate(HIMAGELIST);
                if (!HIMAGELIST.IsNull)
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
