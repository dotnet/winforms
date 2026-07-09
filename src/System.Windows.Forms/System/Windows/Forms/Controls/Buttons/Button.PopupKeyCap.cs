// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Rendering.Button;

namespace System.Windows.Forms;

public partial class Button
{
    private AnimatedPopupButtonRenderer? _popupKeyCapRenderer;

    private AnimatedPopupButtonRenderer PopupKeyCapRenderer =>
        _popupKeyCapRenderer ??= new AnimatedPopupButtonRenderer(this);

    /// <summary>
    ///  Gets a value indicating whether this button paints with the animated, concave key-cap renderer used for
    ///  <see cref="FlatStyle.Popup"/> under modern visual styles or dark mode.
    /// </summary>
    internal bool IsPopupKeyCapAppearance =>
        FlatStyle == FlatStyle.Popup
            && (Application.IsDarkModeEnabled || VisualStylesMode >= VisualStylesMode.Net11);

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs pevent)
    {
        if (IsPopupKeyCapAppearance)
        {
            // Owner-paint the key cap directly (bypassing the button adapters) so its hover/press animation can
            // be driven continuously by the shared animation manager, mirroring the CheckBox toggle switch.
            using GraphicsStateScope scope = new(pevent.Graphics);
            PopupKeyCapRenderer.RenderControl(pevent.Graphics);
            return;
        }

        base.OnPaint(pevent);
    }

    /// <inheritdoc/>
    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        base.OnMouseDown(mevent);
        UpdatePopupKeyCapInteractionState();
    }

    private void UpdatePopupKeyCapInteractionState()
    {
        if (IsPopupKeyCapAppearance)
        {
            PopupKeyCapRenderer.SetInteractionState(MouseIsOver, MouseIsDown);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _popupKeyCapRenderer?.Dispose();
            _popupKeyCapRenderer = null;
        }

        base.Dispose(disposing);
    }
}
