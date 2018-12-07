// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace WinForms.Common.Tests
{
    public static class CommonTestHelper
    {
        // helper method to generate theory data from all values of an enum type
        internal static TheoryData<T> GetEnumTheoryData<T>() where T : Enum
        {
            var data = new TheoryData<T>();
            foreach (T item in Enum.GetValues(typeof(T)))
                data.Add(item);
            return data;
        }

        // helper method to generate invalid theory data for an enum type
        // This method assumes that int.MinValue and int.MaxValue are not in the enum
        internal static TheoryData<T> GetEnumTheoryDataInvalid<T>() where T : Enum
        {
            var data = new TheoryData<T>();
            // This boxing is necessary because you can't cast an int to a generic,
            // even if the generic is guaranteed to be an enum
            data.Add((T)(object)int.MinValue);
            data.Add((T)(object)int.MaxValue);
            return data;
        }

        #region Primitives

        // helper method to generate theory data for all values of a boolean
        internal static TheoryData<bool> GetBoolTheoryData()
        {
            var data = new TheoryData<bool>();
            data.Add(true);
            data.Add(false);
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<int> GetIntTheoryData()
        {
            var data = new TheoryData<int>();
            data.Add(int.MinValue);
            data.Add(int.MaxValue);
            data.Add(0);
            data.Add(1);
            data.Add(-1);
            data.Add(int.MaxValue / 2);
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<uint> GetUIntTheoryData()
        {
            var data = new TheoryData<uint>();
            data.Add(int.MaxValue);
            data.Add(0);
            data.Add(1);
            data.Add(int.MaxValue / 2);
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<int> GetNIntTheoryData()
        {
            var data = new TheoryData<int>();
            data.Add(int.MinValue);
            data.Add(-1);
            data.Add(int.MinValue / 2);
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<float> GetFloatTheoryData()
        {
            var data = new TheoryData<float>();
            data.Add(float.MaxValue);
            data.Add(float.MinValue);
            data.Add(Single.Epsilon);
            data.Add(Single.Epsilon * -1);
            data.Add(Single.NegativeInfinity); // not sure about these two
            data.Add(Single.PositiveInfinity); // 2
            data.Add(0);
            data.Add(-1);
            data.Add(1);
            data.Add(float.MaxValue / 2);
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<float> GetUFloatTheoryData()
        {
            var data = new TheoryData<float>();
            data.Add(float.MaxValue);
            data.Add(Single.Epsilon);
            data.Add(Single.PositiveInfinity); // not sure about this one
            data.Add(0);
            data.Add(1);
            data.Add(float.MaxValue / 2);
            return data;
        }

        // helper method to generate theory data for a span of string values
        internal static TheoryData<string> GetStringTheoryData()
        {
            var data = new TheoryData<string>();
            data.Add(string.Empty);
            data.Add("reasonable");
            return data;
        }

        #endregion        
    }
}
