// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Stores the <see cref='StatusBar'/>
    ///  control panel's information.
    /// </summary>
    [
    ToolboxItem(false),
    DesignTimeVisible(false),
    DefaultProperty(nameof(Text))
    ]
    public class StatusBarPanel : Component, ISupportInitialize
    {
        private const int DEFAULTWIDTH = 100;
        private const int DEFAULTMINWIDTH = 10;
        private const int PANELTEXTINSET = 3;
        private const int PANELGAP = 2;

        private string text = string.Empty;
        private string name = string.Empty;
        private string toolTipText = string.Empty;
        private Icon icon = null;

        private HorizontalAlignment alignment = HorizontalAlignment.Left;
        private StatusBarPanelBorderStyle borderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.Sunken;
        private StatusBarPanelStyle style = StatusBarPanelStyle.Text;

        // these are package scope so the parent can get at them.
        //
        private StatusBar parent = null;
        private int width = DEFAULTWIDTH;
        private int right = 0;
        private int minWidth = DEFAULTMINWIDTH;
        private int index = 0;
        private StatusBarPanelAutoSize autoSize = StatusBarPanelAutoSize.None;

        private bool initializing = false;

        private object userData;

        /// <summary>
        ///  Initializes a new default instance of the <see cref='StatusBarPanel'/> class.
        /// </summary>
        public StatusBarPanel()
        {
        }

        /// <summary>
        ///  Gets or sets the <see cref='Alignment'/>
        ///  property.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(HorizontalAlignment.Left),
        Localizable(true),
        SRDescription(nameof(SR.StatusBarPanelAlignmentDescr))
        ]
        public HorizontalAlignment Alignment
        {
            get
            {
                return alignment;
            }

            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                }
                if (alignment != value)
                {
                    alignment = value;
                    Realize();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the <see cref='AutoSize'/>
        ///  property.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(StatusBarPanelAutoSize.None),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.StatusBarPanelAutoSizeDescr))
        ]
        public StatusBarPanelAutoSize AutoSize
        {
            get
            {
                return autoSize;
            }

            set
            {
                //valid values are 0x1 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)StatusBarPanelAutoSize.None, (int)StatusBarPanelAutoSize.Contents))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StatusBarPanelAutoSize));
                }
                if (autoSize != value)
                {
                    autoSize = value;
                    UpdateSize();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the <see cref='BorderStyle'/>
        ///
        ///  property.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(System.Windows.Forms.StatusBarPanelBorderStyle.Sunken),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.StatusBarPanelBorderStyleDescr))
        ]
        public StatusBarPanelBorderStyle BorderStyle
        {
            get
            {
                return borderStyle;
            }

            set
            {
                //valid values are 0x1 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)StatusBarPanelBorderStyle.None, (int)StatusBarPanelBorderStyle.Sunken))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StatusBarPanelBorderStyle));
                }
                if (borderStyle != value)
                {
                    borderStyle = value;
                    Realize();
                    if (Created)
                    {
                        parent.Invalidate();
                    }
                }
            }
        }

        internal bool Created
        {
            get
            {
                return parent != null && parent.ArePanelsRealized();
            }
        }

        /// <summary>
        ///  Gets or sets the <see cref='Icon'/>
        ///  property.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(null),
        Localizable(true),
        SRDescription(nameof(SR.StatusBarPanelIconDescr))
        ]
        public Icon Icon
        {
            get
            {
                // unfortunately we have no way of getting the icon from the control.
                return icon;
            }

            set
            {

                if (value != null && (((Icon)value).Height > SystemInformation.SmallIconSize.Height || ((Icon)value).Width > SystemInformation.SmallIconSize.Width))
                {
                    icon = new Icon(value, SystemInformation.SmallIconSize);
                }
                else
                {
                    icon = value;
                }

                if (Created)
                {
                    IntPtr handle = (icon == null) ? IntPtr.Zero : icon.Handle;
                    parent.SendMessage(NativeMethods.SB_SETICON, (IntPtr)GetIndex(), handle);

                }
                UpdateSize();
                if (Created)
                {
                    parent.Invalidate();
                }
            }
        }

        /// <summary>
        ///  Expose index internally
        /// </summary>
        internal int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }
        /// <summary>
        ///  Gets or sets the minimum width the <see cref='StatusBarPanel'/> can be within the <see cref='StatusBar'/>
        ///  control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(DEFAULTMINWIDTH),
        Localizable(true),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.StatusBarPanelMinWidthDescr))
        ]
        public int MinWidth
        {
            get
            {
                return minWidth;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(MinWidth), value, 0));
                }

                if (value != minWidth)
                {
                    minWidth = value;

                    UpdateSize();
                    if (minWidth > Width)
                    {
                        Width = value;
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets the name of the panel.
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            Localizable(true),
            SRDescription(nameof(SR.StatusBarPanelNameDescr))
            ]
        public string Name
        {
            get
            {
                return WindowsFormsUtils.GetComponentName(this, name);
            }
            set
            {
                name = value;
                if (Site != null)
                {
                    Site.Name = name;
                }
            }
        }

        /// <summary>
        ///  Represents the <see cref='StatusBar'/>
        ///  control which hosts the
        ///  panel.
        /// </summary>
        [Browsable(false)]
        public StatusBar Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        ///  Expose a direct setter for parent internally
        /// </summary>
        internal StatusBar ParentInternal
        {
            set
            {
                parent = value;
            }
        }

        /// <summary>
        ///  Expose right internally
        /// </summary>
        internal int Right
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
            }
        }

        /// <summary>
        ///  Gets or sets the style of the panel.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(StatusBarPanelStyle.Text),
        SRDescription(nameof(SR.StatusBarPanelStyleDescr))
        ]
        public StatusBarPanelStyle Style
        {
            get { return style; }
            set
            {
                //valid values are 0x1 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)StatusBarPanelStyle.Text, (int)StatusBarPanelStyle.OwnerDraw))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StatusBarPanelStyle));
                }
                if (style != value)
                {
                    style = value;
                    Realize();
                    if (Created)
                    {
                        parent.Invalidate();
                    }
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

        /// <summary>
        ///  Gets or sets the text of the panel.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(""),
        SRDescription(nameof(SR.StatusBarPanelTextDescr))
        ]
        public string Text
        {
            get
            {
                if (text == null)
                {
                    return "";
                }
                else
                {
                    return text;
                }
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (!Text.Equals(value))
                {

                    if (value.Length == 0)
                    {
                        text = null;
                    }
                    else
                    {
                        text = value;
                    }
                    Realize();
                    UpdateSize();
                }
            }
        }

        /// <summary>
        ///  Gets
        ///  or sets the panel's tool tip text.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(""),
        SRDescription(nameof(SR.StatusBarPanelToolTipTextDescr))
        ]
        public string ToolTipText
        {
            get
            {
                if (toolTipText == null)
                {
                    return "";
                }
                else
                {
                    return toolTipText;
                }
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (!ToolTipText.Equals(value))
                {

                    if (value.Length == 0)
                    {
                        toolTipText = null;
                    }
                    else
                    {
                        toolTipText = value;
                    }

                    if (Created)
                    {
                        parent.UpdateTooltip(this);
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets the width of the <see cref='StatusBarPanel'/> within the <see cref='StatusBar'/>
        ///  control.
        /// </summary>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(DEFAULTWIDTH),
        SRDescription(nameof(SR.StatusBarPanelWidthDescr))
        ]
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (!initializing && value < minWidth)
                {
                    throw new ArgumentOutOfRangeException(nameof(Width), SR.WidthGreaterThanMinWidth);
                }

                width = value;
                UpdateSize();
            }
        }

        /// <summary>
        ///  Handles tasks required when the control is being initialized.
        /// </summary>
        public void BeginInit()
        {
            initializing = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (parent != null)
                {
                    int index = GetIndex();
                    if (index != -1)
                    {
                        parent.Panels.RemoveAt(index);
                    }
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Called when initialization of the control is complete.
        /// </summary>
        public void EndInit()
        {
            initializing = false;

            if (Width < MinWidth)
            {
                Width = MinWidth;
            }
        }

        /// <summary>
        ///  Gets the width of the contents of the panel
        /// </summary>
        internal int GetContentsWidth(bool newPanel)
        {
            string text;
            if (newPanel)
            {
                if (this.text == null)
                {
                    text = string.Empty;
                }
                else
                {
                    text = this.text;
                }
            }
            else
            {
                text = Text;
            }

            Graphics g = parent.CreateGraphicsInternal();
            Size sz = Size.Ceiling(g.MeasureString(text, parent.Font));
            if (icon != null)
            {
                sz.Width += icon.Size.Width + 5;
            }
            g.Dispose();

            int width = sz.Width + SystemInformation.BorderSize.Width * 2 + PANELTEXTINSET * 2 + PANELGAP;
            return Math.Max(width, minWidth);
        }

        /// <summary>
        ///  Returns the index of the panel by making the parent control search
        ///  for it within its list.
        /// </summary>
        private int GetIndex()
        {
            return index;
        }

        /// <summary>
        ///  Sets all the properties for this panel.
        /// </summary>
        internal void Realize()
        {
            if (Created)
            {
                string text;
                string sendText;
                int border = 0;

                if (this.text == null)
                {
                    text = string.Empty;
                }
                else
                {
                    text = this.text;
                }

                HorizontalAlignment align = alignment;
                // Translate the alignment for Rtl apps
                //
                if (parent.RightToLeft == RightToLeft.Yes)
                {
                    switch (align)
                    {
                        case HorizontalAlignment.Left:
                            align = HorizontalAlignment.Right;
                            break;
                        case HorizontalAlignment.Right:
                            align = HorizontalAlignment.Left;
                            break;
                    }
                }

                switch (align)
                {
                    case HorizontalAlignment.Center:
                        sendText = "\t" + text;
                        break;
                    case HorizontalAlignment.Right:
                        sendText = "\t\t" + text;
                        break;
                    default:
                        sendText = text;
                        break;
                }
                switch (borderStyle)
                {
                    case StatusBarPanelBorderStyle.None:
                        border |= NativeMethods.SBT_NOBORDERS;
                        break;
                    case StatusBarPanelBorderStyle.Sunken:
                        break;
                    case StatusBarPanelBorderStyle.Raised:
                        border |= NativeMethods.SBT_POPOUT;
                        break;
                }
                switch (style)
                {
                    case StatusBarPanelStyle.Text:
                        break;
                    case StatusBarPanelStyle.OwnerDraw:
                        border |= NativeMethods.SBT_OWNERDRAW;
                        break;
                }

                int wparam = GetIndex() | border;
                if (parent.RightToLeft == RightToLeft.Yes)
                {
                    wparam |= NativeMethods.SBT_RTLREADING;
                }

                int result = (int)UnsafeNativeMethods.SendMessage(new HandleRef(parent, parent.Handle), NativeMethods.SB_SETTEXT, (IntPtr)wparam, sendText);

                if (result == 0)
                {
                    throw new InvalidOperationException(SR.UnableToSetPanelText);
                }

                if (icon != null && style != StatusBarPanelStyle.OwnerDraw)
                {
                    parent.SendMessage(NativeMethods.SB_SETICON, (IntPtr)GetIndex(), icon.Handle);
                }
                else
                {
                    parent.SendMessage(NativeMethods.SB_SETICON, (IntPtr)GetIndex(), IntPtr.Zero);
                }

                if (style == StatusBarPanelStyle.OwnerDraw)
                {
                    RECT rect = new RECT();
                    result = (int)UnsafeNativeMethods.SendMessage(new HandleRef(parent, parent.Handle), NativeMethods.SB_GETRECT, (IntPtr)GetIndex(), ref rect);

                    if (result != 0)
                    {
                        parent.Invalidate(Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom));
                    }
                }
            }
        }

        private void UpdateSize()
        {
            if (autoSize == StatusBarPanelAutoSize.Contents)
            {
                ApplyContentSizing();
            }
            else
            {
                if (Created)
                {
                    parent.DirtyLayout();
                    parent.PerformLayout();
                }
            }
        }

        private void ApplyContentSizing()
        {
            if (autoSize == StatusBarPanelAutoSize.Contents &&
                parent != null)
            {
                int newWidth = GetContentsWidth(false);
                if (newWidth != Width)
                {
                    Width = newWidth;
                    if (Created)
                    {
                        parent.DirtyLayout();
                        parent.PerformLayout();
                    }
                }
            }
        }

        /// <summary>
        ///  Retrieves a string that contains information about the
        ///  panel.
        /// </summary>
        public override string ToString()
        {
            return "StatusBarPanel: {" + Text + "}";
        }
    }
}
