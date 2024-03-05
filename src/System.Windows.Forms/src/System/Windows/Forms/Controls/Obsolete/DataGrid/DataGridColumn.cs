// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("DataGridColumnStyle has been deprecated.")]
public abstract class DataGridColumnStyle : Component, IDataGridColumnStyleEditingNotificationService
{
    private DataGridTableStyle dataGridTableStyle;
    internal int fontHeight = -1;
    private string mappingName = "";
    private string headerName = "";
    private bool invalid;
    private string nullText = "SR.GetString(SR.DataGridNullText)";
    private bool updating;
    internal int width = -1;
    private bool isDefault;
    private static readonly object EventAlignment = new object();
    private static readonly object EventPropertyDescriptor = new object();
    private static readonly object EventHeaderText = new object();
    private static readonly object EventMappingName = new object();
    private static readonly object EventNullText = new object();
    private static readonly object EventReadOnly = new object();
    private static readonly object EventWidth = new object();

    public DataGridColumnStyle()
    {
        throw new PlatformNotSupportedException();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")] // Shipped like this in Everett.
    public DataGridColumnStyle(PropertyDescriptor prop) : this()
    {
        throw new PlatformNotSupportedException();
    }

    internal DataGridColumnStyle(PropertyDescriptor prop, bool isDefault) : this(prop)
    {
        this.isDefault = isDefault;
        if (isDefault)
        {
            // take the header name from the property name
            headerName = prop.Name;
            mappingName = prop.Name;
        }
    }

#if DEBUG
    internal bool IsDefault
    {
        get
        {
            return isDefault;
        }
    }
#endif // debug

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual HorizontalAlignment Alignment
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
    public event EventHandler AlignmentChanged
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

    // PM team has reviewed and decided on naming changes already
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    protected internal virtual void UpdateUI(CurrencyManager source, int rowNum, string displayText)
    {
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public AccessibleObject HeaderAccessibleObject
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual PropertyDescriptor PropertyDescriptor
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
    public event EventHandler PropertyDescriptorChanged
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

    protected virtual AccessibleObject CreateHeaderAccessibleObject()
    {
        return new DataGridColumnHeaderAccessibleObject(this);
    }

    protected virtual void SetDataGrid(DataGrid value)
    {
        SetDataGridInColumn(value);
    }

    protected virtual void SetDataGridInColumn(DataGrid value)
    {
        // we need to set up the PropertyDescriptor
        if (PropertyDescriptor is null && value is not null)
        {
            CurrencyManager lm = value.ListManager;
            if (lm is null)
                return;
            PropertyDescriptorCollection propCollection = lm.GetItemProperties();
            int propCount = propCollection.Count;
            for (int i = 0; i < propCollection.Count; i++)
            {
                PropertyDescriptor prop = propCollection[i];
                if (!typeof(IList).IsAssignableFrom(prop.PropertyType) && prop.Name.Equals(HeaderText))
                {
                    PropertyDescriptor = prop;
                    return;
                }
            }
        }
    }

    internal void SetDataGridInternalInColumn(DataGrid value)
    {
        if (value is null || value.Initializing)
            return;
        SetDataGridInColumn(value);
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual DataGridTableStyle DataGridTableStyle
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal void SetDataGridTableInColumn(DataGridTableStyle value, bool force)
    {
        if (dataGridTableStyle is not null && dataGridTableStyle.Equals(value) && !force)
            return;
        if (value is not null)
        {
            if (value.DataGrid is not null && !value.DataGrid.Initializing)
            {
                SetDataGridInColumn(value.DataGrid);
            }
        }

        dataGridTableStyle = value;
    }

    protected int FontHeight
    {
        get
        {
            if (fontHeight != -1)
            {
                return fontHeight;
            }
            else if (DataGridTableStyle is not null)
            {
                return DataGridTableStyle.DataGrid.FontHeight;
            }
            else
            {
                return DataGridTableStyle.defaultFontHeight;
            }
        }
    }

    /// <devdoc>
    ///    <para>
    ///       Indicates whether the Font property should be persisted.
    ///    </para>
    /// </devdoc>
    private bool ShouldSerializeFont()
    {
        return false;
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler FontChanged
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
    public virtual string HeaderText
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
    public event EventHandler HeaderTextChanged
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

    private bool ShouldSerializeHeaderText()
    {
        return (headerName.Length != 0);
    }

    public void ResetHeaderText()
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual string NullText
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
    public event EventHandler NullTextChanged
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

#if false
        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Visible"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the column is visible.
        ///    </para>
        /// </devdoc>
        [DefaultValue(true)]
        public virtual bool Visible {
            get {
                return visible;
            }
            set {
                if (visible == value)
                    return;
                visible = value;
                RaisePropertyChanged(EventArgs.Empty"Visible");
                Invalidate();
            }
        }
#endif

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual int Width
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
    public event EventHandler WidthChanged
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

    protected void BeginUpdate()
    {
        updating = true;
    }

    protected void EndUpdate()
    {
        updating = false;
        if (invalid)
        {
            invalid = false;
            Invalidate();
        }
    }

    internal virtual bool WantArrows
    {
        get
        {
            return false;
        }
    }

    internal virtual string GetDisplayText(object value)
    {
        return value.ToString();
    }

    private void ResetNullText()
    {
        NullText = "SR.GetString(SR.DataGridNullText)";
    }

    private bool ShouldSerializeNullText()
    {
        return (!"SR.GetString(SR.DataGridNullText)".Equals(nullText));
    }

    protected internal abstract Size GetPreferredSize(Graphics g, object value);

    protected internal abstract int GetMinimumHeight();

    protected internal abstract int GetPreferredHeight(Graphics g, object value);

    // PM team has reviewed and decided on naming changes already
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    protected internal virtual object GetColumnValueAtRow(CurrencyManager source, int rowNum)
    {
        CheckValidDataSource(source);
        if (PropertyDescriptor is null)
        {
            throw new InvalidOperationException("SR.GetString(SR.DataGridColumnNoPropertyDescriptor)");
        }

        object value = PropertyDescriptor.GetValue(source[rowNum]);
        return value;
    }

    protected virtual void Invalidate()
    {
        if (updating)
        {
            invalid = true;
            return;
        }

        DataGridTableStyle table = DataGridTableStyle;
        if (table is not null)
            table.InvalidateColumn(this);
    }

    protected void CheckValidDataSource(CurrencyManager value)
    {
        if (value is null)
        {
            throw new ArgumentNullException("DataGridColumnStyle.CheckValidDataSource(DataSource value), value is null");
        }

        PropertyDescriptor myPropDesc = PropertyDescriptor;
        if (myPropDesc is null)
        {
            throw new InvalidOperationException("SR.GetString(SR.DataGridColumnUnbound, HeaderText)");
        }

#if false
            DataTable myDataTable = myTable.DataTable;
            if (myDataColumn.Table != myDataTable) {
                throw new InvalidOperationException(SR.GetString(SR.DataGridColumnDataSourceMismatch, Header));
            }

            /* FOR DEMO: danielhe: DataGridColumnStyle::CheckValidDataSource: make the check better */
            if (((DataView) value.DataSource).Table is null) {
                throw new InvalidOperationException(SR.GetString(SR.DataGridColumnNoDataTable, Header));
            }
            else {
                /* FOR DEMO: danielhe: DataGridColumnStyle::CheckValidDataSource: make the check better */
                if (!myTable.DataTable.Equals(((DataView) value.DataSource).Table)) {
                    throw new InvalidOperationException(SR.GetString(SR.DataGridColumnNoDataSource, Header, myTable.DataTable.TableName));
                }
            }
#endif // false
    }

    // PM team has reviewed and decided on naming changes already
    protected internal abstract void Abort(int rowNum);

    // PM team has reviewed and decided on naming changes already
    protected internal abstract bool Commit(CurrencyManager dataSource, int rowNum);

    protected internal virtual void Edit(CurrencyManager source,
                                         int rowNum,
                                         Rectangle bounds,
                                         bool readOnly)
    {
        Edit(source, rowNum, bounds, readOnly, null, true);
    }

    protected internal virtual void Edit(CurrencyManager source,
                               int rowNum,
                               Rectangle bounds,
                               bool readOnly,
                               string displayText)
    {
        Edit(source, rowNum, bounds, readOnly, displayText, true);
    }

    protected internal abstract void Edit(CurrencyManager source,
                                int rowNum,
                                Rectangle bounds,
                                bool readOnly,
                                string displayText,
                                bool cellIsVisible);

    internal virtual bool MouseDown(int rowNum, int x, int y)
    {
        return false;
    }

    protected internal virtual void EnterNullValue()
    {
    }

    internal virtual bool KeyPress(int rowNum, Keys keyData)
    {
        // if this is read only then do not do anything
        if (ReadOnly || (DataGridTableStyle is not null && DataGridTableStyle.DataGrid is not null && DataGridTableStyle.DataGrid.ReadOnly))
            return false;
        if (keyData == (Keys.Control | Keys.NumPad0) || keyData == (Keys.Control | Keys.D0))
        {
            EnterNullValue();
            return true;
        }

        return false;
    }

    protected internal virtual void ConcedeFocus()
    {
    }

    protected internal abstract void Paint(Drawing.Graphics g1, Graphics g, Rectangle bounds, CurrencyManager source, int rowNum);

    protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight);

    protected internal virtual void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum,
                                Brush backBrush, Brush foreBrush, bool alignToRight)
    {
        Paint(g, bounds, source, rowNum, alignToRight);
    }

    private void OnPropertyDescriptorChanged(EventArgs e)
    {
        EventHandler eh = Events[EventPropertyDescriptor] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    private void OnAlignmentChanged(EventArgs e)
    {
        EventHandler eh = Events[EventAlignment] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    private void OnHeaderTextChanged(EventArgs e)
    {
        EventHandler eh = Events[EventHeaderText] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    private void OnMappingNameChanged(EventArgs e)
    {
        EventHandler eh = Events[EventMappingName] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    private void OnReadOnlyChanged(EventArgs e)
    {
        EventHandler eh = Events[EventReadOnly] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    private void OnNullTextChanged(EventArgs e)
    {
        EventHandler eh = Events[EventNullText] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    private void OnWidthChanged(EventArgs e)
    {
        EventHandler eh = Events[EventWidth] as EventHandler;
        if (eh is not null)
            eh(this, e);
    }

    protected internal virtual void SetColumnValueAtRow(CurrencyManager source, int rowNum, object value)
    {
        CheckValidDataSource(source);

        if (source.Position != rowNum)
            throw new ArgumentException("SR.GetString(SR.DataGridColumnListManagerPosition)");
        if (source[rowNum] is IEditableObject)
            ((IEditableObject)source[rowNum]).BeginEdit();
        PropertyDescriptor.SetValue(source[rowNum], value);
    }

    internal protected virtual void ColumnStartedEditing(Control editingControl)
    {
        DataGridTableStyle.DataGrid.ColumnStartedEditing(editingControl);
    }

    void IDataGridColumnStyleEditingNotificationService.ColumnStartedEditing(Control editingControl)
    {
        ColumnStartedEditing(editingControl);
    }

    protected internal virtual void ReleaseHostedControl()
    {
    }

    protected class CompModSwitches
    {
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static TraceSwitch DGEditColumnEditing
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }
    }

    [Runtime.InteropServices.ComVisible(true)]
    [Obsolete("DataGridColumnHeaderAccessibleObject has been deprecated.")]
    protected class DataGridColumnHeaderAccessibleObject : AccessibleObject
    {
        public DataGridColumnHeaderAccessibleObject(DataGridColumnStyle owner) : this()
        {
            throw new PlatformNotSupportedException();
        }

        public DataGridColumnHeaderAccessibleObject() : base()
        {
            throw new PlatformNotSupportedException();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Rectangle Bounds
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Name
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        protected DataGridColumnStyle Owner
        {
            get;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleObject Parent
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        private DataGrid DataGrid
        {
            get;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleRole Role
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
