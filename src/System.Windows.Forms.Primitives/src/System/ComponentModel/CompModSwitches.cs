// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.ComponentModel
{
    internal static class CompModSwitches
    {
        private static TraceSwitch activeX;
        private static TraceSwitch flowLayout;
        private static TraceSwitch dataCursor;
        private static TraceSwitch dataGridCursor;
        private static TraceSwitch dataGridEditing;
        private static TraceSwitch dataGridKeys;
        private static TraceSwitch dataGridLayout;
        private static TraceSwitch dataGridPainting;
        private static TraceSwitch dataGridParents;
        private static TraceSwitch dataGridScrolling;
        private static TraceSwitch dataGridSelection;
        private static TraceSwitch dataObject;
        private static TraceSwitch dataView;
        private static TraceSwitch debugGridView;
        private static TraceSwitch dgCaptionPaint;
        private static TraceSwitch dgEditColumnEditing;
        private static TraceSwitch dgRelationShpRowLayout;
        private static TraceSwitch dgRelationShpRowPaint;
        private static TraceSwitch dgRowPaint;
        private static TraceSwitch dragDrop;
        private static TraceSwitch imeMode;
        private static TraceSwitch msaa;
        private static TraceSwitch msoComponentManager;
        private static TraceSwitch layoutPerformance;
        private static TraceSwitch layoutSuspendResume;
        private static TraceSwitch richLayout;
        private static TraceSwitch setBounds;

        private static BooleanSwitch lifetimeTracing;

        private static TraceSwitch s_handleLeak;
        private static BooleanSwitch s_traceCollect;
        private static BooleanSwitch s_commonDesignerServices;

        public static TraceSwitch ActiveX
        {
            get
            {
                if (activeX == null)
                {
                    activeX = new TraceSwitch("ActiveX", "Debug ActiveX sourcing");
                }
                return activeX;
            }
        }

        public static TraceSwitch DataCursor
        {
            get
            {
                if (dataCursor == null)
                {
                    dataCursor = new TraceSwitch("Microsoft.WFC.Data.DataCursor", "DataCursor");
                }
                return dataCursor;
            }
        }

        public static TraceSwitch DataGridCursor
        {
            get
            {
                if (dataGridCursor == null)
                {
                    dataGridCursor = new TraceSwitch("DataGridCursor", "DataGrid cursor tracing");
                }
                return dataGridCursor;
            }
        }

        public static TraceSwitch DataGridEditing
        {
            get
            {
                if (dataGridEditing == null)
                {
                    dataGridEditing = new TraceSwitch("DataGridEditing", "DataGrid edit related tracing");
                }
                return dataGridEditing;
            }
        }

        public static TraceSwitch DataGridKeys
        {
            get
            {
                if (dataGridKeys == null)
                {
                    dataGridKeys = new TraceSwitch("DataGridKeys", "DataGrid keystroke management tracing");
                }
                return dataGridKeys;
            }
        }

        public static TraceSwitch DataGridLayout
        {
            get
            {
                if (dataGridLayout == null)
                {
                    dataGridLayout = new TraceSwitch("DataGridLayout", "DataGrid layout tracing");
                }
                return dataGridLayout;
            }
        }

        public static TraceSwitch DataGridPainting
        {
            get
            {
                if (dataGridPainting == null)
                {
                    dataGridPainting = new TraceSwitch("DataGridPainting", "DataGrid Painting related tracing");
                }
                return dataGridPainting;
            }
        }

        public static TraceSwitch DataGridParents
        {
            get
            {
                if (dataGridParents == null)
                {
                    dataGridParents = new TraceSwitch("DataGridParents", "DataGrid parent rows");
                }
                return dataGridParents;
            }
        }

        public static TraceSwitch DataGridScrolling
        {
            get
            {
                if (dataGridScrolling == null)
                {
                    dataGridScrolling = new TraceSwitch("DataGridScrolling", "DataGrid scrolling");
                }
                return dataGridScrolling;
            }
        }

        public static TraceSwitch DataGridSelection
        {
            get
            {
                if (dataGridSelection == null)
                {
                    dataGridSelection = new TraceSwitch("DataGridSelection", "DataGrid selection management tracing");
                }
                return dataGridSelection;
            }
        }

        public static TraceSwitch DataObject
        {
            get
            {
                if (dataObject == null)
                {
                    dataObject = new TraceSwitch("DataObject", "Enable tracing for the DataObject class.");
                }
                return dataObject;
            }
        }

        public static TraceSwitch DataView
        {
            get
            {
                if (dataView == null)
                {
                    dataView = new TraceSwitch("DataView", "DataView");
                }
                return dataView;
            }
        }

        public static TraceSwitch DebugGridView
        {
            get
            {
                if (debugGridView == null)
                {
                    debugGridView = new TraceSwitch("PSDEBUGGRIDVIEW", "Debug PropertyGridView");
                }
                return debugGridView;
            }
        }

        public static TraceSwitch DGCaptionPaint
        {
            get
            {
                if (dgCaptionPaint == null)
                {
                    dgCaptionPaint = new TraceSwitch("DGCaptionPaint", "DataGridCaption");
                }
                return dgCaptionPaint;
            }
        }

        public static TraceSwitch DGEditColumnEditing
        {
            get
            {
                if (dgEditColumnEditing == null)
                {
                    dgEditColumnEditing = new TraceSwitch("DGEditColumnEditing", "Editing related tracing");
                }
                return dgEditColumnEditing;
            }
        }

        public static TraceSwitch DGRelationShpRowLayout
        {
            get
            {
                if (dgRelationShpRowLayout == null)
                {
                    dgRelationShpRowLayout = new TraceSwitch("DGRelationShpRowLayout", "Relationship row layout");
                }
                return dgRelationShpRowLayout;
            }
        }

        public static TraceSwitch DGRelationShpRowPaint
        {
            get
            {
                if (dgRelationShpRowPaint == null)
                {
                    dgRelationShpRowPaint = new TraceSwitch("DGRelationShpRowPaint", "Relationship row painting");
                }
                return dgRelationShpRowPaint;
            }
        }

        public static TraceSwitch DGRowPaint
        {
            get
            {
                if (dgRowPaint == null)
                {
                    dgRowPaint = new TraceSwitch("DGRowPaint", "DataGrid Simple Row painting stuff");
                }
                return dgRowPaint;
            }
        }

        public static TraceSwitch DragDrop
        {
            get
            {
                if (dragDrop == null)
                {
                    dragDrop = new TraceSwitch("DragDrop", "Debug OLEDragDrop support in Controls");
                }
                return dragDrop;
            }
        }

        public static TraceSwitch FlowLayout
        {
            get
            {
                if (flowLayout == null)
                {
                    flowLayout = new TraceSwitch("FlowLayout", "Debug flow layout");
                }
                return flowLayout;
            }
        }

        public static TraceSwitch ImeMode
        {
            get
            {
                if (imeMode == null)
                {
                    imeMode = new TraceSwitch("ImeMode", "Debug IME Mode");
                }
                return imeMode;
            }
        }

        public static TraceSwitch LayoutPerformance
        {
            get
            {
                if (layoutPerformance == null)
                {
                    layoutPerformance = new TraceSwitch("LayoutPerformance", "Tracks layout events which impact performance.");
                }
                return layoutPerformance;
            }
        }

        public static TraceSwitch LayoutSuspendResume
        {
            get
            {
                if (layoutSuspendResume == null)
                {
                    layoutSuspendResume = new TraceSwitch("LayoutSuspendResume", "Tracks SuspendLayout/ResumeLayout.");
                }
                return layoutSuspendResume;
            }
        }

        public static BooleanSwitch LifetimeTracing
        {
            get
            {
                if (lifetimeTracing == null)
                {
                    lifetimeTracing = new BooleanSwitch("LifetimeTracing", "Track lifetime events. This will cause objects to track the stack at creation and dispose.");
                }
                return lifetimeTracing;
            }
        }

        public static TraceSwitch MSAA
        {
            get
            {
                if (msaa == null)
                {
                    msaa = new TraceSwitch("MSAA", "Debug Microsoft Active Accessibility");
                }
                return msaa;
            }
        }

        public static TraceSwitch MSOComponentManager
        {
            get
            {
                if (msoComponentManager == null)
                {
                    msoComponentManager = new TraceSwitch("MSOComponentManager", "Debug MSO Component Manager support");
                }
                return msoComponentManager;
            }
        }

        public static TraceSwitch RichLayout
        {
            get
            {
                if (richLayout == null)
                {
                    richLayout = new TraceSwitch("RichLayout", "Debug layout in RichControls");
                }
                return richLayout;
            }
        }

        public static TraceSwitch SetBounds
        {
            get
            {
                if (setBounds == null)
                {
                    setBounds = new TraceSwitch("SetBounds", "Trace changes to control size/position.");
                }
                return setBounds;
            }
        }

        public static TraceSwitch HandleLeak
        {
            get
            {
                if (s_handleLeak == null)
                {
                    s_handleLeak = new TraceSwitch("HANDLELEAK", "HandleCollector: Track Win32 Handle Leaks");
                }
                return s_handleLeak;
            }
        }

        public static BooleanSwitch TraceCollect
        {
            get
            {
                if (s_traceCollect == null)
                {
                    s_traceCollect = new BooleanSwitch("TRACECOLLECT", "HandleCollector: Trace HandleCollector operations");
                }
                return s_traceCollect;
            }
        }

        public static BooleanSwitch CommonDesignerServices
        {
            get
            {
                if (s_commonDesignerServices == null)
                {
                    s_commonDesignerServices = new BooleanSwitch("CommonDesignerServices", "Assert if any common designer service is not found.");
                }
                return s_commonDesignerServices;
            }
        }
    }
}
