﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using System.Numerics;
using System.Windows.Forms.Metafiles;
using static Interop;

namespace System
{
    /// <summary>
    ///  Holder for tracking HDC state.
    /// </summary>
    internal unsafe class DeviceContextState
    {
        // Not all state is handled yet. Backfilling in as we write specific tests. Of special note is that we don't
        // have tracking for Save/RestoreDC yet.

        private readonly List<State> _savedStates = new List<State>();
        private State _currentState;

        /// <summary>
        ///  Initialize the current state of <paramref name="hdc"/>.
        /// </summary>
        public DeviceContextState(HDC hdc)
        {
            MapMode = (Gdi32.MM)PInvoke.GetMapMode(hdc);
            BackColor = PInvoke.GetBkColor(hdc);
            TextColor = PInvoke.GetTextColor(hdc);
            Rop2Mode = (Gdi32.R2)PInvoke.GetROP2(hdc);
            TextAlign = (Gdi32.TA)PInvoke.GetTextAlign(hdc);
            BackgroundMode = (Gdi32.BKMODE)PInvoke.GetBkMode(hdc);

            Matrix3x2 transform = default;
            PInvoke.GetWorldTransform(hdc, (XFORM*)(void*)&transform);
            Transform = transform;

            Point point = default;
            PInvoke.GetBrushOrgEx(hdc, &point);
            BrushOrigin = point;

            var hfont = PInvoke.GetCurrentObject(hdc, OBJ_TYPE.OBJ_FONT);
            PInvoke.GetObject(hfont, out LOGFONTW logfont);
            SelectedFont = logfont;

            var hpen = PInvoke.GetCurrentObject(hdc, OBJ_TYPE.OBJ_PEN);
            PInvoke.GetObject(hpen, out Gdi32.LOGPEN logpen);
            SelectedPen = logpen;

            var hbrush = PInvoke.GetCurrentObject(hdc, OBJ_TYPE.OBJ_BRUSH);
            PInvoke.GetObject(hbrush, out Gdi32.LOGBRUSH logbrush);
            SelectedBrush = logbrush;
        }

        public Gdi32.MM MapMode { get => _currentState.MapMode; set => _currentState.MapMode = value; }
        public Gdi32.R2 Rop2Mode { get => _currentState.Rop2Mode; set => _currentState.Rop2Mode = value; }
        public COLORREF BackColor { get => _currentState.BackColor; set => _currentState.BackColor = value; }
        public COLORREF TextColor { get => _currentState.TextColor; set => _currentState.TextColor = value; }
        public Point BrushOrigin { get => _currentState.BrushOrigin; set => _currentState.BrushOrigin = value; }
        public Gdi32.TA TextAlign { get => _currentState.TextAlign; set => _currentState.TextAlign = value; }
        public Gdi32.BKMODE BackgroundMode { get => _currentState.BackgroundMode; set => _currentState.BackgroundMode = value; }
        public LOGFONTW SelectedFont { get => _currentState.SelectedFont; set => _currentState.SelectedFont = value; }
        public Gdi32.LOGBRUSH SelectedBrush { get => _currentState.SelectedBrush; set => _currentState.SelectedBrush = value; }
        public EXTLOGPEN32 SelectedPen { get => _currentState.SelectedPen; set => _currentState.SelectedPen = value; }
        public Point LastBeginPathBrushOrigin { get => _currentState.LastBeginPathBrushOrigin; set => _currentState.LastBeginPathBrushOrigin = value; }
        public bool InPath { get => _currentState.InPath; set => _currentState.InPath = value; }
        public Matrix3x2 Transform { get => _currentState.Transform; set => _currentState.Transform = value; }
        public RECT[] ClipRegion { get => _currentState.ClipRegion; set => _currentState.ClipRegion = value; }

        private struct State
        {
            public Gdi32.MM MapMode { get; set; }
            public Gdi32.R2 Rop2Mode { get; set; }
            public COLORREF BackColor { get; set; }
            public COLORREF TextColor { get; set; }
            public Point BrushOrigin { get; set; }
            public Gdi32.TA TextAlign { get; set; }
            public Gdi32.BKMODE BackgroundMode { get; set; }
            public LOGFONTW SelectedFont { get; set; }
            public Gdi32.LOGBRUSH SelectedBrush { get; set; }
            public EXTLOGPEN32 SelectedPen { get; set; }
            public Point LastBeginPathBrushOrigin { get; set; }
            public bool InPath { get; set; }
            public Matrix3x2 Transform { get; set; }
            public RECT[] ClipRegion { get; set; }
        }

        /// <summary>
        ///  When using to parse a metafile, this is the list of known created objects.
        /// </summary>
        public List<EmfRecord> GdiObjects { get; } = new List<EmfRecord>();

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
                HGDIOBJ hgdiobj = PInvoke.GetStockObject((GET_STOCK_OBJECT_FLAGS)selectionRecord->StockObject);

                switch (selectionRecord->StockObject)
                {
                    case Gdi32.StockObject.ANSI_FIXED_FONT:
                    case Gdi32.StockObject.OEM_FIXED_FONT:
                    case Gdi32.StockObject.ANSI_VAR_FONT:
                    case Gdi32.StockObject.SYSTEM_FONT:
                    case Gdi32.StockObject.DEVICE_DEFAULT_FONT:
                    case Gdi32.StockObject.SYSTEM_FIXED_FONT:
                    case Gdi32.StockObject.DEFAULT_GUI_FONT:
                        PInvoke.GetObject(hgdiobj, out LOGFONTW logfont);
                        SelectedFont = logfont;
                        break;
                    case Gdi32.StockObject.WHITE_BRUSH:
                    case Gdi32.StockObject.LTGRAY_BRUSH:
                    case Gdi32.StockObject.GRAY_BRUSH:
                    case Gdi32.StockObject.DKGRAY_BRUSH:
                    case Gdi32.StockObject.BLACK_BRUSH:
                    case Gdi32.StockObject.NULL_BRUSH:
                    case Gdi32.StockObject.DC_BRUSH:
                        PInvoke.GetObject(hgdiobj, out Gdi32.LOGBRUSH logBrush);
                        SelectedBrush = logBrush;
                        break;
                    case Gdi32.StockObject.WHITE_PEN:
                    case Gdi32.StockObject.BLACK_PEN:
                    case Gdi32.StockObject.NULL_PEN:
                    case Gdi32.StockObject.DC_PEN:
                        PInvoke.GetObject(hgdiobj, out Gdi32.LOGPEN logPen);
                        SelectedPen = logPen;
                        break;
                    case Gdi32.StockObject.DEFAULT_PALETTE:
                        break;
                }

                return;
            }

            // WARNING: You can not use fields that index out of the saved EmfRecord's contents here as the struct
            // is just a copy of the original. Any pointers or offsets outside of the struct aren't valid.

            EmfRecord savedRecord = GdiObjects[(int)selectionRecord->index];
            switch (savedRecord.Type)
            {
                case Gdi32.EMR.EXTCREATEFONTINDIRECTW:
                    SelectedFont = savedRecord.ExtCreateFontIndirectWRecord->elfw.elfLogFont;
                    break;
                case Gdi32.EMR.CREATEPEN:
                    SelectedPen = savedRecord.CreatePenRecord->lopn;
                    break;
                case Gdi32.EMR.EXTCREATEPEN:
                    SelectedPen = savedRecord.ExtCreatePenRecord->elp;
                    break;
                case Gdi32.EMR.CREATEBRUSHINDIRECT:
                    SelectedBrush = savedRecord.CreateBrushIndirectRecord->lb;
                    break;
            }
        }

        public Point TransformPoint(Point point) => Transform.TransformPoint(point);
    }
}
