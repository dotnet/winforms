// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Design
{
    /*
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
            BackColor = SystemColors.Window;
            Text = SR.DataGridNoneString;
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.UseTextForAccessibility, true);
            AccessibleRole = AccessibleRole.ComboBox;
            TabStop = true;
            Click += BindingFormattingWindowsFormsEditorService_Click;
            AccessibleName = SR.BindingFormattingDialogBindingPickerAccName;

            button = new DropDownButton(this);
            button.FlatStyle = FlatStyle.Popup;
            button.Image = CreateDownArrow();
            button.Padding = new System.Windows.Forms.Padding(0);
            button.BackColor = SystemColors.Control;
            button.ForeColor = SystemColors.ControlText;
            button.Click += new EventHandler(button_Click);
            button.Size = new Size(SystemInformation.VerticalScrollBarArrowHeight, (int)Font.Height + 2);
            button.AccessibleName = SR.BindingFormattingDialogDataSourcePickerDropDownAccName;
            // button.Dock = DockStyle.Right;

            Controls.Add(button);
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
            base.SetBoundsCore(x, y, width, PreferredHeight, specified);

            int buttonHeight = Height - 2;
            int buttonWidth = SystemInformation.HorizontalScrollBarThumbWidth;

            int buttonLeft = Width - buttonWidth - 2;
            int buttonTop = 1;
            if (RightToLeft == RightToLeft.No)
            {
                button.Bounds = new Rectangle(buttonTop, buttonLeft, buttonWidth, buttonHeight);
            }
            else
            {
                button.Bounds = new Rectangle(buttonTop, 2, buttonWidth, buttonHeight);
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

        public ITypeDescriptorContext Context
        {
            set
            {
                context = value;
            }
        }

        // ITypeDescriptorContext
        IContainer ITypeDescriptorContext.Container
        {
            get
            {
                if (ownerComponent == null)
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
                return ownerComponent;
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
            if (context != null)
            {
                context.OnComponentChanged();
            }
        }

        bool ITypeDescriptorContext.OnComponentChanging()
        {
            if (context != null)
            {
                return context.OnComponentChanging();
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
            else if (context != null)
            {
                return context.GetService(type);
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
            if (dropDownHolder == null)
            {
                dropDownHolder = new DropDownHolder(this);
            }

            dropDownHolder.SetComponent(ctl);

            dropDownHolder.Location = PointToScreen(new Point(0, Height));

            try
            {
                dropDownHolder.Visible = true;

                UnsafeNativeMethods.SetWindowLong(new HandleRef(dropDownHolder, dropDownHolder.Handle),
                                                  NativeMethods.GWL_HWNDPARENT,
                                                  new HandleRef(this, Handle));

                dropDownHolder.FocusComponent();
                dropDownHolder.DoModalLoop();
            }
            finally
            {
                UnsafeNativeMethods.SetWindowLong(new HandleRef(dropDownHolder, dropDownHolder.Handle),
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
                return binding;
            }
            set
            {
                if (binding == value)
                {
                    return;
                }

                binding = value;
                // update the text
                if (binding != null)
                {
                    Text = ConstructDisplayTextFromBinding(binding);
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
            set
            {
                defaultDataSourceUpdateMode = value;
            }
        }

        public IComponent OwnerComponent
        {
            set
            {
                ownerComponent = value;
            }
        }

        public string PropertyName
        {
            set
            {
                propertyName = value;
            }
        }

        public event EventHandler PropertyValueChanged
        {
            add
            {
                propertyValueChanged += value;
            }
            remove
            {
                propertyValueChanged -= value;
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "this dialog should be enabled only when the user picked a property from the properties tree view");
            DropDownPicker();
        }

        private static string ConstructDisplayTextFromBinding(Binding binding)
        {
            string result;
            if (binding.DataSource == null)
            {
                result = SR.DataGridNoneString;
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
                    result = SR.BindingFormattingDialogList;
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
            if (designBindingPicker == null)
            {
                designBindingPicker = new DesignBindingPicker();
                designBindingPicker.Width = Width;
            }

            DesignBinding initialDesignBinding = null;

            if (binding != null)
            {
                initialDesignBinding = new DesignBinding(binding.DataSource, binding.BindingMemberInfo.BindingMember);
            }

            expanded = true;

            DesignBinding designBinding = designBindingPicker.Pick(this,
                                                                        this,
                                                                        true,
                                                                        true,
                                                                        false,
                                                                        null,
                                                                        string.Empty,
                                                                        initialDesignBinding);

            expanded = false;

            // the user did not make any change
            if (designBinding == null)
            {
                return;
            }

            // construct the new binding from the designBindingPicker and compare to the oldBinding
            Binding oldBinding = binding;
            Binding newBinding = null;

            string formatString = oldBinding != null ? oldBinding.FormatString : string.Empty;
            IFormatProvider formatInfo = oldBinding != null ? oldBinding.FormatInfo : null;
            object nullValue = oldBinding != null ? oldBinding.NullValue : null;
            DataSourceUpdateMode updateMode = oldBinding != null ? oldBinding.DataSourceUpdateMode : defaultDataSourceUpdateMode;

            if (designBinding.DataSource != null && !string.IsNullOrEmpty(designBinding.DataMember))
            {
                newBinding = new Binding(propertyName,
                                         designBinding.DataSource,
                                         designBinding.DataMember,
                                         true,
                                         updateMode,
                                         nullValue,
                                         formatString,
                                         formatInfo);
            }

            // this is the new binding
            Binding = newBinding;

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

                ComboBoxRenderer.DrawTextBox(p.Graphics, rect, string.Empty, Font, state);

                Graphics g = p.Graphics;

                rect.Inflate(-2, -2);

                ControlPaint.DrawBorder(g, rect, backBrush.Color, ButtonBorderStyle.None);

                rect.Inflate(-1, -1);

                if (RightToLeft == RightToLeft.Yes)
                {
                    rect.X += button.Width;
                }

                rect.Width -= button.Width;

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
                if (!string.IsNullOrEmpty(text))
                {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Near;

                    Rectangle clientRect = ClientRectangle;
                    Rectangle textRect = new Rectangle(clientRect.X, clientRect.Y, clientRect.Width, clientRect.Height);

                    if (RightToLeft == RightToLeft.Yes)
                    {
                        textRect.X += button.Width;
                    }
                    textRect.Width -= button.Width;

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
            if (propertyValueChanged != null)
            {
                propertyValueChanged(this, e);
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
                case Interop.WindowMessages.WM_GETOBJECT:
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
                height = Math.Min(height, owner.Height - 2);
                width = SystemInformation.HorizontalScrollBarThumbWidth;
                y = 1;
                if (Parent != null)
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
                    mouseIsDown = false;
                    mouseIsOver = false;
                }
            }

            protected override void OnKeyDown(KeyEventArgs kevent)
            {
                base.OnKeyDown(kevent);

                if (kevent.KeyData == Keys.Space)
                {
                    mouseIsDown = true;
                    Invalidate();
                }
            }

            protected override void OnKeyUp(KeyEventArgs kevent)
            {
                base.OnKeyUp(kevent);
                if (mouseIsDown)
                {
                    mouseIsDown = false;
                    Invalidate();
                }
            }

            protected override void OnLostFocus(EventArgs e)
            {
                base.OnLostFocus(e);
                mouseIsDown = false;
                Invalidate();
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                if (!mouseIsOver)
                {
                    mouseIsOver = true;
                    Invalidate();
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                if (mouseIsOver || mouseIsDown)
                {
                    mouseIsOver = false;
                    mouseIsDown = false;
                    Invalidate();
                }
            }

            protected override void OnMouseDown(MouseEventArgs mevent)
            {
                base.OnMouseDown(mevent);
                if (mevent.Button == MouseButtons.Left)
                {
                    mouseIsDown = true;
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
                        if (mouseIsDown)
                        {
                            mouseIsDown = false;
                            Invalidate();
                        }
                    }
                    else
                    {
                        if (!mouseIsDown)
                        {
                            mouseIsDown = true;
                            Invalidate();
                        }
                    }
                }
            }

            protected override void OnMouseUp(MouseEventArgs mevent)
            {
                base.OnMouseUp(mevent);

                if (mouseIsDown)
                {
                    mouseIsDown = false;
                    Invalidate();
                }
            }

            protected override void OnPaint(PaintEventArgs pevent)
            {
                base.OnPaint(pevent);

                if (VisualStyleRenderer.IsSupported)
                {
                    ComboBoxState cbState = ComboBoxState.Normal;

                    if (!Enabled)
                    {
                        cbState = ComboBoxState.Disabled;
                    }
                    if (mouseIsDown && mouseIsOver)
                    {
                        cbState = ComboBoxState.Pressed;
                    }
                    else if (mouseIsOver)
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
                    case Interop.WindowMessages.WM_KILLFOCUS:
                    case Interop.WindowMessages.WM_CANCELMODE:
                    case Interop.WindowMessages.WM_CAPTURECHANGED:
                        mouseIsDown = false;
                        Invalidate();
                        base.WndProc(ref m);
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }
        }
    }*/
}
