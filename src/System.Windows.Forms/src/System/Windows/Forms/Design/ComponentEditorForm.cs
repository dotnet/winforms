// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides a user interface for <see cref="WindowsFormsComponentEditor"/>.
    /// </summary>
    [ToolboxItem(false)]
    public partial class ComponentEditorForm : Form
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
        ///  Initializes a new instance of the <see cref="ComponentEditorForm"/> class.
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
            if (!dirty)
            {
                return;
            }

            if (component.Site.TryGetService(out IComponentChangeService changeService))
            {
                try
                {
                    changeService.OnComponentChanging(component, null);
                }
                catch (CheckoutException e) when (e == CheckoutException.Canceled)
                {
                    return;
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

            changeService?.OnComponentChanged(component);

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
            if (component.Site is not null)
            {
                IUIService uiService = (IUIService)component.Site.GetService(typeof(IUIService));
                if (uiService is not null)
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

            if (pageSites is not null)
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
            if (site is not null)
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

            if (pageSites is not null)
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
            Controls.AddRange(new Control[]
            {
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
    }
}
