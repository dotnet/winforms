// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the appearance and text formatting and behavior of a <see cref='System.Windows.Forms.DataGrid'/>
    /// control column.
    /// </summary>
    [
    ToolboxItem(false),
    DesignTimeVisible(false),
    DefaultProperty("Header"),
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors") // Shipped in Everett
    ]
    public abstract class DataGridColumnStyle : Component, IDataGridColumnStyleEditingNotificationService
    {

        private HorizontalAlignment alignment = HorizontalAlignment.Left;
        private PropertyDescriptor propertyDescriptor = null;
        private DataGridTableStyle dataGridTableStyle = null;
        private Font font = null;
        internal int fontHeight = -1;
        private string mappingName = "";
        private string headerName = "";
        private bool invalid = false;
        private string nullText = SR.DataGridNullText;
        private bool readOnly = false;
        private bool updating = false;
        internal int width = -1;
        private bool isDefault = false;
        AccessibleObject headerAccessibleObject = null;

        private static readonly object EventAlignment = new object();
        private static readonly object EventPropertyDescriptor = new object();
        private static readonly object EventHeaderText = new object();
        private static readonly object EventMappingName = new object();
        private static readonly object EventNullText = new object();
        private static readonly object EventReadOnly = new object();
        private static readonly object EventWidth = new object();

        /// <summary>
        /// In a derived class, initializes a new instance of the
        /// <see cref='System.Windows.Forms.DataGridColumnStyle'/> class.
        /// </summary>
        public DataGridColumnStyle()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.DataGridColumnStyle'/>
        /// class with the specified <see cref='T:System.ComponentModel.PropertyDescriptor'/>.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Changing this would be a breaking change.")]
        public DataGridColumnStyle(PropertyDescriptor prop) : this()
        {
            this.PropertyDescriptor = prop;
            if (prop != null)
                this.readOnly = prop.IsReadOnly;
        }

        internal DataGridColumnStyle(PropertyDescriptor prop, bool isDefault) : this(prop)
        {
            this.isDefault = isDefault;
            if (isDefault)
            {
                // take the header name from the property name
                this.headerName = prop.Name;
                this.mappingName = prop.Name;
            }
        }

#if DEBUG
        internal bool IsDefault {
            get {
                return this.isDefault;
            }
        }
#endif // debug

        /// <summary>
        /// Gets or sets the alignment of text in a column.
        /// </summary>
        [SRCategory(nameof(SR.CatDisplay)),
        Localizable(true),
        DefaultValue(HorizontalAlignment.Left)]
        public virtual HorizontalAlignment Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                //valid values are 0x0 to 0x2.
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridLineStyle));
                }
                if (alignment != value)
                {
                    alignment = value;
                    OnAlignmentChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public event EventHandler AlignmentChanged
        {
            add
            {
                Events.AddHandler(EventAlignment, value);
            }
            remove
            {
                Events.RemoveHandler(EventAlignment, value);
            }
        }

        /// <summary>
        /// When overridden in a derived class, updates the value of a specified row with
        /// the given text.
        /// </summary>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal virtual void UpdateUI(CurrencyManager source, int rowNum, string displayText)
        {
        }

        /// <summary>
        /// Gets or sets the background color of the column.
        /// </summary>
        [Browsable(false)]
        public AccessibleObject HeaderAccessibleObject
        {
            get
            {
                if (headerAccessibleObject == null)
                {
                    headerAccessibleObject = CreateHeaderAccessibleObject();
                }
                return headerAccessibleObject;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref='System.Data.DataColumn'/> that determines the
        /// attributes of data displayed by the <see cref='System.Windows.Forms.DataGridColumnStyle'/>.
        /// </summary>
        [DefaultValue(null), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return propertyDescriptor;
            }
            set
            {
                if (propertyDescriptor != value)
                {
                    propertyDescriptor = value;
                    OnPropertyDescriptorChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler PropertyDescriptorChanged
        {
            add
            {
                Events.AddHandler(EventPropertyDescriptor, value);
            }
            remove
            {
                Events.RemoveHandler(EventPropertyDescriptor, value);
            }
        }

        protected virtual AccessibleObject CreateHeaderAccessibleObject()
        {
            return new DataGridColumnHeaderAccessibleObject(this);
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref='System.Windows.Forms.DataGrid'/>
        /// control that this column belongs to.
        /// </summary>
        protected virtual void SetDataGrid(DataGrid value)
        {
            SetDataGridInColumn(value);
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref='System.Windows.Forms.DataGrid'/>
        /// for the column.
        /// </summary>
        protected virtual void SetDataGridInColumn(DataGrid value)
        {
            // we need to set up the PropertyDescriptor
            if (this.PropertyDescriptor == null && value != null)
            {
                CurrencyManager lm = value.ListManager;
                if (lm == null) return;
                PropertyDescriptorCollection propCollection = lm.GetItemProperties();
                int propCount = propCollection.Count;
                for (int i = 0; i < propCollection.Count; i++)
                {
                    PropertyDescriptor prop = propCollection[i];
                    if (!typeof(IList).IsAssignableFrom(prop.PropertyType) && prop.Name.Equals(this.HeaderText))
                    {
                        this.PropertyDescriptor = prop;
                        return;
                    }
                }
            }
        }

        internal void SetDataGridInternalInColumn(DataGrid value)
        {
            if (value == null || value.Initializing)
                return;
            SetDataGridInColumn(value);
        }

        /// <summary>
        /// Gets the System.Windows.Forms.DataGridTableStyle for the column.
        /// </summary>
        [Browsable(false)]
        public virtual DataGridTableStyle DataGridTableStyle
        {
            get
            {
                return dataGridTableStyle;
            }
        }

        internal void SetDataGridTableInColumn(DataGridTableStyle value, bool force)
        {
            if (dataGridTableStyle != null && dataGridTableStyle.Equals(value) && !force)
                return;
            if (value != null)
            {
                if (value.DataGrid != null && !value.DataGrid.Initializing)
                {
                    this.SetDataGridInColumn(value.DataGrid);
                }
            }
            dataGridTableStyle = value;
        }

        /// <summary>
        /// Gets the height of the column's font.
        /// </summary>
        protected int FontHeight
        {
            get
            {
                if (fontHeight != -1)
                {
                    return fontHeight;
                }
                else if (DataGridTableStyle != null)
                {
                    return DataGridTableStyle.DataGrid.FontHeight;
                }
                else
                {
                    return DataGridTableStyle.defaultFontHeight;
                }
            }
        }

        /// <summary>
        /// Indicates whether the Font property should be persisted.
        /// </summary>
        private bool ShouldSerializeFont()
        {
            return font != null;
        }

        public event EventHandler FontChanged
        {
            add
            {
            }
            remove
            {
            }
        }

        /// <summary>
        /// Gets or sets the text of the column header.
        /// </summary>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatDisplay))
        ]
        public virtual string HeaderText
        {
            get
            {
                return headerName;
            }
            set
            {
                if (value == null)
                    value = "";

                if (headerName.Equals(value))
                    return;

                headerName = value;
                OnHeaderTextChanged(EventArgs.Empty);
                // we only invalidate columns that are visible ( ie, their propertyDescriptor is not null)
                if (this.PropertyDescriptor != null)
                    Invalidate();
            }
        }

        public event EventHandler HeaderTextChanged
        {
            add
            {
                Events.AddHandler(EventHeaderText, value);
            }
            remove
            {
                Events.RemoveHandler(EventHeaderText, value);
            }
        }

        [
        Editor("System.Windows.Forms.Design.DataGridColumnStyleMappingNameEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor)),
        Localizable(true),
        DefaultValue("")
        ]
        public string MappingName
        {
            get
            {
                return mappingName;
            }
            set
            {
                if (value == null)
                    value = "";
                if (mappingName.Equals(value))
                    return;
                string originalMappingName = mappingName;
                // this may throw
                mappingName = value;
                try
                {
                    if (this.dataGridTableStyle != null)
                        this.dataGridTableStyle.GridColumnStyles.CheckForMappingNameDuplicates(this);
                }
                catch
                {
                    mappingName = originalMappingName;
                    throw;
                }
                OnMappingNameChanged(EventArgs.Empty);
            }
        }

        public event EventHandler MappingNameChanged
        {
            add
            {
                Events.AddHandler(EventMappingName, value);
            }
            remove
            {
                Events.RemoveHandler(EventMappingName, value);
            }
        }

        /// <summary>
        /// Indicates whether the System.Windows.Forms.DataGridColumnStyle.HeaderText property
        /// should be persisted.
        /// </summary>
        private bool ShouldSerializeHeaderText()
        {
            return (headerName.Length != 0);
        }

        /// <summary>
        /// Resets the System.Windows.Forms.DataGridColumnStyle.HeaderText to its default value.
        /// </summary>
        public void ResetHeaderText()
        {
            HeaderText = "";
        }

        /// <summary>
        /// Gets or sets the text that is displayed when the column contains a null
        /// value.
        /// </summary>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatDisplay))
        ]
        public virtual string NullText
        {
            get
            {
                return nullText;
            }
            set
            {
                if (nullText != null && nullText.Equals(value))
                    return;
                nullText = value;
                OnNullTextChanged(EventArgs.Empty);
                Invalidate();
            }
        }

        public event EventHandler NullTextChanged
        {
            add
            {
                Events.AddHandler(EventNullText, value);
            }
            remove
            {
                Events.RemoveHandler(EventNullText, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data in the column cannot be edited.
        /// </summary>
        [DefaultValue(false)]
        public virtual bool ReadOnly
        {
            get
            {
                return readOnly;
            }
            set
            {
                if (readOnly != value)
                {
                    readOnly = value;
                    OnReadOnlyChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler ReadOnlyChanged
        {
            add
            {
                Events.AddHandler(EventReadOnly, value);
            }
            remove
            {
                Events.RemoveHandler(EventReadOnly, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the column.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        DefaultValue(100)
        ]
        public virtual int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (width != value)
                {
                    width = value;
                    DataGrid grid = this.DataGridTableStyle == null ? null : DataGridTableStyle.DataGrid;
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
            add
            {
                Events.AddHandler(EventWidth, value);
            }
            remove
            {
                Events.RemoveHandler(EventWidth, value);
            }
        }

        /// <summary>
        /// Suspends the painting of the column until the <see cref='System.Windows.Forms.DataGridColumnStyle.EndUpdate'/>
        /// method is called.
        /// </summary>
        protected void BeginUpdate()
        {
            updating = true;
        }

        /// <summary>
        /// Resumes the painting of columns suspended by calling the
        /// <see cref='System.Windows.Forms.DataGridColumnStyle.BeginUpdate'/> method.
        /// </summary>
        protected void EndUpdate()
        {
            updating = false;
            if (invalid)
            {
                invalid = false;
                Invalidate();
            }
        }

        internal virtual string GetDisplayText(object value)
        {
            return value.ToString();
        }

        private void ResetNullText()
        {
            NullText = SR.DataGridNullText;
        }


        private bool ShouldSerializeNullText()
        {
            return (!SR.DataGridNullText.Equals(nullText));
        }

        /// <summary>
        /// When overridden in a derived class, gets the optimum width and height of the
        /// specified value.
        /// </summary>
        protected internal abstract Size GetPreferredSize(Graphics g, object value);

        /// <summary>
        /// Gets the minimum height of a row.
        /// </summary>
        protected internal abstract int GetMinimumHeight();

        /// <summary>
        /// When overridden in a derived class, gets the height to be used in for
        /// automatically resizing columns.
        /// </summary>
        protected internal abstract int GetPreferredHeight(Graphics g, object value);

        /// <summary>
        /// Gets the value in the specified row from the specified System.Windows.Forms.ListManager.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Fixing this would be a breaking change")]
        protected internal virtual object GetColumnValueAtRow(CurrencyManager source, int rowNum)
        {
            CheckValidDataSource(source);
            if (PropertyDescriptor == null)
            {
                throw new InvalidOperationException(SR.DataGridColumnNoPropertyDescriptor);
            }
            object value = PropertyDescriptor.GetValue(source[rowNum]);
            return value;
        }

        /// <summary>
        /// Redraws the column and causes a paint message to be sent to the control.
        /// </summary>
        protected virtual void Invalidate()
        {
            if (updating)
            {
                invalid = true;
                return;
            }
            DataGridTableStyle table = this.DataGridTableStyle;
            if (table != null)
                table.InvalidateColumn(this);
        }

        /// <summary>
        /// Checks if the specified DataView is valid.
        /// </summary>
        protected void CheckValidDataSource(CurrencyManager value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "DataGridColumnStyle.CheckValidDataSource(DataSource value), value == null");
            }
            // the code may delete a gridColumn that was editing
            // in that case, we still have to push the value into the backEnd
            // and we only need the propertyDescriptor to push the value.
            // (take a look at gridEditAndDeleteEditColumn)
            //
            // DataGridTableStyle myTable = this.DataGridTableStyle;
            PropertyDescriptor myPropDesc = this.PropertyDescriptor;
            if (myPropDesc == null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridColumnUnbound, HeaderText));
            }
        }

        /// <summary>
        /// When overridden in a derived class, initiates a request to interrrupt an edit
        /// procedure.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Fixing this would be a breaking change.")]
        protected internal abstract void Abort(int rowNum);

        /// <summary>
        /// When overridden in a derived class, inititates a request to complete an
        /// editing procedure.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Fixing this would be a breaking change.")]
        protected internal abstract bool Commit(CurrencyManager dataSource, int rowNum);

        /// <summary>
        /// When overridden in a deriving class, prepares a cell for editing.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal virtual void Edit(CurrencyManager source,
                                             int rowNum,
                                             Rectangle bounds,
                                             bool readOnly)
        {
            Edit(source, rowNum, bounds, readOnly, null, true);
        }

        /// <summary>
        /// Prepares the cell for editing, passing the specified <see cref='System.Data.DataView'/>,
        /// row number, <see cref='System.Drawing.Rectangle'/>, argument indicating whether
        /// the column is read-only, and the text to display in the new control.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Fixing this would be a breaking change.")]
        protected internal virtual void Edit(CurrencyManager source,
                                   int rowNum,
                                   Rectangle bounds,
                                   bool readOnly,
                                   string displayText)
        {
            Edit(source, rowNum, bounds, readOnly, displayText, true);
        }

        /// <summary>
        /// When overridden in a deriving class, prepares a cell for editing.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Fixing this would be a breaking change.")]
        protected internal abstract void Edit(CurrencyManager source,
                                    int rowNum,
                                    Rectangle bounds,
                                    bool readOnly,
                                    string displayText,
                                    bool cellIsVisible);

        /// <summary>
        /// Indicates whether the a mouse down event occurred at the specified row, at
        /// the specified x and y coordinates.
        /// </summary>
        internal virtual bool MouseDown(int rowNum, int x, int y)
        {
            return false;
        }

        /// <summary>
        /// When overriden in a derived class, enters a <see cref='T:System.DBNull.Value' qualify='true'/>
        /// into the column.
        /// </summary>
        protected internal virtual void EnterNullValue()
        {
        }

        /// <summary>
        /// Provides a handler for determining which key was pressed, and whether to
        /// process it.
        /// </summary>
        internal virtual bool KeyPress(int rowNum, Keys keyData)
        {
            // if this is read only then do not do anything
            if (this.ReadOnly || (this.DataGridTableStyle != null && this.DataGridTableStyle.DataGrid != null && this.DataGridTableStyle.DataGrid.ReadOnly))
                return false;
            if (keyData == (Keys.Control | Keys.NumPad0) || keyData == (Keys.Control | Keys.D0))
            {
                EnterNullValue();
                return true;
            }
            return false;
        }

        /// <summary>
        /// When overridden in a derived class, directs the column to concede focus with
        /// an appropriate action.
        /// </summary>
        protected internal virtual void ConcedeFocus()
        {
        }

        /// <summary>
        /// Paints the a <see cref='System.Windows.Forms.DataGridColumnStyle'/> with the specified
        /// <see cref='System.Drawing.Graphics'/>, <see cref='System.Drawing.Rectangle'/>,
        /// System.Windows.Forms.CurrencyManager, and row number.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Fixing this would be a breaking change")]
        protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum);

        /// <summary>
        /// When overridden in a derived class, paints a <see cref='System.Windows.Forms.DataGridColumnStyle'/>
        /// with the specified <see cref='System.Drawing.Graphics'/>, <see cref='System.Drawing.Rectangle'/>,
        /// see Rectangle, row number, and alignment.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Fixing this would be a breaking change")]
        protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight);

        /// <summary>
        /// Paints a <see cref='System.Windows.Forms.DataGridColumnStyle'/> with the specified <see cref='System.Drawing.Graphics'/>, <see cref='System.Drawing.Rectangle'/>, see System.Data.DataView, row number, background color, foreground color, and alignment.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Fixing this would be a breaking change")]
        protected internal virtual void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum,
                                    Brush backBrush, Brush foreBrush, bool alignToRight)
        {
            Paint(g, bounds, source, rowNum, alignToRight);
        }

        private void OnPropertyDescriptorChanged(EventArgs e)
        {
            EventHandler eh = Events[EventPropertyDescriptor] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnAlignmentChanged(EventArgs e)
        {
            EventHandler eh = Events[EventAlignment] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnHeaderTextChanged(EventArgs e)
        {
            EventHandler eh = Events[EventHeaderText] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnMappingNameChanged(EventArgs e)
        {
            EventHandler eh = Events[EventMappingName] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnReadOnlyChanged(EventArgs e)
        {
            EventHandler eh = Events[EventReadOnly] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnNullTextChanged(EventArgs e)
        {
            EventHandler eh = Events[EventNullText] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnWidthChanged(EventArgs e)
        {
            EventHandler eh = Events[EventWidth] as EventHandler;
            if (eh != null)
                eh(this, e);
        }

        /// <summary>
        /// Sets the value in a specified row with the value from a specified see DataView.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Fixing this would be a breaking change")]
        protected internal virtual void SetColumnValueAtRow(CurrencyManager source, int rowNum, object value)
        {
            CheckValidDataSource(source);

            if (source.Position != rowNum)
                throw new ArgumentException(SR.DataGridColumnListManagerPosition, "rowNum");
            if (source[rowNum] is IEditableObject)
                ((IEditableObject)source[rowNum]).BeginEdit();
            this.PropertyDescriptor.SetValue(source[rowNum], value);
        }

        internal protected virtual void ColumnStartedEditing(Control editingControl)
        {
            this.DataGridTableStyle.DataGrid.ColumnStartedEditing(editingControl);
        }

        void IDataGridColumnStyleEditingNotificationService.ColumnStartedEditing(Control editingControl)
        {
            this.ColumnStartedEditing(editingControl);
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


        [System.Runtime.InteropServices.ComVisible(true)]
        protected class DataGridColumnHeaderAccessibleObject : AccessibleObject
        {
            DataGridColumnStyle owner = null;

            public DataGridColumnHeaderAccessibleObject(DataGridColumnStyle owner) : this()
            {
                Debug.Assert(owner != null, "DataGridColumnHeaderAccessibleObject must have a valid owner DataGridColumn");
                this.owner = owner;

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
                    if (this.owner.PropertyDescriptor == null)
                        return Rectangle.Empty;

                    DataGrid dg = this.DataGrid;
                    if (dg.DataGridRowsLength == 0)
                        return Rectangle.Empty;

                    // we need to find this column's offset in the gridColumnCollection...
                    GridColumnStylesCollection cols = this.owner.dataGridTableStyle.GridColumnStyles;
                    int offset = -1;
                    for (int i = 0; i < cols.Count; i++)
                        if (cols[i] == this.owner)
                        {
                            offset = i;
                            break;
                        }
                    Debug.Assert(offset >= 0, "this column must be in a collection, otherwise its bounds are useless");
                    Rectangle rect = dg.GetCellBounds(0, offset);
                    // now add the Y coordinate of the datagrid.Layout.Data. it should be the same as
                    // dataGrid.Layout.ColumnHeaders
                    rect.Y = dg.GetColumnHeadersRect().Y;
                    return dg.RectangleToScreen(rect);
                }
            }

            public override string Name
            {
                get
                {
                    return Owner.headerName;
                }
            }

            protected DataGridColumnStyle Owner
            {
                get
                {
                    return owner;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return DataGrid.AccessibilityObject;
                }
            }

            private DataGrid DataGrid
            {
                get
                {
                    return owner.dataGridTableStyle.dataGrid;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.ColumnHeader;
                }
            }

            public override AccessibleObject Navigate(AccessibleNavigation navdir)
            {
                switch (navdir)
                {
                    case AccessibleNavigation.Right:
                    case AccessibleNavigation.Next:
                    case AccessibleNavigation.Down:
                        return Parent.GetChild(1 + Owner.dataGridTableStyle.GridColumnStyles.IndexOf(Owner) + 1);
                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                    case AccessibleNavigation.Previous:
                        return Parent.GetChild(1 + Owner.dataGridTableStyle.GridColumnStyles.IndexOf(Owner) - 1);

                }

                return null;

            }
        }
    }
}
