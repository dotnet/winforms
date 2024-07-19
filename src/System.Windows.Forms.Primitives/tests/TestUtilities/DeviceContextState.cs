// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Numerics;
using System.Windows.Forms.Metafiles;

namespace System;

/// <summary>
///  Holder for tracking HDC state.
/// </summary>
internal unsafe class DeviceContextState
{
    // Not all state is handled yet. Backfilling in as we write specific tests. Of special note is that we don't
    // have tracking for Save/RestoreDC yet.

    private readonly List<State> _savedStates = [];
    private State _currentState;

    /// <summary>
    ///  Initialize the current state of <paramref name="hdc"/>.
    /// </summary>
    public DeviceContextState(HDC hdc)
    {
        MapMode = PInvoke.GetMapMode(hdc);
        BackColor = PInvoke.GetBkColor(hdc);
        TextColor = PInvoke.GetTextColor(hdc);
        Rop2Mode = PInvoke.GetROP2(hdc);
        TextAlign = PInvoke.GetTextAlign(hdc);
        BackgroundMode = PInvoke.GetBkMode(hdc);

        Matrix3x2 transform = default;
        PInvoke.GetWorldTransform(hdc, (XFORM*)(void*)&transform);
        Transform = transform;

        Point point = default;
        PInvoke.GetBrushOrgEx(hdc, &point);
        BrushOrigin = point;

        var hfont = PInvoke.GetCurrentObject(hdc, OBJ_TYPE.OBJ_FONT);
        PInvokeCore.GetObject(hfont, out LOGFONTW logfont);
        SelectedFont = logfont;

        var hpen = PInvoke.GetCurrentObject(hdc, OBJ_TYPE.OBJ_PEN);
        PInvokeCore.GetObject(hpen, out LOGPEN logpen);
        SelectedPen = logpen;

        var hbrush = PInvoke.GetCurrentObject(hdc, OBJ_TYPE.OBJ_BRUSH);
        PInvokeCore.GetObject(hbrush, out LOGBRUSH logbrush);
        SelectedBrush = logbrush;
    }

    public HDC_MAP_MODE MapMode { get => _currentState.MapMode; set => _currentState.MapMode = value; }
    public R2_MODE Rop2Mode { get => _currentState.Rop2Mode; set => _currentState.Rop2Mode = value; }
    public COLORREF BackColor { get => _currentState.BackColor; set => _currentState.BackColor = value; }
    public COLORREF TextColor { get => _currentState.TextColor; set => _currentState.TextColor = value; }
    public Point BrushOrigin { get => _currentState.BrushOrigin; set => _currentState.BrushOrigin = value; }
    public TEXT_ALIGN_OPTIONS TextAlign { get => _currentState.TextAlign; set => _currentState.TextAlign = value; }
    public BACKGROUND_MODE BackgroundMode { get => _currentState.BackgroundMode; set => _currentState.BackgroundMode = value; }
    public LOGFONTW SelectedFont { get => _currentState.SelectedFont; set => _currentState.SelectedFont = value; }
    public LOGBRUSH SelectedBrush { get => _currentState.SelectedBrush; set => _currentState.SelectedBrush = value; }
    public EXTLOGPEN32 SelectedPen { get => _currentState.SelectedPen; set => _currentState.SelectedPen = value; }
    public Point LastBeginPathBrushOrigin { get => _currentState.LastBeginPathBrushOrigin; set => _currentState.LastBeginPathBrushOrigin = value; }
    public bool InPath { get => _currentState.InPath; set => _currentState.InPath = value; }
    public Matrix3x2 Transform { get => _currentState.Transform; set => _currentState.Transform = value; }
    public RECT[] ClipRegion { get => _currentState.ClipRegion; set => _currentState.ClipRegion = value; }

    private struct State
    {
        public HDC_MAP_MODE MapMode { get; set; }
        public R2_MODE Rop2Mode { get; set; }
        public COLORREF BackColor { get; set; }
        public COLORREF TextColor { get; set; }
        public Point BrushOrigin { get; set; }
        public TEXT_ALIGN_OPTIONS TextAlign { get; set; }
        public BACKGROUND_MODE BackgroundMode { get; set; }
        public LOGFONTW SelectedFont { get; set; }
        public LOGBRUSH SelectedBrush { get; set; }
        public EXTLOGPEN32 SelectedPen { get; set; }
        public Point LastBeginPathBrushOrigin { get; set; }
        public bool InPath { get; set; }
        public Matrix3x2 Transform { get; set; }
        public RECT[] ClipRegion { get; set; }
    }

    /// <summary>
    ///  When using to parse a metafile, this is the list of known created objects.
    /// </summary>
    public List<EmfRecord> GdiObjects { get; } = [];

    /// <summary>
    ///  Adds the given object to <see cref="GdiObjects"/>.
    /// </summary>
    public void AddGdiObject(ref EmfRecord record, int index)
    {
        // Ensure we have capacity
        if (GdiObjects.Capacity <= index)
        {
            GdiObjects.Capacity = index + 1;
        }

        // Fill in any gaps if we have them
        while (GdiObjects.Count <= index)
        {
            GdiObjects.Add(default);
        }

        GdiObjects[index] = record;
    }

    public void SaveDC() => _savedStates.Add(_currentState);

    public void RestoreDC(int state)
    {
        int index;
        if (state > 0)
        {
            // Positive removes a specific state
            index = state - 1;
        }
        else
        {
            // Negative is relative (-1 is last saved state)
            index = _savedStates.Count + state;
        }

        _currentState = _savedStates[index];
        _savedStates.RemoveRange(index, _savedStates.Count - index);
    }

    /// <summary>
    ///  Applies the given selection record to the state.
    /// </summary>
    public void SelectGdiObject(EMRINDEXRECORD* selectionRecord)
    {
        // Not all records are handled yet. Backfilling in as we write specific tests.

        if (selectionRecord->IsStockObject)
        {
            HGDIOBJ hgdiobj = PInvokeCore.GetStockObject(selectionRecord->StockObject);

            switch (selectionRecord->StockObject)
            {
                case GET_STOCK_OBJECT_FLAGS.ANSI_FIXED_FONT:
                case GET_STOCK_OBJECT_FLAGS.OEM_FIXED_FONT:
                case GET_STOCK_OBJECT_FLAGS.ANSI_VAR_FONT:
                case GET_STOCK_OBJECT_FLAGS.SYSTEM_FONT:
                case GET_STOCK_OBJECT_FLAGS.DEVICE_DEFAULT_FONT:
                case GET_STOCK_OBJECT_FLAGS.SYSTEM_FIXED_FONT:
                case GET_STOCK_OBJECT_FLAGS.DEFAULT_GUI_FONT:
                    PInvokeCore.GetObject(hgdiobj, out LOGFONTW logfont);
                    SelectedFont = logfont;
                    break;
                case GET_STOCK_OBJECT_FLAGS.WHITE_BRUSH:
                case GET_STOCK_OBJECT_FLAGS.LTGRAY_BRUSH:
                case GET_STOCK_OBJECT_FLAGS.GRAY_BRUSH:
                case GET_STOCK_OBJECT_FLAGS.DKGRAY_BRUSH:
                case GET_STOCK_OBJECT_FLAGS.BLACK_BRUSH:
                case GET_STOCK_OBJECT_FLAGS.NULL_BRUSH:
                case GET_STOCK_OBJECT_FLAGS.DC_BRUSH:
                    PInvokeCore.GetObject(hgdiobj, out LOGBRUSH logBrush);
                    SelectedBrush = logBrush;
                    break;
                case GET_STOCK_OBJECT_FLAGS.WHITE_PEN:
                case GET_STOCK_OBJECT_FLAGS.BLACK_PEN:
                case GET_STOCK_OBJECT_FLAGS.NULL_PEN:
                case GET_STOCK_OBJECT_FLAGS.DC_PEN:
                    PInvokeCore.GetObject(hgdiobj, out LOGPEN logPen);
                    SelectedPen = logPen;
                    break;
                case GET_STOCK_OBJECT_FLAGS.DEFAULT_PALETTE:
                    break;
            }

            return;
        }

        // WARNING: You can not use fields that index out of the saved EmfRecord's contents here as the struct
        // is just a copy of the original. Any pointers or offsets outside of the struct aren't valid.

        EmfRecord savedRecord = GdiObjects[(int)selectionRecord->index];
        switch (savedRecord.Type)
        {
            case ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTCREATEFONTINDIRECTW:
                SelectedFont = savedRecord.ExtCreateFontIndirectWRecord->elfw.elfLogFont;
                break;
            case ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEPEN:
                SelectedPen = savedRecord.CreatePenRecord->lopn;
                break;
            case ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTCREATEPEN:
                SelectedPen = savedRecord.ExtCreatePenRecord->elp;
                break;
            case ENHANCED_METAFILE_RECORD_TYPE.EMR_CREATEBRUSHINDIRECT:
                SelectedBrush = savedRecord.CreateBrushIndirectRecord->lb;
                break;
        }
    }

    public Point TransformPoint(Point point) => Transform.TransformPoint(point);
}
