// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class maps an OLE_COLOR to a managed Color editor.
    /// </summary>
    internal class Com2ColorConverter : Com2DataTypeToManagedDataTypeConverter
    {
        public override Type ManagedType => typeof(Color);

        public override object ConvertNativeToManaged(object? nativeValue, Com2PropertyDescriptor pd)
        {
            int intVal = 0;

            // Get the integer value.
            if (nativeValue is uint nativeValueAsUint)
            {
                intVal = (int)nativeValueAsUint;
            }
            else if (nativeValue is int nativeValueAsInt)
            {
                intVal = nativeValueAsInt;
            }

            return ColorTranslator.FromOle(intVal);
        }

        public override object ConvertManagedToNative(object? managedValue, Com2PropertyDescriptor pd, ref bool cancelSet)
        {
            // Don't cancel the set.
            cancelSet = false;

            // We default to black.
            managedValue ??= Color.Black;

            if (managedValue is Color managedValueAsColor)
            {
                return ColorTranslator.ToOle(managedValueAsColor);
            }

            Debug.Fail($"Don't know how to set type:{managedValue.GetType().Name}");
            return 0;
        }
    }
}
