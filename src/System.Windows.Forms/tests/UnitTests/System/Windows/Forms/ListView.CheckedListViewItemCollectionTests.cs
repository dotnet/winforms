// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListView_CheckedListViewItemCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CheckedListViewItemCollection_Ctor_OwnerIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("owner", () => { new ListView.CheckedListViewItemCollection(null); });
        }
    }
}
