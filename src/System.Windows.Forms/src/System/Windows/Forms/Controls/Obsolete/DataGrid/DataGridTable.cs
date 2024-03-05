// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("DataGridTableStyle has been deprecated.")]
public class DataGridTableStyle : Component, IDataGridEditingService
{
    internal DataGrid dataGrid;

    // relationship UI
    private int relationshipHeight;
    internal const int relationshipSpacing = 1;
    private Rectangle relationshipRect = Rectangle.Empty;
    private int focusedRelation = -1;
    private int focusedTextWidth;

    // will contain a list of relationships that this table has
    private ArrayList relationsList = new ArrayList(2);

    private static readonly object EventAllowSorting = new object();
    private static readonly object EventGridLineColor = new object();
    private static readonly object EventGridLineStyle = new object();
    private static readonly object EventHeaderBackColor = new object();
    private static readonly object EventHeaderForeColor = new object();
    private static readonly object EventHeaderFont = new object();
    private static readonly object EventLinkColor = new object();
    private static readonly object EventLinkHoverColor = new object();
    private static readonly object EventPreferredColumnWidth = new object();
    private static readonly object EventPreferredRowHeight = new object();
    private static readonly object EventColumnHeadersVisible = new object();
    private static readonly object EventRowHeaderWidth = new object();
    private static readonly object EventSelectionBackColor = new object();
    private static readonly object EventSelectionForeColor = new object();
    private static readonly object EventMappingName = new object();
    private static readonly object EventAlternatingBackColor = new object();
    private static readonly object EventBackColor = new object();
    private static readonly object EventForeColor = new object();
    private static readonly object EventReadOnly = new object();
    private static readonly object EventRowHeadersVisible = new object();
    private const int defaultPreferredColumnWidth = 75;
    internal static readonly Font defaultFont = Control.DefaultFont;
    internal static readonly int defaultFontHeight = defaultFont.Height;
    private SolidBrush alternatingBackBrush = DefaultAlternatingBackBrush;
    private SolidBrush backBrush = DefaultBackBrush;
    private SolidBrush foreBrush = DefaultForeBrush;
    private SolidBrush gridLineBrush = DefaultGridLineBrush;
    internal SolidBrush headerBackBrush = DefaultHeaderBackBrush;
    internal Font headerFont; // this is ambient property to Font value.
    internal SolidBrush headerForeBrush = DefaultHeaderForeBrush;
    internal Pen headerForePen = DefaultHeaderForePen;
    private SolidBrush linkBrush = DefaultLinkBrush;
    internal int preferredColumnWidth = defaultPreferredColumnWidth;
    private int prefferedRowHeight = defaultFontHeight + 3;
    private SolidBrush selectionBackBrush = DefaultSelectionBackBrush;
    private SolidBrush selectionForeBrush = DefaultSelectionForeBrush;

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool AllowSorting
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler AllowSortingChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color AlternatingBackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler AlternatingBackColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public void ResetAlternatingBackColor()
    {
        throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeAlternatingBackColor()
    {
        return !AlternatingBackBrush.Equals(DefaultAlternatingBackBrush);
    }

    internal SolidBrush AlternatingBackBrush
    {
        get
        {
            return alternatingBackBrush;
        }
    }

    protected bool ShouldSerializeBackColor()
    {
        return !DefaultBackBrush.Equals(backBrush);
    }

    protected bool ShouldSerializeForeColor()
    {
        return DefaultForeBrush.Equals(foreBrush);
    }

    internal SolidBrush BackBrush
    {
        get
        {
            return backBrush;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color BackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler BackColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    public void ResetBackColor()
    {
        throw new PlatformNotSupportedException();
    }

    internal int BorderWidth
    {
        get
        {
            DataGrid dataGrid = DataGrid;
            if (dataGrid is null)
                return 0;
            // if the user set the GridLineStyle property on the dataGrid.
            // then use the value of that property
            DataGridLineStyle gridStyle;
            int gridLineWidth;
            if (IsDefault)
            {
                gridStyle = DataGrid.GridLineStyle;
                gridLineWidth = DataGrid.GridLineWidth;
            }
            else
            {
                gridStyle = GridLineStyle;
                gridLineWidth = GridLineWidth;
            }

            if (gridStyle == DataGridLineStyle.None)
                return 0;

            return gridLineWidth;
        }
    }

    internal static SolidBrush DefaultAlternatingBackBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.Window;
        }
    }

    internal static SolidBrush DefaultBackBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.Window;
        }
    }

    internal static SolidBrush DefaultForeBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.WindowText;
        }
    }

    private static SolidBrush DefaultGridLineBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.Control;
        }
    }

    private static SolidBrush DefaultHeaderBackBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.Control;
        }
    }

    private static SolidBrush DefaultHeaderForeBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.ControlText;
        }
    }

    private static Pen DefaultHeaderForePen
    {
        get
        {
            return new Pen(SystemColors.ControlText);
        }
    }

    private static SolidBrush DefaultLinkBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.HotTrack;
        }
    }

    private static SolidBrush DefaultSelectionBackBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.ActiveCaption;
        }
    }

    private static SolidBrush DefaultSelectionForeBrush
    {
        get
        {
            return (SolidBrush)SystemBrushes.ActiveCaptionText;
        }
    }

    internal int FocusedRelation
    {
        get
        {
            return focusedRelation;
        }
        set
        {
            if (focusedRelation != value)
            {
                focusedRelation = value;
                if (focusedRelation == -1)
                {
                    focusedTextWidth = 0;
                }
                else
                {
                    Graphics g = DataGrid.CreateGraphicsInternal();
                    focusedTextWidth = (int)Math.Ceiling(g.MeasureString(((string)RelationsList[focusedRelation]), DataGrid.LinkFont).Width);
                    g.Dispose();
                }
            }
        }
    }

    internal int FocusedTextWidth
    {
        get
        {
            return focusedTextWidth;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color ForeColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ForeColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal SolidBrush ForeBrush
    {
        get
        {
            return foreBrush;
        }
    }

    public void ResetForeColor()
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color GridLineColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler GridLineColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected virtual bool ShouldSerializeGridLineColor()
    {
        return !GridLineBrush.Equals(DefaultGridLineBrush);
    }

    public void ResetGridLineColor()
    {
        throw new PlatformNotSupportedException();
    }

    internal SolidBrush GridLineBrush
    {
        get
        {
            return gridLineBrush;
        }
    }

    internal int GridLineWidth
    {
        get
        {
            Debug.Assert(GridLineStyle == DataGridLineStyle.Solid || GridLineStyle == DataGridLineStyle.None, "are there any other styles?");
            return GridLineStyle == DataGridLineStyle.Solid ? 1 : 0;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridLineStyle GridLineStyle
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler GridLineStyleChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color HeaderBackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler HeaderBackColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal SolidBrush HeaderBackBrush
    {
        get
        {
            return headerBackBrush;
        }
    }

    protected virtual bool ShouldSerializeHeaderBackColor()
    {
        return !HeaderBackBrush.Equals(DefaultHeaderBackBrush);
    }

    public void ResetHeaderBackColor()
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Font HeaderFont
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler HeaderFontChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    private bool ShouldSerializeHeaderFont()
    {
        return (headerFont is not null);
    }

    public void ResetHeaderFont()
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color HeaderForeColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler HeaderForeColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected virtual bool ShouldSerializeHeaderForeColor()
    {
        return !HeaderForePen.Equals(DefaultHeaderForePen);
    }

    public void ResetHeaderForeColor()
    {
        throw new PlatformNotSupportedException();
    }

    internal SolidBrush HeaderForeBrush
    {
        get
        {
            return headerForeBrush;
        }
    }

    internal Pen HeaderForePen
    {
        get
        {
            return headerForePen;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color LinkColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler LinkColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected virtual bool ShouldSerializeLinkColor()
    {
        return !LinkBrush.Equals(DefaultLinkBrush);
    }

    public void ResetLinkColor()
    {
        throw new PlatformNotSupportedException();
    }

    internal Brush LinkBrush
    {
        get
        {
            return linkBrush;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color LinkHoverColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler LinkHoverColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected virtual bool ShouldSerializeLinkHoverColor()
    {
        return false;
    }

    internal Rectangle RelationshipRect
    {
        get
        {
            if (relationshipRect.IsEmpty)
            {
                ComputeRelationshipRect();
            }

            return relationshipRect;
        }
    }

    private Rectangle ComputeRelationshipRect()
    {
        if (relationshipRect.IsEmpty && DataGrid.AllowNavigation)
        {
            Debug.WriteLineIf(CompModSwitches.DGRelationShpRowLayout.TraceVerbose, "GetRelationshipRect grinding away");
            Graphics g = DataGrid.CreateGraphicsInternal();
            relationshipRect = default(Rectangle);
            relationshipRect.X = 0; // indentWidth;
                                    // relationshipRect.Y = base.Height - BorderWidth;

            // Determine the width of the widest relationship name
            int longestRelationship = 0;
            for (int r = 0; r < RelationsList.Count; ++r)
            {
                int rwidth = (int)Math.Ceiling(g.MeasureString(((string)RelationsList[r]), DataGrid.LinkFont).Width)
;
                if (rwidth > longestRelationship)
                    longestRelationship = rwidth;
            }

            g.Dispose();

            relationshipRect.Width = longestRelationship + 5;
            relationshipRect.Width += 2; // relationshipRect border;
            relationshipRect.Height = BorderWidth + relationshipHeight * RelationsList.Count;
            relationshipRect.Height += 2; // relationship border
            if (RelationsList.Count > 0)
                relationshipRect.Height += 2 * relationshipSpacing;
        }

        return relationshipRect;
    }

    internal void ResetRelationsUI()
    {
        relationshipRect = Rectangle.Empty;
        focusedRelation = -1;
        relationshipHeight = dataGrid.LinkFontHeight + relationshipSpacing;
    }

    internal int RelationshipHeight
    {
        get
        {
            return relationshipHeight;
        }
    }

    public void ResetLinkHoverColor()
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int PreferredColumnWidth
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler PreferredColumnWidthChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int PreferredRowHeight
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler PreferredRowHeightChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    private void ResetPreferredRowHeight()
    {
        PreferredRowHeight = defaultFontHeight + 3;
    }

    protected bool ShouldSerializePreferredRowHeight()
    {
        return prefferedRowHeight != defaultFontHeight + 3;
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool ColumnHeadersVisible
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ColumnHeadersVisibleChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool RowHeadersVisible
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler RowHeadersVisibleChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int RowHeaderWidth
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler RowHeaderWidthChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color SelectionBackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler SelectionBackColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal SolidBrush SelectionBackBrush
    {
        get
        {
            return selectionBackBrush;
        }
    }

    internal SolidBrush SelectionForeBrush
    {
        get
        {
            return selectionForeBrush;
        }
    }

    protected bool ShouldSerializeSelectionBackColor()
    {
        return !DefaultSelectionBackBrush.Equals(selectionBackBrush);
    }

    public void ResetSelectionBackColor()
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Color SelectionForeColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler SelectionForeColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected virtual bool ShouldSerializeSelectionForeColor()
    {
        return !SelectionForeBrush.Equals(DefaultSelectionForeBrush);
    }

    public void ResetSelectionForeColor()
    {
        throw new PlatformNotSupportedException();
    }

    private void InvalidateInside()
    {
        if (DataGrid is not null)
        {
            DataGrid.InvalidateInside();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly DataGridTableStyle DefaultTableStyle = new DataGridTableStyle(true);

    public DataGridTableStyle(bool isDefaultTableStyle)
    {
        throw new PlatformNotSupportedException();
    }

    public DataGridTableStyle() : this(false)
    {
        throw new PlatformNotSupportedException();
    }

    public DataGridTableStyle(CurrencyManager listManager) : this()
    {
        throw new PlatformNotSupportedException();
    }

    internal void SetRelationsList(CurrencyManager listManager)
    {
        PropertyDescriptorCollection propCollection = listManager.GetItemProperties();
        Debug.Assert(!IsDefault, "the grid can set the relations only on a table that was manually added by the user");
        int propCount = propCollection.Count;
        if (relationsList.Count > 0)
            relationsList.Clear();
        for (int i = 0; i < propCount; i++)
        {
            PropertyDescriptor prop = propCollection[i];
            Debug.Assert(prop is not null, "prop is null: how that happened?");
            if (PropertyDescriptorIsARelation(prop))
            {
                // relation
                relationsList.Add(prop.Name);
            }
        }
    }

    internal void SetGridColumnStylesCollection(CurrencyManager listManager)
    {
        PropertyDescriptorCollection propCollection = listManager.GetItemProperties();

        // we need to clear the relations list
        if (relationsList.Count > 0)
            relationsList.Clear();

        Debug.Assert(propCollection is not null, "propCollection is null: how that happened?");
        int propCount = propCollection.Count;
        for (int i = 0; i < propCount; i++)
        {
            PropertyDescriptor prop = propCollection[i];
            Debug.Assert(prop is not null, "prop is null: how that happened?");
            // do not take into account the properties that are browsable.
            if (!prop.IsBrowsable)
                continue;
            if (PropertyDescriptorIsARelation(prop))
            {
                // relation
                relationsList.Add(prop.Name);
            }
            else
            {
                // column
                DataGridColumnStyle col = CreateGridColumn(prop);
            }
        }
    }

    private static bool PropertyDescriptorIsARelation(PropertyDescriptor prop)
    {
        return typeof(IList).IsAssignableFrom(prop.PropertyType) && !typeof(Array).IsAssignableFrom(prop.PropertyType);
    }

    internal protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop)
    {
        return CreateGridColumn(prop, false);
    }

    internal protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop, bool isDefault)
    {
        DataGridColumnStyle ret = null;
        Type dataType = prop.PropertyType;

        if (dataType.Equals(typeof(bool)))
            ret = new DataGridBoolColumn(prop, isDefault);
        else if (dataType.Equals(typeof(string)))
            ret = new DataGridTextBoxColumn(prop, isDefault);
        else if (dataType.Equals(typeof(DateTime)))
            ret = new DataGridTextBoxColumn(prop, "d", isDefault);

        else if (dataType.Equals(typeof(short)) ||
                 dataType.Equals(typeof(int)) ||
                 dataType.Equals(typeof(long)) ||
                 dataType.Equals(typeof(ushort)) ||
                 dataType.Equals(typeof(uint)) ||
                 dataType.Equals(typeof(ulong)) ||
                 dataType.Equals(typeof(decimal)) ||
                 dataType.Equals(typeof(double)) ||
                 dataType.Equals(typeof(float)) ||
                 dataType.Equals(typeof(byte)) ||
                 dataType.Equals(typeof(sbyte)))
        {
            ret = new DataGridTextBoxColumn(prop, "G", isDefault);
        }
        else
        {
            ret = new DataGridTextBoxColumn(prop, isDefault);
        }

        return ret;
    }

    internal void ResetRelationsList()
    {
        relationsList.Clear();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string MappingName
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler MappingNameChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal ArrayList RelationsList
    {
        get
        {
            return relationsList;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual GridColumnStylesCollection GridColumnStyles
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal void SetInternalDataGrid(DataGrid dG, bool force)
    {
        if (dataGrid is not null && dataGrid.Equals(dG) && !force)
            return;
        else
        {
            dataGrid = dG;
            if (dG is not null && dG.Initializing)
                return;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual DataGrid DataGrid
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ReadOnly
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ReadOnlyChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    public bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber)
    {
        throw new PlatformNotSupportedException();
    }

    public bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort)
    {
        throw new PlatformNotSupportedException();
    }

    internal void InvalidateColumn(DataGridColumnStyle column)
    {
        int index = GridColumnStyles.IndexOf(column);
        if (index >= 0 && DataGrid is not null)
            DataGrid.InvalidateColumn(index);
    }

    private void OnColumnCollectionChanged(object sender, CollectionChangeEventArgs e)
    {
        try
        {
            DataGrid grid = DataGrid;
            DataGridColumnStyle col = e.Element as DataGridColumnStyle;
            if (e.Action == CollectionChangeAction.Add)
            {
                if (col is not null)
                    col.SetDataGridInternalInColumn(grid);
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                if (col is not null)
                    col.SetDataGridInternalInColumn(null);
            }
            else
            {
                // refresh
                Debug.Assert(e.Action == CollectionChangeAction.Refresh, "there are only Add, Remove and Refresh in the CollectionChangeAction");
            }

            if (grid is not null)
                grid.OnColumnCollectionChanged(this, e);
        }
        finally
        {
        }
    }

    protected virtual void OnReadOnlyChanged(EventArgs e)
    {
        EventHandler eh = Events[EventReadOnly] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnMappingNameChanged(EventArgs e)
    {
        EventHandler eh = Events[EventMappingName] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnAlternatingBackColorChanged(EventArgs e)
    {
        EventHandler eh = Events[EventAlternatingBackColor] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnForeColorChanged(EventArgs e)
    {
        EventHandler eh = Events[EventBackColor] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnBackColorChanged(EventArgs e)
    {
        EventHandler eh = Events[EventForeColor] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnAllowSortingChanged(EventArgs e)
    {
        EventHandler eh = Events[EventAllowSorting] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnGridLineColorChanged(EventArgs e)
    {
        EventHandler eh = Events[EventGridLineColor] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnGridLineStyleChanged(EventArgs e)
    {
        EventHandler eh = Events[EventGridLineStyle] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnHeaderBackColorChanged(EventArgs e)
    {
        EventHandler eh = Events[EventHeaderBackColor] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnHeaderFontChanged(EventArgs e)
    {
        EventHandler eh = Events[EventHeaderFont] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnHeaderForeColorChanged(EventArgs e)
    {
        EventHandler eh = Events[EventHeaderForeColor] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnLinkColorChanged(EventArgs e)
    {
        EventHandler eh = Events[EventLinkColor] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnLinkHoverColorChanged(EventArgs e)
    {
        EventHandler eh = Events[EventLinkHoverColor] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnPreferredRowHeightChanged(EventArgs e)
    {
        EventHandler eh = Events[EventPreferredRowHeight] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnPreferredColumnWidthChanged(EventArgs e)
    {
        EventHandler eh = Events[EventPreferredColumnWidth] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnColumnHeadersVisibleChanged(EventArgs e)
    {
        EventHandler eh = Events[EventColumnHeadersVisible] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnRowHeadersVisibleChanged(EventArgs e)
    {
        EventHandler eh = Events[EventRowHeadersVisible] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnRowHeaderWidthChanged(EventArgs e)
    {
        EventHandler eh = Events[EventRowHeaderWidth] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnSelectionForeColorChanged(EventArgs e)
    {
        EventHandler eh = Events[EventSelectionForeColor] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected virtual void OnSelectionBackColorChanged(EventArgs e)
    {
        EventHandler eh = Events[EventSelectionBackColor] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GridColumnStylesCollection cols = GridColumnStyles;
            if (cols is not null)
            {
                for (int i = 0; i < cols.Count; i++)
                    cols[i].Dispose();
            }
        }

        base.Dispose(disposing);
    }

    internal bool IsDefault
    {
        get;
    }
}
