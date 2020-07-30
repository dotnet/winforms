// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

using System.Drawing;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class maps an OLE_COLOR to a managed Color editor.
    /// </summary>
    internal class Com2ColorConverter : Com2DataTypeToManagedDataTypeConverter
    {
        /// <summary>
        ///  Returns the managed type that this editor maps the property type to.
        /// </summary>
        public override Type ManagedType
        {
            get
            {
                return typeof(Color);
            }
        }

        /// <summary>
        ///  Converts the native value into a managed value
        /// </summary>
        public override object ConvertNativeToManaged(object nativeValue, Com2PropertyDescriptor pd)
        {
            object baseValue = nativeValue;
            int intVal = 0;

            // get the integer value out of the native...
            //
            if (nativeValue is uint)
            {
                intVal = (int)(uint)nativeValue;
            }
            else if (nativeValue is int)
            {
                intVal = (int)nativeValue;
            }

            return ColorTranslator.FromOle(intVal);
        }

        /// <summary>
        ///  Converts the managed value into a native value
        /// </summary>
        public override object ConvertManagedToNative(object managedValue, Com2PropertyDescriptor pd, ref bool cancelSet)
        {
            // don't cancel the set
            cancelSet = false;

            // we default to black.
            //
            if (managedValue is null)
            {
                managedValue = Color.Black;
            }

            if (managedValue is Color)
            {
                return ColorTranslator.ToOle(((Color)managedValue));
            }
            Debug.Fail("Don't know how to set type:" + managedValue.GetType().Name);
            return 0;
        }
    }
}
