// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal unsafe readonly struct EmfRecord
    {
        public Gdi32.HDC HDC { get; }
        private readonly Gdi32.HGDIOBJ* _lpht;
        private readonly Gdi32.ENHMETARECORD* _lpmr;
        private readonly int _nHandles;
        public IntPtr Data { get; }

        public EmfRecord(
            Gdi32.HDC hdc,
            Gdi32.HGDIOBJ* lpht,
            Gdi32.ENHMETARECORD* lpmr,
            int nHandles,
            IntPtr data)
        {
            HDC = hdc;
            _lpht = lpht;
            _lpmr = lpmr;
            _nHandles = nHandles;
            Data = data;
        }

        public Gdi32.EMR Type => _lpmr->iType;
        public ReadOnlySpan<uint> Params => _lpmr->dParm;
        public ReadOnlySpan<Gdi32.HGDIOBJ> Handles => new ReadOnlySpan<Gdi32.HGDIOBJ>(_lpht, _nHandles);

        public ENHMETAHEADER* HeaderRecord => Type == Gdi32.EMR.HEADER ? (ENHMETAHEADER*)_lpmr : null;
        public EMREXTSELECTCLIPRGN* ExtSelectClipRgnRecord
            => Type == Gdi32.EMR.EXTSELECTCLIPRGN ? (EMREXTSELECTCLIPRGN*)_lpmr : null;
        public EMRPOINTRECORD* SetViewportOrgExRecord
            => Type == Gdi32.EMR.SETVIEWPORTORGEX ? (EMRPOINTRECORD*)_lpmr : null;
        public EMRPOINTRECORD* SetBrushOrgExRecord
            => Type == Gdi32.EMR.SETBRUSHORGEX ? (EMRPOINTRECORD*)_lpmr : null;
        public EMRPOINTRECORD* SetWindowOrgExRecord
            => Type == Gdi32.EMR.SETWINDOWORGEX ? (EMRPOINTRECORD*)_lpmr : null;
        public EMRPOINTRECORD* OffsetClipRgnRecord
            => Type == Gdi32.EMR.OFFSETCLIPRGN ? (EMRPOINTRECORD*)_lpmr : null;
        public EMRPOINTRECORD* MoveToExRecord
            => Type == Gdi32.EMR.MOVETOEX ? (EMRPOINTRECORD*)_lpmr : null;
        public EMRPOINTRECORD* LineToRecord
            => Type == Gdi32.EMR.LINETO ? (EMRPOINTRECORD*)_lpmr : null;
        public EMRCREATEBRUSHINDIRECT* CreateBrushIndirectRecord
            => Type == Gdi32.EMR.CREATEBRUSHINDIRECT ? (EMRCREATEBRUSHINDIRECT*)_lpmr : null;
        public EMRENUMRECORD<Gdi32.R2>* SetROP2Record
            => Type == Gdi32.EMR.SETROP2 ? (EMRENUMRECORD<Gdi32.R2>*)_lpmr : null;
        public EMRENUMRECORD<Gdi32.BKMODE>* SetBkModeRecord
            => Type == Gdi32.EMR.SETBKMODE ? (EMRENUMRECORD<Gdi32.BKMODE>*)_lpmr : null;
        public EMRCREATEPEN* CreatePenRecord
            => Type == Gdi32.EMR.CREATEPEN ? (EMRCREATEPEN*)_lpmr : null;
        public EMREXTCREATEPEN* ExtCreatePenRecord
            => Type == Gdi32.EMR.EXTCREATEPEN ? (EMREXTCREATEPEN*)_lpmr : null;
        public EMRINDEXRECORD* SelectObjectRecord
            => Type == Gdi32.EMR.SELECTOBJECT ? (EMRINDEXRECORD*)_lpmr : null;
        public EMRINDEXRECORD* DeleteObjectRecord
            => Type == Gdi32.EMR.DELETEOBJECT ? (EMRINDEXRECORD*)_lpmr : null;
        public EMRBITBLT* BitBltRecord
            => Type == Gdi32.EMR.BITBLT ? (EMRBITBLT*)_lpmr : null;
        public EMRENUMRECORD<Gdi32.ICM>* SetIcmModeRecord
            => Type == Gdi32.EMR.SETICMMODE ? (EMRENUMRECORD<Gdi32.ICM>*)_lpmr : null;
        public EMRPOLY16* Polygon16Record
            => Type == Gdi32.EMR.POLYGON16 ? (EMRPOLY16*)_lpmr : null;
        public EMRPOLY16* Polyline16Record
            => Type == Gdi32.EMR.POLYLINE16 ? (EMRPOLY16*)_lpmr : null;
        public EMRPOLY16* PolyBezier16Record
            => Type == Gdi32.EMR.POLYBEZIER16 ? (EMRPOLY16*)_lpmr : null;
        public EMRPOLY16* PolylineTo16Record
            => Type == Gdi32.EMR.POLYLINETO16 ? (EMRPOLY16*)_lpmr : null;
        public EMRPOLY16* PolyBezierTo16Record
            => Type == Gdi32.EMR.POLYBEZIERTO16 ? (EMRPOLY16*)_lpmr : null;
        public EMRPOLYPOLY16* PolyPolyline16Record
            => Type == Gdi32.EMR.POLYPOLYLINE16 ? (EMRPOLYPOLY16*)_lpmr : null;
        public EMRPOLYPOLY16* PolyPolygon16Record
            => Type == Gdi32.EMR.POLYPOLYGON16 ? (EMRPOLYPOLY16*)_lpmr : null;
        public EMRSETWORLDTRANSFORM* SetWorldTransformRecord
            => Type == Gdi32.EMR.SETWORLDTRANSFORM ? (EMRSETWORLDTRANSFORM*)_lpmr : null;
        public EMRMODIFYWORLDTRANSFORM* ModifyWorldTransformRecord
            => Type == Gdi32.EMR.MODIFYWORLDTRANSFORM ? (EMRMODIFYWORLDTRANSFORM*)_lpmr : null;
        public EMRSETCOLOR* SetBkColorRecord
            => Type == Gdi32.EMR.SETBKCOLOR ? (EMRSETCOLOR*)_lpmr : null;
        public EMRSETCOLOR* SetTextColorRecord
            => Type == Gdi32.EMR.SETTEXTCOLOR ? (EMRSETCOLOR*)_lpmr : null;
        public EMRCREATEDIBPATTERNBRUSHPT* CreateDibPatternBrushPtRecord
            => Type == Gdi32.EMR.CREATEDIBPATTERNBRUSHPT ? (EMRCREATEDIBPATTERNBRUSHPT*)_lpmr : null;
        public EMRENUMRECORD<Gdi32.TA>* SetTextAlignRecord
            => Type == Gdi32.EMR.SETTEXTALIGN ? (EMRENUMRECORD<Gdi32.TA>*)_lpmr : null;
        public EMREXTCREATEFONTINDIRECTW* ExtCreateFontIndirectWRecord
            => Type == Gdi32.EMR.EXTCREATEFONTINDIRECTW ? (EMREXTCREATEFONTINDIRECTW*)_lpmr : null;
        public EMREXTTEXTOUTW* ExtTextOutWRecord
            => Type == Gdi32.EMR.EXTTEXTOUTW ? (EMREXTTEXTOUTW*)_lpmr : null;
        public EMRENUMRECORD<Gdi32.MM>* SetMapModeRecord
            => Type == Gdi32.EMR.SETMAPMODE ? (EMRENUMRECORD<Gdi32.MM>*)_lpmr : null;
        public EMRRECTRECORD* FillPathRecord
            => Type == Gdi32.EMR.FILLPATH ? (EMRRECTRECORD*)_lpmr : null;
        public EMRRECTRECORD* StrokeAndFillPathRecord
            => Type == Gdi32.EMR.STROKEANDFILLPATH ? (EMRRECTRECORD*)_lpmr : null;
        public EMRRECTRECORD* StrokePathRecord
            => Type == Gdi32.EMR.STROKEPATH ? (EMRRECTRECORD*)_lpmr : null;
        public EMRRECTRECORD* ExcludeClipRectRecord
            => Type == Gdi32.EMR.EXCLUDECLIPRECT ? (EMRRECTRECORD*)_lpmr : null;
        public EMRRECTRECORD* IntersetClipRectRecord
            => Type == Gdi32.EMR.INTERSECTCLIPRECT ? (EMRRECTRECORD*)_lpmr : null;
        public EMRRECTRECORD* EllipseRecord
            => Type == Gdi32.EMR.ELLIPSE ? (EMRRECTRECORD*)_lpmr : null;
        public EMRRECTRECORD* RectangleRecord
            => Type == Gdi32.EMR.RECTANGLE ? (EMRRECTRECORD*)_lpmr : null;
        public EMRRESTOREDC* RestoreDCRecord
            => Type == Gdi32.EMR.RESTOREDC ? (EMRRESTOREDC*)_lpmr : null;

        public override string ToString() => Type switch
        {
            // Note that not all records have special handling yet- we're filling these in as we go.
            Gdi32.EMR.HEADER => HeaderRecord->ToString(),
            Gdi32.EMR.EXTSELECTCLIPRGN => ExtSelectClipRgnRecord->ToString(),
            Gdi32.EMR.SETVIEWPORTORGEX => SetViewportOrgExRecord->ToString(),
            Gdi32.EMR.SETWINDOWORGEX => SetWindowOrgExRecord->ToString(),
            Gdi32.EMR.OFFSETCLIPRGN => OffsetClipRgnRecord->ToString(),
            Gdi32.EMR.MOVETOEX => MoveToExRecord->ToString(),
            Gdi32.EMR.LINETO => LineToRecord->ToString(),
            Gdi32.EMR.CREATEBRUSHINDIRECT => CreateBrushIndirectRecord->ToString(),
            Gdi32.EMR.SETBKMODE => SetBkModeRecord->ToString(),
            Gdi32.EMR.SETROP2 => SetROP2Record->ToString(),
            Gdi32.EMR.CREATEPEN => CreatePenRecord->ToString(),
            Gdi32.EMR.EXTCREATEPEN => ExtCreatePenRecord->ToString(),
            Gdi32.EMR.SELECTOBJECT => SelectObjectRecord->ToString(),
            Gdi32.EMR.DELETEOBJECT => DeleteObjectRecord->ToString(),
            Gdi32.EMR.BITBLT => BitBltRecord->ToString(),
            Gdi32.EMR.SETICMMODE => SetIcmModeRecord->ToString(),
            Gdi32.EMR.POLYLINE16 => Polyline16Record->ToString(),
            Gdi32.EMR.POLYBEZIER16 => PolyBezier16Record->ToString(),
            Gdi32.EMR.POLYGON16 => Polygon16Record->ToString(),
            Gdi32.EMR.POLYBEZIERTO16 => PolyBezierTo16Record->ToString(),
            Gdi32.EMR.POLYLINETO16 => PolylineTo16Record->ToString(),
            Gdi32.EMR.POLYPOLYGON16 => PolyPolygon16Record->ToString(),
            Gdi32.EMR.POLYPOLYLINE16 => PolyPolyline16Record->ToString(),
            Gdi32.EMR.SETWORLDTRANSFORM => SetWorldTransformRecord->ToString(),
            Gdi32.EMR.MODIFYWORLDTRANSFORM => ModifyWorldTransformRecord->ToString(),
            Gdi32.EMR.SETTEXTCOLOR => SetTextColorRecord->ToString(),
            Gdi32.EMR.SETBKCOLOR => SetBkColorRecord->ToString(),
            Gdi32.EMR.CREATEDIBPATTERNBRUSHPT => CreateDibPatternBrushPtRecord->ToString(),
            Gdi32.EMR.SETTEXTALIGN => SetTextAlignRecord->ToString(),
            Gdi32.EMR.EXTCREATEFONTINDIRECTW => ExtCreateFontIndirectWRecord->ToString(),
            Gdi32.EMR.EXTTEXTOUTW => ExtTextOutWRecord->ToString(),
            Gdi32.EMR.SETMAPMODE => SetMapModeRecord->ToString(),
            Gdi32.EMR.FILLPATH => FillPathRecord->ToString(),
            Gdi32.EMR.STROKEANDFILLPATH => StrokeAndFillPathRecord->ToString(),
            Gdi32.EMR.STROKEPATH => StrokePathRecord->ToString(),
            Gdi32.EMR.EXCLUDECLIPRECT => ExcludeClipRectRecord->ToString(),
            Gdi32.EMR.INTERSECTCLIPRECT => IntersetClipRectRecord->ToString(),
            Gdi32.EMR.ELLIPSE => EllipseRecord->ToString(),
            Gdi32.EMR.RECTANGLE => RectangleRecord->ToString(),
            Gdi32.EMR.RESTOREDC => RestoreDCRecord->ToString(),
            _ => $"[EMR{Type}]"
        };

        public string ToString(DeviceContextState state) => Type switch
        {
            Gdi32.EMR.POLYLINE16 => Polyline16Record->ToString(state),
            Gdi32.EMR.POLYGON16 => Polygon16Record->ToString(state),
            Gdi32.EMR.POLYPOLYGON16 => PolyPolygon16Record->ToString(state),
            Gdi32.EMR.POLYPOLYLINE16 => PolyPolyline16Record->ToString(state),
            _ => ToString()
        };
    }
}
