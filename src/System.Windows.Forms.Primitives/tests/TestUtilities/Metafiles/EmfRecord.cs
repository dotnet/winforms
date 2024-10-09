// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable
using Windows.Win32.UI.ColorSystem;

namespace System.Windows.Forms.Metafiles;

internal readonly unsafe struct EmfRecord
{
    public HDC HDC { get; }
    private readonly HANDLETABLE* _lpht;
    private readonly ENHMETARECORD* _lpmr;
    private readonly int _nHandles;
    public LPARAM Data { get; }

    public EmfRecord(
        HDC hdc,
        HANDLETABLE* lpht,
        ENHMETARECORD* lpmr,
        int nHandles,
        LPARAM data)
    {
        HDC = hdc;
        _lpht = lpht;
        _lpmr = lpmr;
        _nHandles = nHandles;
        Data = data;
    }

    public ENHANCED_METAFILE_RECORD_TYPE Type => _lpmr->iType;
    public ReadOnlySpan<uint> Params => _lpmr->dParm.AsSpan((int)(_lpmr->nSize / sizeof(uint)) - 2);
    public ReadOnlySpan<HGDIOBJ> Handles => new(_lpht, _nHandles);

    public ENHMETAHEADER* HeaderRecord => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_HEADER ? (ENHMETAHEADER*)_lpmr : null;
    public EMREXTSELECTCLIPRGN* ExtSelectClipRgnRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTSELECTCLIPRGN ? (EMREXTSELECTCLIPRGN*)_lpmr : null;
    public EMRPOINTRECORD* SetViewportOrgExRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SETVIEWPORTORGEX ? (EMRPOINTRECORD*)_lpmr : null;
    public EMRPOINTRECORD* SetBrushOrgExRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SETBRUSHORGEX ? (EMRPOINTRECORD*)_lpmr : null;
    public EMRPOINTRECORD* SetWindowOrgExRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SETWINDOWORGEX ? (EMRPOINTRECORD*)_lpmr : null;
    public EMRPOINTRECORD* OffsetClipRgnRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_OFFSETCLIPRGN ? (EMRPOINTRECORD*)_lpmr : null;
    public EMRPOINTRECORD* MoveToExRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_MOVETOEX ? (EMRPOINTRECORD*)_lpmr : null;
    public EMRPOINTRECORD* LineToRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_LINETO ? (EMRPOINTRECORD*)_lpmr : null;
    public EMRCREATEBRUSHINDIRECT* CreateBrushIndirectRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEBRUSHINDIRECT ? (EMRCREATEBRUSHINDIRECT*)_lpmr : null;
    public EMRENUMRECORD<R2_MODE>* SetROP2Record
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SETROP2 ? (EMRENUMRECORD<R2_MODE>*)_lpmr : null;
    public EMRENUMRECORD<BACKGROUND_MODE>* SetBkModeRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SETBKMODE ? (EMRENUMRECORD<BACKGROUND_MODE>*)_lpmr : null;
    public EMRCREATEPEN* CreatePenRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEPEN ? (EMRCREATEPEN*)_lpmr : null;
    public EMREXTCREATEPEN* ExtCreatePenRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTCREATEPEN ? (EMREXTCREATEPEN*)_lpmr : null;
    public EMRINDEXRECORD* SelectObjectRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SELECTOBJECT ? (EMRINDEXRECORD*)_lpmr : null;
    public EMRINDEXRECORD* DeleteObjectRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_DELETEOBJECT ? (EMRINDEXRECORD*)_lpmr : null;
    public EMRBITBLT* BitBltRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_BITBLT ? (EMRBITBLT*)_lpmr : null;
    public EMRENUMRECORD<ICM_MODE>* SetIcmModeRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SETICMMODE ? (EMRENUMRECORD<ICM_MODE>*)_lpmr : null;
    public EMRPOLY16* Polygon16Record
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYGON16 ? (EMRPOLY16*)_lpmr : null;
    public EMRPOLY16* Polyline16Record
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYLINE16 ? (EMRPOLY16*)_lpmr : null;
    public EMRPOLY16* PolyBezier16Record
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYBEZIER16 ? (EMRPOLY16*)_lpmr : null;
    public EMRPOLY16* PolylineTo16Record
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYLINETO16 ? (EMRPOLY16*)_lpmr : null;
    public EMRPOLY16* PolyBezierTo16Record
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYBEZIERTO16 ? (EMRPOLY16*)_lpmr : null;
    public EMRPOLYPOLY16* PolyPolyline16Record
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYLINE16 ? (EMRPOLYPOLY16*)_lpmr : null;
    public EMRPOLYPOLY16* PolyPolygon16Record
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYGON16 ? (EMRPOLYPOLY16*)_lpmr : null;
    public EMRSETWORLDTRANSFORM* SetWorldTransformRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SETWORLDTRANSFORM ? (EMRSETWORLDTRANSFORM*)_lpmr : null;
    public EMRMODIFYWORLDTRANSFORM* ModifyWorldTransformRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_MODIFYWORLDTRANSFORM ? (EMRMODIFYWORLDTRANSFORM*)_lpmr : null;
    public EMRSETCOLOR* SetBkColorRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SETBKCOLOR ? (EMRSETCOLOR*)_lpmr : null;
    public EMRSETCOLOR* SetTextColorRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SETTEXTCOLOR ? (EMRSETCOLOR*)_lpmr : null;
    public EMRCREATEDIBPATTERNBRUSHPT* CreateDibPatternBrushPtRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEDIBPATTERNBRUSHPT ? (EMRCREATEDIBPATTERNBRUSHPT*)_lpmr : null;
    public EMRENUMRECORD<TEXT_ALIGN_OPTIONS>* SetTextAlignRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SETTEXTALIGN ? (EMRENUMRECORD<TEXT_ALIGN_OPTIONS>*)_lpmr : null;
    public EMREXTCREATEFONTINDIRECTW* ExtCreateFontIndirectWRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTCREATEFONTINDIRECTW ? (EMREXTCREATEFONTINDIRECTW*)_lpmr : null;
    public EMREXTTEXTOUTW* ExtTextOutWRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTTEXTOUTW ? (EMREXTTEXTOUTW*)_lpmr : null;
    public EMRENUMRECORD<HDC_MAP_MODE>* SetMapModeRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_SETMAPMODE ? (EMRENUMRECORD<HDC_MAP_MODE>*)_lpmr : null;
    public EMRRECTRECORD* FillPathRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_FILLPATH ? (EMRRECTRECORD*)_lpmr : null;
    public EMRRECTRECORD* StrokeAndFillPathRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_STROKEANDFILLPATH ? (EMRRECTRECORD*)_lpmr : null;
    public EMRRECTRECORD* StrokePathRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_STROKEPATH ? (EMRRECTRECORD*)_lpmr : null;
    public EMRRECTRECORD* ExcludeClipRectRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_EXCLUDECLIPRECT ? (EMRRECTRECORD*)_lpmr : null;
    public EMRRECTRECORD* IntersetClipRectRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_INTERSECTCLIPRECT ? (EMRRECTRECORD*)_lpmr : null;
    public EMRRECTRECORD* EllipseRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_ELLIPSE ? (EMRRECTRECORD*)_lpmr : null;
    public EMRRECTRECORD* RectangleRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_RECTANGLE ? (EMRRECTRECORD*)_lpmr : null;
    public EMRRESTOREDC* RestoreDCRecord
        => Type == ENHANCED_METAFILE_RECORD_TYPE.EMR_RESTOREDC ? (EMRRESTOREDC*)_lpmr : null;

    public override string ToString() => Type switch
    {
        // Note that not all records have special handling yet- we're filling these in as we go.
        ENHANCED_METAFILE_RECORD_TYPE.EMR_HEADER => HeaderRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTSELECTCLIPRGN => ExtSelectClipRgnRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_SETVIEWPORTORGEX => SetViewportOrgExRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_SETWINDOWORGEX => SetWindowOrgExRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_OFFSETCLIPRGN => OffsetClipRgnRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_MOVETOEX => MoveToExRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_LINETO => LineToRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEBRUSHINDIRECT => CreateBrushIndirectRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_SETBKMODE => SetBkModeRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_SETROP2 => SetROP2Record->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEPEN => CreatePenRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTCREATEPEN => ExtCreatePenRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_SELECTOBJECT => SelectObjectRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_DELETEOBJECT => DeleteObjectRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_BITBLT => BitBltRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_SETICMMODE => SetIcmModeRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYLINE16 => Polyline16Record->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYBEZIER16 => PolyBezier16Record->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYGON16 => Polygon16Record->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYBEZIERTO16 => PolyBezierTo16Record->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYLINETO16 => PolylineTo16Record->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYGON16 => PolyPolygon16Record->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYLINE16 => PolyPolyline16Record->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_SETWORLDTRANSFORM => SetWorldTransformRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_MODIFYWORLDTRANSFORM => ModifyWorldTransformRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_SETTEXTCOLOR => SetTextColorRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_SETBKCOLOR => SetBkColorRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEDIBPATTERNBRUSHPT => CreateDibPatternBrushPtRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_SETTEXTALIGN => SetTextAlignRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTCREATEFONTINDIRECTW => ExtCreateFontIndirectWRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTTEXTOUTW => ExtTextOutWRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_SETMAPMODE => SetMapModeRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_FILLPATH => FillPathRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_STROKEANDFILLPATH => StrokeAndFillPathRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_STROKEPATH => StrokePathRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_EXCLUDECLIPRECT => ExcludeClipRectRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_INTERSECTCLIPRECT => IntersetClipRectRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_ELLIPSE => EllipseRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_RECTANGLE => RectangleRecord->ToString(),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_RESTOREDC => RestoreDCRecord->ToString(),
        _ => $"[EMR{Type}]"
    };

    public string ToString(DeviceContextState state) => Type switch
    {
        ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYLINE16 => Polyline16Record->ToString(state),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYGON16 => Polygon16Record->ToString(state),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYGON16 => PolyPolygon16Record->ToString(state),
        ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYLINE16 => PolyPolyline16Record->ToString(state),
        _ => ToString()
    };
}
