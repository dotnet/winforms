// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.PropertyGridInternal {

    using System.Diagnostics;
    using System;
    using System.Drawing;

    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;
    using System.Windows.Forms.ButtonInternal;
    using Microsoft.Win32;

    internal sealed class DropDownButton : Button {

        private bool useComboBoxTheme = false;

        private bool ignoreMouse;

        public DropDownButton() {
            SetStyle(ControlStyles.Selectable, true);
            SetAccessibleName();
        }


        // when the holder is open, we don't fire clicks
        //
        public bool IgnoreMouse {

            get {
                return ignoreMouse;
            }
            set {
                ignoreMouse = value;
            }
        }

        public bool UseComboBoxTheme {
            set {
                if (useComboBoxTheme != value) {
                    useComboBoxTheme = value;
                    if (AccessibilityImprovements.Level1) {
                        SetAccessibleName();
                    }
                    Invalidate();
                }
            }
        }

        protected override void OnClick(EventArgs e) {
            if (!IgnoreMouse) {
                base.OnClick(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            if (!IgnoreMouse) {
                base.OnMouseUp(e);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            if (!IgnoreMouse) {
                base.OnMouseDown(e);
            }
        }
        

        protected override void OnPaint(PaintEventArgs pevent) {
            base.OnPaint(pevent);
            
            if (Application.RenderWithVisualStyles & useComboBoxTheme) {
                ComboBoxState cbState = ComboBoxState.Normal;
        
                if (base.MouseIsDown) {
                    cbState = ComboBoxState.Pressed;
                }
                else if (base.MouseIsOver) {
                    cbState = ComboBoxState.Hot;
                }

                Rectangle dropDownButtonRect = new Rectangle(0, 0, Width, Height);
                if (cbState == ComboBoxState.Normal) {
                    pevent.Graphics.FillRectangle(SystemBrushes.Window, dropDownButtonRect);
                }
                if (!DpiHelper.IsScalingRequirementMet) {
                    ComboBoxRenderer.DrawDropDownButton(pevent.Graphics, dropDownButtonRect, cbState);
                }
                else {
                    ComboBoxRenderer.DrawDropDownButtonForHandle(pevent.Graphics, dropDownButtonRect, cbState, this.HandleInternal);
                }

                if (AccessibilityImprovements.Level1) {
                    // Redraw focus cues
                    // For consistency with other PropertyGrid buttons, i.e. those opening system dialogs ("..."), that always show visual cues when focused,
                    // we need to do the same for this custom button, painted as ComboBox control part (drop-down).
                    if (Focused) {
                        dropDownButtonRect.Inflate(-1, -1);
                        ControlPaint.DrawFocusRectangle(pevent.Graphics, dropDownButtonRect, ForeColor, BackColor);
                    }
                }
            }
        }

        private void SetAccessibleName() {
            if (AccessibilityImprovements.Level1 && useComboBoxTheme) {
                this.AccessibleName = SR.PropertyGridDropDownButtonComboBoxAccessibleName;
            }
            else {
                this.AccessibleName = SR.PropertyGridDropDownButtonAccessibleName;
            }
        }

        internal override ButtonBaseAdapter CreateStandardAdapter() {
            return new DropDownButtonAdapter(this);
        }        
    }

    internal class DropDownButtonAdapter : ButtonStandardAdapter {

        internal DropDownButtonAdapter(ButtonBase control) : base(control) {}

        private void DDB_Draw3DBorder(System.Drawing.Graphics g, Rectangle r, bool raised) {
            if (Control.BackColor != SystemColors.Control && SystemInformation.HighContrast) {
                if (raised) {
                    Color c = ControlPaint.LightLight(Control.BackColor);
                    ControlPaint.DrawBorder(g, r,
                                            c, 1, ButtonBorderStyle.Outset,
                                            c, 1, ButtonBorderStyle.Outset,
                                            c, 2, ButtonBorderStyle.Inset,
                                            c, 2, ButtonBorderStyle.Inset);
                }
                else {
                    ControlPaint.DrawBorder(g, r, ControlPaint.Dark(Control.BackColor), ButtonBorderStyle.Solid);
                }
            }
            else {
                if (raised) {
                    Color c = ControlPaint.Light(Control.BackColor);
                    ControlPaint.DrawBorder(g, r,
                                            c, 1, ButtonBorderStyle.Solid,
                                            c, 1, ButtonBorderStyle.Solid,
                                            Control.BackColor, 2, ButtonBorderStyle.Outset,
                                            Control.BackColor, 2, ButtonBorderStyle.Outset);

                    Rectangle inside = r;
                    inside.Offset(1,1);
                    inside.Width -= 3;
                    inside.Height -= 3;
                    c = ControlPaint.LightLight(Control.BackColor);
                    ControlPaint.DrawBorder(g, inside,
                                            c, 1, ButtonBorderStyle.Solid,
                                            c, 1, ButtonBorderStyle.Solid,
                                            c, 1, ButtonBorderStyle.None,
                                            c, 1, ButtonBorderStyle.None);
                }
                else {
                    ControlPaint.DrawBorder(g, r, ControlPaint.Dark(Control.BackColor), ButtonBorderStyle.Solid);
                }
            }
        }
        
        internal override void PaintUp(PaintEventArgs pevent, CheckState state) {
            base.PaintUp(pevent, state);
            if (!Application.RenderWithVisualStyles) {
                DDB_Draw3DBorder(pevent.Graphics, Control.ClientRectangle, true);
            }
            else {
                Color c = SystemColors.Window;
                Rectangle rect = Control.ClientRectangle;
                rect.Inflate(0, -1);
                ControlPaint.DrawBorder(pevent.Graphics, rect,
                                        c, 1, ButtonBorderStyle.None,
                                        c, 1, ButtonBorderStyle.None,
                                        c, 1, ButtonBorderStyle.Solid,
                                        c, 1, ButtonBorderStyle.None);
            }
        }

        internal override void DrawImageCore(Graphics graphics, Image image, Rectangle imageBounds, Point imageStart, ButtonBaseAdapter.LayoutData layout) {
             ControlPaint.DrawImageReplaceColor(graphics, image, imageBounds, Color.Black, Control.ForeColor);
             //ControlPaint.DrawImageColorized(graphics, image, imageBounds , Control.ForeColor);
        } 
    }
}

