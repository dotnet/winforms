// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms.Layout;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <include file='doc\GridPanel.uex' path='docs/doc[@for="GridPanel"]/*' />
    [ProvideProperty("ColumnSpan", typeof(Control))]
    [ProvideProperty("RowSpan", typeof(Control))]
    [ProvideProperty("Row", typeof(Control))]
    [ProvideProperty("Column", typeof(Control))]
    [ProvideProperty("CellPosition", typeof(Control))]
    [DefaultProperty(nameof(ColumnCount))]    
    [DesignerSerializer("System.Windows.Forms.Design.TableLayoutPanelCodeDomSerializer, " + AssemblyRef.SystemDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign)]
    [Docking(DockingBehavior.Never)]
    [Designer("System.Windows.Forms.Design.TableLayoutPanelDesigner, " + AssemblyRef.SystemDesign)]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [SRDescription(nameof(SR.DescriptionTableLayoutPanel))]
    public class TableLayoutPanel : Panel, IExtenderProvider {
        private TableLayoutSettings _tableLayoutSettings;
        private static readonly object EventCellPaint = new object();

        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.TableLayoutPanel"]/*' />
        public TableLayoutPanel() {
            _tableLayoutSettings = TableLayout.CreateSettings(this);
        }
        
        /// <include file='doc\GridPanel.uex' path='docs/doc[@for="GridPanel.LayoutEngine"]/*' />
        public override LayoutEngine LayoutEngine {
            get { return TableLayout.Instance; }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TableLayoutSettings LayoutSettings {
            get {
                return _tableLayoutSettings;
            }
            set {
                if (value != null && value.IsStub) {
                    // WINRES only scenario.
                    // we only support table layout settings that have been created from a type converter.  
                    // this is here for localization (WinRes) support.
                    using (new LayoutTransaction(this, this, PropertyNames.LayoutSettings)) {
                        // apply RowStyles, ColumnStyles, Row & Column assignments.
                        _tableLayoutSettings.ApplySettings(value);  
                    }
                }
                else {
                    throw new NotSupportedException(SR.TableLayoutSettingSettingsIsNotSupported);
                }
                
            }
        }

    
        
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.BorderStyle"]/*' />
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        Localizable(true)
        ]
        public new BorderStyle BorderStyle {
            get { return base.BorderStyle; }
            set { 
                base.BorderStyle = value; 
                Debug.Assert(BorderStyle == value, "BorderStyle should be the same as we set it");
            }
        }


        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.CellBorderStyle"]/*' />
        [
        DefaultValue(TableLayoutPanelCellBorderStyle.None), 
        SRCategory(nameof(SR.CatAppearance)), 
        SRDescription(nameof(SR.TableLayoutPanelCellBorderStyleDescr)),
        Localizable(true)
        ]
        public TableLayoutPanelCellBorderStyle CellBorderStyle {
            get { return _tableLayoutSettings.CellBorderStyle; }
            set { 
                _tableLayoutSettings.CellBorderStyle = value; 

                // PERF: dont turn on ResizeRedraw unless we know we need it.
                if (value != TableLayoutPanelCellBorderStyle.None) {
                    SetStyle(ControlStyles.ResizeRedraw, true);
                }                
                this.Invalidate();
                Debug.Assert(CellBorderStyle == value, "CellBorderStyle should be the same as we set it");
            }
        }

        private int CellBorderWidth {
            get { return _tableLayoutSettings.CellBorderWidth; }
        }

        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.Controls"]/*' />
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [SRDescription(nameof(SR.ControlControlsDescr))]
        public new TableLayoutControlCollection Controls {
            get { return (TableLayoutControlCollection)base.Controls; }
        }
        
        /// <include file='doc\GridPanel.uex' path='docs/doc[@for="GridPanel.ColumnCount"]/*' />
        /// <devdoc>
        /// This sets the maximum number of columns allowed on this table instead of allocating
        /// actual spaces for these columns. So it is OK to set ColumnCount to Int32.MaxValue without
        /// causing out of memory exception
        /// </devdoc>
        [SRDescription(nameof(SR.GridPanelColumnsDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(0)]
        [Localizable(true)]
        public int ColumnCount {
            get { return _tableLayoutSettings.ColumnCount; }
            set { 
                _tableLayoutSettings.ColumnCount = value; 
                Debug.Assert(ColumnCount == value, "ColumnCount should be the same as we set it");
            }
        }
 
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.GrowStyle"]/*' />
        /// <devdoc>
        ///       Specifies if a TableLayoutPanel will gain additional rows or columns once its existing cells
        ///       become full.  If the value is 'FixedSize' then the TableLayoutPanel will throw an exception
        ///       when the TableLayoutPanel is over-filled.
        /// </devdoc>
        [SRDescription(nameof(SR.TableLayoutPanelGrowStyleDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(TableLayoutPanelGrowStyle.AddRows)]
        public TableLayoutPanelGrowStyle GrowStyle {
            get {
                return _tableLayoutSettings.GrowStyle;
            }
            set {
                _tableLayoutSettings.GrowStyle = value;
            }
        }

        /// <include file='doc\GridPanel.uex' path='docs/doc[@for="GridPanel.RowCount"]/*' />
        /// <devdoc>
        /// This sets the maximum number of rows allowed on this table instead of allocating
        /// actual spaces for these rows. So it is OK to set RowCount to Int32.MaxValue without
        /// causing out of memory exception
        /// </devdoc>
        [SRDescription(nameof(SR.GridPanelRowsDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(0)]
        [Localizable(true)]
        public int RowCount {
            get { return _tableLayoutSettings.RowCount; }
            set { _tableLayoutSettings.RowCount = value; }
        }

        /// <include file='doc\GridPanel.uex' path='docs/doc[@for="GridPanel.RowStyles"]/*' />
        [SRDescription(nameof(SR.GridPanelRowStylesDescr))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [SRCategory(nameof(SR.CatLayout))]
        [DisplayName("Rows")]
        [MergableProperty(false)]
        [Browsable(false)]
        public TableLayoutRowStyleCollection RowStyles {
            get { return _tableLayoutSettings.RowStyles; }
        }

        /// <include file='doc\GridPanel.uex' path='docs/doc[@for="GridPanel.ColumnStyles"]/*' />
        [SRDescription(nameof(SR.GridPanelColumnStylesDescr))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [SRCategory(nameof(SR.CatLayout))]
        [DisplayName("Columns")]
        [Browsable(false)]
        [MergableProperty(false)]        
        public TableLayoutColumnStyleCollection ColumnStyles {
            get { return _tableLayoutSettings.ColumnStyles; }
        }

        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.CreateControlsInstance"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Control.ControlCollection CreateControlsInstance() {
            return new TableLayoutControlCollection(this);
        }

        private bool ShouldSerializeControls() {
            TableLayoutControlCollection collection = this.Controls;
            return collection != null && collection.Count > 0;
        }
        
        #region Extended Properties
        /// <include file='doc\GridPanel.uex' path='docs/doc[@for="GridPanel.IExtenderProvider.CanExtend"]/*' />
        /// <internalonly/>
        bool IExtenderProvider.CanExtend(object obj) {
            Control control = obj as Control;
            return control != null && control.Parent == this;
        }

        /// <include file='doc\GridPanel.uex' path='docs/doc[@for="GridPanel.GetColumnSpan"]/*' />
        [SRDescription(nameof(SR.GridPanelGetColumnSpanDescr))]
        [DefaultValue(1)]
        [SRCategory(nameof(SR.CatLayout))]
        [DisplayName("ColumnSpan")]
        public int GetColumnSpan(Control control) {
            return _tableLayoutSettings.GetColumnSpan(control);
        }

        /// <include file='doc\GridPanel.uex' path='docs/doc[@for="GridPanel.SetColumnSpan"]/*' />
        public void SetColumnSpan(Control control, int value) {
            // layout.SetColumnSpan() throws ArgumentException if out of range.
            _tableLayoutSettings.SetColumnSpan(control, value);
            Debug.Assert(GetColumnSpan(control) == value, "GetColumnSpan should be the same as we set it");
        }

        /// <include file='doc\GridPanel.uex' path='docs/doc[@for="GridPanel.GetRowSpan"]/*' />
        [SRDescription(nameof(SR.GridPanelGetRowSpanDescr))]
        [DefaultValue(1)]
        [SRCategory(nameof(SR.CatLayout))]
        [DisplayName("RowSpan")]
        public int GetRowSpan(Control control) {
            return _tableLayoutSettings.GetRowSpan(control);
        }
        
        /// <include file='doc\GridPanel.uex' path='docs/doc[@for="GridPanel.SetRowSpan"]/*' />
        public void SetRowSpan(Control control, int value) {
            // layout.SetRowSpan() throws ArgumentException if out of range.
            _tableLayoutSettings.SetRowSpan(control, value);
            Debug.Assert(GetRowSpan(control) == value, "GetRowSpan should be the same as we set it");
        }

        //get the row position of the control
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.GetRow"]/*' />
        [DefaultValue(-1)]  //if change this value, also change the SerializeViaAdd in TableLayoutControlCollectionCodeDomSerializer
        [SRDescription(nameof(SR.GridPanelRowDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DisplayName("Row")]
        public int GetRow(Control control) {
            return _tableLayoutSettings.GetRow(control);
        }

        //set the row position of the control
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.SetRow"]/*' />
        public void SetRow(Control control, int row) {
            _tableLayoutSettings.SetRow(control, row);
            Debug.Assert(GetRow(control) == row, "GetRow should be the same as we set it");
        }

        //get the row and column position of the control
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.GetRow"]/*' />
        [DefaultValue(typeof(TableLayoutPanelCellPosition), "-1,-1")]  //if change this value, also change the SerializeViaAdd in TableLayoutControlCollectionCodeDomSerializer
        [SRDescription(nameof(SR.GridPanelCellPositionDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DisplayName("Cell")]
        public TableLayoutPanelCellPosition GetCellPosition(Control control) {
            return _tableLayoutSettings.GetCellPosition(control);
        }

        //set the row and column of the control
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.SetRow"]/*' />
        public void SetCellPosition(Control control, TableLayoutPanelCellPosition position) {
            _tableLayoutSettings.SetCellPosition(control, position);
        }

        

        //get the column position of the control
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.GetColumn"]/*' />
        [DefaultValue(-1)]  //if change this value, also change the SerializeViaAdd in TableLayoutControlCollectionCodeDomSerializer
        [SRDescription(nameof(SR.GridPanelColumnDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DisplayName("Column")]
        public int GetColumn(Control control) {
            return _tableLayoutSettings.GetColumn(control);
        }
        
        //set the column position of the control
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.SetColumn"]/*' />
        public void SetColumn(Control control, int column) {
            _tableLayoutSettings.SetColumn(control, column);
            Debug.Assert(GetColumn(control) == column, "GetColumn should be the same as we set it");
        }
        
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.GetControlFromPosition"]/*' />
        /// <devdoc>
        /// get the control which covers the specified row and column. return null if we can't find one
        /// </devdoc>
        public Control GetControlFromPosition (int column, int row) {
            return (Control)_tableLayoutSettings.GetControlFromPosition(column, row);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Control instead of IArrangedElement intentionally
        public TableLayoutPanelCellPosition GetPositionFromControl (Control control) {
            return _tableLayoutSettings.GetPositionFromControl(control);
        }
        
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.ColumnWidths"]/*' />
        /// <devdoc>
        /// This returns an array representing the widths (in pixels) of the columns in the TableLayoutPanel.
        /// </devdoc>        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int[] GetColumnWidths() {
             TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(this);
             if (containerInfo.Columns == null) {
                 return new int[0];
             }
             
             int[] cw = new int[containerInfo.Columns.Length];
             for(int i = 0; i < containerInfo.Columns.Length; i++) {
                 cw[i] = containerInfo.Columns[i].MinSize;
             }
             return cw;
        }      
        
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.RowWidths"]/*' />
        /// <devdoc>
        /// This returns an array representing the heights (in pixels) of the rows in the TableLayoutPanel.
        /// </devdoc>        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int[] GetRowHeights() {
            TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(this);
            if (containerInfo.Rows == null) {
                return new int[0];
            }
            
            int[] rh = new int[containerInfo.Rows.Length];
            for(int i = 0; i < containerInfo.Rows.Length; i++) {
                rh[i] = containerInfo.Rows[i].MinSize;
            }
            return rh;
        }
		

        #endregion

        #region PaintCode

        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.CellPaint"]/*' />
        [SRCategory(nameof(SR.CatAppearance)), SRDescription(nameof(SR.TableLayoutPanelOnPaintCellDescr))]
        public event TableLayoutCellPaintEventHandler CellPaint {
            add {
                Events.AddHandler(EventCellPaint, value);
            }
            remove {
                Events.RemoveHandler(EventCellPaint, value);
            }
        }

        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.OnLayout"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    When a layout fires, make sure we're painting all of our
        ///    cell borders.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnLayout(LayoutEventArgs levent) {
            base.OnLayout(levent);
            this.Invalidate();
        }
        

        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.OnCellPaint"]/*' />
        protected virtual void OnCellPaint(TableLayoutCellPaintEventArgs e) {
            TableLayoutCellPaintEventHandler handler = (TableLayoutCellPaintEventHandler)Events[EventCellPaint];
            if (handler != null) {
                handler(this, e);
            }
        }
        
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutPanel.OnPaint"]/*' />
        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);
            
           

            // paint borderstyles on top of the background image in WM_ERASEBKGND
            
            int cellBorderWidth = this.CellBorderWidth;
            TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(this);
            TableLayout.Strip[] colStrips = containerInfo.Columns;
            TableLayout.Strip[] rowStrips = containerInfo.Rows;
            TableLayoutPanelCellBorderStyle cellBorderStyle = this.CellBorderStyle;

            
            
            if (colStrips == null || rowStrips == null) {
                return;
            }
            int cols = colStrips.Length;
            int rows = rowStrips.Length;

            int totalColumnWidths = 0, totalColumnHeights = 0;

            Graphics g = e.Graphics;
            Rectangle displayRect = DisplayRectangle;
            Rectangle clipRect = e.ClipRectangle;

            //leave the space for the border
            int startx;  
            bool isRTL = (RightToLeft == RightToLeft.Yes);
            if (isRTL) {
                startx = displayRect.Right - (cellBorderWidth / 2);
            }
            else {
                startx = displayRect.X + (cellBorderWidth / 2);  
            }
            
            for (int i = 0; i < cols; i++) { 
                int starty = displayRect.Y + (cellBorderWidth / 2);

                if (isRTL) {
                    startx -= colStrips[i].MinSize;
                }
                
                for (int j = 0; j < rows; j++) {                 
                    Rectangle outsideCellBounds = new Rectangle(startx, starty, ((TableLayout.Strip)colStrips[i]).MinSize, ((TableLayout.Strip)rowStrips[j]).MinSize);                
                 
                    Rectangle insideCellBounds = new Rectangle(outsideCellBounds.X + (cellBorderWidth + 1) / 2, outsideCellBounds.Y + (cellBorderWidth + 1)/ 2, outsideCellBounds.Width - (cellBorderWidth + 1) / 2, outsideCellBounds.Height - (cellBorderWidth + 1) / 2);

                    if (clipRect.IntersectsWith(insideCellBounds)) {
                        //first, call user's painting code
                        using (TableLayoutCellPaintEventArgs pcea = new TableLayoutCellPaintEventArgs(g, clipRect, insideCellBounds, i, j)) {
                            OnCellPaint(pcea);
                        }
                        // paint the table border on top.
                        ControlPaint.PaintTableCellBorder(cellBorderStyle, g, outsideCellBounds);  
                    }
                    starty += rowStrips[j].MinSize;
                    // Only sum this up once...
                    if (i == 0) {
                        totalColumnHeights += rowStrips[j].MinSize;
                    }
                }
                
                if (!isRTL) {
                    startx += colStrips[i].MinSize;
                }                    
                totalColumnWidths += colStrips[i].MinSize;
            }

            
            if (!HScroll && !VScroll && cellBorderStyle != TableLayoutPanelCellBorderStyle.None) {
                Rectangle tableBounds = new Rectangle(cellBorderWidth/2 + displayRect.X, cellBorderWidth/2 + displayRect.Y, displayRect.Width - cellBorderWidth, displayRect.Height - cellBorderWidth);
                // paint the border of the table if we are not auto scrolling.
                // if the borderStyle is Inset or Outset, we can only paint the lower bottom half since otherwise we will have 1 pixel loss at the border.
                if (cellBorderStyle == TableLayoutPanelCellBorderStyle.Inset) {
                    g.DrawLine(SystemPens.ControlDark, tableBounds.Right, tableBounds.Y, tableBounds.Right, tableBounds.Bottom);
                    g.DrawLine(SystemPens.ControlDark, tableBounds.X, tableBounds.Y + tableBounds.Height - 1, tableBounds.X + tableBounds.Width - 1, tableBounds.Y + tableBounds.Height - 1);
                }
                else if (cellBorderStyle == TableLayoutPanelCellBorderStyle.Outset) {  
                    using (Pen pen = new Pen(SystemColors.Window)) {
                        g.DrawLine(pen, tableBounds.X + tableBounds.Width - 1, tableBounds.Y, tableBounds.X + tableBounds.Width - 1, tableBounds.Y + tableBounds.Height - 1);
                        g.DrawLine(pen, tableBounds.X, tableBounds.Y + tableBounds.Height - 1, tableBounds.X + tableBounds.Width - 1, tableBounds.Y + tableBounds.Height - 1);
                    }
                }
                else {
                    ControlPaint.PaintTableCellBorder(cellBorderStyle, g, tableBounds);     
                }
                ControlPaint.PaintTableControlBorder(cellBorderStyle, g, displayRect);
            }
            else {
                ControlPaint.PaintTableControlBorder(cellBorderStyle, g, displayRect);
            }
            
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ScaleCore(float dx, float dy) {

            base.ScaleCore(dx, dy);
            ScaleAbsoluteStyles(new SizeF(dx,dy));
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ScaleControl"]/*' />
        /// <devdoc>
        ///     Scale this form.  Form overrides this to enforce a maximum / minimum size.
        /// </devdoc>
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
            base.ScaleControl(factor, specified);
            ScaleAbsoluteStyles(factor);           
        }

        private void ScaleAbsoluteStyles(SizeF factor) {
            TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(this);
            int i = 0;
            
             // The last row/column can be larger than the 
             // absolutely styled column width.
             int lastRowHeight = -1;
             int lastRow = containerInfo.Rows.Length -1;
             if (containerInfo.Rows.Length > 0) {
                lastRowHeight = containerInfo.Rows[lastRow].MinSize;
             }
    
             int lastColumnHeight = -1;
             int lastColumn = containerInfo.Columns.Length -1;
             if (containerInfo.Columns.Length > 0) {
                lastColumnHeight = containerInfo.Columns[containerInfo.Columns.Length -1].MinSize;
             }  
    
             foreach(ColumnStyle cs in ColumnStyles) {               
                 if (cs.SizeType == SizeType.Absolute){
                     if (i == lastColumn && lastColumnHeight > 0) {
                          // the last column is typically expanded to fill the table. use the actual
                          // width in this case.
                          cs.Width = (float)Math.Round(lastColumnHeight * factor.Width);
                     }
                     else {
                         cs.Width = (float)Math.Round(cs.Width * factor.Width);
                     }
                 }
                 i++;
             }
    
             i = 0;
            
             foreach(RowStyle rs in RowStyles) {
                 if (rs.SizeType == SizeType.Absolute) {
                     if (i == lastRow && lastRowHeight > 0) {
                          // the last row is typically expanded to fill the table. use the actual
                          // width in this case.
                         rs.Height = (float)Math.Round(lastRowHeight * factor.Height);
                     }
                     else {
                         rs.Height = (float)Math.Round(rs.Height * factor.Height);
                     }
                 }
             }

        }

        #endregion 
    }

    #region ControlCollection
    /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutControlCollection"]/*' />
    /// <devdoc>
    /// <para>Represents a collection of controls on the TableLayoutPanel.</para>
    /// </devdoc>
    [ListBindable(false)]
    [DesignerSerializer("System.Windows.Forms.Design.TableLayoutControlCollectionCodeDomSerializer, " + AssemblyRef.SystemDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign)]
    public class TableLayoutControlCollection : Control.ControlCollection {
        private TableLayoutPanel _container;
        
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutControlCollection.TableLayoutControlCollection"]/*' />
        public TableLayoutControlCollection(TableLayoutPanel container) : base(container) {
            _container = (TableLayoutPanel)container;
        }

        //the container of this TableLayoutControlCollection
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutControlCollection.Container"]/*' />
        public TableLayoutPanel Container {
            get { return _container; }
        }

        //Add control to cell (x, y) on the table. The control becomes absolutely positioned if neither x nor y is equal to -1
        /// <include file='doc\TableLayoutPanel.uex' path='docs/doc[@for="TableLayoutControlCollection.Add"]/*' />
        public virtual void Add(Control control, int column, int row) {
            base.Add(control);
            _container.SetColumn(control, column);
            _container.SetRow(control, row);
        }
    }
    #endregion
}   
