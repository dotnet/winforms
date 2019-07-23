// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    internal class TextBoxAutoCompleteSourceConverter : EnumConverter
    {
        public TextBoxAutoCompleteSourceConverter(Type type) : base(type)
        {
        }

        /// <summary>
        ///  Gets a collection of standard values for the data type this validator is
        ///  designed for.
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            StandardValuesCollection values = base.GetStandardValues(context);
            ArrayList list = new ArrayList();
            int count = values.Count;
            for (int i = 0; i < count; i++)
            {
                string currentItemText = values[i].ToString();
                if (!currentItemText.Equals("ListItems"))
                {
                    list.Add(values[i]);
                }
            }
            return new StandardValuesCollection(list);

        }
    }
}

