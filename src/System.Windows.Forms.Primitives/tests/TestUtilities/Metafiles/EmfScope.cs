// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal ref struct EmfScope
    {
        public Gdi32.HDC HDC { get; }
        private Gdi32.HENHMETAFILE _hmf;

        public EmfScope(Gdi32.HDC hdc)
        {
            HDC = hdc;
            _hmf = default;
        }

        public unsafe static EmfScope Create()
            => new EmfScope(Gdi32.CreateEnhMetaFileW(default, null, null, null));

        public Gdi32.HENHMETAFILE HENHMETAFILE
        {
            get
            {
                if (HDC.IsNull)
                {
                    return default;
                }

                if (_hmf.IsNull)
                {
                    _hmf = Gdi32.CloseEnhMetaFile(HDC);
                }

                return _hmf;
            }
        }

        public delegate bool ProcessRecordDelegate(ref EmfRecord record);

        public unsafe void Enumerate(ProcessRecordDelegate enumerator)
        {
            GCHandle enumeratorHandle = GCHandle.Alloc(enumerator);
            try
            {
                IntPtr callback = Marshal.GetFunctionPointerForDelegate<Gdi32.Enhmfenumproc>(CallBack);
                Gdi32.EnumEnhMetaFile(default, HENHMETAFILE, CallBack, (IntPtr)enumeratorHandle, null);
            }
            finally
            {
                if (enumeratorHandle.IsAllocated)
                {
                    enumeratorHandle.Free();
                }
            }
        }

        private static unsafe BOOL CallBack(
            Gdi32.HDC hdc,
            Gdi32.HGDIOBJ* lpht,
            Gdi32.ENHMETARECORD* lpmr,
            int nHandles,
            IntPtr data)
        {
            GCHandle enumeratorHandle = GCHandle.FromIntPtr(data);
            ProcessRecordDelegate enumerator = enumeratorHandle.Target as ProcessRecordDelegate;
            var record = new EmfRecord(hdc, lpht, lpmr, nHandles, data);
            return enumerator(ref record).ToBOOL();
        }

        public void Dispose()
        {
            if (!HDC.IsNull)
            {
                Gdi32.DeleteEnhMetaFile(HENHMETAFILE);
            }
        }
    }
}
