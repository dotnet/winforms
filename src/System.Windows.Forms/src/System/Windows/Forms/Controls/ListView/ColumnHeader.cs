// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms;

/// <summary>
///  Displays a single column header in a <see cref="Forms.ListView"/> control.
/// </summary>
[DesignTimeVisible(false)]
[DefaultProperty(nameof(Text))]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
[ToolboxItem(false)]
[TypeConverter(typeof(ColumnHeaderConverter))]
public partial class ColumnHeader : Component, ICloneable
{
    // disable C# compiler warning #0414: field assigned unused value
#pragma warning disable 0414
    internal int _index = -1;
#pragma warning restore 0414
    internal string? _text;
    internal string _name;
    internal int _width = 60;
    internal int _correspondingListViewSubItemIndex = -1;

    // Use TextAlign property instead of this member variable, always
    private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
    private bool _textAlignInitialized;
    private AccessibleObject? _accessibilityObject;
    private readonly ColumnHeaderImageListIndexer _imageIndexer;

    // We need to send some messages to ListView when it gets initialized.
    internal ListView? OwnerListview
    {
        get
        {
            return ListView;
        }
        set
        {
            int width = Width;

            ListView = value;

            // The below properties are set into the listview.
            Width = width;
        }
    }

    /// <summary>
    ///  Creates a new ColumnHeader object
    /// </summary>
    public ColumnHeader()
    {
        _imageIndexer = new ColumnHeaderImageListIndexer(this);
        _name = string.Empty;
    }

    /// <summary>
    ///  Creates a new ColumnHeader object
    /// </summary>
    public ColumnHeader(int imageIndex) : this()
    {
        ImageIndex = imageIndex;
    }

    /// <summary>
    ///  Creates a new ColumnHeader object
    /// </summary>
    public ColumnHeader(string imageKey) : this()
    {
        ImageKey = imageKey;
    }

    internal AccessibleObject AccessibilityObject
    {
        get
        {
            _accessibilityObject ??= new ListViewColumnHeaderAccessibleObject(this);

            return _accessibilityObject;
        }
    }

    internal int ActualImageIndex_Internal
    {
        get
        {
            int imgIndex = _imageIndexer.ActualIndex;
            if (ImageList is null || ImageList.Images is null || imgIndex >= ImageList.Images.Count)
            {
                // the ImageIndex equivalent of a ImageKey that does not exist in the ImageList
                return -1;
            }
            else
            {
                return imgIndex;
            }
        }
    }

    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ColumnHeaderDisplayIndexDescr))]
    public int DisplayIndex
    {
        get
        {
            return DisplayIndexInternal;
        }

        set
        {
            // When the list is being deserialized we need
            // to take the display index as is. ListView
            // does correctly synchronize the indices.
            if (ListView is null)
            {
                DisplayIndexInternal = value;
                return;
            }

            if (value < 0 || value > (ListView.Columns.Count - 1))
            {
                throw new ArgumentOutOfRangeException(nameof(DisplayIndex), SR.ColumnHeaderBadDisplayIndex);
            }

            int lowDI = Math.Min(DisplayIndexInternal, value);
            int hiDI = Math.Max(DisplayIndexInternal, value);
            int[] colsOrder = new int[ListView.Columns.Count];

            // set the display indices. This is not an expensive operation because
            // we only set an integer in the column header class
            bool hdrMovedForward = value > DisplayIndexInternal;
            ColumnHeader? movedHdr = null;
            for (int i = 0; i < ListView.Columns.Count; i++)
            {
                ColumnHeader hdr = ListView.Columns[i];
                if (hdr.DisplayIndex == DisplayIndexInternal)
                {
                    movedHdr = hdr;
                }
                else if (hdr.DisplayIndex >= lowDI && hdr.DisplayIndex <= hiDI)
                {
                    hdr.DisplayIndexInternal -= hdrMovedForward ? 1 : -1;
                }

                if (i != Index)
                {
                    colsOrder[hdr.DisplayIndexInternal] = i;
                }
            }

            if (movedHdr is not null)
            {
                movedHdr.DisplayIndexInternal = value;
                colsOrder[movedHdr.DisplayIndexInternal] = movedHdr.Index;
            }

            SetDisplayIndices(colsOrder);
        }
    }

    internal int DisplayIndexInternal { get; set; } = -1;

    /// <summary>
    ///  The index of this column. This index does not necessarily correspond
    ///  to the current visual position of the column in the ListView, because the
    ///  user may order columns if the allowColumnReorder property is true.
    /// </summary>
    [Browsable(false)]
    public int Index
    {
        get
        {
            if (ListView is not null)
            {
                return ListView.GetColumnIndex(this);
            }

            return -1;
        }
    }

    [DefaultValue(ImageList.Indexer.DefaultIndex)]
    [TypeConverter(typeof(ImageIndexConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ImageIndex
    {
        get
        {
            if (_imageIndexer.Index != ImageList.Indexer.DefaultIndex && ImageList is not null && _imageIndexer.Index >= ImageList.Images.Count)
            {
                return ImageList.Images.Count - 1;
            }

            return _imageIndexer.Index;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, ImageList.Indexer.DefaultIndex);

            if (_imageIndexer.Index == value && value != ImageList.Indexer.DefaultIndex)
            {
                return;
            }

            _imageIndexer.Index = value;

            if (ListView is not null && ListView.IsHandleCreated)
            {
                ListView.SetColumnInfo(LVCOLUMNW_MASK.LVCF_IMAGE, this);
            }
        }
    }

    [Browsable(false)]
    public ImageList? ImageList
    {
        // we added the ImageList property so that the ImageIndexConverter can find our image list
        get
        {
            return _imageIndexer.ImageList;
        }
    }

    [DefaultValue(ImageList.Indexer.DefaultKey)]
    [TypeConverter(typeof(ImageKeyConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ImageKey
    {
        get
        {
            return _imageIndexer.Key;
        }
        set
        {
            if (value == _imageIndexer.Key && !string.Equals(value, ImageList.Indexer.DefaultKey))
            {
                return;
            }

            _imageIndexer.Key = value;

            if (ListView is not null && ListView.IsHandleCreated)
            {
                ListView.SetColumnInfo(LVCOLUMNW_MASK.LVCF_IMAGE, this);
            }
        }
    }

    /// <summary>
    ///  Returns the ListView control that this column is displayed in. May be null
    /// </summary>
    [Browsable(false)]
    public ListView? ListView { get; private set; }

    /// <summary>
    ///  The Name of the column header
    /// </summary>
    [Browsable(false)]
    [SRDescription(nameof(SR.ColumnHeaderNameDescr))]
    public string? Name
    {
        get
        {
            return WindowsFormsUtils.GetComponentName(this, _name);
        }
        set
        {
            if (value is null)
            {
                _name = string.Empty;
            }
            else
            {
                _name = value;
            }

            if (Site is not null)
            {
                Site.Name = value;
            }
        }
    }

    /// <summary>
    ///  The text displayed in the column header
    /// </summary>
    [Localizable(true)]
    [SRDescription(nameof(SR.ColumnCaption))]
    [AllowNull]
    public string Text
    {
        get
        {
            return _text ?? nameof(ColumnHeader);
        }
        set
        {
            if (value is null)
            {
                _text = string.Empty;
            }
            else
            {
                _text = value;
            }

            ListView?.SetColumnInfo(LVCOLUMNW_MASK.LVCF_TEXT, this);
        }
    }

    /// <summary>
    ///  The horizontal alignment of the text contained in this column
    /// </summary>
    [SRDescription(nameof(SR.ColumnAlignment))]
    [Localizable(true)]
    [DefaultValue(HorizontalAlignment.Left)]
    public HorizontalAlignment TextAlign
    {
        get
        {
            if (!_textAlignInitialized && (ListView is not null))
            {
                _textAlignInitialized = true;
                // See below for an explanation of (Index != 0)
                // Added !IsMirrored
                if ((Index != 0) && (ListView.RightToLeft == RightToLeft.Yes) && !ListView.IsMirrored)
                {
                    _textAlign = HorizontalAlignment.Right;
                }
            }

            return _textAlign;
        }
        set
        {
            // valid values are 0x0 to 0x2.
            SourceGenerated.EnumValidator.Validate(value);

            _textAlign = value;

            // The first column must be left-aligned
            if (Index == 0 && _textAlign != HorizontalAlignment.Left)
            {
                _textAlign = HorizontalAlignment.Left;
            }

            if (ListView is not null)
            {
                ListView.SetColumnInfo(LVCOLUMNW_MASK.LVCF_FMT, this);
                ListView.Invalidate();
            }
        }
    }

    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag { get; set; }

    internal int WidthInternal
    {
        get
        {
            return _width;
        }
    }

    /// <summary>
    ///  The width of the column in pixels.
    /// </summary>
    [SRDescription(nameof(SR.ColumnWidth))]
    [Localizable(true)]
    [DefaultValue(60)]
    public int Width
    {
        get
        {
            // Since we can't keep our private width in sync with the real width because
            // we don't get notified when the user changes it, we need to get this info
            // from the underlying control every time we're asked.
            // The underlying control will only report the correct width if it's in Report view
            if (ListView is not null && ListView.IsHandleCreated && !ListView.Disposing && ListView.View == View.Details)
            {
                // Make sure this column has already been added to the ListView, else just return width
                HWND hwndHdr = (HWND)PInvokeCore.SendMessage(ListView, PInvoke.LVM_GETHEADER);
                if (!hwndHdr.IsNull)
                {
                    int nativeColumnCount = (int)PInvokeCore.SendMessage(hwndHdr, PInvoke.HDM_GETITEMCOUNT);
                    if (Index < nativeColumnCount)
                    {
                        _width = (int)PInvokeCore.SendMessage(ListView, PInvoke.LVM_GETCOLUMNWIDTH, (WPARAM)Index);
                    }
                }
            }

            return _width;
        }
        set
        {
            _width = value;
            ListView?.SetColumnWidth(Index, ColumnHeaderAutoResizeStyle.None);
        }
    }

    public void AutoResize(ColumnHeaderAutoResizeStyle headerAutoResize)
    {
        if (headerAutoResize is < ColumnHeaderAutoResizeStyle.None or > ColumnHeaderAutoResizeStyle.ColumnContent)
        {
            throw new InvalidEnumArgumentException(nameof(headerAutoResize), (int)headerAutoResize, typeof(ColumnHeaderAutoResizeStyle));
        }

        ListView?.AutoResizeColumn(Index, headerAutoResize);
    }

    /// <summary>
    ///  Creates an identical ColumnHeader, unattached to any ListView
    /// </summary>
    public object Clone()
    {
        Type clonedType = GetType();
        ColumnHeader columnHeader;

        if (clonedType == typeof(ColumnHeader))
        {
            columnHeader = new ColumnHeader();
        }
        else
        {
            columnHeader = (ColumnHeader)Activator.CreateInstance(clonedType)!;
        }

        columnHeader._text = _text;
        columnHeader.Width = _width;
        columnHeader._textAlign = TextAlign;
        return columnHeader;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (ListView is not null)
            {
                int index = Index;
                if (index != -1)
                {
                    ListView.Columns.RemoveAt(index);
                }
            }
        }

        base.Dispose(disposing);
    }

    internal void ReleaseUiaProvider()
    {
        PInvoke.UiaDisconnectProvider(_accessibilityObject);
        _accessibilityObject = null;
    }

    private void ResetText()
    {
        Text = null;
    }

    // Set the display indices of the listview columns
    private unsafe void SetDisplayIndices(int[] cols)
    {
        if (ListView is not null && ListView.IsHandleCreated && !ListView.Disposing)
        {
            fixed (int* pCols = cols)
            {
                PInvokeCore.SendMessage(ListView, PInvoke.LVM_SETCOLUMNORDERARRAY, (WPARAM)cols.Length, (LPARAM)pCols);
            }
        }
    }

    private bool ShouldSerializeName()
    {
        return !string.IsNullOrEmpty(Name);
    }

    private bool ShouldSerializeDisplayIndex()
    {
        return DisplayIndex != Index;
    }

    internal bool ShouldSerializeText()
    {
        return _text is not null;
    }

    /// <summary>
    ///  Returns a string representation of this column header
    /// </summary>
    public override string ToString()
    {
        return $"{nameof(ColumnHeader)}: Text: {Text}";
    }
}
