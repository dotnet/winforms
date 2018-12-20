// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Windows.Forms;    
    using System.Globalization;
    
    /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Displays a single column header in a <see cref='System.Windows.Forms.ListView'/>
    ///       control.
    ///
    ///    </para>
    /// </devdoc>
    [
    ToolboxItem(false),
    DesignTimeVisible(false),
    DefaultProperty(nameof(Text)),
    TypeConverterAttribute(typeof(ColumnHeaderConverter))
    ]
    public class ColumnHeader : Component, ICloneable {

// disable csharp compiler warning #0414: field assigned unused value
#pragma warning disable 0414
        internal int index = -1;
#pragma warning restore 0414
        internal string text = null;
        internal string name = null;
        internal int width = 60;
        // Use TextAlign property instead of this member variable, always
        private HorizontalAlignment textAlign = HorizontalAlignment.Left;
        private bool textAlignInitialized = false;
        private int displayIndexInternal = -1;
        private ColumnHeaderImageListIndexer imageIndexer = null;

        object userData;

        private ListView listview;
        // We need to send some messages to ListView when it gets initialized.
        internal ListView OwnerListview
        {
            get
            {
                return listview;
            }
            set
            {
                int width = this.Width;

                listview = value;
                
                // The below properties are set into the listview.
                this.Width = width;
            }
        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.ColumnHeader"]/*' />
        /// <devdoc>
        ///     Creates a new ColumnHeader object
        /// </devdoc>
        public ColumnHeader() {
            imageIndexer = new ColumnHeaderImageListIndexer(this);
        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.ColumnHeader1"]/*' />
        /// <devdoc>
        ///     Creates a new ColumnHeader object
        /// </devdoc>
        public ColumnHeader(int imageIndex) : this () {
            this.ImageIndex = imageIndex;
        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.ColumnHeader2"]/*' />
        /// <devdoc>
        ///     Creates a new ColumnHeader object
        /// </devdoc>
        public ColumnHeader(string imageKey) : this () {
            this.ImageKey = imageKey;
        }

        internal int ActualImageIndex_Internal {
            get {
                int imgIndex = this.imageIndexer.ActualIndex;
                if (this.ImageList == null || this.ImageList.Images == null || imgIndex >= this.ImageList.Images.Count) {
                    // the ImageIndex equivalent of a ImageKey that does not exist in the ImageList
                    return -1;
                } else {
                    return imgIndex;
                }
            }
        }


        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.DisplayIndex"]/*' />
	[
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
	SRCategory(nameof(SR.CatBehavior)),
	SRDescription(nameof(SR.ColumnHeaderDisplayIndexDescr))
	]
        public int DisplayIndex {
            get {
                return this.DisplayIndexInternal;
            }
		
	    set {

                // When the list is being deserialized we need
                // to take the display index as is. ListView
                // does correctly synchronize the indices.
                if (this.listview == null) {
                   this.DisplayIndexInternal = value;
                   return;
                }

	        if (value < 0 || value> (this.listview.Columns.Count - 1)) {
                    throw new ArgumentOutOfRangeException(nameof(DisplayIndex), SR.ColumnHeaderBadDisplayIndex);
	        }

                int lowDI = Math.Min(this.DisplayIndexInternal, value);
                int hiDI = Math.Max(this.DisplayIndexInternal, value);
                int[] colsOrder = new int[this.listview.Columns.Count];

                // set the display indices. This is not an expensive operation because
                // we only set an integer in the column header class
                bool hdrMovedForward = value > this.DisplayIndexInternal;
                ColumnHeader movedHdr = null;
                for (int i = 0; i < this.listview.Columns.Count; i ++) {

                    ColumnHeader hdr = this.listview.Columns[i];
                    if (hdr.DisplayIndex == this.DisplayIndexInternal) {
                        movedHdr = hdr;
                    } else if (hdr.DisplayIndex >= lowDI && hdr.DisplayIndex <= hiDI) {
                        hdr.DisplayIndexInternal -= hdrMovedForward ? 1 : -1;
                    }
                    if (i != this.Index) {
                       colsOrder[ hdr.DisplayIndexInternal ] = i;
                    }
                }

                movedHdr.DisplayIndexInternal = value;
                colsOrder[ movedHdr.DisplayIndexInternal ] = movedHdr.Index;
                SetDisplayIndices( colsOrder );
	    }
	}

        internal int DisplayIndexInternal {
            get {
                return this.displayIndexInternal;
            }
            set {
                this.displayIndexInternal = value;
            }
        }

                /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.Index"]/*' />
        /// <devdoc>
        ///     The index of this column.  This index does not necessarily correspond
        ///     to the current visual position of the column in the ListView, because the
        ///     user may orerder columns if the allowColumnReorder property is true.
        /// </devdoc>
        [ Browsable(false)]
        public int Index {
            get {
                if (listview != null)
                    return listview.GetColumnIndex(this);
                return -1;  
            }
        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.ImageIndex"]/*' />
        [
        DefaultValue(-1),
        TypeConverterAttribute(typeof(ImageIndexConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RefreshProperties(RefreshProperties.Repaint),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int ImageIndex {
            get {
                if (imageIndexer.Index != -1 && ImageList != null && imageIndexer.Index >= ImageList.Images.Count) {
                    return ImageList.Images.Count - 1;
                } 
                return imageIndexer.Index;
            }
            set {
                if (value < -1) {
                    throw new ArgumentOutOfRangeException(nameof(ImageIndex), string.Format(SR.InvalidLowBoundArgumentEx, "ImageIndex", (value).ToString(CultureInfo.CurrentCulture), (-1).ToString(CultureInfo.CurrentCulture)));
                }

                if (imageIndexer.Index != value) {
                    imageIndexer.Index = value;

                    if (ListView != null && ListView.IsHandleCreated) {
                        ListView.SetColumnInfo(NativeMethods.LVCF_IMAGE, this);
                    }
                }
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ImageList"]/*' />
        [Browsable(false)]
        public ImageList ImageList {
            // we added the ImageList property so that the ImageIndexConverter can find our image list
            get {
                return this.imageIndexer.ImageList;
            }
        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.ImageKey"]/*' />
        [
        DefaultValue(""),
        TypeConverterAttribute(typeof(ImageKeyConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RefreshProperties(RefreshProperties.Repaint),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public string ImageKey {
            get {
                return imageIndexer.Key;
            }
            set {
                if (value != imageIndexer.Key) {
                    imageIndexer.Key = value;

                    if (ListView != null && ListView.IsHandleCreated) {
                        ListView.SetColumnInfo(NativeMethods.LVCF_IMAGE, this);
                    }
                }
            }
        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.ListView"]/*' />
        /// <devdoc>
        ///     Returns the ListView control that this column is displayed in.  May be null
        /// </devdoc>
        [ Browsable(false) ]
        public ListView ListView {
            get {
                return this.listview;
            }
        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.Text"]/*' />
        /// <devdoc>
        ///     The Name of the column header
        /// </devdoc>
        [
        Browsable(false),
        SRDescription(nameof(SR.ColumnHeaderNameDescr))
        ]
        public string Name {
            get {
                return WindowsFormsUtils.GetComponentName(this,name);
            }
            set {
                if (value == null) {
                    this.name = "";
                }
                else {
                    this.name = value;
                }
                if(Site != null) {
                    Site.Name = value;
                }
            }

        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.Text"]/*' />
        /// <devdoc>
        ///     The text displayed in the column header
        /// </devdoc>
        [
        Localizable(true),
        SRDescription(nameof(SR.ColumnCaption))
        ]
        public string Text {
            get {
                return(text != null ? text : "ColumnHeader");
            }
            set {
                if (value == null) {
                    this.text = "";
                }
                else {
                    this.text = value;
                }
                if (listview != null) {
                    listview.SetColumnInfo(NativeMethods.LVCF_TEXT, this);
                }
            }

        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.TextAlign"]/*' />
        /// <devdoc>
        ///     The horizontal alignment of the text contained in this column
        /// </devdoc>
        [
        SRDescription(nameof(SR.ColumnAlignment)),
        Localizable(true),
        DefaultValue(HorizontalAlignment.Left)
        ]
        public HorizontalAlignment TextAlign {
            get {
                if (!textAlignInitialized && (listview != null))
                {
                        textAlignInitialized = true;
                        // See below for an explanation of (Index != 0)
                        //Added !IsMirrored
                        if ((Index != 0) && (listview.RightToLeft == RightToLeft.Yes) && !listview.IsMirrored)
                        {
                                this.textAlign = HorizontalAlignment.Right;
                        }
                }
                return this.textAlign;
            }
            set {
                //valid values are 0x0 to 0x2. 
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                }

                this.textAlign = value;
                
                // The first column must be left-aligned
                if (Index == 0 && this.textAlign != HorizontalAlignment.Left) {
                    this.textAlign = HorizontalAlignment.Left;
                }

                if (listview != null) {
                    listview.SetColumnInfo(NativeMethods.LVCF_FMT, this);
                    listview.Invalidate();
                }
            }
        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.Tag"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag {
            get {
                return userData;
            }
            set {
                userData = value;
            }
        }

        internal int WidthInternal {
            get {
                return width;
            }
        }
        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.Width"]/*' />
        /// <devdoc>
        ///     The width of the column in pixels.
        /// </devdoc>
        [
        SRDescription(nameof(SR.ColumnWidth)),
        Localizable(true),
        DefaultValue(60)
        ]
        public int Width {
            get {
                // Since we can't keep our private width in sync with the real width because
                // we don't get notified when the user changes it, we need to get this info
                // from the underlying control every time we're asked.
                // The underlying control will only report the correct width if it's in Report view
                if (listview != null && listview.IsHandleCreated && !listview.Disposing && listview.View == View.Details) {
                    
                    // Make sure this column has already been added to the ListView, else just return width
                    //
                    IntPtr hwndHdr = UnsafeNativeMethods.SendMessage(new HandleRef(listview, listview.Handle), NativeMethods.LVM_GETHEADER, 0, 0);
                    if (hwndHdr != IntPtr.Zero) {
                        int nativeColumnCount = (int)UnsafeNativeMethods.SendMessage(new HandleRef(listview, hwndHdr), NativeMethods.HDM_GETITEMCOUNT, 0, 0);
                        if (Index < nativeColumnCount) {
                            width = (int)UnsafeNativeMethods.SendMessage(new HandleRef(listview, listview.Handle), NativeMethods.LVM_GETCOLUMNWIDTH, Index, 0);
                        }
                    }
                }

                return width;
            }
            set {
                this.width = value;
                if (listview != null)
                    listview.SetColumnWidth(Index, ColumnHeaderAutoResizeStyle.None);
                }
        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.AutoResize"]/*' />
        public void AutoResize(ColumnHeaderAutoResizeStyle headerAutoResize) {

            if (headerAutoResize < ColumnHeaderAutoResizeStyle.None || headerAutoResize > ColumnHeaderAutoResizeStyle.ColumnContent) {
                throw new InvalidEnumArgumentException(nameof(headerAutoResize), (int)headerAutoResize, typeof(ColumnHeaderAutoResizeStyle));
            }

            if (this.listview != null) {
                this.listview.AutoResizeColumn(this.Index, headerAutoResize);
            }
        }
        

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.Clone"]/*' />
        /// <devdoc>
        ///     Creates an identical ColumnHeader, unattached to any ListView
        /// </devdoc>
        public object Clone() {
            Type clonedType = this.GetType();
            ColumnHeader columnHeader = null;

            if (clonedType == typeof(ColumnHeader)) {
                columnHeader = new ColumnHeader();
            }
            else {
                columnHeader = (ColumnHeader)Activator.CreateInstance(clonedType);
            }

            columnHeader.text = text;
            columnHeader.Width = width;
            columnHeader.textAlign = TextAlign;
            return columnHeader;
        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.Dispose"]/*' />
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (listview != null) {
                    int index = Index;
                    if (index != -1) {
                        listview.Columns.RemoveAt(index);
                    }
                }
            }
            base.Dispose(disposing);
        }

	private void ResetText() {
		Text = null;
	}

        // Set the display indices of the listview columns
        private void SetDisplayIndices(int[] cols) {

            if (this.listview.IsHandleCreated && !this.listview.Disposing) {
  	       UnsafeNativeMethods.SendMessage(new HandleRef(this.listview, this.listview.Handle), NativeMethods.LVM_SETCOLUMNORDERARRAY, cols.Length, cols);
	    }
        }

        private bool ShouldSerializeName() {
            return !string.IsNullOrEmpty(this.name);
        }

        private bool ShouldSerializeDisplayIndex() {
            return this.DisplayIndex != this.Index;
        }

        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.ShouldPersistText"]/*' />
        internal bool ShouldSerializeText() {
            return(text != null);
        }
        
        /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation of this column header
        /// </devdoc>
        public override string ToString() {
            return "ColumnHeader: Text: " + Text;
        }

        internal class ColumnHeaderImageListIndexer : ImageList.Indexer {
            private ColumnHeader owner = null;
            public ColumnHeaderImageListIndexer(ColumnHeader ch) {
                owner = ch;
            }

            public override ImageList ImageList {
                get {
                    if (owner != null && owner.ListView != null) {
                        return owner.ListView.SmallImageList;
                    }
                    return null;
                }
                set {
                    Debug.Assert(false, "We should never set the image list");
                }
            }
        }
    }
}
