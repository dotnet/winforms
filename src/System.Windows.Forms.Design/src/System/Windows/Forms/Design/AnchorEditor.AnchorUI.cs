// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design;

public sealed partial class AnchorEditor
{
    /// <summary>
    ///  User Interface for the AnchorEditor.
    /// </summary>
    private class AnchorUI : Control
    {
        private readonly SpringControl _bottom;
        private readonly ContainerPlaceholder _container = new();
        private readonly ControlPlaceholder _control = new();
        private readonly SpringControl _left;
        private readonly SpringControl _right;
        private readonly SpringControl[] _tabOrder;
        private readonly SpringControl _top;
        private IWindowsFormsEditorService? _editorService;
        private AnchorStyles _oldAnchor;

        public AnchorUI()
        {
            _left = new SpringControl(this) { AccessibleRole = AccessibleRole.CheckButton };
            _right = new SpringControl(this) { AccessibleRole = AccessibleRole.CheckButton };
            _top = new SpringControl(this) { AccessibleRole = AccessibleRole.CheckButton };
            _bottom = new SpringControl(this) { AccessibleRole = AccessibleRole.CheckButton };
            _tabOrder = [_left, _top, _right, _bottom];

            InitializeComponent();
        }

        public object? Value { get; private set; }

        public void End()
        {
            _editorService = null;
            Value = null;
        }

        public virtual AnchorStyles GetSelectedAnchor()
        {
            AnchorStyles baseVar = 0;
            if (_left.IsSolid)
            {
                baseVar |= AnchorStyles.Left;
            }

            if (_top.IsSolid)
            {
                baseVar |= AnchorStyles.Top;
            }

            if (_bottom.IsSolid)
            {
                baseVar |= AnchorStyles.Bottom;
            }

            if (_right.IsSolid)
            {
                baseVar |= AnchorStyles.Right;
            }

            return baseVar;
        }

        internal virtual void InitializeComponent()
        {
            int XBORDER = SystemInformation.Border3DSize.Width;
            int YBORDER = SystemInformation.Border3DSize.Height;
            SuspendLayout();
            SetBounds(0, 0, 90, 90);

            AccessibleName = SR.AnchorEditorAccName;

            _container.Location = new Point(0, 0);
            _container.Size = new Size(90, 90);
            _container.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            _control.Location = new Point(30, 30);
            _control.Size = new Size(30, 30);
            _control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            _right.Location = new Point(60, 40);
            _right.Size = new Size(30 - XBORDER, 10);
            _right.TabIndex = 2;
            _right.TabStop = true;
            _right.Anchor = AnchorStyles.Right;
            _right.AccessibleName = SR.AnchorEditorRightAccName;

            _left.Location = new Point(XBORDER, 40);
            _left.Size = new Size(30 - XBORDER, 10);
            _left.TabIndex = 0;
            _left.TabStop = true;
            _left.Anchor = AnchorStyles.Left;
            _left.AccessibleName = SR.AnchorEditorLeftAccName;

            _top.Location = new Point(40, YBORDER);
            _top.Size = new Size(10, 30 - YBORDER);
            _top.TabIndex = 1;
            _top.TabStop = true;
            _top.Anchor = AnchorStyles.Top;
            _top.AccessibleName = SR.AnchorEditorTopAccName;

            _bottom.Location = new Point(40, 60);
            _bottom.Size = new Size(10, 30 - YBORDER);
            _bottom.TabIndex = 3;
            _bottom.TabStop = true;
            _bottom.Anchor = AnchorStyles.Bottom;
            _bottom.AccessibleName = SR.AnchorEditorBottomAccName;

            Controls.Clear();
            Controls.AddRange(
            [
                _container
            ]);

            _container.Controls.Clear();
            _container.Controls.AddRange(
            [
                _control,
                _top,
                _left,
                _bottom,
                _right
            ]);
            ResumeLayout(false);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _top.Focus();
        }

        private void SetValue()
        {
            Value = GetSelectedAnchor();
        }

        public void Start(IWindowsFormsEditorService edSvc, object? value)
        {
            _editorService = edSvc;
            Value = value;

            if (value is AnchorStyles anchorStyles)
            {
                _left.IsSolid = (anchorStyles & AnchorStyles.Left) == AnchorStyles.Left;
                _top.IsSolid = (anchorStyles & AnchorStyles.Top) == AnchorStyles.Top;
                _bottom.IsSolid = (anchorStyles & AnchorStyles.Bottom) == AnchorStyles.Bottom;
                _right.IsSolid = (anchorStyles & AnchorStyles.Right) == AnchorStyles.Right;
                _oldAnchor = anchorStyles;
            }
            else
            {
                _oldAnchor = AnchorStyles.Top | AnchorStyles.Left;
            }
        }

        private void Teardown(bool saveAnchor)
        {
            if (!saveAnchor)
            {
                Value = _oldAnchor;
            }

            _editorService!.CloseDropDown();
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
            private readonly AnchorUI _picker;
            internal bool _focused;
            internal bool _solid;

            public SpringControl(AnchorUI picker)
            {
                _picker = picker ?? throw new ArgumentNullException(nameof(picker));
                TabStop = true;
            }

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new SpringControlAccessibleObject(this);
            }

            public bool IsSolid
            {
                get => _solid;
                set
                {
                    if (_solid != value)
                    {
                        _solid = value;
                        _picker.SetValue();
                        Invalidate();
                    }
                }
            }

            protected override void OnGotFocus(EventArgs e)
            {
                if (!_focused)
                {
                    _focused = true;
                    Invalidate();
                }

                base.OnGotFocus(e);
            }

            protected override void OnLostFocus(EventArgs e)
            {
                if (_focused)
                {
                    _focused = false;
                    Invalidate();
                }

                base.OnLostFocus(e);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                IsSolid = !_solid;
                Focus();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Rectangle rc = ClientRectangle;

                if (_solid)
                {
                    e.Graphics.FillRectangle(SystemBrushes.ControlDark, rc);
                    e.Graphics.DrawRectangle(SystemPens.WindowFrame, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                }
                else
                {
                    ControlPaint.DrawFocusRectangle(e.Graphics, rc);
                }

                if (_focused)
                {
                    rc.Inflate(-2, -2);
                    ControlPaint.DrawFocusRectangle(e.Graphics, rc);
                }
            }

            protected override bool ProcessDialogChar(char charCode)
            {
                if (charCode == ' ')
                {
                    IsSolid = !_solid;
                    return true;
                }

                return base.ProcessDialogChar(charCode);
            }

            protected override bool ProcessDialogKey(Keys keyData)
            {
                if ((keyData & Keys.KeyCode) == Keys.Return && (keyData & (Keys.Alt | Keys.Control)) == 0)
                {
                    _picker.Teardown(true);
                    return true;
                }

                if ((keyData & Keys.KeyCode) == Keys.Escape && (keyData & (Keys.Alt | Keys.Control)) == 0)
                {
                    _picker.Teardown(false);
                    return true;
                }

                if ((keyData & Keys.KeyCode) == Keys.Tab && (keyData & (Keys.Alt | Keys.Control)) == 0)
                {
                    for (int i = 0; i < _picker._tabOrder.Length; i++)
                    {
                        if (_picker._tabOrder[i] == this)
                        {
                            i += (keyData & Keys.Shift) == 0 ? 1 : -1;
                            i = i < 0 ? i + _picker._tabOrder.Length : i % _picker._tabOrder.Length;
                            _picker._tabOrder[i].Focus();
                            break;
                        }
                    }

                    return true;
                }

                return base.ProcessDialogKey(keyData);
            }

            private class SpringControlAccessibleObject : ControlAccessibleObject
            {
                public SpringControlAccessibleObject(SpringControl owner) : base(owner)
                {
                }

                public override string DefaultAction => ((SpringControl)Owner!).IsSolid
                    ? SR.AccessibleActionUncheck
                    : SR.AccessibleActionCheck;

                public override AccessibleStates State
                {
                    get
                    {
                        AccessibleStates state = base.State;

                        if (((SpringControl)Owner!).IsSolid)
                        {
                            state |= AccessibleStates.Checked;
                        }

                        return state;
                    }
                }
            }
        }
    }
}
