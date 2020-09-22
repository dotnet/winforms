// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.Internal;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides a user interface for <see cref='WindowsFormsComponentEditor'/>.
    /// </summary>
    [ToolboxItem(false)]
    public class ComponentEditorForm : Form
    {
        private readonly IComponent component;
        private readonly Type[] pageTypes;
        private ComponentEditorPageSite[] pageSites;
        private Size maxSize = System.Drawing.Size.Empty;
        private int initialActivePage;
        private int activePage;
        private bool dirty;
        private bool firstActivate;

        private readonly Panel pageHost = new Panel();
        private PageSelector selector;
        private ImageList selectorImageList;
        private Button okButton;
        private Button cancelButton;
        private Button applyButton;
        private Button helpButton;

        // private DesignerTransaction transaction;

        private const int BUTTON_WIDTH = 80;
        private const int BUTTON_HEIGHT = 23;
        private const int BUTTON_PAD = 6;
        private const int MIN_SELECTOR_WIDTH = 90;
        private const int SELECTOR_PADDING = 10;
        private const int STRIP_HEIGHT = 4;

        /// <summary>
        ///  Initializes a new instance of the <see cref='ComponentEditorForm'/> class.
        /// </summary>
        public ComponentEditorForm(object component, Type[] pageTypes) : base()
        {
            if (!(component is IComponent))
            {
                throw new ArgumentException(SR.ComponentEditorFormBadComponent, nameof(component));
            }
            this.component = (IComponent)component;
            this.pageTypes = pageTypes;
            dirty = false;
            firstActivate = true;
            activePage = -1;
            initialActivePage = 0;

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            Icon = null;
            StartPosition = FormStartPosition.CenterParent;

            OnNewObjects();
            OnConfigureUI();
        }

        /// <summary>
        ///  Applies any changes in the set of ComponentPageControl to the actual component.
        /// </summary>
        internal virtual void ApplyChanges(bool lastApply)
        {
            if (dirty)
            {
                IComponentChangeService changeService = null;

                if (component.Site != null)
                {
                    changeService = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
                    if (changeService != null)
                    {
                        try
                        {
                            changeService.OnComponentChanging(component, null);
                        }
                        catch (CheckoutException e)
                        {
                            if (e == CheckoutException.Canceled)
                            {
                                return;
                            }
                            throw;
                        }
                    }
                }

                for (int n = 0; n < pageSites.Length; n++)
                {
                    if (pageSites[n].Dirty)
                    {
                        pageSites[n].GetPageControl().ApplyChanges();
                        pageSites[n].Dirty = false;
                    }
                }

                if (changeService != null)
                {
                    changeService.OnComponentChanged(component, null, null, null);
                }

                applyButton.Enabled = false;
                cancelButton.Text = SR.CloseCaption;
                dirty = false;

                if (lastApply == false)
                {
                    for (int n = 0; n < pageSites.Length; n++)
                    {
                        pageSites[n].GetPageControl().OnApplyComplete();
                    }
                }
            }
        }

        /// <summary>
        ///  Hide the property
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get => base.AutoSize;
            set => base.AutoSize = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  Handles ok/cancel/apply/help button click events
        /// </summary>
        private void OnButtonClick(object sender, EventArgs e)
        {
            if (sender == okButton)
            {
                ApplyChanges(true);
                DialogResult = DialogResult.OK;
            }
            else if (sender == cancelButton)
            {
                DialogResult = DialogResult.Cancel;
            }
            else if (sender == applyButton)
            {
                ApplyChanges(false);
            }
            else if (sender == helpButton)
            {
                ShowPageHelp();
            }
        }

        /// <summary>
        ///  Lays out the UI of the form.
        /// </summary>
        private void OnConfigureUI()
        {
            Font uiFont = Control.DefaultFont;
            if (component.Site != null)
            {
                IUIService uiService = (IUIService)component.Site.GetService(typeof(IUIService));
                if (uiService != null)
                {
                    uiFont = (Font)uiService.Styles["DialogFont"];
                }
            }

            Font = uiFont;

            okButton = new Button();
            cancelButton = new Button();
            applyButton = new Button();
            helpButton = new Button();

            selectorImageList = new ImageList
            {
                ImageSize = new Size(16, 16)
            };
            selector = new PageSelector
            {
                ImageList = selectorImageList
            };
            selector.AfterSelect += new TreeViewEventHandler(OnSelChangeSelector);

            Label grayStrip = new Label
            {
                BackColor = SystemColors.ControlDark
            };

            int selectorWidth = MIN_SELECTOR_WIDTH;

            if (pageSites != null)
            {
                // Add the nodes corresponding to the pages
                for (int n = 0; n < pageSites.Length; n++)
                {
                    ComponentEditorPage page = pageSites[n].GetPageControl();

                    string title = page.Title;
                    Graphics graphics = CreateGraphicsInternal();
                    int titleWidth = (int)graphics.MeasureString(title, Font).Width;
                    graphics.Dispose();
                    selectorImageList.Images.Add(page.Icon.ToBitmap());

                    selector.Nodes.Add(new TreeNode(title, n, n));
                    if (titleWidth > selectorWidth)
                    {
                        selectorWidth = titleWidth;
                    }
                }
            }
            selectorWidth += SELECTOR_PADDING;

            string caption = string.Empty;
            ISite site = component.Site;
            if (site != null)
            {
                caption = string.Format(SR.ComponentEditorFormProperties, site.Name);
            }
            else
            {
                caption = SR.ComponentEditorFormPropertiesNoName;
            }
            Text = caption;

            Rectangle pageHostBounds = new Rectangle(2 * BUTTON_PAD + selectorWidth, 2 * BUTTON_PAD + STRIP_HEIGHT,
                                                     maxSize.Width, maxSize.Height);
            pageHost.Bounds = pageHostBounds;
            grayStrip.Bounds = new Rectangle(pageHostBounds.X, BUTTON_PAD,
                                             pageHostBounds.Width, STRIP_HEIGHT);

            if (pageSites != null)
            {
                Rectangle pageBounds = new Rectangle(0, 0, pageHostBounds.Width, pageHostBounds.Height);
                for (int n = 0; n < pageSites.Length; n++)
                {
                    ComponentEditorPage page = pageSites[n].GetPageControl();
                    page.GetControl().Bounds = pageBounds;
                }
            }

            int xFrame = SystemInformation.FixedFrameBorderSize.Width;
            Rectangle bounds = pageHostBounds;
            Size size = new Size(bounds.Width + 3 * (BUTTON_PAD + xFrame) + selectorWidth,
                                   bounds.Height + STRIP_HEIGHT + 4 * BUTTON_PAD + BUTTON_HEIGHT +
                                   2 * xFrame + SystemInformation.CaptionHeight);
            Size = size;

            selector.Bounds = new Rectangle(BUTTON_PAD, BUTTON_PAD,
                                            selectorWidth, bounds.Height + STRIP_HEIGHT + 2 * BUTTON_PAD + BUTTON_HEIGHT);

            bounds.X = bounds.Width + bounds.X - BUTTON_WIDTH;
            bounds.Y = bounds.Height + bounds.Y + BUTTON_PAD;
            bounds.Width = BUTTON_WIDTH;
            bounds.Height = BUTTON_HEIGHT;

            helpButton.Bounds = bounds;
            helpButton.Text = SR.HelpCaption;
            helpButton.Click += new EventHandler(OnButtonClick);
            helpButton.Enabled = false;
            helpButton.FlatStyle = FlatStyle.System;

            bounds.X -= (BUTTON_WIDTH + BUTTON_PAD);
            applyButton.Bounds = bounds;
            applyButton.Text = SR.ApplyCaption;
            applyButton.Click += new EventHandler(OnButtonClick);
            applyButton.Enabled = false;
            applyButton.FlatStyle = FlatStyle.System;

            bounds.X -= (BUTTON_WIDTH + BUTTON_PAD);
            cancelButton.Bounds = bounds;
            cancelButton.Text = SR.CancelCaption;
            cancelButton.Click += new EventHandler(OnButtonClick);
            cancelButton.FlatStyle = FlatStyle.System;
            CancelButton = cancelButton;

            bounds.X -= (BUTTON_WIDTH + BUTTON_PAD);
            okButton.Bounds = bounds;
            okButton.Text = SR.OKCaption;
            okButton.Click += new EventHandler(OnButtonClick);
            okButton.FlatStyle = FlatStyle.System;
            AcceptButton = okButton;

            Controls.Clear();
            Controls.AddRange(new Control[] {
                selector,
                grayStrip,
                pageHost,
                okButton,
                cancelButton,
                applyButton,
                helpButton
            });

            // continuing with the old autoscale base size stuff, it works,
            // and is currently set to a non-standard height
            AutoScaleBaseSize = new Size(5, 14);
            ApplyAutoScaling();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (firstActivate)
            {
                firstActivate = false;

                selector.SelectedNode = selector.Nodes[initialActivePage];
                pageSites[initialActivePage].Active = true;
                activePage = initialActivePage;

                helpButton.Enabled = pageSites[activePage].GetPageControl().SupportsHelp();
            }
        }

        //
        protected override void OnHelpRequested(HelpEventArgs e)
        {
            base.OnHelpRequested(e);
            ShowPageHelp();
        }

        /// <summary>
        ///  Called to initialize this form with the new component.
        /// </summary>
        private void OnNewObjects()
        {
            pageSites = null;
            maxSize = new Size(3 * (BUTTON_WIDTH + BUTTON_PAD), 24 * pageTypes.Length);

            pageSites = new ComponentEditorPageSite[pageTypes.Length];

            // create sites for them
            //
            for (int n = 0; n < pageTypes.Length; n++)
            {
                pageSites[n] = new ComponentEditorPageSite(pageHost, pageTypes[n], component, this);
                ComponentEditorPage page = pageSites[n].GetPageControl();

                Size pageSize = page.Size;
                if (pageSize.Width > maxSize.Width)
                {
                    maxSize.Width = pageSize.Width;
                }

                if (pageSize.Height > maxSize.Height)
                {
                    maxSize.Height = pageSize.Height;
                }
            }

            // and set them all to an ideal size
            //
            for (int n = 0; n < pageSites.Length; n++)
            {
                pageSites[n].GetPageControl().Size = maxSize;
            }
        }

        /// <summary>
        ///  Handles switching between pages.
        /// </summary>
        protected virtual void OnSelChangeSelector(object source, TreeViewEventArgs e)
        {
            if (firstActivate == true)
            {
                // treeview seems to fire a change event when it is first setup before
                // the form is activated
                return;
            }

            int newPage = selector.SelectedNode.Index;
            Debug.Assert((newPage >= 0) && (newPage < pageSites.Length),
                         "Invalid page selected");

            if (newPage == activePage)
            {
                return;
            }

            if (activePage != -1)
            {
                if (pageSites[activePage].AutoCommit)
                {
                    ApplyChanges(false);
                }

                pageSites[activePage].Active = false;
            }

            activePage = newPage;
            pageSites[activePage].Active = true;
            helpButton.Enabled = pageSites[activePage].GetPageControl().SupportsHelp();
        }

        /// <summary>
        ///  Provides a method to override in order to pre-process input messages before
        ///  they are dispatched.
        /// </summary>
        public override bool PreProcessMessage(ref Message msg)
        {
            if (null != pageSites && pageSites[activePage].GetPageControl().IsPageMessage(ref msg))
            {
                return true;
            }

            return base.PreProcessMessage(ref msg);
        }

        /// <summary>
        ///  Sets the controls of the form to dirty.  This enables the "apply"
        ///  button.
        /// </summary>
        internal virtual void SetDirty()
        {
            dirty = true;
            applyButton.Enabled = true;
            cancelButton.Text = SR.CancelCaption;
        }

        /// <summary>
        ///  Shows the form. The form will have no owner window.
        /// </summary>
        public virtual DialogResult ShowForm()
        {
            return ShowForm(null, 0);
        }

        /// <summary>
        ///  Shows the form and the specified page. The form will have no owner window.
        /// </summary>
        public virtual DialogResult ShowForm(int page)
        {
            return ShowForm(null, page);
        }

        /// <summary>
        ///  Shows the form with the specified owner.
        /// </summary>
        public virtual DialogResult ShowForm(IWin32Window owner)
        {
            return ShowForm(owner, 0);
        }

        /// <summary>
        ///  Shows the form and the specified page with the specified owner.
        /// </summary>
        public virtual DialogResult ShowForm(IWin32Window owner, int page)
        {
            initialActivePage = page;
            ShowDialog(owner);
            return DialogResult;
        }

        /// <summary>
        ///  Shows help for the active page.
        /// </summary>
        private void ShowPageHelp()
        {
            Debug.Assert(activePage != -1);

            if (pageSites[activePage].GetPageControl().SupportsHelp())
            {
                pageSites[activePage].GetPageControl().ShowHelp();
            }
        }

        /// <summary>
        ///  Implements a standard version of ComponentEditorPageSite for use within a
        ///  ComponentEditorForm.
        /// </summary>
        private sealed class ComponentEditorPageSite : IComponentEditorPageSite
        {
            internal IComponent component;
            internal ComponentEditorPage pageControl;
            internal Control parent;
            internal bool isActive;
            internal bool isDirty;
            private readonly ComponentEditorForm form;

            /// <summary>
            ///  Creates the page site.
            /// </summary>
            internal ComponentEditorPageSite(Control parent, Type pageClass, IComponent component, ComponentEditorForm form)
            {
                this.component = component;
                this.parent = parent;
                isActive = false;
                isDirty = false;

                this.form = form ?? throw new ArgumentNullException(nameof(form));

                try
                {
                    pageControl = (ComponentEditorPage)Activator.CreateInstance(pageClass);
                }
                catch (TargetInvocationException e)
                {
                    Debug.Fail(e.ToString());
                    throw new TargetInvocationException(string.Format(SR.ExceptionCreatingCompEditorControl, e.ToString()), e.InnerException);
                }

                pageControl.SetSite(this);
                pageControl.SetComponent(component);
            }

            /// <summary>
            ///  Called by the ComponentEditorForm to activate / deactivate the page.
            /// </summary>
            internal bool Active
            {
                set
                {
                    if (value)
                    {
                        // make sure the page has been created
                        pageControl.CreateControl();

                        // activate it and give it focus
                        pageControl.Activate();
                    }
                    else
                    {
                        pageControl.Deactivate();
                    }
                    isActive = value;
                }
            }

            internal bool AutoCommit
            {
                get
                {
                    return pageControl.CommitOnDeactivate;
                }
            }

            internal bool Dirty
            {
                get
                {
                    return isDirty;
                }
                set
                {
                    isDirty = value;
                }
            }

            /// <summary>
            ///  Called by a page to return a parenting control for itself.
            /// </summary>
            public Control GetControl()
            {
                return parent;
            }

            /// <summary>
            ///  Called by the ComponentEditorForm to get the actual page.
            /// </summary>
            internal ComponentEditorPage GetPageControl()
            {
                return pageControl;
            }

            /// <summary>
            ///  Called by a page to mark it's contents as dirty.
            /// </summary>
            public void SetDirty()
            {
                if (isActive)
                {
                    Dirty = true;
                }

                form.SetDirty();
            }
        }

        //  This should be moved into a shared location
        //  Its a duplication of what exists in the StyleBuilder.
        internal sealed class PageSelector : TreeView
        {
            private const int PADDING_VERT = 3;
            private const int PADDING_HORZ = 4;

            private const int SIZE_ICON_X = 16;
            private const int SIZE_ICON_Y = 16;

            private const int STATE_NORMAL = 0;
            private const int STATE_SELECTED = 1;
            private const int STATE_HOT = 2;

            private Gdi32.HBRUSH _hbrushDither;

            public PageSelector()
            {
                HotTracking = true;
                HideSelection = false;
                BackColor = SystemColors.Control;
                Indent = 0;
                LabelEdit = false;
                Scrollable = false;
                ShowLines = false;
                ShowPlusMinus = false;
                ShowRootLines = false;
                BorderStyle = BorderStyle.None;
                Indent = 0;
                FullRowSelect = true;
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;

                    cp.ExStyle |= (int)User32.WS_EX.STATICEDGE;
                    return cp;
                }
            }

            private unsafe void CreateDitherBrush()
            {
                Debug.Assert(_hbrushDither.IsNull, "Brush should not be recreated.");

                short* patternBits = stackalloc short[]
                {
                    unchecked((short)0xAAAA), unchecked((short)0x5555), unchecked((short)0xAAAA), unchecked((short)0x5555),
                    unchecked((short)0xAAAA), unchecked((short)0x5555), unchecked((short)0xAAAA), unchecked((short)0x5555)
                };

                Gdi32.HBITMAP hbitmapTemp = Gdi32.CreateBitmap(8, 8, 1, 1, patternBits);
                Debug.Assert(
                    !hbitmapTemp.IsNull,
                    "could not create dither bitmap. Page selector UI will not be correct");

                if (!hbitmapTemp.IsNull)
                {
                    _hbrushDither = Gdi32.CreatePatternBrush(hbitmapTemp);

                    Debug.Assert(
                        !_hbrushDither.IsNull,
                        "Unable to created dithered brush. Page selector UI will not be correct");

                    Gdi32.DeleteObject(hbitmapTemp);
                }
            }

            private unsafe void DrawTreeItem(
                string itemText,
                int imageIndex,
                Gdi32.HDC dc,
                RECT rcIn,
                int state,
                int backColor,
                int textColor)
            {
                Size size = new Size();
                var rc2 = new RECT();
                var rc = new RECT(rcIn.left, rcIn.top, rcIn.right, rcIn.bottom);
                ImageList imagelist = ImageList;

                IntPtr hfontOld = IntPtr.Zero;

                // Select the font of the dialog, so we don't get the underlined font
                // when the item is being tracked
                using var fontSelection = new Gdi32.SelectObjectScope(
                    dc,
                    (state & STATE_HOT) != 0 ? (Gdi32.HGDIOBJ)Parent.FontHandle : default);

                GC.KeepAlive(Parent);

                // Fill the background
                if (((state & STATE_SELECTED) != 0) && !_hbrushDither.IsNull)
                {
                    FillRectDither(dc, rcIn);
                    Gdi32.SetBkMode(dc, Gdi32.BKMODE.TRANSPARENT);
                }
                else
                {
                    Gdi32.SetBkColor(dc, backColor);
                    Gdi32.ExtTextOutW(dc, 0, 0, Gdi32.ETO.CLIPPED | Gdi32.ETO.OPAQUE, ref rc, null, 0, null);
                }

                // Get the height of the font
                Gdi32.GetTextExtentPoint32W(dc, itemText, itemText.Length, ref size);

                // Draw the caption
                rc2.left = rc.left + SIZE_ICON_X + 2 * PADDING_HORZ;
                rc2.top = rc.top + (((rc.bottom - rc.top) - size.Height) >> 1);
                rc2.bottom = rc2.top + size.Height;
                rc2.right = rc.right;
                Gdi32.SetTextColor(dc, textColor);
                User32.DrawTextW(
                    dc,
                    itemText,
                    itemText.Length,
                    ref rc2,
                    User32.DT.LEFT | User32.DT.VCENTER | User32.DT.END_ELLIPSIS | User32.DT.NOPREFIX);

                ComCtl32.ImageList.Draw(
                    imagelist,
                    imageIndex,
                    dc,
                    PADDING_HORZ,
                    rc.top + (((rc.bottom - rc.top) - SIZE_ICON_Y) >> 1),
                    ComCtl32.ILD.TRANSPARENT);

                // Draw the hot-tracking border if needed
                if ((state & STATE_HOT) != 0)
                {
                    int savedColor;

                    // top left
                    savedColor = Gdi32.SetBkColor(dc, ColorTranslator.ToWin32(SystemColors.ControlLightLight));
                    rc2.left = rc.left;
                    rc2.top = rc.top;
                    rc2.bottom = rc.top + 1;
                    rc2.right = rc.right;
                    Gdi32.ExtTextOutW(dc, 0, 0, Gdi32.ETO.OPAQUE, ref rc2, null, 0, null);
                    rc2.bottom = rc.bottom;
                    rc2.right = rc.left + 1;
                    Gdi32.ExtTextOutW(dc, 0, 0, Gdi32.ETO.OPAQUE, ref rc2, null, 0, null);

                    // bottom right
                    Gdi32.SetBkColor(dc, ColorTranslator.ToWin32(SystemColors.ControlDark));
                    rc2.left = rc.left;
                    rc2.right = rc.right;
                    rc2.top = rc.bottom - 1;
                    rc2.bottom = rc.bottom;
                    Gdi32.ExtTextOutW(dc, 0, 0, Gdi32.ETO.OPAQUE, ref rc2, null, 0, null);
                    rc2.left = rc.right - 1;
                    rc2.top = rc.top;
                    Gdi32.ExtTextOutW(dc, 0, 0, Gdi32.ETO.OPAQUE, ref rc2, null, 0, null);

                    Gdi32.SetBkColor(dc, savedColor);
                }
            }

            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);

                int itemHeight = (int)User32.SendMessageW(this, (User32.WM)ComCtl32.TVM.GETITEMHEIGHT);
                itemHeight += 2 * PADDING_VERT;
                User32.SendMessageW(this, (User32.WM)ComCtl32.TVM.SETITEMHEIGHT, (IntPtr)itemHeight);

                if (_hbrushDither.IsNull)
                {
                    CreateDitherBrush();
                }
            }

            private unsafe void OnCustomDraw(ref Message m)
            {
                ComCtl32.NMTVCUSTOMDRAW* nmtvcd = (ComCtl32.NMTVCUSTOMDRAW*)m.LParam;
                switch (nmtvcd->nmcd.dwDrawStage)
                {
                    case ComCtl32.CDDS.PREPAINT:
                        m.Result = (IntPtr)(ComCtl32.CDRF.NOTIFYITEMDRAW | ComCtl32.CDRF.NOTIFYPOSTPAINT);
                        break;
                    case ComCtl32.CDDS.ITEMPREPAINT:
                        {
                            TreeNode itemNode = TreeNode.FromHandle(this, nmtvcd->nmcd.dwItemSpec);
                            if (itemNode != null)
                            {
                                int state = STATE_NORMAL;
                                ComCtl32.CDIS itemState = nmtvcd->nmcd.uItemState;
                                if (((itemState & ComCtl32.CDIS.HOT) != 0) || ((itemState & ComCtl32.CDIS.FOCUS) != 0))
                                {
                                    state |= STATE_HOT;
                                }

                                if ((itemState & ComCtl32.CDIS.SELECTED) != 0)
                                {
                                    state |= STATE_SELECTED;
                                }

                                DrawTreeItem(
                                    itemNode.Text,
                                    itemNode.ImageIndex,
                                    nmtvcd->nmcd.hdc,
                                    nmtvcd->nmcd.rc,
                                    state,
                                    ColorTranslator.ToWin32(SystemColors.Control),
                                    ColorTranslator.ToWin32(SystemColors.ControlText));
                            }
                            m.Result = (IntPtr)ComCtl32.CDRF.SKIPDEFAULT;
                        }
                        break;
                    case ComCtl32.CDDS.POSTPAINT:
                        m.Result = (IntPtr)ComCtl32.CDRF.SKIPDEFAULT;
                        break;
                    default:
                        m.Result = (IntPtr)ComCtl32.CDRF.DODEFAULT;
                        break;
                }
            }

            protected override void OnHandleDestroyed(EventArgs e)
            {
                base.OnHandleDestroyed(e);

                if (!RecreatingHandle && !_hbrushDither.IsNull)
                {
                    Gdi32.DeleteObject(_hbrushDither);
                    _hbrushDither = default;
                }
            }

            private void FillRectDither(Gdi32.HDC dc, RECT rc)
            {
                Gdi32.HGDIOBJ hbrushOld = Gdi32.SelectObject(dc, _hbrushDither);

                if (!hbrushOld.IsNull)
                {
                    int oldTextColor = Gdi32.SetTextColor(dc, ColorTranslator.ToWin32(SystemColors.ControlLightLight));
                    int oldBackColor = Gdi32.SetBkColor(dc, ColorTranslator.ToWin32(SystemColors.Control));

                    Gdi32.PatBlt(dc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, Gdi32.ROP.PATCOPY);
                    Gdi32.SetTextColor(dc, oldTextColor);
                    Gdi32.SetBkColor(dc, oldBackColor);
                }
            }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == (int)(User32.WM.REFLECT_NOTIFY))
                {
                    User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;
                    if (nmhdr->code == (int)ComCtl32.NM.CUSTOMDRAW)
                    {
                        OnCustomDraw(ref m);
                        return;
                    }
                }

                base.WndProc(ref m);
            }
        }
    }
}
