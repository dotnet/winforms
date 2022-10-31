// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop.Ole32;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class maps an OLE_COLOR to a managed Color editor.
    /// </summary>
    internal class Com2FontConverter : Com2DataTypeToManagedDataTypeConverter
    {
        private IntPtr _lastHandle = IntPtr.Zero;
        private Font? _lastFont;

        public override bool AllowExpand => true;

        public override Type ManagedType => typeof(Font);

        public override object ConvertNativeToManaged(object? nativeValue, Com2PropertyDescriptor pd)
        {
            if (nativeValue is not IFont nativeFont)
            {
                _lastHandle = IntPtr.Zero;
                _lastFont = Control.DefaultFont;
                return _lastFont;
            }

            IntPtr fontHandle = nativeFont.hFont;

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

            cancelSet = true;

            if (_lastFont is not null && _lastFont.Equals(managedValue))
            {
                return null;
            }

            _lastFont = (Font)managedValue;

            if (pd.GetNativeValue(pd.TargetObject) is IFont nativeFont)
            {
                bool changed = ControlPaint.FontToIFont(_lastFont, nativeFont);

                if (changed)
                {
                    _lastFont = null;
                    ConvertNativeToManaged(nativeFont, pd);
                }
            }

            return null;
        }
    }
}
