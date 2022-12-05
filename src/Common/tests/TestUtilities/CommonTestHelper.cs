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
            var data = new TheoryData<Enum>();
            foreach (Enum item in Enum.GetValues(enumType))
            {
                data.Add(item);
            }

            return data;
        }

        public static TheoryData GetEnumTypeTheoryDataWitIdentity(Type enumType)
        {
            int counter = 0;
            var data = new TheoryData<string, int>();
            foreach (Enum item in Enum.GetValues(enumType))
            {
                data.Add(item.ToString(), counter++);
                data.Add(((int)(object)item).ToString(), counter++);
            }

            return data;
        }

        public static TheoryData<Enum> GetEnumTypeTheoryDataInvalid(Type enumType)
        {
            var data = new TheoryData<Enum>();
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
            var data = new TheoryData<Enum>();
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
        {
            var data = new TheoryData<bool>
            {
                true,
                false
            };
            return data;
        }

        // helper method to generate theory data for some values of a int
        public static TheoryData<int> GetIntTheoryData()
        {
            var data = new TheoryData<int>
            {
                int.MinValue,
                int.MaxValue,
                0,
                1,
                -1,
                int.MaxValue / 2
            };
            return data;
        }

        public static TheoryData<int> GetNonNegativeIntTheoryData()
        {
            var data = new TheoryData<int>
            {
                int.MaxValue,
                0,
                1,
                int.MaxValue / 2
            };
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<uint> GetUIntTheoryData()
        {
            var data = new TheoryData<uint>
            {
                int.MaxValue,
                0,
                1,
                int.MaxValue / 2
            };
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<int> GetNIntTheoryData()
        {
            var data = new TheoryData<int>
            {
                int.MinValue,
                -1,
                int.MinValue / 2
            };
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<float> GetFloatTheoryData()
        {
            var data = new TheoryData<float>
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
            return data;
        }

        // helper method to generate theory data for some values of a int
        internal static TheoryData<float> GetUFloatTheoryData()
        {
            var data = new TheoryData<float>
            {
                float.MaxValue,
                float.Epsilon,
                float.PositiveInfinity, // not sure about this one
                0,
                1,
                float.MaxValue / 2
            };
            return data;
        }

        // helper method to generate theory data for a span of string values
        private const string reasonable = nameof(reasonable);

        public static TheoryData<string> GetStringTheoryData()
        {
            var data = new TheoryData<string>
            {
                string.Empty,
                reasonable
            };
            return data;
        }

        public static TheoryData<string?> GetStringWithNullTheoryData()
        {
            var data = new TheoryData<string?>
            {
                null,
                string.Empty,
                reasonable
            };
            return data;
        }

        public static TheoryData<string?> GetNullOrEmptyStringTheoryData()
        {
            var data = new TheoryData<string?>
            {
                null,
                string.Empty
            };
            return data;
        }

        public static TheoryData<string?, string> GetStringNormalizedTheoryData()
        {
            var data = new TheoryData<string?, string>
            {
                { null, string.Empty },
                { string.Empty, string.Empty },
                { reasonable, reasonable }
            };
            return data;
        }

        public static TheoryData<string, string, int> GetCtrlBackspaceData()
        {
            var data = new TheoryData<string, string, int>
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
            return data;
        }

        public static TheoryData<string, string, int> GetCtrlBackspaceRepeatedData()
        {
            var data = new TheoryData<string, string, int>
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
            return data;
        }

        public static TheoryData<char> GetCharTheoryData()
        {
            var data = new TheoryData<char>
            {
                '\0',
                'a'
            };
            return data;
        }

        public static TheoryData<IntPtr> GetIntPtrTheoryData()
        {
            var data = new TheoryData<IntPtr>
            {
                (IntPtr)(-1),
                IntPtr.Zero,
                (IntPtr)1
            };
            return data;
        }

        public static TheoryData<Guid> GetGuidTheoryData()
        {
            var data = new TheoryData<Guid>
            {
                Guid.Empty,
                Guid.NewGuid()
            };
            return data;
        }

        public static TheoryData<Color> GetColorTheoryData()
        {
            var data = new TheoryData<Color>
            {
                Color.Red,
                Color.Blue,
                Color.Black
            };
            return data;
        }

        public static TheoryData<Color> GetColorWithEmptyTheoryData()
        {
            var data = new TheoryData<Color>
            {
                Color.Red,
                Color.Empty
            };
            return data;
        }

        public static TheoryData<Point> GetPointTheoryData() => GetPointTheoryData(TestIncludeType.All);

        public static TheoryData<Point> GetPointTheoryData(TestIncludeType includeType)
        {
            var data = new TheoryData<Point>();
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
            var data = new TheoryData<Size>();
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
        {
            var data = new TheoryData<Size>
            {
                new Size(),
                new Size(1, 2)
            };
            return data;
        }

        public static TheoryData<Rectangle> GetRectangleTheoryData()
        {
            var data = new TheoryData<Rectangle>
            {
                new Rectangle(),
                new Rectangle(1, 2, 3, 4),
                new Rectangle(-1, -2, -3, -4)
            };
            return data;
        }

        public static TheoryData<Type?, bool> GetConvertFromTheoryData()
        {
            var data = new TheoryData<Type?, bool>
            {
                { typeof(bool), false },
                { typeof(InstanceDescriptor), true },
                { typeof(int), false },
                { typeof(double), false },
                { null, false }
            };
            return data;
        }

        public static TheoryData<EventArgs?> GetEventArgsTheoryData()
        {
            var data = new TheoryData<EventArgs?>
            {
                null,
                new EventArgs()
            };
            return data;
        }

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
