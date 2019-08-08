// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    internal class SpecialFolderEnumConverter : EnumConverter
    {
        public SpecialFolderEnumConverter(Type type) : base(type)
        {
        }

        /// <summary>
        ///  Personal appears twice in type editor because its numeric value matches with MyDocuments.
        ///  This code filters out the duplicate value.
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            StandardValuesCollection values = base.GetStandardValues(context);
            var list = new ArrayList();
            int count = values.Count;
            bool personalSeen = false;
            for (int i = 0; i < count; i++)
            {
                if (values[i] is Environment.SpecialFolder &&
                   values[i].Equals(Environment.SpecialFolder.Personal))
                {
                    if (!personalSeen)
                    {
                        personalSeen = true;
                        list.Add(values[i]);
                    }
                }
                else
                {
                    list.Add(values[i]);
                }
            }

            return new StandardValuesCollection(list);
        }

        protected override IComparer Comparer => SpecialFolderEnumComparer.Default;

        private class SpecialFolderEnumComparer : IComparer
        {
            public static readonly SpecialFolderEnumComparer Default = new SpecialFolderEnumComparer();

            public int Compare(object a, object b)
            {
                return string.Compare(a.ToString(), b.ToString(), StringComparison.InvariantCulture);
            }
        }
    }
}