// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the appearance and text formatting and behavior of a <see cref='DataGrid'/> control column.
    /// </summary>
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [DefaultProperty(nameof(HeaderText))]
    public abstract class DataGridColumnStyle : Component, IDataGridColumnStyleEditingNotificationService
    {
        private HorizontalAlignment _alignment = HorizontalAlignment.Left;
        private PropertyDescriptor _propertyDescriptor = null;
        private DataGridTableStyle _dataGridTableStyle = null;
        private readonly Font _font = null;
        private string _mappingName = string.Empty;
        private string _headerName = string.Empty;
        private bool _invalid = false;
        private string _nullText = SR.DataGridNullText;
        private bool _readOnly = false;
        private bool _updating = false;
        internal int _width = -1;
        private AccessibleObject _headerAccessibleObject = null;

        private static readonly object s_alignmentEvent = new object();
        private static readonly object s_propertyDescriptorEvent = new object();
        private static readonly object s_headerTextEvent = new object();
        private static readonly object s_mappingNameEvent = new object();
        private static readonly object s_nullTextEvent = new object();
        private static readonly object s_readOnlyEvent = new object();
        private static readonly object s_widthEvent = new object();

        /// <summary>
        ///  In a derived class, initializes a new instance of the
        /// <see cref='DataGridColumnStyle'/> class.
        /// </summary>
        public DataGridColumnStyle()
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='DataGridColumnStyle'/>
        ///  class with the specified <see cref='T:System.ComponentModel.PropertyDescriptor'/>.
        /// </summary>
        public DataGridColumnStyle(PropertyDescriptor prop) : this()
        {
            PropertyDescriptor = prop;
            if (prop != null)
            {
                _readOnly = prop.IsReadOnly;
            }
        }

        private protected DataGridColumnStyle(PropertyDescriptor prop, bool isDefault) : this(prop)
        {
#if DEBUG
            IsDefault = isDefault;
#endif
            if (isDefault && prop != null)
            {
                // take the header name from the property name
                _headerName = prop.Name;
                _mappingName = prop.Name;
            }
        }

#if DEBUG
        internal bool IsDefault { get; }
#endif

        /// <summary>
        ///  Gets or sets the alignment of text in a column.
        /// </summary>
        [SRCategory(nameof(SR.CatDisplay))]
        [Localizable(true)]
        [DefaultValue(HorizontalAlignment.Left)]
        public virtual HorizontalAlignment Alignment
        {
            get => _alignment;
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridLineStyle));
                }

                if (_alignment != value)
                {
                    _alignment = value;
                    OnAlignmentChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public event EventHandler AlignmentChanged
        {
            add => Events.AddHandler(s_alignmentEvent, value);
            remove => Events.RemoveHandler(s_alignmentEvent, value);
        }

        /// <summary>
        ///  When overridden in a derived class, updates the value of a specified row with
        ///  the given text.
        /// </summary>
        protected internal virtual void UpdateUI(CurrencyManager source, int rowNum, string displayText)
        {
        }

        /// <summary>
        ///  Gets or sets the background color of the column.
        /// </summary>
        [Browsable(false)]
        public AccessibleObject HeaderAccessibleObject
        {
            get => _headerAccessibleObject ?? (_headerAccessibleObject = CreateHeaderAccessibleObject());
        }

        /// <summary>
        ///  Gets or sets the <see cref='Data.DataColumn'/> that determines the
        ///  attributes of data displayed by the <see cref='DataGridColumnStyle'/>.
        /// </summary>
        [DefaultValue(null)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual PropertyDescriptor PropertyDescriptor
        {
            get => _propertyDescriptor;
            set
            {
                if (_propertyDescriptor != value)
                {
                    _propertyDescriptor = value;
                    OnPropertyDescriptorChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler PropertyDescriptorChanged
        {
            add => Events.AddHandler(s_propertyDescriptorEvent, value);
            remove => Events.RemoveHandler(s_propertyDescriptorEvent, value);
        }

        protected virtual AccessibleObject CreateHeaderAccessibleObject()
        {
            return new DataGridColumnHeaderAccessibleObject(this);
        }

        /// <summary>
        ///  When overridden in a derived class, sets the <see cref='DataGrid'/>
        ///  control that this column belongs to.
        /// </summary>
        protected virtual void SetDataGrid(DataGrid value)
        {
            SetDataGridInColumn(value);
        }

        /// <summary>
        ///  When overridden in a derived class, sets the <see cref='DataGrid'/>
        ///  for the column.
        /// </summary>
        protected virtual void SetDataGridInColumn(DataGrid value)
        {
            // we need to set up the PropertyDescriptor
            if (PropertyDescriptor == null && value != null)
            {
                CurrencyManager lm = value.ListManager;
                if (lm == null)
                {
                    return;
                }

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
            if (value == null || value.Initializing)
            {
                return;
            }

            SetDataGridInColumn(value);
        }

        /// <summary>
        ///  Gets the System.Windows.Forms.DataGridTableStyle for the column.
        /// </summary>
        [Browsable(false)]
        public virtual DataGridTableStyle DataGridTableStyle => _dataGridTableStyle;

        internal void SetDataGridTableInColumn(DataGridTableStyle value, bool force)
        {
            if (_dataGridTableStyle != null && _dataGridTableStyle.Equals(value) && !force)
            {
                return;
            }

            if (value != null && value.DataGrid != null && !value.DataGrid.Initializing)
            {
                SetDataGridInColumn(value.DataGrid);
            }

            _dataGridTableStyle = value;
        }

        /// <summary>
        ///  Gets the height of the column's font.
        /// </summary>
        protected int FontHeight
        {
            get => DataGridTableStyle?.DataGrid?.FontHeight ?? DataGridTableStyle.defaultFontHeight;
        }

        /// <summary>
        ///  Indicates whether the Font property should be persisted.
        /// </summary>
        private bool ShouldSerializeFont() => _font != null;

        public event EventHandler FontChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        ///  Gets or sets the text of the column header.
        /// </summary>
        [Localizable(true)]
        [SRCategory(nameof(SR.CatDisplay))]
        public virtual string HeaderText
        {
            get => _headerName;
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (!_headerName.Equals(value))
                {
                    _headerName = value;
                    OnHeaderTextChanged(EventArgs.Empty);
                    // we only invalidate columns that are visible ( ie, their propertyDescriptor is not null)
                    if (PropertyDescriptor != null)
                    {
                        Invalidate();
                    }
                }
            }
        }

        public event EventHandler HeaderTextChanged
        {
            add => Events.AddHandler(s_headerTextEvent, value);
            remove => Events.RemoveHandler(s_headerTextEvent, value);
        }

        [Editor("System.Windows.Forms.Design.DataGridColumnStyleMappingNameEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor))]
        [Localizable(true)]
        [DefaultValue("")]
        public string MappingName
        {
            get => _mappingName;
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (!_mappingName.Equals(value))
                {
                    string originalMappingName = _mappingName;
                    _mappingName = value;
                    try
                    {
                        _dataGridTableStyle?.GridColumnStyles.CheckForMappingNameDuplicates(this);
                    }
                    catch
                    {
                        _mappingName = originalMappingName;
                        throw;
                    }

                    OnMappingNameChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler MappingNameChanged
        {
            add => Events.AddHandler(s_mappingNameEvent, value);
            remove => Events.RemoveHandler(s_mappingNameEvent, value);
        }

        /// <summary>
        ///  Indicates whether the System.Windows.Forms.DataGridColumnStyle.HeaderText property
        ///  should be persisted.
        /// </summary>
        private bool ShouldSerializeHeaderText()
        {
            return (_headerName.Length != 0);
        }

        /// <summary>
        ///  Resets the System.Windows.Forms.DataGridColumnStyle.HeaderText to its default value.
        /// </summary>
        public void ResetHeaderText() => HeaderText = string.Empty;

        /// <summary>
        ///  Gets or sets the text that is displayed when the column contains a null
        ///  value.
        /// </summary>
        [Localizable(true)]
        [SRCategory(nameof(SR.CatDisplay))]
        public virtual string NullText
        {
            get => _nullText;
            set
            {
                if (_nullText != value)
                {
                    _nullText = value;
                    OnNullTextChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public event EventHandler NullTextChanged
        {
            add => Events.AddHandler(s_nullTextEvent, value);
            remove => Events.RemoveHandler(s_nullTextEvent, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the data in the column cannot be edited.
        /// </summary>
        [DefaultValue(false)]
        public virtual bool ReadOnly
        {
            get => _readOnly;
            set
            {
                if (_readOnly != value)
                {
                    _readOnly = value;
                    OnReadOnlyChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler ReadOnlyChanged
        {
            add => Events.AddHandler(s_readOnlyEvent, value);
            remove => Events.RemoveHandler(s_readOnlyEvent, value);
        }

        /// <summary>
        ///  Gets or sets the width of the column.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true)]
        [DefaultValue(100)]
        public virtual int Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    DataGrid grid = DataGridTableStyle?.DataGrid;
                    if (grid != null)
                    {
                        // rearrange the scroll bars
                        grid.PerformLayout();

                        // force the grid to repaint
                        grid.InvalidateInside();
                    }

                    OnWidthChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler WidthChanged
        {
            add => Events.AddHandler(s_widthEvent, value);
            remove => Events.RemoveHandler(s_widthEvent, value);
        }

        /// <summary>
        ///  Suspends the painting of the column until the <see cref='EndUpdate'/>
        ///  method is called.
        /// </summary>
        protected void BeginUpdate() => _updating = true;

        /// <summary>
        ///  Resumes the painting of columns suspended by calling the
        /// <see cref='BeginUpdate'/> method.
        /// </summary>
        protected void EndUpdate()
        {
            _updating = false;
            if (_invalid)
            {
                _invalid = false;
                Invalidate();
            }
        }

        internal virtual string GetDisplayText(object value) => value.ToString();

        private void ResetNullText() => NullText = SR.DataGridNullText;

        private bool ShouldSerializeNullText() => !SR.DataGridNullText.Equals(_nullText);

        /// <summary>
        ///  When overridden in a derived class, gets the optimum width and height of the
        ///  specified value.
        /// </summary>
        protected internal abstract Size GetPreferredSize(Graphics g, object value);

        /// <summary>
        ///  Gets the minimum height of a row.
        /// </summary>
        protected internal abstract int GetMinimumHeight();

        /// <summary>
        ///  When overridden in a derived class, gets the height to be used in for
        ///  automatically resizing columns.
        /// </summary>
        protected internal abstract int GetPreferredHeight(Graphics g, object value);

        /// <summary>
        ///  Gets the value in the specified row from the specified System.Windows.Forms.ListManager.
        /// </summary>
        protected internal virtual object GetColumnValueAtRow(CurrencyManager source, int rowNum)
        {
            CheckValidDataSource(source);
            PropertyDescriptor descriptor = PropertyDescriptor;
            if (descriptor == null)
            {
                throw new InvalidOperationException(SR.DataGridColumnNoPropertyDescriptor);
            }

            return descriptor.GetValue(source[rowNum]);
        }

        /// <summary>
        ///  Redraws the column and causes a paint message to be sent to the control.
        /// </summary>
        protected virtual void Invalidate()
        {
            if (_updating)
            {
                _invalid = true;
                return;
            }

            DataGridTableStyle?.InvalidateColumn(this);
        }

        /// <summary>
        ///  Checks if the specified DataView is valid.
        /// </summary>
        protected void CheckValidDataSource(CurrencyManager value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // The code may delete a gridColumn that was editing.
            // In that case, we still have to push the value into the backend
            // and we only need the propertyDescriptor to push the value.
            // (take a look at gridEditAndDeleteEditColumn)
            if (PropertyDescriptor == null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridColumnUnbound, HeaderText));
            }
        }

        /// <summary>
        ///  When overridden in a derived class, initiates a request to interrrupt an edit
        ///  procedure.
        /// </summary>
        protected internal abstract void Abort(int rowNum);

        /// <summary>
        ///  When overridden in a derived class, inititates a request to complete an
        ///  editing procedure.
        /// </summary>
        protected internal abstract bool Commit(CurrencyManager dataSource, int rowNum);

        /// <summary>
        ///  When overridden in a deriving class, prepares a cell for editing.
        /// </summary>
        protected internal virtual void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly)
        {
            Edit(source, rowNum, bounds, readOnly, null, true);
        }

        /// <summary>
        ///  Prepares the cell for editing, passing the specified <see cref='Data.DataView'/>,
        ///  row number, <see cref='Rectangle'/>, argument indicating whether
        ///  the column is read-only, and the text to display in the new control.
        /// </summary>
        protected internal virtual void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText)
        {
            Edit(source, rowNum, bounds, readOnly, displayText, true);
        }

        /// <summary>
        ///  When overridden in a deriving class, prepares a cell for editing.
        /// </summary>
        protected internal abstract void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible);

        /// <summary>
        ///  Indicates whether the a mouse down event occurred at the specified row, at
        ///  the specified x and y coordinates.
        /// </summary>
        internal virtual bool MouseDown(int rowNum, int x, int y)
        {
            return false;
        }

        /// <summary>
        ///  When overriden in a derived class, enters a <see cref='T:System.DBNull.Value' qualify='true'/>
        ///  into the column.
        /// </summary>
        protected internal virtual void EnterNullValue()
        {
        }

        /// <summary>
        ///  Provides a handler for determining which key was pressed, and whether to
        ///  process it.
        /// </summary>
        internal virtual bool KeyPress(int rowNum, Keys keyData)
        {
            // if this is read only then do not do anything
            if (ReadOnly || (DataGridTableStyle != null && DataGridTableStyle.DataGrid != null && DataGridTableStyle.DataGrid.ReadOnly))
            {
                return false;
            }
            if (keyData == (Keys.Control | Keys.NumPad0) || keyData == (Keys.Control | Keys.D0))
            {
                EnterNullValue();
                return true;
            }

            return false;
        }

        /// <summary>
        ///  When overridden in a derived class, directs the column to concede focus with
        ///  an appropriate action.
        /// </summary>
        protected internal virtual void ConcedeFocus()
        {
        }

        /// <summary>
        ///  Paints the a <see cref='DataGridColumnStyle'/> with the specified
        /// <see cref='Graphics'/>, <see cref='Rectangle'/>,
        ///  System.Windows.Forms.CurrencyManager, and row number.
        /// </summary>
        protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum);

        /// <summary>
        ///  When overridden in a derived class, paints a <see cref='DataGridColumnStyle'/>
        ///  with the specified <see cref='Graphics'/>, <see cref='Rectangle'/>,
        ///  see Rectangle, row number, and alignment.
        /// </summary>
        protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight);

        /// <summary>
        ///  Paints a <see cref='DataGridColumnStyle'/> with the specified <see cref='Graphics'/>, <see cref='Rectangle'/>, see System.Data.DataView, row number, background color, foreground color, and alignment.
        /// </summary>
        protected internal virtual void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum,
                                    Brush backBrush, Brush foreBrush, bool alignToRight)
        {
            Paint(g, bounds, source, rowNum, alignToRight);
        }

        private void OnPropertyDescriptorChanged(EventArgs e)
        {
            EventHandler eh = Events[s_propertyDescriptorEvent] as EventHandler;
            eh?.Invoke(this, e);
        }

        private void OnAlignmentChanged(EventArgs e)
        {
            EventHandler eh = Events[s_alignmentEvent] as EventHandler;
            eh?.Invoke(this, e);
        }

        private void OnHeaderTextChanged(EventArgs e)
        {
            EventHandler eh = Events[s_headerTextEvent] as EventHandler;
            eh?.Invoke(this, e);
        }

        private void OnMappingNameChanged(EventArgs e)
        {
            EventHandler eh = Events[s_mappingNameEvent] as EventHandler;
            eh?.Invoke(this, e);
        }

        private void OnReadOnlyChanged(EventArgs e)
        {
            EventHandler eh = Events[s_readOnlyEvent] as EventHandler;
            eh?.Invoke(this, e);
        }

        private void OnNullTextChanged(EventArgs e)
        {
            EventHandler eh = Events[s_nullTextEvent] as EventHandler;
            eh?.Invoke(this, e);
        }

        private void OnWidthChanged(EventArgs e)
        {
            EventHandler eh = Events[s_widthEvent] as EventHandler;
            eh?.Invoke(this, e);
        }

        /// <summary>
        ///  Sets the value in a specified row with the value from a specified see DataView.
        /// </summary>
        protected internal virtual void SetColumnValueAtRow(CurrencyManager source, int rowNum, object value)
        {
            CheckValidDataSource(source);
            PropertyDescriptor descriptor = PropertyDescriptor;
            if (descriptor == null)
            {
                throw new InvalidOperationException(SR.DataGridColumnNoPropertyDescriptor);
            }

            if (source.Position != rowNum)
            {
                throw new ArgumentException(SR.DataGridColumnListManagerPosition, nameof(rowNum));
            }
            if (source[rowNum] is IEditableObject editableObject)
            {
                editableObject.BeginEdit();
            }

            descriptor.SetValue(source[rowNum], value);
        }

        protected internal virtual void ColumnStartedEditing(Control editingControl)
        {
            DataGridTableStyle?.DataGrid?.ColumnStartedEditing(editingControl);
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
            private static TraceSwitch dgEditColumnEditing;

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
        }

        [ComVisible(true)]
        protected class DataGridColumnHeaderAccessibleObject : AccessibleObject
        {
            public DataGridColumnHeaderAccessibleObject(DataGridColumnStyle owner) : this()
            {
                Debug.Assert(owner != null, "DataGridColumnHeaderAccessibleObject must have a valid owner DataGridColumn");
                Owner = owner;
            }

            public DataGridColumnHeaderAccessibleObject() : base()
            {
            }

            public override Rectangle Bounds
            {
                get
                {
                    // we need to get the width and the X coordinate of this column on the screen
                    // we can't just cache this, cause the column may be moved in the collection
                    if (Owner.PropertyDescriptor == null)
                    {
                        return Rectangle.Empty;
                    }

                    DataGrid dg = DataGrid;
                    if (dg.DataGridRowsLength == 0)
                    {
                        return Rectangle.Empty;
                    }

                    // We need to find this column's offset in the gridColumnCollection.
                    GridColumnStylesCollection cols = Owner._dataGridTableStyle.GridColumnStyles;
                    int offset = -1;
                    for (int i = 0; i < cols.Count; i++)
                    {
                        if (cols[i] == Owner)
                        {
                            offset = i;
                            break;
                        }
                    }

                    Debug.Assert(offset >= 0, "this column must be in a collection, otherwise its bounds are useless");
                    Rectangle rect = dg.GetCellBounds(0, offset);
                    // Now add the Y coordinate of the datagrid.Layout.Data.
                    // It should be the same as dataGrid.Layout.ColumnHeaders
                    rect.Y = dg.GetColumnHeadersRect().Y;
                    return dg.RectangleToScreen(rect);
                }
            }

            public override string Name => Owner._headerName;

            protected DataGridColumnStyle Owner { get; }

            public override AccessibleObject Parent => DataGrid.AccessibilityObject;

            private DataGrid DataGrid => Owner._dataGridTableStyle.dataGrid;

            public override AccessibleRole Role => AccessibleRole.ColumnHeader;

            public override AccessibleObject Navigate(AccessibleNavigation navdir)
            {
                switch (navdir)
                {
                    case AccessibleNavigation.Right:
                    case AccessibleNavigation.Next:
                    case AccessibleNavigation.Down:
                        return Parent.GetChild(1 + Owner._dataGridTableStyle.GridColumnStyles.IndexOf(Owner) + 1);
                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                    case AccessibleNavigation.Previous:
                        return Parent.GetChild(1 + Owner._dataGridTableStyle.GridColumnStyles.IndexOf(Owner) - 1);

                }

                return null;
            }
        }
    }
}
