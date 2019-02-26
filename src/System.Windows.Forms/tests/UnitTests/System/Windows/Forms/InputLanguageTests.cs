// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class InputLanguageTests
    {
        [Fact]
        public void InputLanguage_DefaultInputLanguage_Get_ReturnsExpected()
        {
            InputLanguage language = InputLanguage.DefaultInputLanguage;
            Assert.NotSame(language, InputLanguage.DefaultInputLanguage);
            Assert.NotEqual(IntPtr.Zero, language.Handle);
            Assert.NotNull(language.Culture);
        }

        [Fact]
        public void InputLanguage_CurrentInputLanguage_Get_ReturnsExpected()
        {
            InputLanguage language = InputLanguage.CurrentInputLanguage;
            Assert.NotSame(language, InputLanguage.CurrentInputLanguage);
            Assert.NotEqual(IntPtr.Zero, language.Handle);
            Assert.NotNull(language.Culture);
        }

        [Fact]
        public void InputLanguage_CurrentInputLanguage_Set_GetReturnsExpected()
        {
            InputLanguage language = InputLanguage.CurrentInputLanguage;
            try
            {
                // Set null.
                InputLanguage.CurrentInputLanguage = null;
                Assert.Equal(InputLanguage.DefaultInputLanguage, InputLanguage.CurrentInputLanguage);

                // Set other.
                InputLanguage.CurrentInputLanguage = language;
                Assert.Equal(language, InputLanguage.CurrentInputLanguage);
            }
            catch
            {
                InputLanguage.CurrentInputLanguage = language;
            }
        }

        public static IEnumerable<object[]> Equals_TestData()
        {
            yield return new object[] { InputLanguage.DefaultInputLanguage, InputLanguage.DefaultInputLanguage, true };
            yield return new object[] { InputLanguage.DefaultInputLanguage, new object(), false };
            yield return new object[] { InputLanguage.DefaultInputLanguage, null, false };
        }

        [Theory]
        [MemberData(nameof(Equals_TestData))]
        public void InputLanguage_Equals_Invoke_ReturnsExpected(InputLanguage language, object value, bool expected)
        {
            Assert.Equal(expected, language.Equals(value));
        }

        [Fact]
        public void InputLanguage_FromCulture_NullCulture_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("culture", () => InputLanguage.FromCulture(null));
        }
    }
}
