// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms.ComponentModel.Com2Interop;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    /// <summary>
    ///  Simple derivation of the com2enumconverter that allows us to intercept
    ///  the call to GetStandardValues so we can on-demand update the enum values.
    /// </summary>
    private class AxEnumConverter : Com2EnumConverter
    {
        private readonly AxPropertyDescriptor _target;

        public AxEnumConverter(AxPropertyDescriptor target, Com2Enum com2Enum)
            : base(com2Enum)
        {
            _target = target;
        }

        public override StandardValuesCollection? GetStandardValues(ITypeDescriptorContext? context)
        {
            // Make sure the converter has been properly refreshed by calling the Converter property.
            _ = _target.Converter;
            return base.GetStandardValues(context);
        }
    }
}
