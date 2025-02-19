// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.Versioning;
using System.Windows.Forms.VisualStyles;
using Windows.Win32.UI.Accessibility;
using System.Runtime.InteropServices;
using System.Collections;

namespace System.Windows.Forms.Design;

internal class BindingFormattingWindowsFormsEditorService : Panel, IWindowsFormsEditorService, IServiceProvider, ITypeDescriptorContext
{
    private ITypeDescriptorContext _context = null;

    private DropDownHolder _dropDownHolder;
    private DropDownButton _button;
    private EventHandler _propertyValueChanged;

    private Binding _binding;
    private IComponent _ownerComponent;
    private DataSourceUpdateMode _defaultDataSourceUpdateMode = DataSourceUpdateMode.OnValidation;
    private DesignBindingPicker _designBindingPicker;
    private string _propertyName = string.Empty;

    private bool _expanded = false;

    public BindingFormattingWindowsFormsEditorService()
    {
        BackColor = SystemColors.Window;
        Text = SR.DataGridNoneString;
        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.Selectable, true);
        SetStyle(ControlStyles.UseTextForAccessibility, true);
        AccessibleRole = AccessibleRole.ComboBox;
        TabStop = true;
        Click += BindingFormattingWindowsFormsEditorService_Click;
        AccessibleName = SR.BindingFormattingDialogBindingPickerAccName;

        _button = new DropDownButton(this);
        _button.FlatStyle = FlatStyle.Popup;
        _button.Image = CreateDownArrow();
        _button.Padding = new System.Windows.Forms.Padding(0);
        _button.BackColor = SystemColors.Control;
        _button.ForeColor = SystemColors.ControlText;
        _button.Click += new EventHandler(button_Click);
        _button.Size = new Drawing.Size(SystemInformation.VerticalScrollBarArrowHeight, (int)Font.Height + 2);
        _button.AccessibleName = SR.BindingFormattingDialogDataSourcePickerDropDownAccName;
        // button.Dock = DockStyle.Right;

        Controls.Add(_button);
    }

    private void BindingFormattingWindowsFormsEditorService_Click(object sender, EventArgs e)
    {
        if (!_expanded)
        {
            ExpandDropDown();
        }
    }

    [
        SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")
    ]
    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    private Bitmap CreateDownArrow()
    {
        Bitmap bitmap = null;

        try
        {
            Icon icon = new Icon(typeof(BindingFormattingDialog), "BindingFormattingDialog.Arrow.ico");
            bitmap = icon.ToBitmap();
            icon.Dispose();
        }
        catch
        {
            Debug.Fail("non-CLS compliant exception");
            bitmap = new Bitmap(16, 16);
        }

        return bitmap;
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        base.SetBoundsCore(x, y, width, PreferredHeight, specified);

        int buttonHeight = Height - 2;
        int buttonWidth = SystemInformation.HorizontalScrollBarThumbWidth;

        int buttonLeft = Width - buttonWidth - 2;
        int buttonTop = 1;
        if (RightToLeft == RightToLeft.No)
        {
            _button.Bounds = new Rectangle(buttonTop, buttonLeft, buttonWidth, buttonHeight);
        }
        else
        {
            _button.Bounds = new Rectangle(buttonTop, 2, buttonWidth, buttonHeight);
        }
    }

    private int PreferredHeight
    {
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
        get
        {
            Size textSize = TextRenderer.MeasureText("j^", Font, new Size(short.MaxValue, (int)(FontHeight * 1.25)));
            return textSize.Height + SystemInformation.BorderSize.Height * 8 + Padding.Size.Height;
        }
    }

    /*
    public override Size GetPreferredSize(Size proposedSize) {
        Size newSize = proposedSize;
        Size textSize = TextRenderer.MeasureText("j^", Font, new Size(Int16.MaxValue, (int)(FontHeight * 1.25)));
        newSize.Height = (short)(textSize.Height + SystemInformation.BorderSize.Height*8 + Padding.Size.Height);

        return base.GetPreferredSize(newSize);
    }
    */

    public ITypeDescriptorContext Context
    {
        set
        {
            _context = value;
        }
    }

    // ITypeDescriptorContext
    IContainer ITypeDescriptorContext.Container
    {
        get
        {
            if (_ownerComponent is null)
            {
                return null;
            }

            ISite site = _ownerComponent.Site;
            if (site is null)
            {
                return null;
            }

            return site.Container;
        }
    }

    object ITypeDescriptorContext.Instance
    {
        get
        {
            return _ownerComponent;
        }
    }

    PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
    {
        get
        {
            return null;
        }
    }

    void ITypeDescriptorContext.OnComponentChanged()
    {
        if (_context is not null)
        {
            _context.OnComponentChanged();
        }
    }

    bool ITypeDescriptorContext.OnComponentChanging()
    {
        if (_context is not null)
        {
            return _context.OnComponentChanging();
        }
        else
        {
            // can change the component
            return true;
        }
    }


    // IServiceProvider
    object IServiceProvider.GetService(Type type)
    {
        if (type == typeof(IWindowsFormsEditorService))
        {
            return this;
        }
        else if (_context is not null)
        {
            return _context.GetService(type);
        }
        else
        {
            return null;
        }
    }

    // IWindowsFormsEditorService
    void IWindowsFormsEditorService.CloseDropDown()
    {
        _dropDownHolder.SetComponent(null);
        _dropDownHolder.Visible = false;
    }

    void IWindowsFormsEditorService.DropDownControl(Control ctl)
    {
        if (_dropDownHolder is null)
        {
            _dropDownHolder = new DropDownHolder(this);
        }

        _dropDownHolder.SetComponent(ctl);

        _dropDownHolder.Location = PointToScreen(new Drawing.Point(0, Height));

        try
        {
            _dropDownHolder.Visible = true;

            UnsafeNativeMethods.SetWindowLong(new HandleRef(_dropDownHolder, _dropDownHolder.Handle),
                                              NativeMethods.GWL_HWNDPARENT,
                                              new HandleRef(this, Handle));

            _dropDownHolder.FocusComponent();
            _dropDownHolder.DoModalLoop();
        }
        finally
        {
            UnsafeNativeMethods.SetWindowLong(new HandleRef(_dropDownHolder, _dropDownHolder.Handle),
                                              NativeMethods.GWL_HWNDPARENT,
                                              new HandleRef(null, IntPtr.Zero));
            Focus();
        }
    }

    DialogResult IWindowsFormsEditorService.ShowDialog(Form form)
    {
        return form.ShowDialog();
    }


    // implementation details
    public Binding Binding
    {
        get
        {
            return _binding;
        }
        set
        {
            if (_binding == value)
            {
                return;
            }

            _binding = value;
            // update the text
            if (_binding is not null)
            {
                Text = ConstructDisplayTextFromBinding(_binding);
            }
            else
            {
                Text = SR.DataGridNoneString;
            }

            Invalidate();
        }
    }

    public DataSourceUpdateMode DefaultDataSourceUpdateMode
    {
        /* Omit this until someone needs to call it (avoids FxCop warning)...
        get
        {
            return defaultDataSourceUpdateMode;
        }
        */
        set
        {
            _defaultDataSourceUpdateMode = value;
        }
    }

    public IComponent OwnerComponent
    {
        set
        {
            _ownerComponent = value;
        }
    }

    public string PropertyName
    {
        set
        {
            _propertyName = value;
        }
    }

    private DropDownButton Button { get => _button; set => _button = value; }

    public event EventHandler PropertyValueChanged
    {
        add
        {
            _propertyValueChanged += value;
        }
        remove
        {
            _propertyValueChanged -= value;
        }
    }

    private void button_Click(object sender, EventArgs e)
    {
        Debug.Assert(!String.IsNullOrEmpty(_propertyName), "this dialog should be enabled only when the user picked a property from the properties tree view");
        DropDownPicker();
    }

    private static string ConstructDisplayTextFromBinding(Binding binding)
    {
        string result;
        if (binding.DataSource is null)
        {
            result = SR.DataGridNoneString;
        }
        else
        {
            if (binding.DataSource is IComponent)
            {
                IComponent comp = binding.DataSource as IComponent;
                if (comp.Site is not null)
                {
                    result = comp.Site.Name;
                }
                else
                {
                    result = "";
                }
            }
            else if (binding.DataSource is IListSource || binding.DataSource is IList || binding.DataSource is Array)
            {
                result = SR.BindingFormattingDialogList;
            }
            else
            {
                string? typeName = TypeDescriptor.GetClassName(binding.DataSource);
                int lastDot = typeName.LastIndexOf('.');
                if (lastDot != -1)
                {
                    typeName = typeName[(lastDot + 1)..];
                }

                result = string.Format(CultureInfo.CurrentCulture, "({0})", typeName);
            }
        }

        result += " - " + binding.BindingMemberInfo.BindingMember;

        return result;
    }

    private void DropDownPicker()
    {
        // drop the design binding picker
        if (_designBindingPicker is null)
        {
            _designBindingPicker = new DesignBindingPicker();
            _designBindingPicker.Width = Width;
        }

        DesignBinding initialDesignBinding = null;

        if (_binding is not null)
        {
            initialDesignBinding = new DesignBinding(_binding.DataSource, _binding.BindingMemberInfo.BindingMember);
        }

        _expanded = true;

        DesignBinding designBinding = _designBindingPicker.Pick(this, /*ITypeDescriptorService*/
                                                                    this, /*IServiceProvider*/
                                                                    true, /*showDataSources*/
                                                                    true, /*showDataMembers*/
                                                                    false, /*selectListMembers*/
                                                                    null,
                                                                    String.Empty,
                                                                    initialDesignBinding);

        _expanded = false;

        // the user did not make any change
        if (designBinding is null)
        {
            return;
        }

        // construct the new binding from the designBindingPicker and compare to the oldBinding
        //
        Binding oldBinding = _binding;
        Binding newBinding = null;

        string formatString = oldBinding is not null ? oldBinding.FormatString : String.Empty;
        IFormatProvider formatInfo = oldBinding is not null ? oldBinding.FormatInfo : null;
        object nullValue = oldBinding is not null ? oldBinding.NullValue : null;
        DataSourceUpdateMode updateMode = oldBinding is not null ? oldBinding.DataSourceUpdateMode : _defaultDataSourceUpdateMode;

        if (designBinding.DataSource is not null && !String.IsNullOrEmpty(designBinding.DataMember))
        {
            newBinding = new Binding(_propertyName,
                                     designBinding.DataSource,
                                     designBinding.DataMember,
                                     true, /*formattingEnabled*/
                                     updateMode,
                                     nullValue,
                                     formatString,
                                     formatInfo);
        }

        // this is the new binding
        Binding = newBinding;

        bool bindingChanged = (newBinding is null || oldBinding is not null);
        bindingChanged = bindingChanged || (newBinding is not null && oldBinding is null);
        bindingChanged = bindingChanged || (newBinding is not null && oldBinding is not null &&
                         (newBinding.DataSource != oldBinding.DataSource || !newBinding.BindingMemberInfo.Equals(oldBinding.BindingMemberInfo)));

        if (bindingChanged)
        {
            OnPropertyValueChanged(EventArgs.Empty);
        }
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs p)
    {
        base.OnPaint(p);

        string text = Text;
        if (ComboBoxRenderer.IsSupported)
        {
            Rectangle rect = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            ComboBoxState state;
            SolidBrush foreBrush;
            SolidBrush backBrush;

            bool enabled = Enabled;

            if (!enabled)
            {
                foreBrush = (SolidBrush)SystemBrushes.ControlDark;
                backBrush = (SolidBrush)SystemBrushes.Control;
                state = ComboBoxState.Disabled;
            }
            else if (ContainsFocus)
            {
                foreBrush = (SolidBrush)SystemBrushes.HighlightText;
                backBrush = (SolidBrush)SystemBrushes.Highlight;
                state = ComboBoxState.Hot;
            }
            else
            {
                foreBrush = (SolidBrush)SystemBrushes.WindowText;
                backBrush = (SolidBrush)SystemBrushes.Window;
                state = ComboBoxState.Normal;
            }

            ComboBoxRenderer.DrawTextBox(p.Graphics, rect, String.Empty, Font, state);

            Graphics g = p.Graphics;

            rect.Inflate(-2, -2);

            ControlPaint.DrawBorder(g, rect, backBrush.Color, ButtonBorderStyle.None);

            rect.Inflate(-1, -1);

            if (RightToLeft == RightToLeft.Yes)
            {
                rect.X += _button.Width;
            }

            rect.Width -= _button.Width;

            g.FillRectangle(backBrush, rect);

            TextFormatFlags flags = TextFormatFlags.VerticalCenter;
            if (RightToLeft == RightToLeft.No)
            {
                flags |= TextFormatFlags.Left;
            }
            else
            {
                flags |= TextFormatFlags.Right;
            }

            if (ContainsFocus)
            {
                ControlPaint.DrawFocusRectangle(g, rect, Color.Empty, backBrush.Color);
            }

            TextRenderer.DrawText(g, text, Font, rect, foreBrush.Color, flags);

        }
        else
        {
            if (!String.IsNullOrEmpty(text))
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Near;

                Rectangle clientRect = ClientRectangle;
                Rectangle textRect = new Rectangle(clientRect.X, clientRect.Y, clientRect.Width, clientRect.Height);

                if (RightToLeft == RightToLeft.Yes)
                {
                    textRect.X += _button.Width;
                }

                textRect.Width -= _button.Width;

                TextFormatFlags flags = TextFormatFlags.VerticalCenter;
                if (RightToLeft == RightToLeft.No)
                {
                    flags |= TextFormatFlags.Left;
                }
                else
                {
                    flags |= TextFormatFlags.Right;
                }

                TextRenderer.DrawText(p.Graphics, text, Font, textRect, ForeColor, flags);

                stringFormat.Dispose();
            }
        }

    }

    protected void OnPropertyValueChanged(EventArgs e)
    {
        if (_propertyValueChanged is not null)
        {
            _propertyValueChanged(this, e);
        }
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        Keys modifiers = Control.ModifierKeys;
        if (((modifiers & Keys.Alt) == Keys.Alt && (keyData & Keys.KeyCode) == Keys.Down) || keyData == Keys.F4)
        {
            DropDownPicker();
            return true;
        }

        return base.ProcessDialogKey(keyData);
    }

    private void ExpandDropDown()
    {
        BeginInvoke(new Action(DropDownPicker));
    }

    private void CollapseDropDown()
    {
        Focus();
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case NativeMethods.WM_GETOBJECT:
                m.Result = AutomationInteropProvider.ReturnRawElementProvider(Handle, m.WParam, m.LParam, (IRawElementProviderSimple)(new BindingFormattingWindowsFormsEditorServiceUiaProvider(this)));
                return;
        }

        base.WndProc(ref m);
    }

    private class BindingFormattingWindowsFormsEditorServiceUiaProvider : ControlUiaProvider, IExpandCollapseProvider
    {
        public BindingFormattingWindowsFormsEditorServiceUiaProvider(BindingFormattingWindowsFormsEditorService owner) : base(owner)
        {
        }

        private BindingFormattingWindowsFormsEditorService Service
        {
            get
            {
                return _owner as BindingFormattingWindowsFormsEditorService;
            }
        }

        public override object GetPatternProvider(int patternId)
        {
            if (patternId == ExpandCollapsePatternIdentifiers.Pattern.Id)
            {
                return this as IExpandCollapseProvider;
            }

            return base.GetPatternProvider(patternId);
        }

        #region IValueProvider

        public override void SetValue(string newValue)
        {
            // not supported
        }

        public override string Value
        {
            get
            {
                return Service.Text;
            }
        }

        #endregion

        #region IExpandCollapseProvider

        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return Service._expanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            }
        }

        public void Collapse()
        {
            if (Service._expanded)
            {
                Service.CollapseDropDown();
            }
        }

        public void Expand()
        {
            if (!Service._expanded)
            {
                Service.ExpandDropDown();
            }
        }

        #endregion
    }

    private class DropDownButton : Button
    {
        private bool _mouseIsDown;
        private bool _mouseIsOver;
        private BindingFormattingWindowsFormsEditorService? _owner;

        public DropDownButton(BindingFormattingWindowsFormsEditorService owner) : base()
        {
            _owner = owner;
            TabStop = false;
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(17, 19);
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // DANIELHE: we are trying to do something similar to anchoring
            height = Math.Min(height, _owner.Height - 2);
            width = SystemInformation.HorizontalScrollBarThumbWidth;
            y = 1;
            if (Parent is not null)
            {
                if (Parent.RightToLeft == RightToLeft.No)
                {
                    x = Parent.Width - width - 1;
                }
                else
                {
                    x = 1;
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (!Enabled)
            {
                _mouseIsDown = false;
                _mouseIsOver = false;
            }
        }

        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            base.OnKeyDown(kevent);

            if (kevent.KeyData == Keys.Space)
            {
                _mouseIsDown = true;
                Invalidate();
            }
        }

        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            base.OnKeyUp(kevent);
            if (_mouseIsDown)
            {
                _mouseIsDown = false;
                Invalidate();
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _mouseIsDown = false;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (!_mouseIsOver)
            {
                _mouseIsOver = true;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_mouseIsOver || _mouseIsDown)
            {
                _mouseIsOver = false;
                _mouseIsDown = false;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                _mouseIsDown = true;
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);

            if (mevent.Button != MouseButtons.None)
            {
                Rectangle r = ClientRectangle;
                if (!r.Contains(mevent.X, mevent.Y))
                {
                    if (_mouseIsDown)
                    {
                        _mouseIsDown = false;
                        Invalidate();
                    }
                }
                else
                {
                    if (!_mouseIsDown)
                    {
                        _mouseIsDown = true;
                        Invalidate();
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            if (_mouseIsDown)
            {
                _mouseIsDown = false;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (VisualStyles.VisualStyleRenderer.IsSupported)
            {
                ComboBoxState cbState = ComboBoxState.Normal;

                if (!Enabled)
                {
                    cbState = ComboBoxState.Disabled;
                }
                if (_mouseIsDown && _mouseIsOver)
                {
                    cbState = ComboBoxState.Pressed;
                }
                else if (_mouseIsOver)
                {
                    cbState = ComboBoxState.Hot;
                }

                ComboBoxRenderer.DrawDropDownButton(pevent.Graphics, pevent.ClipRectangle, cbState);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_KILLFOCUS:
                case NativeMethods.WM_CANCELMODE:
                case NativeMethods.WM_CAPTURECHANGED:
                    _mouseIsDown = false;
                    Invalidate();
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
