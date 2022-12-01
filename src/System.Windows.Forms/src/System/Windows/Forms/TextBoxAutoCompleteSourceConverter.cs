// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    internal class TextBoxAutoCompleteSourceConverter : EnumConverter
    {
        public TextBoxAutoCompleteSourceConverter(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)]
            Type type) : base(type)
        {
        }

        /// <summary>
        ///  Gets a collection of standard values for the data type this validator is
        ///  designed for.
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        {
            StandardValuesCollection values = base.GetStandardValues(context);
            List<object> list = new();
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] is object currentItem)
                {
                    if (string.Equals(currentItem.ToString(), "ListItems"))
                    {
                        list.Add(currentItem);
                    }
                }
            }

            return new StandardValuesCollection(list);
        }
    }
}
