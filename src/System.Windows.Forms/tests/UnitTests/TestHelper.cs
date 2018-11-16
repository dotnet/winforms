// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public static class TestHelper
    {
        // helper method to generate theory data from all values of an enum type
        public static TheoryData<T> GetEnumTheoryData<T>() where T : Enum
        {
            var data = new TheoryData<T>();
            foreach (T item in Enum.GetValues(typeof(T)))
                data.Add(item);
            return data;
        }

        // helper method to generate invalid theory data for an enum type
        // This method assumes that int.MinValue and int.MaxValue are not in the enum
        public static TheoryData<T> GetEnumTheoryDataInvalid<T>() where T : Enum
        {
            var data = new TheoryData<T>();
            // This boxing is necessary because you can't cast an int to a generic,
            // even if the generic is guaranteed to be an enum
            data.Add((T)(object)int.MinValue);
            data.Add((T)(object)int.MaxValue);
            return data;
        }

        // helper method to generate theory data for all values of a boolean
        public static TheoryData<bool> GetBoolTheoryData()
        {
            var data = new TheoryData<bool>();
            data.Add(true);
            data.Add(false);
            return data;
        }
    }
}
