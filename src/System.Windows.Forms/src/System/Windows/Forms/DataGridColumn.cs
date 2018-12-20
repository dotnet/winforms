// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms{
    using System.Security.Permissions;
    using System.Runtime.Remoting;
    using System.ComponentModel;
    using System;
    using System.Collections;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Win32;
    using System.Runtime.InteropServices;

    /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="IDataGridColumnStyleEditingNotificationService"]/*' />
    public interface IDataGridColumnStyleEditingNotificationService {
        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="IDataGridColumnStyleEditingNotificationService.ColumnStartedEditing"]/*' />
        void ColumnStartedEditing(Control editingControl);
    }

    /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle"]/*' />
    /// <devdoc>
    ///    <para>Specifies the appearance and text formatting and behavior of
    ///       a <see cref='System.Windows.Forms.DataGrid'/> control column.</para>
    /// </devdoc>
    [
    ToolboxItem(false),
    DesignTimeVisible(false),
    DefaultProperty("Header"),
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors") // Shipped in Everett
    ]
    public abstract class DataGridColumnStyle : Component, IDataGridColumnStyleEditingNotificationService {

        private HorizontalAlignment alignment            = HorizontalAlignment.Left;
        // private SolidBrush         alternatingBackBrush = null;
        // private SolidBrush     backBrush            = null;
        // SolidBrush    foreBrush            = null;
        private PropertyDescriptor propertyDescriptor = null;
        private DataGridTableStyle dataGridTableStyle        = null;
        private Font         font                 = null;
        internal int         fontHeight           = -1;
        private string       mappingName       = "";
        private string       headerName       = "";
        private bool         invalid              = false;
        private string       nullText            = SR.DataGridNullText;
        private bool         readOnly             = false;
        private bool         updating             = false;
        //private bool        visible              = true;
        internal int         width                = -1;
        private bool         isDefault            = false;
        AccessibleObject     headerAccessibleObject = null;

        private static readonly object EventAlignment                   = new object();
        private static readonly object EventPropertyDescriptor          = new object();
        private static readonly object EventHeaderText                  = new object();
        private static readonly object EventMappingName                 = new object();
        private static readonly object EventNullText                    = new object();
        private static readonly object EventReadOnly                    = new object();
        private static readonly object EventWidth                       = new object();

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridColumnStyle"]/*' />
        /// <devdoc>
        ///    <para>In a derived class,
        ///       initializes a new instance of the <see cref='System.Windows.Forms.DataGridColumnStyle'/> class.</para>
        /// </devdoc>
        public DataGridColumnStyle() {
        }
        
        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridColumnStyle1"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.DataGridColumnStyle'/> class with the specified <see cref='T:System.ComponentModel.PropertyDescriptor'/>.</para>
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")] // Shipped like this in Everett.
        public DataGridColumnStyle(PropertyDescriptor prop) : this() {
            this.PropertyDescriptor = prop;
            if (prop != null)
                this.readOnly = prop.IsReadOnly;
        }

        internal DataGridColumnStyle(PropertyDescriptor prop, bool isDefault) : this(prop) {
            this.isDefault = isDefault;
            if (isDefault) {
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

        // =------------------------------------------------------------------
        // =        Properties
        // =------------------------------------------------------------------
        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Alignment"]/*' />
        /// <devdoc>
        ///       Gets or sets the alignment of text in a column.
        /// </devdoc>
        [SRCategory(nameof(SR.CatDisplay)),
        Localizable(true),
        DefaultValue(HorizontalAlignment.Left)]
        public virtual HorizontalAlignment Alignment {        
            get {
                return alignment;
            }
            set {
               //valid values are 0x0 to 0x2.
               if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridLineStyle));
               }
                if (alignment != value) {
                    alignment = value;
                    OnAlignmentChanged(EventArgs.Empty);
                    Invalidate();
                 }
            }
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.AlignmentChanged"]/*' />
        public event EventHandler AlignmentChanged {
            add {
                Events.AddHandler(EventAlignment, value);
            }
            remove {
                Events.RemoveHandler(EventAlignment, value);
            }
        }

        /*
        /// <summary>
        ///    <para>Gets or sets the background color of alternating rows for a ledger
        ///       appearance.</para>
        /// </summary>
        /// <value>
        /// <para>A <see cref='System.Drawing.Color'/> that represents the alternating background
        ///    color. The default is the <see cref='System.Windows.Forms.DataGrid.AlternatingBackColor'/> of the
        ///    control.</para>
        /// </value>
        /// <remarks>
        ///    <para>Use this property to set a custom alternating color for each column displayed 
        ///       in the <see cref='System.Drawing.DataGrid'/> control.</para>
        /// </remarks>
        /// <example>
        /// <para>The following example sets the <see cref='System.Windows.Forms.DataGridColumnStyle.AlternatingBackColor'/> property of a specific <see cref='System.Windows.Forms.DataGridColumnStyle'/>
        /// to yellow.</para>
        /// <code lang='VB'>Private Sub SetColumnAlternatingBackColor()
        ///    ' Create a color object.
        ///    Dim c As System.Drawing.Color
        ///    c = System.Drawing.Color.Yellow
        ///    ' Declare an object variable for the DataGridColumnStyle.      
        ///    Dim myGridColumn As DataGridColumnStyle
        ///    myGridColumn = DataGrid1.GridColumns(0)
        ///    ' Set the AlternatingBackColor to the color object.
        ///    myGridColumn.AlternatingBackColor = c
        /// End Sub
        /// </code>
        /// </example>
        /// <seealso cref='System.Windows.Forms.DataGrid.AlternatingBackColor'/>
        /// <seealso cref='System.Windows.Forms.DataGridColumnStyle.BackColor'/>
        [SRCategory(nameof(SR.CatColors))]
        public virtual Color AlternatingBackColor {
            get {
                if (alternatingBackBrush != null) {
                    return alternatingBackBrush.Color;
                }
                DataGrid grid = DataGrid;
                if (grid != null) {
                    return this.DataGridTableStyle.AlternatingBackColor;
                }
                return System.Windows.Forms.DataGridTableStyle.defaultAlternatingBackBrush.Color;
            }
            set {
                if (value != Color.Empty && alternatingBackBrush != null && value.Equals(alternatingBackBrush.Color)) {
                    return;
                }
                this.alternatingBackBrush = new SolidBrush(value);
                RaisePropertyChanged(EventArgs.Empty"AlternatingBackColor");
                Invalidate();
            }
        }
        */

        /*
        /// <summary>
        /// <para>Indicates whether the <see cref='System.Windows.Forms.DataGridColumnStyle.AlternatingBackColor'/>
        /// property should be persisted.</para>
        /// </summary>
        /// <returns>
        /// <para><see langword='true '/>if the property 
        ///    value has been changed from its default; otherwise,
        /// <see langword='false'/>.</para>
        /// </returns>
        /// <remarks>
        ///    <para>You typically use this method only if you are either
        ///       creating a designer for the <see cref='System.Windows.Forms.DataGrid'/>, or creating your own control
        ///       incorporating the <see cref='System.Windows.Forms.DataGrid'/>.</para>
        /// <para>You can use the <see cref='System.Windows.Forms.DataGridColumnStyle.ShouldSerializeAlternatingBackColor'/> method to
        ///    determine whether the property value has changed from its default.</para>
        /// </remarks>
        /// <seealso cref='System.Drawing.DataGridColumnStyle.AlternatingBackColor'/>
        internal bool ShouldSerializeAlternatingBackColor() {
            return alternatingBackBrush != null;
        }
        */

        /*
        /// <summary>
        ///    <para>
        ///       Resets the <see cref='System.Windows.Forms.DataGridColumnStyle.AlternatingBackColor'/>
        ///       property to its default value.
        ///    </para>
        /// </summary>
        /// <remarks>
        ///    <para>
        ///       You typically use this method only if you are either creating a designer for
        ///       the <see cref='System.Windows.Forms.DataGrid'/>, or creating your own control incorporating the
        ///    <see cref='System.Windows.Forms.DataGrid'/>. 
        ///    </para>
        ///    <para>
        ///       You can use the <see cref='System.Windows.Forms.DataGridColumnStyle.ShouldSerializeAlternatingBackColor'/>
        ///       method to determine whether the property value has changed from its default.
        ///    </para>
        ///    <para>
        ///       The OnPropertyChanged
        ///       event occurs when the property value changes.
        ///    </para>
        /// </remarks>
        public void ResetAlternatingBackColor() {
            if (alternatingBackBrush != null) {
                this.alternatingBackBrush = null;
                RaisePropertyChanged(EventArgs.Empty"AlternatingBackColor");
                Invalidate();
            }
        }
        */

        /*
        /// <summary>
        ///    <para>
        ///       Gets either the <see cref='System.Windows.Forms.DataGridColumnStyle.BackColor'/> or the <see cref='System.Windows.Forms.DataGridColumnStyle.AlternatingBackColor'/> of
        ///       a specified row.
        ///    </para>
        /// </summary>
        /// <returns>
        ///    <para>
        ///       A <see cref='System.Drawing.Color'/> that represents the background color.
        ///    </para>
        /// </returns>
        /// <seealso cref='System.Windows.Forms.DataGridColumnStyle.AlternatingBackColor'/>
        /// <seealso cref='System.Windows.Forms.DataGridColumnStyle.BackColor'/>
        /// <seealso cref='System.Windows.Forms.DataGridColumnStyle.GetBackColor'/>
        /// <keyword term=''/>
        public Color GetBackColor(int rowNum) {
            DataGrid grid = DataGrid;
            if (rowNum % 2 == 1 && (grid != null && grid.LedgerStyle))
                return AlternatingBackColor;
            else
                return BackColor;
        }
        */

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.UpdateUI"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a derived class, updates the value of a specified row with 
        ///       the given text.</para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal virtual void UpdateUI(CurrencyManager source, int rowNum, string displayText)
        {
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.HeaderAccessibleObject"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       [To Editor: I think this is going away.]
        ///    </para>
        ///    <para>
        ///       Gets or sets the background color of the column.
        ///    </para>
        /// </devdoc>
        [Browsable(false)]
        public AccessibleObject HeaderAccessibleObject {
            get {
                if (headerAccessibleObject == null) {
                    headerAccessibleObject = CreateHeaderAccessibleObject();
                }
                return headerAccessibleObject;
            }
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.PropertyDescriptor"]/*' />
        /// <devdoc>
        /// <para>Gets or sets the <see cref='System.Data.DataColumn'/> that determines the
        ///    attributes of data displayed by the <see cref='System.Windows.Forms.DataGridColumnStyle'/>.</para>
        /// </devdoc>
        [DefaultValue(null), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual PropertyDescriptor PropertyDescriptor {
            get {
                return propertyDescriptor;
            }
            set {
                if (propertyDescriptor != value) {
                    propertyDescriptor = value;
                    OnPropertyDescriptorChanged(EventArgs.Empty);
                    /*
                    // 


*/
                }
            }
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.PropertyDescriptorChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler PropertyDescriptorChanged {
            add {
                Events.AddHandler(EventPropertyDescriptor, value);
            }
            remove {
                Events.RemoveHandler(EventPropertyDescriptor, value);
            }
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.CreateHeaderAccessibleObject"]/*' />
        /// <devdoc>
        /// <para>Gets the <see cref='System.Windows.Forms.DataGrid'/> control that the <see cref='System.Windows.Forms.DataGridColumnStyle'/> belongs to.</para>
        /// </devdoc>
        /*
        protected virtual DataGrid DataGrid {
            get {
                DataGridTableStyle gridTable = DataGridTableStyle;
                if (gridTable == null)
                    return null;
                return gridTable.DataGrid;
            }
        }
        */

        protected virtual AccessibleObject CreateHeaderAccessibleObject() {
            return new DataGridColumnHeaderAccessibleObject(this);
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.SetDataGrid"]/*' />
        /// <devdoc>
        /// <para>When overridden in a derived class, sets the <see cref='System.Windows.Forms.DataGrid'/> control that this column 
        ///    belongs to.</para>
        /// </devdoc>
        protected virtual void SetDataGrid(DataGrid value)
        {
            SetDataGridInColumn(value);
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.SetDataGridInColumn"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class,
        ///       sets the <see cref='System.Windows.Forms.DataGrid'/> for the column.
        ///    </para>
        /// </devdoc>
        protected virtual void SetDataGridInColumn(DataGrid value) {
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

        internal void SetDataGridInternalInColumn(DataGrid value) {
            if (value == null || value.Initializing)
                return;
            SetDataGridInColumn(value);
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridTableStyle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the System.Windows.Forms.DataGridTableStyle for the column.
        ///    </para>
        /// </devdoc>
        [Browsable(false)]
        public virtual DataGridTableStyle DataGridTableStyle {
            get {
                return dataGridTableStyle;
            }
        }

        internal void SetDataGridTableInColumn(DataGridTableStyle value, bool force) {
            if (dataGridTableStyle != null && dataGridTableStyle.Equals(value) && !force)
                return;
            if (value != null) {
                if (value.DataGrid != null && !value.DataGrid.Initializing) {
                    this.SetDataGridInColumn(value.DataGrid);
                }
            }
            dataGridTableStyle = value;
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.FontHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the height of the column's font.
        ///    </para>
        /// </devdoc>
        protected int FontHeight {
            get {
                if (fontHeight != -1) {
                    return fontHeight;
                }
                else if (DataGridTableStyle!= null) {
                    return DataGridTableStyle.DataGrid.FontHeight;
                }
                else {
                    return DataGridTableStyle.defaultFontHeight;
                }
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Indicates whether the Font property should be persisted.
        ///    </para>
        /// </devdoc>
        private bool ShouldSerializeFont() {
            return font != null;
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.FontChanged"]/*' />
        public event EventHandler FontChanged {
            add {
            }
            remove {
            }
        }


        /*
        /// <summary>
        ///    <para>
        ///       Gets or sets the foreground color of the column.
        ///    </para>
        /// </summary>
        /// <value>
        ///    <para>
        ///       A <see cref='System.Drawing.Color'/> that represents the foreground color. The
        ///       default is the foreground color of the <see cref='System.Windows.Forms.DataGrid'/> control.
        ///    </para>
        /// </value>
        /// <remarks>
        ///    <para>
        ///       The OnPropertyChanged event occurs when the property value
        ///       changes.
        ///    </para>
        /// </remarks>
        /// <example>
        ///    <para>
        ///       The following example sets the <see cref='System.Windows.Forms.DataGridColumnStyle.ForeColor'/> property of
        ///       a given <see cref='System.Windows.Forms.DataGridColumnStyle'/>.
        ///    </para>
        ///    <code lang='VB'>
        /// Dim c As System.Drawing.Color
        /// Dim dgCol As DataGridColumnStyle
        /// c = System.Drawing.CadetBlue
        /// Set dgCol = DataGrid1.GridColumns(0)
        /// dgCol.ForeColor = c
        ///       </code>
        /// </example>
        /// <seealso cref='System.Windows.Forms.DataGridColumnStyle.AlternatingBackColor'/>
        /// <seealso cref='System.Windows.Forms.DataGridColumnStyle.BackColor'/>
        /// <seealso cref='System.Windows.Forms.DataGridColumnStyle.GetBackColor'/>
        /// <keyword term=''/>
        public virtual Color ForeColor {
            get {
                if (foreBrush != null) {
                    return foreBrush.Color;
                }
                DataGrid grid = DataGrid;
                if (grid != null) {
                    return grid.ForeColor;
                }
                return DataGrid.defaultForeBrush.Color;
            }
            set {
                if (value != Color.Empty && foreBrush != null && value.Equals(foreBrush.Color))
                    return;
                this.foreBrush = new SolidBrush(value);
                RaisePropertyChanged(EventArgs.Empty"ForeColor");
                Invalidate();
            }
        }

        // used by the DataGridRow
        internal SolidBrush ForeBrush {
            get {
                if (foreBrush != null) {
                    return foreBrush;
                }
                DataGrid grid = DataGrid;
                if (grid != null) {
                    return grid.ForeBrush;
                }
                return DataGrid.defaultForeBrush;
            }
        }
        */

        /*
        /// <summary>
        ///    <para>
        ///       Indicates if the <see cref='System.Windows.Forms.DataGridColumnStyle.ForeColor'/> property should be
        ///       persisted.
        ///    </para>
        /// </summary>
        /// <returns>
        ///    <para>
        ///    <see langword='true '/>if the property value has been changed from its 
        ///       default; otherwise, <see langword='false'/> .
        ///    </para>
        /// </returns>
        /// <remarks>
        ///    <para>
        ///       You typically use this method only if you are either creating a designer for
        ///       the <see cref='System.Windows.Forms.DataGrid'/>, or creating your own control incorporating the
        ///    <see cref='System.Windows.Forms.DataGrid'/>. 
        ///    </para>
        /// </remarks>
        internal bool ShouldSerializeForeColor() {
            return foreBrush != null;
        }
        */

        /*
        /// <summary>
        ///    <para>
        ///       Resets the <see cref='System.Windows.Forms.DataGridColumnStyle.ForeColor'/> property to its default value.
        ///    </para>
        /// </summary>
        /// <remarks>
        ///    <para>
        ///       You typically use this method if you are either creating a designer for
        ///       the <see cref='System.Windows.Forms.DataGrid'/>, or creating your own control incorporating the
        ///    <see cref='System.Windows.Forms.DataGrid'/>. 
        ///    </para>
        ///    <para>
        ///       You can use the <see cref='System.Windows.Forms.DataGridColumnStyle.ShouldSerializeForeColor'/> method to
        ///       determine whether the property value has changed from its default.
        ///    </para>
        ///    <para>
        ///       The OnPropertyChanged event occurs when the property
        ///       value changes.
        ///    </para>
        /// </remarks>
        public void ResetForeColor() {
            if (foreBrush != null) {
                foreBrush = null;
                RaisePropertyChanged(EventArgs.Empty"ForeColor");
                Invalidate();
            }
        }
        */

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.HeaderText"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the text of the column header.
        ///    </para>
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatDisplay))
        ]
        public virtual string HeaderText {
            get {
                return headerName; 
            }
            set {
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

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.HeaderTextChanged"]/*' />
        public event EventHandler HeaderTextChanged {
            add {
                Events.AddHandler(EventHeaderText, value);
            }
            remove {
                Events.RemoveHandler(EventHeaderText, value);
            }
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.MappingName"]/*' />
        [
        Editor("System.Windows.Forms.Design.DataGridColumnStyleMappingNameEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor)),
        Localizable(true), 
        DefaultValue("")
        ]
        public string MappingName {
            get {
                return mappingName;
            }
            set {
                if (value == null)
                    value = "";
                if (mappingName.Equals(value))
                    return;
                string originalMappingName = mappingName;
                // this may throw
                mappingName = value;
                try {
                    if (this.dataGridTableStyle != null)
                        this.dataGridTableStyle.GridColumnStyles.CheckForMappingNameDuplicates(this);
                } catch {
                    mappingName = originalMappingName;
                    throw;
                }
                OnMappingNameChanged(EventArgs.Empty);
            }
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.MappingNameChanged"]/*' />
        public event EventHandler MappingNameChanged {
            add {
                Events.AddHandler(EventMappingName, value);
            }
            remove {
                Events.RemoveHandler(EventMappingName, value);
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Indicates whether the System.Windows.Forms.DataGridColumnStyle.Header property should be
        ///       persisted.
        ///    </para>
        /// </devdoc>
        private bool ShouldSerializeHeaderText() {
            return(headerName.Length != 0);
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.ResetHeader"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Resets the System.Windows.Forms.DataGridColumnStyle.Header to its default value
        ///       (<see langword='null'/> ).
        ///    </para>
        /// </devdoc>
        public void ResetHeaderText() {
            HeaderText = "";
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.NullText"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the text that is displayed when the column contains a null
        ///       value.</para>
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatDisplay))
        ]
        public virtual string NullText {
            get {
                return nullText;
            }
            set {
                if (nullText != null && nullText.Equals(value))
                    return;
                nullText = value;
                OnNullTextChanged(EventArgs.Empty);
                Invalidate();
            }
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.NullTextChanged"]/*' />
        public event EventHandler NullTextChanged {
            add {
                Events.AddHandler(EventNullText, value);
            }
            remove {
                Events.RemoveHandler(EventNullText, value);
            }
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.ReadOnly"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether the data in the column cannot be edited.</para>
        /// </devdoc>
        [DefaultValue(false)]
        public virtual bool ReadOnly {
            get {
                return readOnly;
            }
            set {
                if (readOnly != value) {
                    readOnly = value;
                    OnReadOnlyChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.ReadOnlyChanged"]/*' />
        public event EventHandler ReadOnlyChanged {
            add {
                Events.AddHandler(EventReadOnly, value);
            }
            remove {
                Events.RemoveHandler(EventReadOnly, value);
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

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Width"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the width of the column.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        DefaultValue(100)
        ]
        public virtual int Width {
            get {
                return width;
            }
            set {
                if (width != value) {
                    width = value;
                    DataGrid grid = this.DataGridTableStyle == null ? null : DataGridTableStyle.DataGrid;
                    if (grid != null) {
                        // rearrange the scroll bars
                        grid.PerformLayout();

                        // force the grid to repaint
                        grid.InvalidateInside();
                    }
                    OnWidthChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.WidthChanged"]/*' />
        public event EventHandler WidthChanged {
            add {
                Events.AddHandler(EventWidth, value);
            }
            remove {
                Events.RemoveHandler(EventWidth, value);
            }
        }

        // =------------------------------------------------------------------
        // =        Methods
        // =------------------------------------------------------------------

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.BeginUpdate"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Suspends the painting of the column until the <see cref='System.Windows.Forms.DataGridColumnStyle.EndUpdate'/>
        ///       method is called.
        ///    </para>
        /// </devdoc>
        protected void BeginUpdate() {
            updating = true;
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.EndUpdate"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Resumes the painting of columns suspended by calling the
        ///    <see cref='System.Windows.Forms.DataGridColumnStyle.BeginUpdate'/> 
        ///    method.
        /// </para>
        /// </devdoc>
        protected void EndUpdate() {
            updating = false;
            if (invalid) {
                invalid = false;
                Invalidate();
            }
        }

        internal virtual bool WantArrows {
            get {
                return false;
            }
        }


        internal virtual string GetDisplayText(object value) {
            return value.ToString();
        }

	private void ResetNullText() {
		NullText = SR.DataGridNullText;
	}
		

        private bool ShouldSerializeNullText() {
            return (!SR.DataGridNullText.Equals(nullText));
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.GetPreferredSize"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a derived class,
        ///       gets the optimum width and height of the specified value.</para>
        /// </devdoc>
        protected internal abstract Size GetPreferredSize(Graphics g, object value);

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.GetMinimumHeight"]/*' />
        /// <devdoc>
        ///    <para>Gets the minimum height of a row.</para>
        /// </devdoc>
        protected internal abstract int GetMinimumHeight();

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.GetPreferredHeight"]/*' />
        /// <devdoc>
        ///    <para>When
        ///       overridden in a derived class, gets the height to be used in for automatically resizing columns.</para>
        /// </devdoc>
        protected internal abstract int GetPreferredHeight(Graphics g, object value);

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.GetColumnValueAtRow"]/*' />
        /// <devdoc>
        ///    <para>Gets the value in the specified row from the specified 
        ///    System.Windows.Forms.ListManager.</para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal virtual object GetColumnValueAtRow(CurrencyManager source, int rowNum) {
            CheckValidDataSource(source);
            if (PropertyDescriptor == null) {
                throw new InvalidOperationException(SR.DataGridColumnNoPropertyDescriptor);
            }
            object value = PropertyDescriptor.GetValue(source[rowNum]);
            return value;
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Invalidate"]/*' />
        /// <devdoc>
        ///    <para>Redraws the column and causes a paint
        ///       message to be sent to the control.</para>
        /// </devdoc>
        protected virtual void Invalidate() {
            if (updating) {
                invalid = true;
                return;
            }
            DataGridTableStyle table = this.DataGridTableStyle;
            if (table != null)
                table.InvalidateColumn(this);
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.CheckValidDataSource"]/*' />
        /// <devdoc>
        /// <para>Checks if the specified DataView is valid.</para>
        /// </devdoc>
        protected void CheckValidDataSource(CurrencyManager value) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value), "DataGridColumnStyle.CheckValidDataSource(DataSource value), value == null");
            }
            // the code may delete a gridColumn that was editing
            // in that case, we still have to push the value into the backEnd
            // and we only need the propertyDescriptor to push the value.
            // (take a look at gridEditAndDeleteEditColumn)
            //
            // DataGridTableStyle myTable = this.DataGridTableStyle;
            PropertyDescriptor myPropDesc = this.PropertyDescriptor;
            if (myPropDesc == null) {
                throw new InvalidOperationException(string.Format(SR.DataGridColumnUnbound, HeaderText));
            }

#if false
            DataTable myDataTable = myTable.DataTable;
            if (myDataColumn.Table != myDataTable) {
                throw new InvalidOperationException(string.Format(SR.DataGridColumnDataSourceMismatch, Header));
            }

            /* FOR DEMO: Microsoft: DataGridColumnStyle::CheckValidDataSource: make the check better */
            if (((DataView) value.DataSource).Table == null) {
                throw new InvalidOperationException(string.Format(SR.DataGridColumnNoDataTable, Header));
            }
            else {
                /* FOR DEMO: Microsoft: DataGridColumnStyle::CheckValidDataSource: make the check better */
                if (!myTable.DataTable.Equals(((DataView) value.DataSource).Table)) {
                    throw new InvalidOperationException(string.Format(SR.DataGridColumnNoDataSource, Header, myTable.DataTable.TableName));
                }
            }
#endif // false
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Abort"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class, initiates a
        ///       request to interrrupt an edit procedure.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal abstract void Abort(int rowNum);

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Commit"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a derived class, inititates a request to complete an
        ///       editing procedure.</para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal abstract bool Commit(CurrencyManager dataSource, int rowNum);

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Edit"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a deriving class, prepares a cell for editing.</para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal virtual void Edit(CurrencyManager source,
                                             int rowNum,
                                             Rectangle bounds,
                                             bool readOnly) {
            Edit(source, rowNum, bounds, readOnly, null, true);
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Edit1"]/*' />
        /// <devdoc>
        ///    <para>Prepares the
        ///       cell for editing, passing the specified <see cref='System.Data.DataView'/>, row number, <see cref='System.Drawing.Rectangle'/>, argument
        ///       indicating whether the column is read-only, and the
        ///       text to display in the new control.</para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal virtual void Edit(CurrencyManager source,
                                   int rowNum,
                                   Rectangle bounds,
                                   bool readOnly,
                                   string displayText) {
            Edit(source, rowNum, bounds, readOnly, displayText, true);
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Edit2"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a deriving class, prepares a cell for editing.</para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal abstract void Edit(CurrencyManager source,
                                    int rowNum,
                                    Rectangle bounds,
                                    bool readOnly,
                                    string displayText,
                                    bool cellIsVisible);

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.MouseDown"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>Indicates whether the a mouse down event occurred at the specified row, at
        ///       the specified x and y coordinates.</para>
        /// </devdoc>
        internal virtual bool MouseDown(int rowNum, int x, int y) {
            return false;
        }

        // this function mainly serves Alt0 functionality
        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.EnterNullValue"]/*' />
        /// <devdoc>
        /// <para>When overriden in a derived class, enters a <see cref='T:System.DBNull.Value' qualify='true'/>
        /// into the column.</para>
        /// </devdoc>
        protected internal virtual void EnterNullValue() {
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.KeyPress"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Provides a handler for determining which key was pressed,
        ///       and whether to process it.
        ///    </para>
        /// </devdoc>
        internal virtual bool KeyPress(int rowNum, Keys keyData) {
            // if this is read only then do not do anything
            if (this.ReadOnly || (this.DataGridTableStyle != null && this.DataGridTableStyle.DataGrid != null && this.DataGridTableStyle.DataGrid.ReadOnly))
                return false;
            if (keyData == (Keys.Control | Keys.NumPad0) || keyData == (Keys.Control| Keys.D0)) {
                EnterNullValue();
                return true;
            }
            return false;
        }

        // will cause the edit control to become invisible when 
        // the user navigates to a focused relation child
        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.ConcedeFocus"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a derived class, directs the column to concede focus with an appropriate action.</para>
        /// </devdoc>
        protected internal virtual void ConcedeFocus() {
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Paint"]/*' />
        /// <devdoc>
        /// <para>Paints the a <see cref='System.Windows.Forms.DataGridColumnStyle'/> with the specified <see cref='System.Drawing.Graphics'/>,
        /// <see cref='System.Drawing.Rectangle'/>, System.Windows.Forms.CurrencyManager, and row number.</para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum);

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Paint1"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a derived class,
        ///       paints a <see cref='System.Windows.Forms.DataGridColumnStyle'/> with the specified <see cref='System.Drawing.Graphics'/>, <see cref='System.Drawing.Rectangle'/>, see Rectangle, row number, and
        ///       alignment.</para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight);

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.Paint2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Paints a <see cref='System.Windows.Forms.DataGridColumnStyle'/> with the specified <see cref='System.Drawing.Graphics'/>, <see cref='System.Drawing.Rectangle'/>, see System.Data.DataView, row number, background color, foreground color, and alignment.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal virtual void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum,
                                    Brush backBrush, Brush foreBrush, bool alignToRight) {
            Paint(g, bounds, source, rowNum, alignToRight);
        }

        private void OnPropertyDescriptorChanged(EventArgs e) {
            EventHandler eh = Events[EventPropertyDescriptor] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnAlignmentChanged(EventArgs e) {
            EventHandler eh = Events[EventAlignment] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnHeaderTextChanged(EventArgs e) {
            EventHandler eh = Events[EventHeaderText] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnMappingNameChanged(EventArgs e) {
            EventHandler eh = Events[EventMappingName] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnReadOnlyChanged(EventArgs e) {
            EventHandler eh = Events[EventReadOnly] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnNullTextChanged(EventArgs e) {
            EventHandler eh = Events[EventNullText] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnWidthChanged(EventArgs e) {
            EventHandler eh = Events[EventWidth] as EventHandler;
            if (eh != null)
                eh(this, e);
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.SetColumnValueAtRow"]/*' />
        /// <devdoc>
        ///    <para>Sets
        ///       the value in a specified row
        ///       with the value from a specified see DataView.</para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected internal virtual void SetColumnValueAtRow(CurrencyManager source, int rowNum, object value) {
            CheckValidDataSource(source);

            if (source.Position != rowNum)
                throw new ArgumentException(SR.DataGridColumnListManagerPosition, "rowNum");
            if (source[rowNum] is IEditableObject)
                ((IEditableObject)source[rowNum]).BeginEdit();
            this.PropertyDescriptor.SetValue(source[rowNum], value);
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.ColumnStartedEditing"]/*' />
        internal protected virtual void ColumnStartedEditing(Control editingControl) {
            this.DataGridTableStyle.DataGrid.ColumnStartedEditing(editingControl);
        }

        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.IDataGridColumnStyleEditingNotificationService.ColumnStartedEditing"]/*' />
        /// <internalonly/>
        void IDataGridColumnStyleEditingNotificationService.ColumnStartedEditing(Control editingControl) {
            this.ColumnStartedEditing(editingControl);
        }


        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.ReleaseHostedControl"]/*' />
        protected internal virtual void ReleaseHostedControl() {
        }
        
        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.CompModSwitches"]/*' />
        /// <internalonly/>
        protected class CompModSwitches {
            private static TraceSwitch dgEditColumnEditing;                                                                
            
            /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.CompModSwitches.DGEditColumnEditing"]/*' />
            public static TraceSwitch DGEditColumnEditing {
                get {
                    if (dgEditColumnEditing == null) {
                        dgEditColumnEditing = new TraceSwitch("DGEditColumnEditing", "Editing related tracing");
                    }
                    return dgEditColumnEditing;
                }
            }
        }

        
        /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridColumnHeaderAccessibleObject"]/*' />
        [System.Runtime.InteropServices.ComVisible(true)]                                                    
        protected class DataGridColumnHeaderAccessibleObject : AccessibleObject {
            DataGridColumnStyle owner = null;

            /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridColumnHeaderAccessibleObject.DataGridColumnHeaderAccessibleObject"]/*' />
            public DataGridColumnHeaderAccessibleObject(DataGridColumnStyle owner) : this() {
                Debug.Assert(owner != null, "DataGridColumnHeaderAccessibleObject must have a valid owner DataGridColumn");
                this.owner = owner;

            }

            /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridColumnHeaderAccessibleObject.DataGridColumnHeaderAccessibleObject1"]/*' />
            public DataGridColumnHeaderAccessibleObject() : base() {
            }

            /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridColumnHeaderAccessibleObject.Bounds"]/*' />
            public override Rectangle Bounds {
                get {
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
                        if (cols[i] == this.owner) {
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

            /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridColumnHeaderAccessibleObject.Name"]/*' />
            public override string Name {
                get {
                    return Owner.headerName;
                }
            }

            /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridColumnHeaderAccessibleObject.Owner"]/*' />
            protected DataGridColumnStyle Owner {
                get {
                    return owner;
                }
            }

            /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridColumnHeaderAccessibleObject.Parent"]/*' />
            public override AccessibleObject Parent {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    return DataGrid.AccessibilityObject;
                }
            }

            private DataGrid DataGrid {
                get {
                    return owner.dataGridTableStyle.dataGrid;
                }
            }

            /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridColumnHeaderAccessibleObject.Role"]/*' />
            public override AccessibleRole Role {
                get {
                    return AccessibleRole.ColumnHeader;
                }
            }

            /// <include file='doc\DataGridColumn.uex' path='docs/doc[@for="DataGridColumnStyle.DataGridColumnHeaderAccessibleObject.Navigate"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navdir) {
                switch (navdir) {
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
