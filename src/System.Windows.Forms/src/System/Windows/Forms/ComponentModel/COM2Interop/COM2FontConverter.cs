// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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
        private Font _lastFont;

        public override bool AllowExpand => true;

        /// <summary>
        ///  Returns the managed type that this editor maps the property type to.
        /// </summary>
        public override Type ManagedType => typeof(Font);

        /// <summary>
        ///  Converts the native value into a managed value
        /// </summary>
        public override object ConvertNativeToManaged(object nativeValue, Com2PropertyDescriptor pd)
        {
            // we're getting an IFont here
            if (!(nativeValue is IFont nativeFont))
            {
                _lastHandle = IntPtr.Zero;
                _lastFont = Control.DefaultFont;
                return _lastFont;
            }

            IntPtr fontHandle = nativeFont.hFont;

            // see if we have this guy cached
            if (fontHandle == _lastHandle && _lastFont != null)
            {
                return _lastFont;
            }

            _lastHandle = fontHandle;

            try
            {
                // this wasn't working because it was converting everything to
                // world units.
                using (Font font = Font.FromHfont(_lastHandle))
                {
                    _lastFont = ControlPaint.FontInPoints(font);
                }
            }
            catch (ArgumentException)
            {
                // we will fail on non-truetype fonts, so
                // just use the default font.
                _lastFont = Control.DefaultFont;
            }

            return _lastFont;
        }

        /// <summary>
        ///  Converts the managed value into a native value
        /// </summary>
        public override object ConvertManagedToNative(object managedValue, Com2PropertyDescriptor pd, ref bool cancelSet)
        {
            // we default to black.
            if (managedValue is null)
            {
                managedValue = Control.DefaultFont;
            }

            cancelSet = true;

            if (_lastFont != null && _lastFont.Equals(managedValue))
            {
                // don't do anything here.
                return null;
            }

            _lastFont = (Font)managedValue;
            IFont nativeFont = (IFont)pd.GetNativeValue(pd.TargetObject);

            // now, push all the values into the native side
            if (nativeFont != null)
            {
                bool changed = ControlPaint.FontToIFont(_lastFont, nativeFont);

                if (changed)
                {
                    // here, we want to pick up a new font from the handle
                    _lastFont = null;
                    ConvertNativeToManaged(nativeFont, pd);
                }
            }

            return null;
        }
    }
}
