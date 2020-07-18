// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms.ButtonInternal
{
    internal class CheckBoxPopupAdapter : CheckBoxBaseAdapter
    {
        internal CheckBoxPopupAdapter(ButtonBase control) : base(control) { }

        internal override void PaintUp(PaintEventArgs e, CheckState state)
        {
            if (Control.Appearance == Appearance.Button)
            {
                ButtonPopupAdapter adapter = new ButtonPopupAdapter(Control);
                adapter.PaintUp(e, Control.CheckState);
            }
            else
            {
                ColorData colors = PaintPopupRender(e).Calculate();
                LayoutData layout = PaintPopupLayout(show3D: false).Layout();

                PaintButtonBackground(e, Control.ClientRectangle, null);

                PaintImage(e, layout);

                DrawCheckBackground(
                    e,
                    layout.checkBounds,
                    colors.windowText,
                    colors.options.HighContrast ? colors.buttonFace : colors.highlight,
                    disabledColors: true,
                    colors);
                ControlPaint.DrawBorderSolid(
                    e,
                    layout.checkBounds,
                    (colors.options.HighContrast && !Control.Enabled) ? colors.windowFrame : colors.buttonShadow);
                DrawCheckOnly(e, layout, colors, colors.windowText, colors.highlight);

                AdjustFocusRectangle(layout);
                PaintField(e, layout, colors, colors.windowText, true);
            }
        }

        internal override void PaintOver(PaintEventArgs e, CheckState state)
        {
            if (Control.Appearance == Appearance.Button)
            {
                ButtonPopupAdapter adapter = new ButtonPopupAdapter(Control);
                adapter.PaintOver(e, Control.CheckState);
            }
            else
            {
                ColorData colors = PaintPopupRender(e).Calculate();
                LayoutData layout = PaintPopupLayout(show3D: true).Layout();

                Region original = e.Graphics.Clip;
                PaintButtonBackground(e, Control.ClientRectangle, null);

                PaintImage(e, layout);

                DrawCheckBackground(
                    e,
                    layout.checkBounds,
                    colors.windowText,
                    colors.options.HighContrast ? colors.buttonFace : colors.highlight,
                    disabledColors: true,
                    colors);

                DrawPopupBorder(e, layout.checkBounds, colors);
                DrawCheckOnly(e, layout, colors, colors.windowText, colors.highlight);

                if (!string.IsNullOrEmpty(Control.Text))
                {
                    e.Graphics.Clip = original;
                    e.Graphics.ExcludeClip(layout.checkArea);
                }

                AdjustFocusRectangle(layout);
                PaintField(e, layout, colors, colors.windowText, true);
            }
        }

        internal override void PaintDown(PaintEventArgs e, CheckState state)
        {
            if (Control.Appearance == Appearance.Button)
            {
                ButtonPopupAdapter adapter = new ButtonPopupAdapter(Control);
                adapter.PaintDown(e, Control.CheckState);
            }
            else
            {
                Graphics g = e.Graphics;
                ColorData colors = PaintPopupRender(e.Graphics).Calculate();
                LayoutData layout = PaintPopupLayout(show3D: true).Layout();

                Region original = e.Graphics.Clip;
                PaintButtonBackground(e, Control.ClientRectangle, null);

                PaintImage(e, layout);

                DrawCheckBackground(e, layout.checkBounds, colors.windowText, colors.buttonFace, true, colors);
                DrawPopupBorder(e, layout.checkBounds, colors);
                DrawCheckOnly(e, layout, colors, colors.windowText, colors.buttonFace);

                AdjustFocusRectangle(layout);
                PaintField(e, layout, colors, colors.windowText, true);
            }
        }

        #region Layout

        protected override ButtonBaseAdapter CreateButtonAdapter()
        {
            return new ButtonPopupAdapter(Control);
        }

        protected override LayoutOptions Layout(PaintEventArgs e)
        {
            LayoutOptions layout = PaintPopupLayout(show3D: true);
            Debug.Assert(layout.GetPreferredSizeCore(LayoutUtils.MaxSize)
                == PaintPopupLayout(show3D: false).GetPreferredSizeCore(LayoutUtils.MaxSize),
                "The state of show3D should not effect PreferredSize");
            return layout;
        }

        internal static LayoutOptions PaintPopupLayout(
            bool show3D,
            int checkSize,
            Rectangle clientRectangle,
            Padding padding,
            bool isDefault,
            Font font,
            string text,
            bool enabled,
            ContentAlignment textAlign,
            RightToLeft rtl,
            Control control = null)
        {
            LayoutOptions layout = CommonLayout(clientRectangle, padding, isDefault, font, text, enabled, textAlign, rtl);
            layout.shadowedText = false;
            if (show3D)
            {
                layout.checkSize = (int)(checkSize * GetDpiScaleRatio(control) + 1);
            }
            else
            {
                layout.checkSize = (int)(checkSize * GetDpiScaleRatio(control));
                layout.checkPaddingSize = 1;
            }
            return layout;
        }

        private LayoutOptions PaintPopupLayout(bool show3D)
        {
            LayoutOptions layout = CommonLayout();
            layout.shadowedText = false;
            if (show3D)
            {
                layout.checkSize = (int)(flatCheckSize * GetDpiScaleRatio() + 1);
            }
            else
            {
                layout.checkSize = (int)(flatCheckSize * GetDpiScaleRatio());
                layout.checkPaddingSize = 1;
            }
            return layout;
        }

        #endregion
    }
}
