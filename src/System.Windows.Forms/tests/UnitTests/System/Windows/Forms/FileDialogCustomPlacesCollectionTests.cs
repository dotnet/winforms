// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class FileDialogCustomPlacesCollectionTests
    {
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void FileDialogCustomPlacesCollection_Add_String_Success(string path)
        {
            var collection = new FileDialogCustomPlacesCollection
            {
                path
            };
            FileDialogCustomPlace place = Assert.Single(collection);
            Assert.Equal(Guid.Empty, place.KnownFolderGuid);
            Assert.Same(path ?? string.Empty, place.Path);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetGuidTheoryData))]
        public void FileDialogCustomPlacesCollection_Add_Guid_Success(Guid knownFolderGuid)
        {
            var collection = new FileDialogCustomPlacesCollection
            {
                knownFolderGuid
            };
            FileDialogCustomPlace place = Assert.Single(collection);
            Assert.Equal(knownFolderGuid, place.KnownFolderGuid);
            Assert.Empty(place.Path);
        }
    }
}
