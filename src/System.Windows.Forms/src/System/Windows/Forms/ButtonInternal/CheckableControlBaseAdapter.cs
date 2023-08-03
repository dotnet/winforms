// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal;

/// <summary>
///  Common class for RadioButtonBaseAdapter and CheckBoxBaseAdapter
/// </summary>
internal abstract class CheckableControlBaseAdapter : ButtonBaseAdapter
{
    private const int StandardCheckSize = 13;
    private ButtonBaseAdapter? _buttonAdapter;

    internal CheckableControlBaseAdapter(ButtonBase control)
        : base(control)
    {
    }

    protected ButtonBaseAdapter ButtonAdapter
    {
        get
        {
            _buttonAdapter ??= CreateButtonAdapter();

            return _buttonAdapter;
        }
    }

    internal override Size GetPreferredSizeCore(Size proposedSize)
    {
        if (Appearance == Appearance.Button)
        {
            return ButtonAdapter.GetPreferredSizeCore(proposedSize);
        }

        LayoutOptions? options = default;
        using (var screen = GdiCache.GetScreenHdc())
        using (PaintEventArgs pe = new PaintEventArgs(screen, default))
        {
            options = Layout(pe);
        }

        return options.GetPreferredSizeCore(proposedSize);
    }

    protected abstract ButtonBaseAdapter CreateButtonAdapter();

    private Appearance Appearance
    {
        get
        {
            if (Control is CheckBox checkBox)
            {
                return checkBox.Appearance;
            }

            if (Control is RadioButton radioButton)
            {
                return radioButton.Appearance;
            }

            Debug.Fail($"Unexpected control type '{Control.GetType().FullName}'");
            return Appearance.Normal;
        }
    }

    internal override LayoutOptions CommonLayout()
    {
        LayoutOptions layout = base.CommonLayout();
        layout.GrowBorderBy1PxWhenDefault = false;
        layout.BorderSize = 0;
        layout.PaddingSize = 0;
        layout.MaxFocus = false;
        layout.FocusOddEvenFixup = true;
        layout.CheckSize = StandardCheckSize;
        return layout;
    }

    internal double GetDpiScaleRatio()
    {
        return GetDpiScaleRatio(Control);
    }

    internal static double GetDpiScaleRatio(Control? control)
    {
        if (DpiHelper.IsPerMonitorV2Awareness
            && control is not null && control.IsHandleCreated)
        {
            return control._deviceDpi / DpiHelper.LogicalDpi;
        }

        return DpiHelper.LogicalToDeviceUnitsScalingFactor;
    }
}
