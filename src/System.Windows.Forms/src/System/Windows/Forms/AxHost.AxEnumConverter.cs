// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Windows.Forms.ComponentModel.Com2Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        /// <summary>
        ///  simple derivation of the com2enumconverter that allows us to intercept
        ///  the call to GetStandardValues so we can on-demand update the enum values.
        /// </summary>
        private class AxEnumConverter : Com2EnumConverter
        {
            private readonly AxPropertyDescriptor target;

            public AxEnumConverter(AxPropertyDescriptor target, Com2Enum com2Enum) : base(com2Enum)
            {
                this.target = target;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                // make sure the converter has been properly refreshed -- calling
                // the Converter property does this.
                //
                TypeConverter tc = this;
                tc = target.Converter;
                return base.GetStandardValues(context);
            }
        }
    }
}
