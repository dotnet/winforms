// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides a design-time editor for specifying the <see cref='System.Windows.Forms.Control.Anchor' />
    ///  property.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class AnchorEditor : UITypeEditor
    {
        private AnchorUI _anchorUI;

        /// <summary>
        ///  Edits the given object value using the editor style provided by GetEditorStyle.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider == null)
            {
                return value;
            }
            if (!(provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService edSvc))
            {
                return value;
            }

            if (_anchorUI == null)
            {
                _anchorUI = new AnchorUI(this);
            }

            _anchorUI.Start(edSvc, value);
            edSvc.DropDownControl(_anchorUI);
            value = _anchorUI.Value;
            _anchorUI.End();

            return value;
        }

        /// <summary>
        ///  Gets the editing style of the Edit method.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        ///  User Interface for the AnchorEditor.
        /// </summary>
        private class AnchorUI : Control
        {
            private readonly SpringControl bottom;
            private readonly ContainerPlaceholder container = new ContainerPlaceholder();
            private readonly ControlPlaceholder control = new ControlPlaceholder();
            private readonly SpringControl left;
            private readonly SpringControl right;
            private readonly SpringControl[] tabOrder;
            private readonly SpringControl top;
            private readonly AnchorEditor editor;
            private IWindowsFormsEditorService edSvc;
            private AnchorStyles oldAnchor;

            public AnchorUI(AnchorEditor editor)
            {
                this.editor = editor;
                left = new SpringControl(this);
                right = new SpringControl(this);
                top = new SpringControl(this);
                bottom = new SpringControl(this);
                tabOrder = new[] { left, top, right, bottom };

                InitializeComponent();
            }

            public object Value { get; private set; }

            public void End()
            {
                edSvc = null;
                Value = null;
            }

            public virtual AnchorStyles GetSelectedAnchor()
            {
                AnchorStyles baseVar = 0;
                if (left.GetSolid())
                {
                    baseVar |= AnchorStyles.Left;
                }

                if (top.GetSolid())
                {
                    baseVar |= AnchorStyles.Top;
                }

                if (bottom.GetSolid())
                {
                    baseVar |= AnchorStyles.Bottom;
                }

                if (right.GetSolid())
                {
                    baseVar |= AnchorStyles.Right;
                }

                return baseVar;
            }

            internal virtual void InitializeComponent()
            {
                int XBORDER = SystemInformation.Border3DSize.Width;
                int YBORDER = SystemInformation.Border3DSize.Height;
                SetBounds(0, 0, 90, 90);

                AccessibleName = SR.AnchorEditorAccName;

                container.Location = new Point(0, 0);
                container.Size = new Size(90, 90);
                container.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

                control.Location = new Point(30, 30);
                control.Size = new Size(30, 30);
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

                right.Location = new Point(60, 40);
                right.Size = new Size(30 - XBORDER, 10);
                right.TabIndex = 2;
                right.TabStop = true;
                right.Anchor = AnchorStyles.Right;
                right.AccessibleName = SR.AnchorEditorRightAccName;

                left.Location = new Point(XBORDER, 40);
                left.Size = new Size(30 - XBORDER, 10);
                left.TabIndex = 0;
                left.TabStop = true;
                left.Anchor = AnchorStyles.Left;
                left.AccessibleName = SR.AnchorEditorLeftAccName;

                top.Location = new Point(40, YBORDER);
                top.Size = new Size(10, 30 - YBORDER);
                top.TabIndex = 1;
                top.TabStop = true;
                top.Anchor = AnchorStyles.Top;
                top.AccessibleName = SR.AnchorEditorTopAccName;

                bottom.Location = new Point(40, 60);
                bottom.Size = new Size(10, 30 - YBORDER);
                bottom.TabIndex = 3;
                bottom.TabStop = true;
                bottom.Anchor = AnchorStyles.Bottom;
                bottom.AccessibleName = SR.AnchorEditorBottomAccName;

                Controls.Clear();
                Controls.AddRange(new Control[]
                {
                    container
                });

                container.Controls.Clear();
                container.Controls.AddRange(new Control[]
                {
                    control,
                    top,
                    left,
                    bottom,
                    right
                });
            }
            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                top.Focus();
            }

            private void SetValue()
            {
                Value = GetSelectedAnchor();
            }

            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                this.edSvc = edSvc;
                Value = value;

                if (value is AnchorStyles)
                {
                    left.SetSolid(((AnchorStyles)value & AnchorStyles.Left) == AnchorStyles.Left);
                    top.SetSolid(((AnchorStyles)value & AnchorStyles.Top) == AnchorStyles.Top);
                    bottom.SetSolid(((AnchorStyles)value & AnchorStyles.Bottom) == AnchorStyles.Bottom);
                    right.SetSolid(((AnchorStyles)value & AnchorStyles.Right) == AnchorStyles.Right);
                    oldAnchor = (AnchorStyles)value;
                }
                else
                {
                    oldAnchor = AnchorStyles.Top | AnchorStyles.Left;
                }
            }

            private void Teardown(bool saveAnchor)
            {
                if (!saveAnchor)
                {
                    Value = oldAnchor;
                }

                edSvc.CloseDropDown();
            }

            private class ContainerPlaceholder : Control
            {
                public ContainerPlaceholder()
                {
                    BackColor = SystemColors.Window;
                    ForeColor = SystemColors.WindowText;
                    TabStop = false;
                }

                protected override void OnPaint(PaintEventArgs e)
                {
                    Rectangle rc = ClientRectangle;
                    ControlPaint.DrawBorder3D(e.Graphics, rc, Border3DStyle.Sunken);
                }
            }

            private class ControlPlaceholder : Control
            {
                public ControlPlaceholder()
                {
                    BackColor = SystemColors.Control;
                    TabStop = false;
                    SetStyle(ControlStyles.Selectable, false);
                }

                protected override void OnPaint(PaintEventArgs e)
                {
                    Rectangle rc = ClientRectangle;
                    ControlPaint.DrawButton(e.Graphics, rc, ButtonState.Normal);
                }
            }

            private class SpringControl : Control
            {
                private readonly AnchorUI picker;
                internal bool focused;
                internal bool solid;

                public SpringControl(AnchorUI picker)
                {
                    this.picker = picker ?? throw new ArgumentException();
                    TabStop = true;
                }

                protected override AccessibleObject CreateAccessibilityInstance()
                {
                    return new SpringControlAccessibleObject(this);
                }

                public virtual bool GetSolid()
                {
                    return solid;
                }

                protected override void OnGotFocus(EventArgs e)
                {
                    if (!focused)
                    {
                        focused = true;
                        Invalidate();
                    }

                    base.OnGotFocus(e);
                }

                protected override void OnLostFocus(EventArgs e)
                {
                    if (focused)
                    {
                        focused = false;
                        Invalidate();
                    }

                    base.OnLostFocus(e);
                }

                protected override void OnMouseDown(MouseEventArgs e)
                {
                    SetSolid(!solid);
                    Focus();
                }

                protected override void OnPaint(PaintEventArgs e)
                {
                    Rectangle rc = ClientRectangle;

                    if (solid)
                    {
                        e.Graphics.FillRectangle(SystemBrushes.ControlDark, rc);
                        e.Graphics.DrawRectangle(SystemPens.WindowFrame, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                    }
                    else
                    {
                        ControlPaint.DrawFocusRectangle(e.Graphics, rc);
                    }

                    if (focused)
                    {
                        rc.Inflate(-2, -2);
                        ControlPaint.DrawFocusRectangle(e.Graphics, rc);
                    }
                }

                protected override bool ProcessDialogChar(char charCode)
                {
                    if (charCode == ' ')
                    {
                        SetSolid(!solid);
                        return true;
                    }

                    return base.ProcessDialogChar(charCode);
                }

                protected override bool ProcessDialogKey(Keys keyData)
                {
                    if ((keyData & Keys.KeyCode) == Keys.Return && (keyData & (Keys.Alt | Keys.Control)) == 0)
                    {
                        picker.Teardown(true);
                        return true;
                    }

                    if ((keyData & Keys.KeyCode) == Keys.Escape && (keyData & (Keys.Alt | Keys.Control)) == 0)
                    {
                        picker.Teardown(false);
                        return true;
                    }

                    if ((keyData & Keys.KeyCode) == Keys.Tab && (keyData & (Keys.Alt | Keys.Control)) == 0)
                    {
                        for (int i = 0; i < picker.tabOrder.Length; i++)
                        {
                            if (picker.tabOrder[i] == this)
                            {
                                i += (keyData & Keys.Shift) == 0 ? 1 : -1;
                                i = i < 0 ? i + picker.tabOrder.Length : i % picker.tabOrder.Length;
                                picker.tabOrder[i].Focus();
                                break;
                            }
                        }

                        return true;
                    }

                    return base.ProcessDialogKey(keyData);
                }

                public virtual void SetSolid(bool value)
                {
                    if (solid != value)
                    {
                        solid = value;
                        picker.SetValue();
                        Invalidate();
                    }
                }

                private class SpringControlAccessibleObject : ControlAccessibleObject
                {
                    public SpringControlAccessibleObject(SpringControl owner) : base(owner)
                    {
                    }

                    public override AccessibleStates State
                    {
                        get
                        {
                            AccessibleStates state = base.State;

                            if (((SpringControl)Owner).GetSolid())
                            {
                                state |= AccessibleStates.Selected;
                            }

                            return state;
                        }
                    }
                }
            }
        }
    }
}
