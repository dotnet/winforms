﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
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

        /// <summary>
        ///  Initialize the current state of <paramref name="hdc"/>.
        /// </summary>
        public DeviceContextState(Gdi32.HDC hdc)
        {
            MapMode = Gdi32.GetMapMode(hdc);
            BackColor = Gdi32.GetBkColor(hdc);
            TextColor = Gdi32.GetTextColor(hdc);
            Rop2Mode = Gdi32.GetROP2(hdc);
            TextAlign = Gdi32.GetTextAlign(hdc);
            BackgroundMode = Gdi32.GetBkMode(hdc);

            Point point = default;
            Gdi32.GetBrushOrgEx(hdc, ref point);
            BrushOrigin = point;

            var hfont = Gdi32.GetCurrentObject(hdc, Gdi32.OBJ.FONT);
            Gdi32.GetObjectW(hfont, out User32.LOGFONTW logfont);
            SelectedFont = logfont;

            var hpen = Gdi32.GetCurrentObject(hdc, Gdi32.OBJ.PEN);
            Gdi32.GetObjectW(hpen, out Gdi32.LOGPEN logpen);
            SelectedPen = logpen;

            var hbrush = Gdi32.GetCurrentObject(hdc, Gdi32.OBJ.BRUSH);
            Gdi32.GetObjectW(hbrush, out Gdi32.LOGBRUSH logbrush);
            SelectedBrush = logbrush;
        }

        public Gdi32.MM MapMode { get; set; }
        public Gdi32.R2 Rop2Mode { get; set; }
        public COLORREF BackColor { get; set; }
        public COLORREF TextColor { get; set; }
        public Point BrushOrigin { get; set; }
        public Gdi32.TA TextAlign { get; set; }
        public Gdi32.BKMODE BackgroundMode { get; set; }
        public User32.LOGFONTW SelectedFont { get; set; }
        public Gdi32.LOGBRUSH SelectedBrush { get; set; }
        public Gdi32.LOGPEN SelectedPen { get; set; }
        public Point LastBeginPathBrushOrigin { get; set; }
        public bool InPath { get; set; }

        /// <summary>
        ///  When using to parse a metafile, this is the list of known created objects.
        /// </summary>
        public List<EmfRecord> GdiObjects { get; } = new List<EmfRecord>();

        /// <summary>
        ///  Adds the given object to <see cref="GdiObjects"/>.
        /// </summary>
        public void AddGdiObject(ref EmfRecord record, int index)
        {
            if (GdiObjects.Capacity <= index)
            {
                GdiObjects.Capacity = index + 1;
            }

            while (GdiObjects.Count <= index)
            {
                GdiObjects.Add(default);
            }

            GdiObjects[index] = record;
        }

        /// <summary>
        ///  Applies the given selection record to the state.
        /// </summary>
        public void SelectGdiObject(EMRINDEXRECORD* selectionRecord)
        {
            // Not all records are handled yet. Backfilling in as we write specific tests.

            if (selectionRecord->IsStockObject)
            {
                Gdi32.HGDIOBJ hgdiobj = Gdi32.GetStockObject(selectionRecord->StockObject);

                switch (selectionRecord->StockObject)
                {
                    case Gdi32.StockObject.ANSI_FIXED_FONT:
                    case Gdi32.StockObject.OEM_FIXED_FONT:
                    case Gdi32.StockObject.ANSI_VAR_FONT:
                    case Gdi32.StockObject.SYSTEM_FONT:
                    case Gdi32.StockObject.DEVICE_DEFAULT_FONT:
                    case Gdi32.StockObject.SYSTEM_FIXED_FONT:
                    case Gdi32.StockObject.DEFAULT_GUI_FONT:
                        Gdi32.GetObjectW(hgdiobj, out User32.LOGFONTW logfont);
                        SelectedFont = logfont;
                        break;
                    case Gdi32.StockObject.WHITE_BRUSH:
                    case Gdi32.StockObject.LTGRAY_BRUSH:
                    case Gdi32.StockObject.GRAY_BRUSH:
                    case Gdi32.StockObject.DKGRAY_BRUSH:
                    case Gdi32.StockObject.BLACK_BRUSH:
                    case Gdi32.StockObject.NULL_BRUSH:
                    case Gdi32.StockObject.DC_BRUSH:
                        Gdi32.GetObjectW(hgdiobj, out Gdi32.LOGBRUSH logBrush);
                        SelectedBrush = logBrush;
                        break;
                    case Gdi32.StockObject.WHITE_PEN:
                    case Gdi32.StockObject.BLACK_PEN:
                    case Gdi32.StockObject.NULL_PEN:
                    case Gdi32.StockObject.DC_PEN:
                        Gdi32.GetObjectW(hgdiobj, out Gdi32.LOGPEN logPen);
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
                case Gdi32.EMR.CREATEBRUSHINDIRECT:
                    SelectedBrush = savedRecord.CreateBrushIndirectRecord->lb;
                    break;
            }
        }
    }
}
