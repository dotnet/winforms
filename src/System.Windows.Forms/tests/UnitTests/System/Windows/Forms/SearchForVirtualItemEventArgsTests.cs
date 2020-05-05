// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class SearchForVirtualItemEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Bool_Bool_Bool_String_Point_SearchDirectionHint_Int_TestData()
        {
            yield return new object[] { true, false, true, null, Point.Empty, (SearchDirectionHint)(SearchDirectionHint.Down + 1), -2 };
            yield return new object[] { false, true, false, "", new Point(1, 2), SearchDirectionHint.Down, -1 };
            yield return new object[] { false, true, false, "text", new Point(-1, -2), SearchDirectionHint.Down, 0 };
            yield return new object[] { false, true, false, "text", new Point(1, 2), SearchDirectionHint.Down, 1 };
        }

        [Theory]
        [MemberData(nameof(Ctor_Bool_Bool_Bool_String_Point_SearchDirectionHint_Int_TestData))]
        public void Ctor_Bool_Bool_Bool_String_Point_SearchDirectionHint_Int(bool isTextSearch, bool isPrefixSearch, bool includeSubItemsInSearch, string text, Point startingPoint, SearchDirectionHint direction, int startIndex)
        {
            var e = new SearchForVirtualItemEventArgs(isTextSearch, isPrefixSearch, includeSubItemsInSearch, text, startingPoint, direction, startIndex);
            Assert.Equal(isTextSearch, e.IsTextSearch);
            Assert.Equal(isPrefixSearch, e.IsPrefixSearch);
            Assert.Equal(includeSubItemsInSearch, e.IncludeSubItemsInSearch);
            Assert.Equal(text, e.Text);
            Assert.Equal(startingPoint, e.StartingPoint);
            Assert.Equal(direction, e.Direction);
            Assert.Equal(startIndex, e.StartIndex);
            Assert.Equal(-1, e.Index);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void Index_Set_GetReturnsExpected(int value)
        {
            var e = new SearchForVirtualItemEventArgs(false, true, false, "text", new Point(1, 2), SearchDirectionHint.Down, 1)
            {
                Index = value
            };
            Assert.Equal(value, e.Index);
        }
    }
}
