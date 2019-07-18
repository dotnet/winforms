// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Displays a single column header in a <see cref='Forms.ListView'/> control.
    /// </summary>
    [
    ToolboxItem(false),
    DesignTimeVisible(false),
    DefaultProperty(nameof(Text)),
    TypeConverter(typeof(ColumnHeaderConverter))
    ]
    public class ColumnHeader : Component, ICloneable
    {
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
        private readonly ColumnHeaderImageListIndexer imageIndexer = null;

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
                int width = Width;

                listview = value;

                // The below properties are set into the listview.
                Width = width;
            }
        }

        /// <summary>
        ///  Creates a new ColumnHeader object
        /// </summary>
        public ColumnHeader()
        {
            imageIndexer = new ColumnHeaderImageListIndexer(this);
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

        internal int ActualImageIndex_Internal
        {
            get
            {
                int imgIndex = imageIndexer.ActualIndex;
                if (ImageList == null || ImageList.Images == null || imgIndex >= ImageList.Images.Count)
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

        [
            Localizable(true),
            RefreshProperties(RefreshProperties.Repaint),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ColumnHeaderDisplayIndexDescr))
        ]
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
                if (listview == null)
                {
                    DisplayIndexInternal = value;
                    return;
                }

                if (value < 0 || value > (listview.Columns.Count - 1))
                {
                    throw new ArgumentOutOfRangeException(nameof(DisplayIndex), SR.ColumnHeaderBadDisplayIndex);
                }

                int lowDI = Math.Min(DisplayIndexInternal, value);
                int hiDI = Math.Max(DisplayIndexInternal, value);
                int[] colsOrder = new int[listview.Columns.Count];

                // set the display indices. This is not an expensive operation because
                // we only set an integer in the column header class
                bool hdrMovedForward = value > DisplayIndexInternal;
                ColumnHeader movedHdr = null;
                for (int i = 0; i < listview.Columns.Count; i++)
                {

                    ColumnHeader hdr = listview.Columns[i];
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

                movedHdr.DisplayIndexInternal = value;
                colsOrder[movedHdr.DisplayIndexInternal] = movedHdr.Index;
                SetDisplayIndices(colsOrder);
            }
        }

        internal int DisplayIndexInternal
        {
            get
            {
                return displayIndexInternal;
            }
            set
            {
                displayIndexInternal = value;
            }
        }

        /// <summary>
        ///  The index of this column.  This index does not necessarily correspond
        ///  to the current visual position of the column in the ListView, because the
        ///  user may orerder columns if the allowColumnReorder property is true.
        /// </summary>
        [Browsable(false)]
        public int Index
        {
            get
            {
                if (listview != null)
                {
                    return listview.GetColumnIndex(this);
                }

                return -1;
            }
        }

        [
        DefaultValue(-1),
        TypeConverter(typeof(ImageIndexConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RefreshProperties(RefreshProperties.Repaint),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int ImageIndex
        {
            get
            {
                if (imageIndexer.Index != -1 && ImageList != null && imageIndexer.Index >= ImageList.Images.Count)
                {
                    return ImageList.Images.Count - 1;
                }
                return imageIndexer.Index;
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ImageIndex), value, -1));
                }

                if (imageIndexer.Index != value)
                {
                    imageIndexer.Index = value;

                    if (ListView != null && ListView.IsHandleCreated)
                    {
                        ListView.SetColumnInfo(NativeMethods.LVCF_IMAGE, this);
                    }
                }
            }
        }

        [Browsable(false)]
        public ImageList ImageList
        {
            // we added the ImageList property so that the ImageIndexConverter can find our image list
            get
            {
                return imageIndexer.ImageList;
            }
        }

        [
        DefaultValue(""),
        TypeConverter(typeof(ImageKeyConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RefreshProperties(RefreshProperties.Repaint),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public string ImageKey
        {
            get
            {
                return imageIndexer.Key;
            }
            set
            {
                if (value != imageIndexer.Key)
                {
                    imageIndexer.Key = value;

                    if (ListView != null && ListView.IsHandleCreated)
                    {
                        ListView.SetColumnInfo(NativeMethods.LVCF_IMAGE, this);
                    }
                }
            }
        }

        /// <summary>
        ///  Returns the ListView control that this column is displayed in.  May be null
        /// </summary>
        [Browsable(false)]
        public ListView ListView
        {
            get
            {
                return listview;
            }
        }

        /// <summary>
        ///  The Name of the column header
        /// </summary>
        [
        Browsable(false),
        SRDescription(nameof(SR.ColumnHeaderNameDescr))
        ]
        public string Name
        {
            get
            {
                return WindowsFormsUtils.GetComponentName(this, name);
            }
            set
            {
                if (value == null)
                {
                    name = string.Empty;
                }
                else
                {
                    name = value;
                }
                if (Site != null)
                {
                    Site.Name = value;
                }
            }

        }

        /// <summary>
        ///  The text displayed in the column header
        /// </summary>
        [
        Localizable(true),
        SRDescription(nameof(SR.ColumnCaption))
        ]
        public string Text
        {
            get
            {
                return (text ?? "ColumnHeader");
            }
            set
            {
                if (value == null)
                {
                    text = string.Empty;
                }
                else
                {
                    text = value;
                }
                if (listview != null)
                {
                    listview.SetColumnInfo(NativeMethods.LVCF_TEXT, this);
                }
            }

        }

        /// <summary>
        ///  The horizontal alignment of the text contained in this column
        /// </summary>
        [
        SRDescription(nameof(SR.ColumnAlignment)),
        Localizable(true),
        DefaultValue(HorizontalAlignment.Left)
        ]
        public HorizontalAlignment TextAlign
        {
            get
            {
                if (!textAlignInitialized && (listview != null))
                {
                    textAlignInitialized = true;
                    // See below for an explanation of (Index != 0)
                    //Added !IsMirrored
                    if ((Index != 0) && (listview.RightToLeft == RightToLeft.Yes) && !listview.IsMirrored)
                    {
                        textAlign = HorizontalAlignment.Right;
                    }
                }
                return textAlign;
            }
            set
            {
                //valid values are 0x0 to 0x2.
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                }

                textAlign = value;

                // The first column must be left-aligned
                if (Index == 0 && textAlign != HorizontalAlignment.Left)
                {
                    textAlign = HorizontalAlignment.Left;
                }

                if (listview != null)
                {
                    listview.SetColumnInfo(NativeMethods.LVCF_FMT, this);
                    listview.Invalidate();
                }
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag
        {
            get
            {
                return userData;
            }
            set
            {
                userData = value;
            }
        }

        internal int WidthInternal
        {
            get
            {
                return width;
            }
        }
        /// <summary>
        ///  The width of the column in pixels.
        /// </summary>
        [
        SRDescription(nameof(SR.ColumnWidth)),
        Localizable(true),
        DefaultValue(60)
        ]
        public int Width
        {
            get
            {
                // Since we can't keep our private width in sync with the real width because
                // we don't get notified when the user changes it, we need to get this info
                // from the underlying control every time we're asked.
                // The underlying control will only report the correct width if it's in Report view
                if (listview != null && listview.IsHandleCreated && !listview.Disposing && listview.View == View.Details)
                {

                    // Make sure this column has already been added to the ListView, else just return width
                    //
                    IntPtr hwndHdr = UnsafeNativeMethods.SendMessage(new HandleRef(listview, listview.Handle), NativeMethods.LVM_GETHEADER, 0, 0);
                    if (hwndHdr != IntPtr.Zero)
                    {
                        int nativeColumnCount = (int)UnsafeNativeMethods.SendMessage(new HandleRef(listview, hwndHdr), NativeMethods.HDM_GETITEMCOUNT, 0, 0);
                        if (Index < nativeColumnCount)
                        {
                            width = (int)UnsafeNativeMethods.SendMessage(new HandleRef(listview, listview.Handle), NativeMethods.LVM_GETCOLUMNWIDTH, Index, 0);
                        }
                    }
                }

                return width;
            }
            set
            {
                width = value;
                if (listview != null)
                {
                    listview.SetColumnWidth(Index, ColumnHeaderAutoResizeStyle.None);
                }
            }
        }

        public void AutoResize(ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            if (headerAutoResize < ColumnHeaderAutoResizeStyle.None || headerAutoResize > ColumnHeaderAutoResizeStyle.ColumnContent)
            {
                throw new InvalidEnumArgumentException(nameof(headerAutoResize), (int)headerAutoResize, typeof(ColumnHeaderAutoResizeStyle));
            }

            if (listview != null)
            {
                listview.AutoResizeColumn(Index, headerAutoResize);
            }
        }

        /// <summary>
        ///  Creates an identical ColumnHeader, unattached to any ListView
        /// </summary>
        public object Clone()
        {
            Type clonedType = GetType();
            ColumnHeader columnHeader = null;

            if (clonedType == typeof(ColumnHeader))
            {
                columnHeader = new ColumnHeader();
            }
            else
            {
                columnHeader = (ColumnHeader)Activator.CreateInstance(clonedType);
            }

            columnHeader.text = text;
            columnHeader.Width = width;
            columnHeader.textAlign = TextAlign;
            return columnHeader;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (listview != null)
                {
                    int index = Index;
                    if (index != -1)
                    {
                        listview.Columns.RemoveAt(index);
                    }
                }
            }
            base.Dispose(disposing);
        }

        private void ResetText()
        {
            Text = null;
        }

        // Set the display indices of the listview columns
        private void SetDisplayIndices(int[] cols)
        {
            if (listview.IsHandleCreated && !listview.Disposing)
            {
                UnsafeNativeMethods.SendMessage(new HandleRef(listview, listview.Handle), NativeMethods.LVM_SETCOLUMNORDERARRAY, cols.Length, cols);
            }
        }

        private bool ShouldSerializeName()
        {
            return !string.IsNullOrEmpty(name);
        }

        private bool ShouldSerializeDisplayIndex()
        {
            return DisplayIndex != Index;
        }

        internal bool ShouldSerializeText()
        {
            return (text != null);
        }

        /// <summary>
        ///  Returns a string representation of this column header
        /// </summary>
        public override string ToString()
        {
            return "ColumnHeader: Text: " + Text;
        }

        internal class ColumnHeaderImageListIndexer : ImageList.Indexer
        {
            private readonly ColumnHeader owner = null;
            public ColumnHeaderImageListIndexer(ColumnHeader ch)
            {
                owner = ch;
            }

            public override ImageList ImageList
            {
                get
                {
                    return owner.ListView?.SmallImageList;
                }
                set
                {
                    Debug.Assert(false, "We should never set the image list");
                }
            }
        }
    }
}
