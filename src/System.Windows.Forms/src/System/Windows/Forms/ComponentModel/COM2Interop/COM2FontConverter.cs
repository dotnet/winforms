// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class maps an IFont to a managed Font.
    /// </summary>
    internal class Com2FontConverter : Com2DataTypeToManagedDataTypeConverter
    {
        private HFONT _lastHandle = HFONT.Null;
        private Font? _lastFont;

        public override bool AllowExpand => true;

        public override Type ManagedType => typeof(Font);

        public override object ConvertNativeToManaged(object? nativeValue, Com2PropertyDescriptor pd)
        {
            if (nativeValue is not IFont.Interface nativeFont)
            {
                _lastHandle = HFONT.Null;
                _lastFont = Control.DefaultFont;
                return _lastFont;
            }

            HFONT fontHandle = nativeFont.hFont;

            // Do we have this cached?
            if (fontHandle == _lastHandle && _lastFont is not null)
            {
                return _lastFont;
            }

            _lastHandle = fontHandle;

            try
            {
                // This wasn't working because it was converting everything to world units.
                using (Font font = Font.FromHfont(_lastHandle))
                {
                    _lastFont = ControlPaint.FontInPoints(font);
                }
            }
            catch (ArgumentException)
            {
                // We will fail on non-truetype fonts, so just use the default font.
                _lastFont = Control.DefaultFont;
            }

            return _lastFont;
        }

        public override object? ConvertManagedToNative(object? managedValue, Com2PropertyDescriptor pd, ref bool cancelSet)
        {
            managedValue ??= Control.DefaultFont;

            // We never set the object back as we have a modifiable IFont handle.
            cancelSet = true;

            if (_lastFont is not null && _lastFont.Equals(managedValue))
            {
                return null;
            }

            _lastFont = (Font)managedValue;

            if (pd.GetNativeValue(pd.TargetObject) is IFont.Interface nativeFont)
            {
                // Apply any differences back to the IFont handle
                ApplyFontSettings(_lastFont, nativeFont, out bool targetChanged);

                if (targetChanged)
                {
                    _lastFont = null;
                    ConvertNativeToManaged(nativeFont, pd);
                }
            }

            return null;
        }

        private static void ApplyFontSettings(Font source, IFont.Interface target, out bool targetChanged)
        {
            targetChanged = false;

            // We need to go through all the pain of the diff here because it looks like setting them all has different
            // results based on the order and each individual IFont implementor.

            if (!source.Name.Equals(target.Name.ToStringAndFree()))
            {
                target.Name = new(source.Name);
                targetChanged = true;
            }

            if (source.SizeInPoints != (float)target.Size)
            {
                target.Size = (CY)source.SizeInPoints;
                targetChanged = true;
            }

            LOGFONTW logfont = LOGFONTW.FromFont(source);

            if (target.Weight != (short)logfont.lfWeight)
            {
                target.Weight = (short)logfont.lfWeight;
                targetChanged = true;
            }

            bool isBold = logfont.lfWeight >= (int)FW.BOLD;
            if (target.Bold != isBold)
            {
                target.Bold = isBold;
                targetChanged = true;
            }

            bool isItalic = logfont.lfItalic != 0;
            if (target.Italic != isItalic)
            {
                target.Italic = isItalic;
                targetChanged = true;
            }

            bool isUnderline = logfont.lfUnderline != 0;
            if (target.Underline != isUnderline)
            {
                target.Underline = isUnderline;
                targetChanged = true;
            }

            bool isStrike = logfont.lfStrikeOut != 0;
            if (target.Strikethrough != isStrike)
            {
                target.Strikethrough = isStrike;
                targetChanged = true;
            }

            if (target.Charset != (short)logfont.lfCharSet)
            {
                target.Charset = (short)logfont.lfCharSet;
                targetChanged = true;
            }
        }
    }
}
