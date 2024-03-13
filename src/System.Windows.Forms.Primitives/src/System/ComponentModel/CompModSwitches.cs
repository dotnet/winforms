// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel;

internal static class CompModSwitches
{
    private static TraceSwitch? s_flowLayout;
    private static TraceSwitch? s_dataCursor;
    private static TraceSwitch? s_dataGridCursor;
    private static TraceSwitch? s_dataGridEditing;
    private static TraceSwitch? s_dataGridKeys;
    private static TraceSwitch? s_dataGridLayout;
    private static TraceSwitch? s_dataGridPainting;
    private static TraceSwitch? s_dataGridParents;
    private static TraceSwitch? s_dataGridScrolling;
    private static TraceSwitch? s_dataGridSelection;
    private static TraceSwitch? s_dataObject;
    private static TraceSwitch? s_dataView;
    private static TraceSwitch? s_debugGridView;
    private static TraceSwitch? s_dgCaptionPaint;
    private static TraceSwitch? s_dgEditColumnEditing;
    private static TraceSwitch? s_dgRelationShpRowLayout;
    private static TraceSwitch? s_dgRelationShpRowPaint;
    private static TraceSwitch? s_dgRowPaint;
    private static TraceSwitch? s_dragDrop;
    private static TraceSwitch? s_imeMode;
    private static TraceSwitch? s_msaa;
    private static TraceSwitch? s_layoutPerformance;
    private static TraceSwitch? s_layoutSuspendResume;
    private static TraceSwitch? s_richLayout;
    private static TraceSwitch? s_setBounds;

    private static BooleanSwitch? s_lifetimeTracing;

    private static TraceSwitch? s_handleLeak;
    private static BooleanSwitch? s_traceCollect;
    private static BooleanSwitch? s_commonDesignerServices;

    public static TraceSwitch DataCursor
    {
        get
        {
            s_dataCursor ??= new TraceSwitch("Microsoft.WFC.Data.DataCursor", "DataCursor");

            return s_dataCursor;
        }
    }

    public static TraceSwitch DataGridCursor
    {
        get
        {
            s_dataGridCursor ??= new TraceSwitch("DataGridCursor", "DataGrid cursor tracing");

            return s_dataGridCursor;
        }
    }

    public static TraceSwitch DataGridEditing
    {
        get
        {
            s_dataGridEditing ??= new TraceSwitch("DataGridEditing", "DataGrid edit related tracing");

            return s_dataGridEditing;
        }
    }

    public static TraceSwitch DataGridKeys
    {
        get
        {
            s_dataGridKeys ??= new TraceSwitch("DataGridKeys", "DataGrid keystroke management tracing");

            return s_dataGridKeys;
        }
    }

    public static TraceSwitch DataGridLayout
    {
        get
        {
            s_dataGridLayout ??= new TraceSwitch("DataGridLayout", "DataGrid layout tracing");

            return s_dataGridLayout;
        }
    }

    public static TraceSwitch DataGridPainting
    {
        get
        {
            s_dataGridPainting ??= new TraceSwitch("DataGridPainting", "DataGrid Painting related tracing");

            return s_dataGridPainting;
        }
    }

    public static TraceSwitch DataGridParents
    {
        get
        {
            s_dataGridParents ??= new TraceSwitch("DataGridParents", "DataGrid parent rows");

            return s_dataGridParents;
        }
    }

    public static TraceSwitch DataGridScrolling
    {
        get
        {
            s_dataGridScrolling ??= new TraceSwitch("DataGridScrolling", "DataGrid scrolling");

            return s_dataGridScrolling;
        }
    }

    public static TraceSwitch DataGridSelection
    {
        get
        {
            s_dataGridSelection ??= new TraceSwitch("DataGridSelection", "DataGrid selection management tracing");

            return s_dataGridSelection;
        }
    }

    public static TraceSwitch DataObject
    {
        get
        {
            s_dataObject ??= new TraceSwitch("DataObject", "Enable tracing for the DataObject class.");

            return s_dataObject;
        }
    }

    public static TraceSwitch DataView
    {
        get
        {
            s_dataView ??= new TraceSwitch("DataView", "DataView");

            return s_dataView;
        }
    }

    public static TraceSwitch DebugGridView
    {
        get
        {
            s_debugGridView ??= new TraceSwitch("PSDEBUGGRIDVIEW", "Debug PropertyGridView");

            return s_debugGridView;
        }
    }

    public static TraceSwitch DGCaptionPaint
    {
        get
        {
            s_dgCaptionPaint ??= new TraceSwitch("DGCaptionPaint", "DataGridCaption");

            return s_dgCaptionPaint;
        }
    }

    public static TraceSwitch DGEditColumnEditing
    {
        get
        {
            s_dgEditColumnEditing ??= new TraceSwitch("DGEditColumnEditing", "Editing related tracing");

            return s_dgEditColumnEditing;
        }
    }

    public static TraceSwitch DGRelationShpRowLayout
    {
        get
        {
            s_dgRelationShpRowLayout ??= new TraceSwitch("DGRelationShpRowLayout", "Relationship row layout");

            return s_dgRelationShpRowLayout;
        }
    }

    public static TraceSwitch DGRelationShpRowPaint
    {
        get
        {
            s_dgRelationShpRowPaint ??= new TraceSwitch("DGRelationShpRowPaint", "Relationship row painting");

            return s_dgRelationShpRowPaint;
        }
    }

    public static TraceSwitch DGRowPaint
    {
        get
        {
            s_dgRowPaint ??= new TraceSwitch("DGRowPaint", "DataGrid Simple Row painting stuff");

            return s_dgRowPaint;
        }
    }

    public static TraceSwitch DragDrop
    {
        get
        {
            s_dragDrop ??= new TraceSwitch("DragDrop", "Debug OLEDragDrop support in Controls");

            return s_dragDrop;
        }
    }

    public static TraceSwitch FlowLayout
    {
        get
        {
            s_flowLayout ??= new TraceSwitch("FlowLayout", "Debug flow layout");

            return s_flowLayout;
        }
    }

    public static TraceSwitch ImeMode
    {
        get
        {
            s_imeMode ??= new TraceSwitch("ImeMode", "Debug IME Mode");

            return s_imeMode;
        }
    }

    public static TraceSwitch LayoutPerformance
    {
        get
        {
            s_layoutPerformance ??= new TraceSwitch("LayoutPerformance", "Tracks layout events which impact performance.");

            return s_layoutPerformance;
        }
    }

    public static TraceSwitch LayoutSuspendResume
    {
        get
        {
            s_layoutSuspendResume ??= new TraceSwitch("LayoutSuspendResume", "Tracks SuspendLayout/ResumeLayout.");

            return s_layoutSuspendResume;
        }
    }

    public static BooleanSwitch LifetimeTracing
    {
        get
        {
            s_lifetimeTracing ??= new BooleanSwitch("LifetimeTracing", "Track lifetime events. This will cause objects to track the stack at creation and dispose.");

            return s_lifetimeTracing;
        }
    }

    public static TraceSwitch MSAA
    {
        get
        {
            s_msaa ??= new TraceSwitch("MSAA", "Debug Microsoft Active Accessibility");

            return s_msaa;
        }
    }

    public static TraceSwitch RichLayout
    {
        get
        {
            s_richLayout ??= new TraceSwitch("RichLayout", "Debug layout in RichControls");

            return s_richLayout;
        }
    }

    public static TraceSwitch SetBounds
    {
        get
        {
            s_setBounds ??= new TraceSwitch("SetBounds", "Trace changes to control size/position.");

            return s_setBounds;
        }
    }

    public static TraceSwitch HandleLeak
    {
        get
        {
            s_handleLeak ??= new TraceSwitch("HANDLELEAK", "HandleCollector: Track Win32 Handle Leaks");

            return s_handleLeak;
        }
    }

    public static BooleanSwitch TraceCollect
    {
        get
        {
            s_traceCollect ??= new BooleanSwitch("TRACECOLLECT", "HandleCollector: Trace HandleCollector operations");

            return s_traceCollect;
        }
    }

    public static BooleanSwitch CommonDesignerServices
    {
        get
        {
            s_commonDesignerServices ??= new BooleanSwitch("CommonDesignerServices", "Assert if any common designer service is not found.");

            return s_commonDesignerServices;
        }
    }
}
