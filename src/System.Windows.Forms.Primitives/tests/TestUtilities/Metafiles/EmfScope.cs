// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal class EmfScope : DisposalTracking.Tracker, IDisposable
    {
        public Gdi32.HDC HDC { get; }
        private Gdi32.HENHMETAFILE _hmf;

        public unsafe EmfScope()
            : this (Gdi32.CreateEnhMetaFileW(hdc: default, lpFilename: null, lprc: null, lpDesc: null))
        {
        }

        public EmfScope(Gdi32.HDC hdc)
        {
            HDC = hdc;
            _hmf = default;
        }

        public unsafe static EmfScope Create() => new EmfScope();

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

        /// <summary>
        ///  Allows enumerating the metafile records while tracking state. <paramref name="state"/> should be
        ///  initialized to the metafile DC state before any drawing has begun.
        /// </summary>
        /// <remarks>
        ///  State is whatever is current *before* the current record is "applied" as it is necessary to understand
        ///  what delta the actual record makes.
        /// </remarks>
        public unsafe void EnumerateWithState(ProcessRecordWithStateDelegate enumerator, DeviceContextState state)
        {
            Enumerate(stateTracker);

            bool stateTracker(ref EmfRecord record)
            {
                bool result = enumerator(ref record, state);

                // This must come *after* calling the nested enumerator so that the record reflects what is *about*
                // to be applied. If we invert the model you wouldn't be able to tell what things like LineTo actually
                // do as they only contain the destination point.
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
                    case Gdi32.EMR.SETROP2:
                        state.Rop2Mode = record.SetROP2Record->iMode;
                        break;
                    case Gdi32.EMR.SETTEXTCOLOR:
                        state.TextColor = record.SetTextColorRecord->crColor;
                        break;
                    case Gdi32.EMR.SETBKCOLOR:
                        state.BackColor = record.SetBkColorRecord->crColor;
                        break;
                    case Gdi32.EMR.MOVETOEX:
                        state.BrushOrigin = record.MoveToExRecord->point;
                        // The documentation indicates that the last MoveTo will be where CloseFigure draws a line to.
                        state.LastBeginPathBrushOrigin = state.BrushOrigin;
                        break;
                    case Gdi32.EMR.LINETO:
                        state.BrushOrigin = record.LineToRecord->point;
                        break;
                    case Gdi32.EMR.BEGINPATH:
                        state.LastBeginPathBrushOrigin = state.BrushOrigin;
                        state.InPath = true;
                        break;
                    case Gdi32.EMR.ABORTPATH:
                    case Gdi32.EMR.ENDPATH:
                    case Gdi32.EMR.CLOSEFIGURE:
                        state.InPath = false;
                        break;
                    case Gdi32.EMR.EXTCREATEFONTINDIRECTW:
                    case Gdi32.EMR.CREATEPALETTE:
                    case Gdi32.EMR.CREATEPEN:
                    case Gdi32.EMR.EXTCREATEPEN:
                    case Gdi32.EMR.CREATEMONOBRUSH:
                    case Gdi32.EMR.CREATEBRUSHINDIRECT:
                    case Gdi32.EMR.CREATEDIBPATTERNBRUSHPT:
                        // All of these records have their index as the first "parameter".
                        state.AddGdiObject(ref record, (int)record.Params[0]);
                        break;
                    case Gdi32.EMR.SELECTOBJECT:
                        state.SelectGdiObject(record.SelectObjectRecord);
                        break;
                    case Gdi32.EMR.DELETEOBJECT:
                        state.GdiObjects[(int)record.DeleteObjectRecord->index] = default;
                        break;
                    case Gdi32.EMR.SETWORLDTRANSFORM:
                        state.Transform = record.SetWorldTransformRecord->xform;
                        break;
                    case Gdi32.EMR.MODIFYWORLDTRANSFORM:
                        var transform = record.ModifyWorldTransformRecord;
                        switch (transform->iMode)
                        {
                            case Gdi32.MWT.IDENTITY:
                                state.Transform = Matrix3x2.Identity;
                                break;
                            case Gdi32.MWT.LEFTMULTIPLY:
                                state.Transform = transform->xform * state.Transform;
                                break;
                            case Gdi32.MWT.RIGHTMULTIPLY:
                                state.Transform = state.Transform * transform->xform;
                                break;
                        }
                        break;
                    case Gdi32.EMR.SAVEDC:
                        state.SaveDC();
                        break;
                    case Gdi32.EMR.RESTOREDC:
                        state.RestoreDC(record.RestoreDCRecord->iRelative);
                        break;
                }

                return result;
            }
        }

        public string RecordsToString()
        {
            StringBuilder sb = new StringBuilder(1024);
            Enumerate((ref EmfRecord record) =>
            {
                sb.AppendLine(record.ToString());
                return true;
            });

            return sb.ToString();
        }

        public string RecordsToStringWithState(DeviceContextState state)
        {
            StringBuilder sb = new StringBuilder(1024);
            EnumerateWithState((ref EmfRecord record, DeviceContextState state) =>
            {
                sb.AppendLine(record.ToString(state));
                return true;
            },
            state);

            return sb.ToString();
        }

        private static unsafe BOOL CallBack(
            Gdi32.HDC hdc,
            Gdi32.HGDIOBJ* lpht,
            Gdi32.ENHMETARECORD* lpmr,
            int nHandles,
            IntPtr data)
        {
            // Note that the record pointer is *only* valid during the callback.
            GCHandle enumeratorHandle = GCHandle.FromIntPtr(data);
            ProcessRecordDelegate enumerator = (ProcessRecordDelegate)enumeratorHandle.Target!;
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

            GC.SuppressFinalize(this);
        }
    }
}
