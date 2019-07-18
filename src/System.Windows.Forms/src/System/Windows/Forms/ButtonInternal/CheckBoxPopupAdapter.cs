// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
                Graphics g = e.Graphics;
                ColorData colors = PaintPopupRender(e.Graphics).Calculate();
                LayoutData layout = PaintPopupLayout(e, false).Layout();

                Region original = e.Graphics.Clip;
                PaintButtonBackground(e, Control.ClientRectangle, null);

                PaintImage(e, layout);

                DrawCheckBackground(e, layout.checkBounds, colors.windowText, colors.options.highContrast ? colors.buttonFace : colors.highlight, true, colors);
                DrawFlatBorder(e.Graphics, layout.checkBounds,
                    (colors.options.highContrast && !Control.Enabled) ? colors.windowFrame : colors.buttonShadow);
                DrawCheckOnly(e, layout, colors, colors.windowText, colors.highlight);

                AdjustFocusRectangle(layout);
                PaintField(e, layout, colors, colors.windowText, true);
            }
        }

        internal override void PaintOver(PaintEventArgs e, CheckState state)
        {
            Graphics g = e.Graphics;
            if (Control.Appearance == Appearance.Button)
            {
                ButtonPopupAdapter adapter = new ButtonPopupAdapter(Control);
                adapter.PaintOver(e, Control.CheckState);
            }
            else
            {
                ColorData colors = PaintPopupRender(e.Graphics).Calculate();
                LayoutData layout = PaintPopupLayout(e, true).Layout();

                Region original = e.Graphics.Clip;
                PaintButtonBackground(e, Control.ClientRectangle, null);

                PaintImage(e, layout);

                DrawCheckBackground(e, layout.checkBounds, colors.windowText, colors.options.highContrast ? colors.buttonFace : colors.highlight, true, colors);
                DrawPopupBorder(g, layout.checkBounds, colors);
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
                LayoutData layout = PaintPopupLayout(e, true).Layout();

                Region original = e.Graphics.Clip;
                PaintButtonBackground(e, Control.ClientRectangle, null);

                PaintImage(e, layout);

                DrawCheckBackground(e, layout.checkBounds, colors.windowText, colors.buttonFace, true, colors);
                DrawPopupBorder(g, layout.checkBounds, colors);
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
            LayoutOptions layout = PaintPopupLayout(e, /* up = */ true);
            Debug.Assert(layout.GetPreferredSizeCore(LayoutUtils.MaxSize)
                == PaintPopupLayout(e, /* up = */ false).GetPreferredSizeCore(LayoutUtils.MaxSize),
                "The state of show3D should not effect PreferredSize");
            return layout;
        }

        internal static LayoutOptions PaintPopupLayout(Graphics g, bool show3D, int checkSize, Rectangle clientRectangle, Padding padding,
                                                       bool isDefault, Font font, string text, bool enabled, ContentAlignment textAlign, RightToLeft rtl,
                                                       Control control = null)
        {
            LayoutOptions layout = CommonLayout(clientRectangle, padding, isDefault, font, text, enabled, textAlign, rtl);
            layout.shadowedText = false;
            if (show3D)
            {
                layout.checkSize = (int)(checkSize * GetDpiScaleRatio(g, control) + 1);
            }
            else
            {
                layout.checkSize = (int)(checkSize * GetDpiScaleRatio(g, control));
                layout.checkPaddingSize = 1;
            }
            return layout;
        }

        private LayoutOptions PaintPopupLayout(PaintEventArgs e, bool show3D)
        {
            LayoutOptions layout = CommonLayout();
            layout.shadowedText = false;
            if (show3D)
            {
                layout.checkSize = (int)(flatCheckSize * GetDpiScaleRatio(e.Graphics) + 1);
            }
            else
            {
                layout.checkSize = (int)(flatCheckSize * GetDpiScaleRatio(e.Graphics));
                layout.checkPaddingSize = 1;
            }
            return layout;
        }

        #endregion
    }
}
