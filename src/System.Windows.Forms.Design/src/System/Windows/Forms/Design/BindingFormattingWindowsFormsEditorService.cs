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
    ITypeDescriptorContext context = null;

    DropDownHolder dropDownHolder;
    DropDownButton button;
    EventHandler propertyValueChanged;

    Binding binding = null;
    IComponent ownerComponent;
    DataSourceUpdateMode defaultDataSourceUpdateMode = DataSourceUpdateMode.OnValidation;
    DesignBindingPicker designBindingPicker;
    string propertyName = string.Empty;

    bool expanded = false;

    public BindingFormattingWindowsFormsEditorService()
    {
        this.BackColor = SystemColors.Window;
        this.Text = SR.GetString(SR.DataGridNoneString);
        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.Selectable, true);
        SetStyle(ControlStyles.UseTextForAccessibility, true);
        this.AccessibleRole = AccessibleRole.ComboBox;
        this.TabStop = true;
        this.Click += BindingFormattingWindowsFormsEditorService_Click;
        this.AccessibleName = SR.GetString(SR.BindingFormattingDialogBindingPickerAccName);

        button = new DropDownButton(this);
        button.FlatStyle = FlatStyle.Popup;
        button.Image = CreateDownArrow();
        button.Padding = new System.Windows.Forms.Padding(0);
        button.BackColor = SystemColors.Control;
        button.ForeColor = SystemColors.ControlText;
        button.Click += new EventHandler(this.button_Click);
        button.Size = new Drawing.Size(SystemInformation.VerticalScrollBarArrowHeight, (int)this.Font.Height + 2);
        button.AccessibleName = SR.GetString(SR.BindingFormattingDialogDataSourcePickerDropDownAccName);
        // button.Dock = DockStyle.Right;

        this.Controls.Add(button);
    }

    private void BindingFormattingWindowsFormsEditorService_Click(object sender, EventArgs e)
    {
        if (!expanded)
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
        base.SetBoundsCore(x, y, width, this.PreferredHeight, specified);

        int buttonHeight = this.Height - 2;
        int buttonWidth = SystemInformation.HorizontalScrollBarThumbWidth;

        int buttonLeft = this.Width - buttonWidth - 2;
        int buttonTop = 1;
        if (this.RightToLeft == RightToLeft.No)
        {
            this.button.Bounds = new Rectangle(buttonTop, buttonLeft, buttonWidth, buttonHeight);
        }
        else
        {
            this.button.Bounds = new Rectangle(buttonTop, 2, buttonWidth, buttonHeight);
        }
    }

    private int PreferredHeight
    {
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
        get
        {
            Drawing.Size textSize = System.Windows.Forms.TextRenderer.MeasureText("j^", this.Font, new Drawing.Size(Int16.MaxValue, (int)(FontHeight * 1.25)));
            return textSize.Height + SystemInformation.BorderSize.Height * 8 + Padding.Size.Height;
        }
    }

    /*
    public override Size GetPreferredSize(Size proposedSize) {
        Size newSize = proposedSize;
        Size textSize = TextRenderer.MeasureText("j^", this.Font, new Size(Int16.MaxValue, (int)(FontHeight * 1.25)));
        newSize.Height = (short)(textSize.Height + SystemInformation.BorderSize.Height*8 + Padding.Size.Height);

        return base.GetPreferredSize(newSize);
    }
    */

    public ITypeDescriptorContext Context
    {
        set
        {
            this.context = value;
        }
    }

    // ITypeDescriptorContext
    IContainer ITypeDescriptorContext.Container
    {
        get
        {
            if (this.ownerComponent == null)
            {
                return null;
            }

            ISite site = ownerComponent.Site;
            if (site == null)
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
            return this.ownerComponent;
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
        if (this.context != null)
        {
            this.context.OnComponentChanged();
        }
    }

    bool ITypeDescriptorContext.OnComponentChanging()
    {
        if (this.context != null)
        {
            return this.context.OnComponentChanging();
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
        else if (this.context != null)
        {
            return this.context.GetService(type);
        }
        else
        {
            return null;
        }
    }

    // IWindowsFormsEditorService
    void IWindowsFormsEditorService.CloseDropDown()
    {
        dropDownHolder.SetComponent(null);
        dropDownHolder.Visible = false;
    }

    void IWindowsFormsEditorService.DropDownControl(Control ctl)
    {
        if (this.dropDownHolder == null)
        {
            this.dropDownHolder = new DropDownHolder(this);
        }

        this.dropDownHolder.SetComponent(ctl);

        this.dropDownHolder.Location = PointToScreen(new Drawing.Point(0, this.Height));

        try
        {
            this.dropDownHolder.Visible = true;

            UnsafeNativeMethods.SetWindowLong(new HandleRef(this.dropDownHolder, this.dropDownHolder.Handle),
                                              NativeMethods.GWL_HWNDPARENT,
                                              new HandleRef(this, this.Handle));

            this.dropDownHolder.FocusComponent();
            this.dropDownHolder.DoModalLoop();
        }
        finally
        {
            UnsafeNativeMethods.SetWindowLong(new HandleRef(this.dropDownHolder, this.dropDownHolder.Handle),
                                              NativeMethods.GWL_HWNDPARENT,
                                              new HandleRef(null, IntPtr.Zero));
            this.Focus();
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
            return this.binding;
        }
        set
        {
            if (this.binding == value)
            {
                return;
            }

            this.binding = value;
            // update the text
            if (this.binding != null)
            {
                this.Text = ConstructDisplayTextFromBinding(this.binding);
            }
            else
            {
                this.Text = SR.GetString(SR.DataGridNoneString);
            }

            Invalidate();
        }
    }

    public DataSourceUpdateMode DefaultDataSourceUpdateMode
    {
        /* Omit this until someone needs to call it (avoids FxCop warning)...
        get
        {
            return this.defaultDataSourceUpdateMode;
        }
        */
        set
        {
            this.defaultDataSourceUpdateMode = value;
        }
    }

    public IComponent OwnerComponent
    {
        set
        {
            this.ownerComponent = value;
        }
    }

    public string PropertyName
    {
        set
        {
            this.propertyName = value;
        }
    }

    public event EventHandler PropertyValueChanged
    {
        add
        {
            this.propertyValueChanged += value;
        }
        remove
        {
            this.propertyValueChanged -= value;
        }
    }

    private void button_Click(object sender, EventArgs e)
    {
        Debug.Assert(!String.IsNullOrEmpty(this.propertyName), "this dialog should be enabled only when the user picked a property from the properties tree view");
        DropDownPicker();
    }

    private static string ConstructDisplayTextFromBinding(Binding binding)
    {
        string result;
        if (binding.DataSource == null)
        {
            result = SR.GetString(SR.DataGridNoneString);
        }
        else
        {
            if (binding.DataSource is IComponent)
            {
                IComponent comp = binding.DataSource as IComponent;
                if (comp.Site != null)
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
                result = SR.GetString(SR.BindingFormattingDialogList);
            }
            else
            {
                string typeName = TypeDescriptor.GetClassName(binding.DataSource);
                int lastDot = typeName.LastIndexOf(".");
                if (lastDot != -1)
                {
                    typeName = typeName.Substring(lastDot + 1);
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
        if (this.designBindingPicker == null)
        {
            this.designBindingPicker = new DesignBindingPicker();
            this.designBindingPicker.Width = this.Width;
        }

        DesignBinding initialDesignBinding = null;

        if (this.binding != null)
        {
            initialDesignBinding = new DesignBinding(this.binding.DataSource, this.binding.BindingMemberInfo.BindingMember);
        }

        expanded = true;

        DesignBinding designBinding = this.designBindingPicker.Pick(this, /*ITypeDescriptorService*/
                                                                    this, /*IServiceProvider*/
                                                                    true, /*showDataSources*/
                                                                    true, /*showDataMembers*/
                                                                    false, /*selectListMembers*/
                                                                    null,
                                                                    String.Empty,
                                                                    initialDesignBinding);

        expanded = false;

        // the user did not make any change
        if (designBinding == null)
        {
            return;
        }

        // construct the new binding from the designBindingPicker and compare to the oldBinding
        //
        Binding oldBinding = this.binding;
        Binding newBinding = null;

        string formatString = oldBinding != null ? oldBinding.FormatString : String.Empty;
        IFormatProvider formatInfo = oldBinding != null ? oldBinding.FormatInfo : null;
        object nullValue = oldBinding != null ? oldBinding.NullValue : null;
        DataSourceUpdateMode updateMode = oldBinding != null ? oldBinding.DataSourceUpdateMode : this.defaultDataSourceUpdateMode;

        if (designBinding.DataSource != null && !String.IsNullOrEmpty(designBinding.DataMember))
        {
            newBinding = new Binding(this.propertyName,
                                     designBinding.DataSource,
                                     designBinding.DataMember,
                                     true, /*formattingEnabled*/
                                     updateMode,
                                     nullValue,
                                     formatString,
                                     formatInfo);
        }

        // this is the new binding
        this.Binding = newBinding;

        bool bindingChanged = (newBinding == null || oldBinding != null);
        bindingChanged = bindingChanged || (newBinding != null && oldBinding == null);
        bindingChanged = bindingChanged || (newBinding != null && oldBinding != null &&
                         (newBinding.DataSource != oldBinding.DataSource || !newBinding.BindingMemberInfo.Equals(oldBinding.BindingMemberInfo)));

        if (bindingChanged)
        {
            OnPropertyValueChanged(EventArgs.Empty);
        }
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        this.Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs p)
    {
        base.OnPaint(p);

        string text = this.Text;
        if (ComboBoxRenderer.IsSupported)
        {
            Rectangle rect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width, this.ClientRectangle.Height);
            ComboBoxState state;
            SolidBrush foreBrush;
            SolidBrush backBrush;

            bool enabled = this.Enabled;

            if (!enabled)
            {
                foreBrush = (SolidBrush)SystemBrushes.ControlDark;
                backBrush = (SolidBrush)SystemBrushes.Control;
                state = ComboBoxState.Disabled;
            }
            else if (this.ContainsFocus)
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

            ComboBoxRenderer.DrawTextBox(p.Graphics, rect, String.Empty, this.Font, state);

            Graphics g = p.Graphics;

            rect.Inflate(-2, -2);

            System.Windows.Forms.ControlPaint.DrawBorder(g, rect, backBrush.Color, ButtonBorderStyle.None);

            rect.Inflate(-1, -1);

            if (this.RightToLeft == RightToLeft.Yes)
            {
                rect.X += this.button.Width;
            }

            rect.Width -= this.button.Width;

            g.FillRectangle(backBrush, rect);

            TextFormatFlags flags = TextFormatFlags.VerticalCenter;
            if (this.RightToLeft == RightToLeft.No)
            {
                flags |= TextFormatFlags.Left;
            }
            else
            {
                flags |= TextFormatFlags.Right;
            }

            if (this.ContainsFocus)
            {
                System.Windows.Forms.ControlPaint.DrawFocusRectangle(g, rect, Color.Empty, backBrush.Color);
            }

            TextRenderer.DrawText(g, text, this.Font, rect, foreBrush.Color, flags);

        }
        else
        {
            if (!String.IsNullOrEmpty(text))
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Near;

                Rectangle clientRect = this.ClientRectangle;
                Rectangle textRect = new Rectangle(clientRect.X, clientRect.Y, clientRect.Width, clientRect.Height);

                if (this.RightToLeft == RightToLeft.Yes)
                {
                    textRect.X += this.button.Width;
                }
                textRect.Width -= this.button.Width;

                TextFormatFlags flags = TextFormatFlags.VerticalCenter;
                if (this.RightToLeft == RightToLeft.No)
                {
                    flags |= TextFormatFlags.Left;
                }
                else
                {
                    flags |= TextFormatFlags.Right;
                }

                TextRenderer.DrawText(p.Graphics, text, this.Font, textRect, this.ForeColor, flags);

                stringFormat.Dispose();
            }
        }

    }

    protected void OnPropertyValueChanged(EventArgs e)
    {
        if (this.propertyValueChanged != null)
        {
            this.propertyValueChanged(this, e);
        }
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        Keys modifiers = System.Windows.Forms.Control.ModifierKeys;
        if (((modifiers & Keys.Alt) == Keys.Alt && (keyData & Keys.KeyCode) == Keys.Down) || keyData == Keys.F4)
        {
            DropDownPicker();
            return true;
        }

        return base.ProcessDialogKey(keyData);
    }
    private void ExpandDropDown()
    {
        this.BeginInvoke(new Action(this.DropDownPicker));
    }

    private void CollapseDropDown()
    {
        this.Focus();
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
                return this.Service.Text;
            }
        }

        #endregion

        #region IExpandCollapseProvider

        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return Service.expanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            }
        }

        public void Collapse()
        {
            if (Service.expanded)
            {
                Service.CollapseDropDown();
            }
        }

        public void Expand()
        {
            if (!Service.expanded)
            {
                Service.ExpandDropDown();
            }
        }

        #endregion
    }

    private class DropDownButton : Button
    {
        private bool mouseIsDown = false;
        private bool mouseIsOver = false;
        private BindingFormattingWindowsFormsEditorService owner = null;

        public DropDownButton(BindingFormattingWindowsFormsEditorService owner) : base()
        {
            this.owner = owner;
            this.TabStop = false;
        }

        protected override Drawing.Size DefaultSize
        {
            get
            {
                return new Drawing.Size(17, 19);
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // DANIELHE: we are trying to do something similar to anchoring
            height = Math.Min(height, this.owner.Height - 2);
            width = SystemInformation.HorizontalScrollBarThumbWidth;
            y = 1;
            if (this.Parent != null)
            {
                if (this.Parent.RightToLeft == RightToLeft.No)
                {
                    x = this.Parent.Width - width - 1;
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
            if (!this.Enabled)
            {
                this.mouseIsDown = false;
                this.mouseIsOver = false;
            }
        }

        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            base.OnKeyDown(kevent);

            if (kevent.KeyData == Keys.Space)
            {
                this.mouseIsDown = true;
                this.Invalidate();
            }
        }

        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            base.OnKeyUp(kevent);
            if (this.mouseIsDown)
            {
                this.mouseIsDown = false;
                this.Invalidate();
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.mouseIsDown = false;
            this.Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (!this.mouseIsOver)
            {
                this.mouseIsOver = true;
                this.Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.mouseIsOver || this.mouseIsDown)
            {
                this.mouseIsOver = false;
                this.mouseIsDown = false;
                this.Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                this.mouseIsDown = true;
                this.Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);

            if (mevent.Button != MouseButtons.None)
            {
                Rectangle r = this.ClientRectangle;
                if (!r.Contains(mevent.X, mevent.Y))
                {
                    if (this.mouseIsDown)
                    {
                        this.mouseIsDown = false;
                        this.Invalidate();
                    }
                }
                else
                {
                    if (!this.mouseIsDown)
                    {
                        this.mouseIsDown = true;
                        this.Invalidate();
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            if (this.mouseIsDown)
            {
                this.mouseIsDown = false;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (VisualStyles.VisualStyleRenderer.IsSupported)
            {
                ComboBoxState cbState = ComboBoxState.Normal;

                if (!this.Enabled)
                {
                    cbState = ComboBoxState.Disabled;
                }
                if (this.mouseIsDown && this.mouseIsOver)
                {
                    cbState = ComboBoxState.Pressed;
                }
                else if (this.mouseIsOver)
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
                    this.mouseIsDown = false;
                    this.Invalidate();
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
