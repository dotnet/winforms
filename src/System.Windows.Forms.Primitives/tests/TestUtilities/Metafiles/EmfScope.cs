// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        public delegate bool ProcessRecordWithStateDelegate(ref EmfRecord record, DeviceContextState state);

        /// <summary>
        ///  Allows enumerating the metafile records while tracking state. <paramref name="state"/> should be
        ///  initialized to the metafile DC state before any drawing has begun.
        /// </summary>
        public unsafe void EnumerateWithState(ProcessRecordWithStateDelegate enumerator, DeviceContextState state)
        {
            List<EmfRecord> gdiObjects = new List<EmfRecord>();
            Enumerate(stateTracker);

            bool stateTracker(ref EmfRecord record)
            {
                int index;
                switch (record.Type)
                {
                    // Not all records are handled yet. Backfilling in as we write specific tests.
                    case Gdi32.EMR.SETTEXTALIGN:
                        state.TextAlign = record.SetTextAlignRecord->iMode;
                        break;
                    case Gdi32.EMR.SETMAPMODE:
                        state.MapMode = record.SetMapModeRecord->iMode;
                        break;
                    case Gdi32.EMR.SETBKMODE:
                        state.BackgroundMode = record.SetBkModeRecord->iMode;
                        break;
                    case Gdi32.EMR.SETTEXTCOLOR:
                        state.TextColor = record.SetTextColorRecord->crColor;
                        break;
                    case Gdi32.EMR.SETBKCOLOR:
                        state.BackColor = record.SetBkColorRecord->crColor;
                        break;
                    case Gdi32.EMR.MOVETOEX:
                        state.BrushOrigin = record.MoveToExRecord->point;
                        break;
                    case Gdi32.EMR.EXTCREATEFONTINDIRECTW:
                        index = (int)record.ExtCreateFontIndirectWRecord->ihFont;
                        AddGdiObject(ref record, index);
                        break;
                    case Gdi32.EMR.SELECTOBJECT:
                        SelectGdiObject((int)record.SelectObjectRecord->index);
                        break;
                }

                return enumerator(ref record, state);
            }

            void AddGdiObject(ref EmfRecord record, int index)
            {
                if (gdiObjects.Capacity <= index)
                {
                    gdiObjects.Capacity = index + 1;
                }

                while (gdiObjects.Count <= index)
                {
                    gdiObjects.Add(default);
                }

                gdiObjects[index] = record;
            }

            void SelectGdiObject(int index)
            {
                // WARNING: You can not use fields that index out of the struct's contents here.

                // Not all records are handled yet. Backfilling in as we write specific tests.
                EmfRecord record = gdiObjects[index];
                switch (record.Type)
                {
                    case Gdi32.EMR.EXTCREATEFONTINDIRECTW:
                        state.SelectedFont = record.ExtCreateFontIndirectWRecord->elfw.elfLogFont.FaceName.ToString();
                        break;
                }
            }
        }

        public List<string> RecordsToString()
        {
            var strings = new List<string>();
            Enumerate((ref EmfRecord record) =>
            {
                strings.Add(record.ToString());
                return true;
            });

            return strings;
        }

        private static unsafe BOOL CallBack(
            Gdi32.HDC hdc,
            Gdi32.HGDIOBJ* lpht,
            Gdi32.ENHMETARECORD* lpmr,
            int nHandles,
            IntPtr data)
        {
            // Note that the record pointer is *only* valid during the callback
            GCHandle enumeratorHandle = GCHandle.FromIntPtr(data);
            ProcessRecordDelegate enumerator = enumeratorHandle.Target as ProcessRecordDelegate;
            var record = new EmfRecord(hdc, lpht, lpmr, nHandles, data);
            return enumerator(ref record).ToBOOL();
        }

        public static implicit operator Gdi32.HDC(in EmfScope scope) => scope.HDC;

        public void Dispose()
        {
            if (!HDC.IsNull)
            {
                Gdi32.DeleteEnhMetaFile(HENHMETAFILE);
            }
        }
    }
}
