// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms.Metafiles;

internal class EmfScope :
#if DEBUG
    DisposalTracking.Tracker,
#endif
    IDisposable
{
    public HDC HDC { get; }
    private HENHMETAFILE _hemf;

    public unsafe EmfScope()
        : this(CreateEnhMetaFile())
    {
    }

    public EmfScope(HDC hdc)
    {
        HDC = hdc;
        _hemf = default;
    }

    public EmfScope(HENHMETAFILE hemf)
    {
        _hemf = hemf;
    }

    private static unsafe HDC CreateEnhMetaFile(
        HDC hdc = default,
        string? lpFilename = null,
        RECT* lprc = null,
        string? lpDesc = null)
    {
        fixed (char* pFileName = lpFilename)
        fixed (char* pDesc = lpDesc)
        {
            HDC metafileHdc = PInvoke.CreateEnhMetaFile(hdc, pFileName, lprc, pDesc);
            if (metafileHdc.IsNull)
            {
                throw new Win32Exception("Could not create metafile");
            }

            return metafileHdc;
        }
    }

    public static unsafe EmfScope Create() => new();

    public HENHMETAFILE HENHMETAFILE
    {
        get
        {
            if (_hemf.IsNull)
            {
                if (HDC.IsNull)
                {
                    return default;
                }

                _hemf = PInvoke.CloseEnhMetaFile(HDC);
            }

            return _hemf;
        }
    }

    public unsafe void Enumerate(ProcessRecordDelegate enumerator)
    {
        GCHandle enumeratorHandle = GCHandle.Alloc(enumerator);
        try
        {
            IntPtr callback = Marshal.GetFunctionPointerForDelegate(CallBack);
            PInvoke.EnumEnhMetaFile(
                default,
                HENHMETAFILE,
                (delegate* unmanaged[Stdcall]<HDC, HANDLETABLE*, ENHMETARECORD*, int, LPARAM, int>)callback,
                (void*)(nint)enumeratorHandle,
                (RECT*)null);
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
    ///  <para>
    ///   State is whatever is current *before* the current record is "applied" as it is necessary to understand
    ///   what delta the actual record makes.
    ///  </para>
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
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_SETTEXTALIGN:
                    state.TextAlign = record.SetTextAlignRecord->iMode;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_SETMAPMODE:
                    state.MapMode = record.SetMapModeRecord->iMode;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_SETBKMODE:
                    state.BackgroundMode = record.SetBkModeRecord->iMode;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_SETROP2:
                    state.Rop2Mode = record.SetROP2Record->iMode;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_SETTEXTCOLOR:
                    state.TextColor = record.SetTextColorRecord->crColor;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_SETBKCOLOR:
                    state.BackColor = record.SetBkColorRecord->crColor;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_MOVETOEX:
                    state.BrushOrigin = record.MoveToExRecord->point;
                    // The documentation indicates that the last MoveTo will be where CloseFigure draws a line to.
                    state.LastBeginPathBrushOrigin = state.BrushOrigin;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_LINETO:
                    state.BrushOrigin = record.LineToRecord->point;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_BEGINPATH:
                    state.LastBeginPathBrushOrigin = state.BrushOrigin;
                    state.InPath = true;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_ABORTPATH:
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_ENDPATH:
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_CLOSEFIGURE:
                    state.InPath = false;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTCREATEFONTINDIRECTW:
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEPALETTE:
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEPEN:
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTCREATEPEN:
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEMONOBRUSH:
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEBRUSHINDIRECT:
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEDIBPATTERNBRUSHPT:
                    // All of these records have their index as the first "parameter".
                    state.AddGdiObject(ref record, (int)record.Params[0]);
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_SELECTOBJECT:
                    state.SelectGdiObject(record.SelectObjectRecord);
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_DELETEOBJECT:
                    state.GdiObjects[(int)record.DeleteObjectRecord->index] = default;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTSELECTCLIPRGN:
                    state.ClipRegion = record.ExtSelectClipRgnRecord->ClippingRectangles;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_SETWORLDTRANSFORM:
                    state.Transform = record.SetWorldTransformRecord->xform;
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_MODIFYWORLDTRANSFORM:
                    var transform = record.ModifyWorldTransformRecord;
                    switch (transform->iMode)
                    {
                        case MODIFY_WORLD_TRANSFORM_MODE.MWT_IDENTITY:
                            state.Transform = Matrix3x2.Identity;
                            break;
                        case MODIFY_WORLD_TRANSFORM_MODE.MWT_LEFTMULTIPLY:
                            state.Transform = transform->xform * state.Transform;
                            break;
                        case MODIFY_WORLD_TRANSFORM_MODE.MWT_RIGHTMULTIPLY:
                            state.Transform = state.Transform * transform->xform;
                            break;
                    }

                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_SAVEDC:
                    state.SaveDC();
                    break;
                case ENHANCED_METAFILE_RECORD_TYPE.EMR_RESTOREDC:
                    state.RestoreDC(record.RestoreDCRecord->iRelative);
                    break;
            }

            return result;
        }
    }

    public string RecordsToString()
    {
        StringBuilder sb = new(1024);
        Enumerate((ref EmfRecord record) =>
        {
            sb.AppendLine(record.ToString());
            return true;
        });

        return sb.ToString();
    }

    public string RecordsToStringWithState(DeviceContextState state)
    {
        StringBuilder sb = new(1024);
        EnumerateWithState((ref EmfRecord record, DeviceContextState state) =>
        {
            sb.AppendLine(record.ToString(state));
            return true;
        },
        state);

        return sb.ToString();
    }

    private static unsafe BOOL CallBack(
        HDC hdc,
        HANDLETABLE* lpht,
        ENHMETARECORD* lpmr,
        int nHandles,
        LPARAM data)
    {
        // Note that the record pointer is *only* valid during the callback.
        GCHandle enumeratorHandle = GCHandle.FromIntPtr(data);
        ProcessRecordDelegate enumerator = (ProcessRecordDelegate)enumeratorHandle.Target!;
        EmfRecord record = new(hdc, lpht, lpmr, nHandles, data);
        return enumerator(ref record);
    }

    public static implicit operator HDC(in EmfScope scope) => scope.HDC;

    public void Dispose()
    {
        if (!HDC.IsNull)
        {
            PInvoke.DeleteEnhMetaFile(HENHMETAFILE);
        }

        GC.SuppressFinalize(this);
    }
}
