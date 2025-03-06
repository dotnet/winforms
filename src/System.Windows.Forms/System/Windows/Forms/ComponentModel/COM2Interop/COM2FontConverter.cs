// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  This class maps an IFont to a managed Font.
/// </summary>
internal sealed unsafe class Com2FontConverter : Com2DataTypeToManagedDataTypeConverter
{
    private HFONT _lastHandle = HFONT.Null;
    private Font? _lastFont;

    public override bool AllowExpand => true;

    public override Type ManagedType => typeof(Font);

    public override object? ConvertNativeToManaged(VARIANT nativeValue, Com2PropertyDescriptor property)
    {
        if (nativeValue.Type != VARENUM.VT_UNKNOWN)
        {
            Debug.Assert(nativeValue.IsEmpty, $"Expected IUnknown, got {nativeValue.Type}");
            return null;
        }

        using var iFont = ComScope<IFont>.TryQueryFrom((IUnknown*)nativeValue, out HRESULT hr);
        if (hr.Failed)
        {
            Debug.Fail($"Failed to get IFont: {hr}");
            _lastHandle = HFONT.Null;
            _lastFont = Control.DefaultFont;
            return _lastFont;
        }

        HFONT fontHandle = iFont.Value->hFont;

        // Do we have this cached?
        if (fontHandle == _lastHandle && _lastFont is not null)
        {
            return _lastFont;
        }

        _lastHandle = fontHandle;

        try
        {
            // This wasn't working because it was converting everything to world units.
            using Font font = Font.FromHfont(_lastHandle);
            _lastFont = ControlPaint.FontInPoints(font);
        }
        catch (ArgumentException)
        {
            // We will fail on non-truetype fonts, so just use the default font.
            _lastFont = Control.DefaultFont;
        }

        return _lastFont;
    }

    public override VARIANT ConvertManagedToNative(object? managedValue, Com2PropertyDescriptor property, ref bool cancelSet)
    {
        managedValue ??= Control.DefaultFont;

        // We never set the value back as we can (and should) just modify the original IFont handle.
        cancelSet = true;

        if (_lastFont is not null && _lastFont.Equals(managedValue))
        {
            return VARIANT.Empty;
        }

        _lastFont = (Font)managedValue;

        using VARIANT nativeValue = property.GetNativeValue(property.TargetObject);
        if (nativeValue.vt != VARENUM.VT_UNKNOWN)
        {
            Debug.Assert(nativeValue.IsEmpty, $"Expected IUnknown, got {nativeValue.Type}");
            return VARIANT.Empty;
        }

        using var font = ComScope<IFont>.TryQueryFrom((IUnknown*)nativeValue, out HRESULT hr);
        if (hr.Succeeded)
        {
            // Apply any differences back to the origianl IFont handle
            ApplyFontSettings(_lastFont, font, out bool targetChanged);

            if (targetChanged)
            {
                // Clear the cached Font
                _lastFont = null;
            }
        }
        else
        {
            Debug.Fail($"Failed to get IFont: {hr}");
        }

        return VARIANT.Empty;
    }

    private static void ApplyFontSettings(Font source, IFont* target, out bool targetChanged)
    {
        targetChanged = false;

        // We need to go through all the pain of the diff here because it looks like setting them all has different
        // results based on the order and each individual IFont implementor.

        if (!source.Name.Equals(target->Name.ToStringAndFree()))
        {
            target->Name = new(source.Name);
            targetChanged = true;
        }

        if (source.SizeInPoints != (float)target->Size)
        {
            target->Size = (CY)source.SizeInPoints;
            targetChanged = true;
        }

        LOGFONTW logfont = source.ToLogicalFont();

        if (target->Weight != (short)logfont.lfWeight)
        {
            target->Weight = (short)logfont.lfWeight;
            targetChanged = true;
        }

        bool isBold = logfont.lfWeight >= (int)FW.BOLD;
        if (target->Bold != isBold)
        {
            target->Bold = isBold;
            targetChanged = true;
        }

        bool isItalic = logfont.lfItalic != 0;
        if (target->Italic != isItalic)
        {
            target->Italic = isItalic;
            targetChanged = true;
        }

        bool isUnderline = logfont.lfUnderline != 0;
        if (target->Underline != isUnderline)
        {
            target->Underline = isUnderline;
            targetChanged = true;
        }

        bool isStrike = logfont.lfStrikeOut != 0;
        if (target->Strikethrough != isStrike)
        {
            target->Strikethrough = isStrike;
            targetChanged = true;
        }

        if (target->Charset != (short)logfont.lfCharSet)
        {
            target->Charset = (short)logfont.lfCharSet;
            targetChanged = true;
        }
    }
}
