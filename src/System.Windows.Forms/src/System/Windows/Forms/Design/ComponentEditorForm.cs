// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides a user interface for <see cref="WindowsFormsComponentEditor"/>.
/// </summary>
[ToolboxItem(false)]
public partial class ComponentEditorForm : Form
{
    private readonly IComponent _component;
    private readonly Type[] _pageTypes;
    private ComponentEditorPageSite[] _pageSites;
    private Size _maxSize = Size.Empty;
    private int _initialActivePage;
    private int _activePage;
    private bool _dirty;
    private bool _firstActivate;

    private readonly Panel _pageHost = new();
    private PageSelector _selector;
    private ImageList _selectorImageList;
    private Button _okButton;
    private Button _cancelButton;
    private Button _applyButton;
    private Button _helpButton;

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
        if (component is not IComponent)
        {
            throw new ArgumentException(SR.ComponentEditorFormBadComponent, nameof(component));
        }

        _component = (IComponent)component;
        _pageTypes = pageTypes;
        _dirty = false;
        _firstActivate = true;
        _activePage = -1;
        _initialActivePage = 0;

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
        if (!_dirty)
        {
            return;
        }

        if (_component.Site.TryGetService(out IComponentChangeService? changeService))
        {
            try
            {
                changeService.OnComponentChanging(_component, null);
            }
            catch (CheckoutException e) when (e == CheckoutException.Canceled)
            {
                return;
            }
        }

        for (int n = 0; n < _pageSites.Length; n++)
        {
            if (_pageSites[n].Dirty)
            {
                _pageSites[n].GetPageControl().ApplyChanges();
                _pageSites[n].Dirty = false;
            }
        }

        changeService?.OnComponentChanged(_component);

        _applyButton.Enabled = false;
        _cancelButton.Text = SR.CloseCaption;
        _dirty = false;

        if (!lastApply)
        {
            for (int n = 0; n < _pageSites.Length; n++)
            {
                _pageSites[n].GetPageControl().OnApplyComplete();
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
    public new event EventHandler? AutoSizeChanged
    {
        add => base.AutoSizeChanged += value;
        remove => base.AutoSizeChanged -= value;
    }

    /// <summary>
    ///  Handles ok/cancel/apply/help button click events
    /// </summary>
    private void OnButtonClick(object? sender, EventArgs e)
    {
        if (sender == _okButton)
        {
            ApplyChanges(true);
            DialogResult = DialogResult.OK;
        }
        else if (sender == _cancelButton)
        {
            DialogResult = DialogResult.Cancel;
        }
        else if (sender == _applyButton)
        {
            ApplyChanges(false);
        }
        else if (sender == _helpButton)
        {
            ShowPageHelp();
        }
    }

    /// <summary>
    ///  Lays out the UI of the form.
    /// </summary>
    [MemberNotNull(nameof(_okButton))]
    [MemberNotNull(nameof(_cancelButton))]
    [MemberNotNull(nameof(_applyButton))]
    [MemberNotNull(nameof(_helpButton))]
    [MemberNotNull(nameof(_selectorImageList))]
    [MemberNotNull(nameof(_selector))]
    private void OnConfigureUI()
    {
        Font? uiFont = DefaultFont;
        if (_component.Site?.TryGetService(out IUIService? uiService) == true)
        {
            uiFont = (Font?)uiService.Styles["DialogFont"];
        }

        Font = uiFont;

        _okButton = new Button();
        _cancelButton = new Button();
        _applyButton = new Button();
        _helpButton = new Button();

        _selectorImageList = new ImageList
        {
            ImageSize = new Size(16, 16)
        };
        _selector = new PageSelector
        {
            ImageList = _selectorImageList
        };
        _selector.AfterSelect += new TreeViewEventHandler(OnSelChangeSelector);

        Label grayStrip = new Label
        {
            BackColor = Application.ApplicationColors.ControlDark
        };

        int selectorWidth = MIN_SELECTOR_WIDTH;

        if (_pageSites is not null && _pageSites.Length != 0)
        {
            using Graphics graphics = CreateGraphicsInternal();

            // Add the nodes corresponding to the pages
            for (int n = 0; n < _pageSites.Length; n++)
            {
                ComponentEditorPage page = _pageSites[n].GetPageControl();

                string title = page.Title;
                int titleWidth = (int)graphics.MeasureString(title, Font).Width;
                _selectorImageList.Images.Add(page.Icon.ToBitmap());

                _selector.Nodes.Add(new TreeNode(title, n, n));
                if (titleWidth > selectorWidth)
                {
                    selectorWidth = titleWidth;
                }
            }
        }

        selectorWidth += SELECTOR_PADDING;

        string caption = string.Empty;
        ISite? site = _component.Site;
        if (site is not null)
        {
            caption = string.Format(SR.ComponentEditorFormProperties, site.Name);
        }
        else
        {
            caption = SR.ComponentEditorFormPropertiesNoName;
        }

        Text = caption;

        Rectangle pageHostBounds = new(2 * BUTTON_PAD + selectorWidth, 2 * BUTTON_PAD + STRIP_HEIGHT,
                                                 _maxSize.Width, _maxSize.Height);
        _pageHost.Bounds = pageHostBounds;
        grayStrip.Bounds = new Rectangle(pageHostBounds.X, BUTTON_PAD,
                                         pageHostBounds.Width, STRIP_HEIGHT);

        if (_pageSites is not null)
        {
            Rectangle pageBounds = new(0, 0, pageHostBounds.Width, pageHostBounds.Height);
            for (int n = 0; n < _pageSites.Length; n++)
            {
                ComponentEditorPage page = _pageSites[n].GetPageControl();
                page.GetControl().Bounds = pageBounds;
            }
        }

        int xFrame = SystemInformation.FixedFrameBorderSize.Width;
        Rectangle bounds = pageHostBounds;
        Size size = new(bounds.Width + 3 * (BUTTON_PAD + xFrame) + selectorWidth,
                               bounds.Height + STRIP_HEIGHT + 4 * BUTTON_PAD + BUTTON_HEIGHT +
                               2 * xFrame + SystemInformation.CaptionHeight);
        Size = size;

        _selector.Bounds = new Rectangle(BUTTON_PAD, BUTTON_PAD,
                                        selectorWidth, bounds.Height + STRIP_HEIGHT + 2 * BUTTON_PAD + BUTTON_HEIGHT);

        bounds.X = bounds.Width + bounds.X - BUTTON_WIDTH;
        bounds.Y = bounds.Height + bounds.Y + BUTTON_PAD;
        bounds.Width = BUTTON_WIDTH;
        bounds.Height = BUTTON_HEIGHT;

        _helpButton.Bounds = bounds;
        _helpButton.Text = SR.HelpCaption;
        _helpButton.Click += new EventHandler(OnButtonClick);
        _helpButton.Enabled = false;
        _helpButton.FlatStyle = FlatStyle.System;

        bounds.X -= (BUTTON_WIDTH + BUTTON_PAD);
        _applyButton.Bounds = bounds;
        _applyButton.Text = SR.ApplyCaption;
        _applyButton.Click += new EventHandler(OnButtonClick);
        _applyButton.Enabled = false;
        _applyButton.FlatStyle = FlatStyle.System;

        bounds.X -= (BUTTON_WIDTH + BUTTON_PAD);
        _cancelButton.Bounds = bounds;
        _cancelButton.Text = SR.CancelCaption;
        _cancelButton.Click += new EventHandler(OnButtonClick);
        _cancelButton.FlatStyle = FlatStyle.System;
        CancelButton = _cancelButton;

        bounds.X -= (BUTTON_WIDTH + BUTTON_PAD);
        _okButton.Bounds = bounds;
        _okButton.Text = SR.OKCaption;
        _okButton.Click += new EventHandler(OnButtonClick);
        _okButton.FlatStyle = FlatStyle.System;
        AcceptButton = _okButton;

        Controls.Clear();
        Controls.AddRange(
        [
            _selector,
            grayStrip,
            _pageHost,
            _okButton,
            _cancelButton,
            _applyButton,
            _helpButton
        ]);

        // Continuing with the old autoscale base size stuff, it works, and is currently set to a non-standard height.
        AutoScaleBaseSize = new Size(5, 14);
#pragma warning disable CS0618 // Type or member is obsolete
        ApplyAutoScaling();
#pragma warning restore CS0618
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        if (_firstActivate)
        {
            _firstActivate = false;

            _selector.SelectedNode = _selector.Nodes[_initialActivePage];
            _pageSites[_initialActivePage].Active = true;
            _activePage = _initialActivePage;

            _helpButton.Enabled = _pageSites[_activePage].GetPageControl().SupportsHelp();
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
    [MemberNotNull(nameof(_pageSites))]
    private void OnNewObjects()
    {
        _maxSize = new Size(3 * (BUTTON_WIDTH + BUTTON_PAD), 24 * _pageTypes.Length);

        _pageSites = new ComponentEditorPageSite[_pageTypes.Length];

        // create sites for them
        for (int n = 0; n < _pageTypes.Length; n++)
        {
            _pageSites[n] = new ComponentEditorPageSite(_pageHost, _pageTypes[n], _component, this);
            ComponentEditorPage page = _pageSites[n].GetPageControl();

            Size pageSize = page.Size;
            if (pageSize.Width > _maxSize.Width)
            {
                _maxSize.Width = pageSize.Width;
            }

            if (pageSize.Height > _maxSize.Height)
            {
                _maxSize.Height = pageSize.Height;
            }
        }

        // and set them all to an ideal size
        for (int n = 0; n < _pageSites.Length; n++)
        {
            _pageSites[n].GetPageControl().Size = _maxSize;
        }
    }

    /// <summary>
    ///  Handles switching between pages.
    /// </summary>
    protected virtual void OnSelChangeSelector(object? source, TreeViewEventArgs e)
    {
        if (_firstActivate)
        {
            // treeview seems to fire a change event when it is first setup before
            // the form is activated
            return;
        }

        int newPage = _selector.SelectedNode!.Index;
        Debug.Assert((newPage >= 0) && (newPage < _pageSites.Length),
                     "Invalid page selected");

        if (newPage == _activePage)
        {
            return;
        }

        if (_activePage != -1)
        {
            if (_pageSites[_activePage].AutoCommit)
            {
                ApplyChanges(false);
            }

            _pageSites[_activePage].Active = false;
        }

        _activePage = newPage;
        _pageSites[_activePage].Active = true;
        _helpButton.Enabled = _pageSites[_activePage].GetPageControl().SupportsHelp();
    }

    /// <summary>
    ///  Provides a method to override in order to pre-process input messages before
    ///  they are dispatched.
    /// </summary>
    public override bool PreProcessMessage(ref Message msg)
    {
        if (_pageSites is not null && _pageSites[_activePage].GetPageControl().IsPageMessage(ref msg))
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
        _dirty = true;
        _applyButton.Enabled = true;
        _cancelButton.Text = SR.CancelCaption;
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
    public virtual DialogResult ShowForm(IWin32Window? owner)
    {
        return ShowForm(owner, 0);
    }

    /// <summary>
    ///  Shows the form and the specified page with the specified owner.
    /// </summary>
    public virtual DialogResult ShowForm(IWin32Window? owner, int page)
    {
        _initialActivePage = page;
        ShowDialog(owner);
        return DialogResult;
    }

    /// <summary>
    ///  Shows help for the active page.
    /// </summary>
    private void ShowPageHelp()
    {
        Debug.Assert(_activePage != -1);

        if (_pageSites[_activePage].GetPageControl().SupportsHelp())
        {
            _pageSites[_activePage].GetPageControl().ShowHelp();
        }
    }
}
