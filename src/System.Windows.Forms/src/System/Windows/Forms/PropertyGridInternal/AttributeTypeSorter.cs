// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal class AttributeTypeSorter : IComparer
    {
        private static IDictionary? s_typeIds;

        public int Compare(object? obj1, object? obj2)
        {
            Attribute? a1 = obj1 as Attribute;
            Attribute? a2 = obj2 as Attribute;

            if (a1 is null && a2 is null)
            {
                return 0;
            }
            else if (a1 is null)
            {
                return -1;
            }
            else if (a2 is null)
            {
                return 1;
            }

            return string.Compare(AttributeTypeSorter.GetTypeIdString(a1), AttributeTypeSorter.GetTypeIdString(a2), false, CultureInfo.InvariantCulture);
        }

        private static string? GetTypeIdString(Attribute a)
        {
            string? result;
            object? typeId = a.TypeId;

            if (typeId is null)
            {
                Debug.Fail("Attribute '" + a.GetType().FullName + "' does not have a typeid.");
                return "";
            }

            if (s_typeIds is null)
            {
                s_typeIds = new Hashtable();
                result = null;
            }
            else
            {
                result = s_typeIds[typeId] as string;
            }

            if (result is null)
            {
                result = typeId.ToString();
                s_typeIds[typeId] = result;
            }

            return result;
        }
    }
}
