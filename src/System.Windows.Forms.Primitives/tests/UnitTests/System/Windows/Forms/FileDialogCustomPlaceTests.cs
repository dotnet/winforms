// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class FileDialogCustomPlaceTests
    {
        public static TheoryData<string?> GetStringWithNullTheoryData()
        {
            var data = new TheoryData<string?>
            {
                null,
                string.Empty,
                "reasonable"
            };
            return data;
        }

        [Theory]
        [MemberData(nameof(GetStringWithNullTheoryData))]
        public void FileDialogCustomPlace_Ctor_String(string? path)
        {
            var place = new FileDialogCustomPlace(path);
            Assert.Equal(Guid.Empty, place.KnownFolderGuid);
            Assert.Same(path ?? string.Empty, place.Path);
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

        [Theory]
        [MemberData(nameof(GetGuidTheoryData))]
        public void FileDialogCustomPlace_Ctor_Guid(Guid knownFolderGuid)
        {
            var place = new FileDialogCustomPlace(knownFolderGuid);
            Assert.Equal(knownFolderGuid, place.KnownFolderGuid);
            Assert.Empty(place.Path);
        }

        [Theory]
        [MemberData(nameof(GetGuidTheoryData))]
        public void FileDialogCustomPlace_KnownFolderGuid_Set_GetReturnsExpected(Guid value)
        {
            var place = new FileDialogCustomPlace("path")
            {
                KnownFolderGuid = value
            };
            Assert.Equal(value, place.KnownFolderGuid);
            Assert.Empty(place.Path);

            // Set same.
            place.KnownFolderGuid = value;
            Assert.Equal(value, place.KnownFolderGuid);
            Assert.Empty(place.Path);
        }

        [Theory]
        [MemberData(nameof(GetStringWithNullTheoryData))]
        public void FileDialogCustomPlace_Path_Set_GetReturnsExpected(string? value)
        {
            var place = new FileDialogCustomPlace(Guid.NewGuid())
            {
                Path = value
            };
            Assert.Same(value ?? string.Empty, place.Path);
            Assert.Equal(Guid.Empty, place.KnownFolderGuid);

            // Set same.
            place.Path = value;
            Assert.Same(value ?? string.Empty, place.Path);
            Assert.Equal(Guid.Empty, place.KnownFolderGuid);
        }

        public static IEnumerable<object[]> ToString_TestData()
        {
            yield return new object[] { new FileDialogCustomPlace("path"), "System.Windows.Forms.FileDialogCustomPlace Path: path KnownFolderGuid: 00000000-0000-0000-0000-000000000000" };
            yield return new object[] { new FileDialogCustomPlace(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)), "System.Windows.Forms.FileDialogCustomPlace Path:  KnownFolderGuid: 00000001-0002-0003-0405-060708090a0b" };
        }

        [Theory]
        [MemberData(nameof(ToString_TestData))]
        public void FileDialogCustomPlace_ToString_Invoke_ReturnsExpected(FileDialogCustomPlace place, string expected)
        {
            Assert.Equal(expected, place.ToString());
        }
    }
}
