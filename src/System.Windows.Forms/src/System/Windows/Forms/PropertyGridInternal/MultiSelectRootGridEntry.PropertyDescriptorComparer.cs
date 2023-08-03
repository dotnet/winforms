// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class MultiSelectRootGridEntry
{
    private class PropertyDescriptorComparer : IComparer
    {
        public int Compare(object? obj1, object? obj2)
        {
            var a1 = obj1 as PropertyDescriptor;
            var a2 = obj2 as PropertyDescriptor;

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

            int result = string.Compare(a1.Name, a2.Name, false, CultureInfo.InvariantCulture);

            if (result == 0)
            {
                result = string.Compare(a1.PropertyType.FullName, a2.PropertyType.FullName, true, CultureInfo.CurrentCulture);
            }

            return result;
        }
    }
}
