// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design.Serialization;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.TestUtilities
{
    public static class CommonTestHelper
    {
        public static TheoryData GetEnumTypeTheoryData(Type enumType)
        {
            TheoryData<Enum> data = new();
            foreach (Enum item in Enum.GetValues(enumType))
            {
                data.Add(item);
            }

            return data;
        }

        public static TheoryData GetEnumTypeTheoryDataWitIdentity(Type enumType)
        {
            int counter = 0;
            TheoryData<string, int> data = new();
            foreach (Enum item in Enum.GetValues(enumType))
            {
                data.Add(item.ToString(), counter++);
                data.Add(((int)(object)item).ToString(), counter++);
            }

            return data;
        }

        public static TheoryData<Enum> GetEnumTypeTheoryDataInvalid(Type enumType)
        {
            TheoryData<Enum> data = new();
            Enum[] values = Enum.GetValues(enumType).Cast<Enum>().OrderBy(p => p).Distinct().ToArray();

            for (int i = 0; i < values.Length - 2; i++)
            {
                int currentVal = Convert.ToInt32(values[i]);
                int nextVal = Convert.ToInt32(values[i + 1]);
                if (nextVal != currentVal + 1)
                {
                    // Not sequential.
                    data.Add((Enum)Enum.ToObject(enumType, currentVal + 1));

                    if (nextVal - 1 != currentVal)
                    {
                        data.Add((Enum)Enum.ToObject(enumType, nextVal - 1));
                    }
                }
            }

            data.Add((Enum)Enum.ToObject(enumType, Convert.ToInt32(values.Min()) - 1));
            data.Add((Enum)Enum.ToObject(enumType, Convert.ToInt32(values.Max()) + 1));
            return data;
        }

        public static TheoryData<Enum> GetEnumTypeTheoryDataInvalidMasked(Type enumType)
        {
            TheoryData<Enum> data = new();
            IEnumerable<Enum> values = Enum.GetValues(enumType).Cast<Enum>().OrderBy(p => p);

            long allMasked = 0;
            foreach (Enum value in values)
            {
                allMasked |= Convert.ToInt64(value);
            }

            data.Add((Enum)Enum.ToObject(enumType, int.MaxValue));
            data.Add((Enum)Enum.ToObject(enumType, allMasked + 1));
            return data;
        }

        #region Primitives

        // helper method to generate theory data for all values of a boolean
        public static TheoryData<bool> GetBoolTheoryData()
            => new()
            {
                true,
                false
            };

        // helper method to generate theory data for some values of a int
        public static TheoryData<int> GetIntTheoryData()
            => new()
            {
                int.MinValue,
                int.MaxValue,
                0,
                1,
                -1,
                int.MaxValue / 2
            };

        public static TheoryData<int> GetNonNegativeIntTheoryData()
            => new()
            {
                int.MaxValue,
                0,
                1,
                int.MaxValue / 2
            };

        // helper method to generate theory data for some values of a int
        internal static TheoryData<uint> GetUIntTheoryData()
            => new()
            {
                int.MaxValue,
                0,
                1,
                int.MaxValue / 2
            };

        // helper method to generate theory data for some values of a int
        internal static TheoryData<int> GetNIntTheoryData()
            => new()
            {
                int.MinValue,
                -1,
                int.MinValue / 2
            };

        // helper method to generate theory data for some values of a int
        internal static TheoryData<float> GetFloatTheoryData()
            => new()
            {
                float.MaxValue,
                float.MinValue,
                float.Epsilon,
                float.Epsilon * -1,
                float.NegativeInfinity, // not sure about these two
                float.PositiveInfinity, // 2
                0,
                -1,
                1,
                float.MaxValue / 2
            };

        // helper method to generate theory data for some values of a int
        internal static TheoryData<float> GetUFloatTheoryData()
            => new()
            {
                float.MaxValue,
                float.Epsilon,
                float.PositiveInfinity, // not sure about this one
                0,
                1,
                float.MaxValue / 2
            };

        // helper method to generate theory data for a span of string values
        private const string reasonable = nameof(reasonable);

        public static TheoryData<string> GetStringTheoryData()
            => new()
            {
                string.Empty,
                reasonable
            };

        public static TheoryData<string?> GetStringWithNullTheoryData()
            => new()
            {
                null,
                string.Empty,
                reasonable
            };

        public static TheoryData<string?> GetNullOrEmptyStringTheoryData()
            => new()
            {
                null,
                string.Empty
            };

        public static TheoryData<string?, string> GetStringNormalizedTheoryData()
            => new()
            {
                { null, string.Empty },
                { string.Empty, string.Empty },
                { reasonable, reasonable }
            };

        public static TheoryData<string, string, int> GetCtrlBackspaceData()
            => new()
            {
                { "aaa", "", 0 },
                { "---", "", 0 },
                { " aaa", "", 0 },
                { " ---", "", 0 },
                { "aaa---", "", 0 },
                { "---aaa", "---", 0 },
                { "aaa---aaa", "aaa---", 0 },
                { "---aaa---", "---", 0 },
                { "a-a", "a-", 0 },
                { "-a-", "", 0 },
                { "--a-", "--", 0 },
                { "abc", "c", -1 },
                { "a,1-b", "a,b", -1 }
            };

        public static TheoryData<string, string, int> GetCtrlBackspaceRepeatedData()
            => new()
            {
                { "aaa", "", 2 },
                { "---", "", 2 },
                { "aaa---aaa", "", 2 },
                { "---aaa---", "", 2 },
                { "aaa bbb", "", 2 },
                { "aaa bbb ccc", "aaa ", 2 },
                { "aaa --- ccc", "", 2 },
                { "1 2 3 4 5 6 7 8 9 0", "1 ", 9 }
            };

        public static TheoryData<char> GetCharTheoryData()
            => new()
            {
                '\0',
                'a'
            };

        public static TheoryData<IntPtr> GetIntPtrTheoryData()
            => new()
            {
                (IntPtr)(-1),
                IntPtr.Zero,
                (IntPtr)1
            };

        public static TheoryData<Guid> GetGuidTheoryData()
            => new()
            {
                Guid.Empty,
                Guid.NewGuid()
            };

        public static TheoryData<Color> GetColorTheoryData()
            => new()
            {
                Color.Red,
                Color.Blue,
                Color.Black
            };

        public static TheoryData<Color> GetColorWithEmptyTheoryData()
            => new()
            {
                Color.Red,
                Color.Empty
            };

        public static TheoryData<Point> GetPointTheoryData() => GetPointTheoryData(TestIncludeType.All);

        public static TheoryData<Point> GetPointTheoryData(TestIncludeType includeType)
        {
            TheoryData<Point> data = new();
            if (!includeType.HasFlag(TestIncludeType.NoPositives))
            {
                data.Add(new Point());
                data.Add(new Point(10));
                data.Add(new Point(1, 2));
            }

            if (!includeType.HasFlag(TestIncludeType.NoNegatives))
            {
                data.Add(new Point(int.MaxValue, int.MinValue));
                data.Add(new Point(-1, -2));
            }

            return data;
        }

        public static TheoryData<Size> GetSizeTheoryData() => GetSizeTheoryData(TestIncludeType.All);

        public static TheoryData<Size> GetSizeTheoryData(TestIncludeType includeType)
        {
            TheoryData<Size> data = new();
            if (!includeType.HasFlag(TestIncludeType.NoPositives))
            {
                data.Add(new Size());
                data.Add(new Size(new Point(1, 1)));
                data.Add(new Size(1, 2));
            }

            if (!includeType.HasFlag(TestIncludeType.NoNegatives))
            {
                data.Add(new Size(-1, 1));
                data.Add(new Size(1, -1));
            }

            return data;
        }

        public static TheoryData<Size> GetPositiveSizeTheoryData()
            => new()
            {
                new Size(),
                new Size(1, 2)
            };

        public static TheoryData<Rectangle> GetRectangleTheoryData()
            => new()
            {
                new Rectangle(),
                new Rectangle(1, 2, 3, 4),
                new Rectangle(-1, -2, -3, -4)
            };

        public static TheoryData<Type?, bool> GetConvertFromTheoryData()
            => new()
            {
                { typeof(bool), false },
                { typeof(InstanceDescriptor), true },
                { typeof(int), false },
                { typeof(double), false },
                { null, false }
            };

        public static TheoryData<EventArgs?> GetEventArgsTheoryData()
            => new()
            {
                null,
                new EventArgs()
            };

        #endregion
    }

    [Flags]
    public enum TestIncludeType
    {
        All,
        NoPositives,
        NoNegatives
    }
}
