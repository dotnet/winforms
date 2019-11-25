// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Moq;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;

namespace System.Windows.Forms.Design.Tests
{
    public class BaseContextMenuStripTests
    {
        [Fact]
        public void BaseContextMenuStrip_Ctor()
        {
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Default);
            mockServiceProvider
                .Setup(s => s.GetService(typeof(IUIService)))
                .Returns(null);

            BaseContextMenuStrip baseContextMenuStrip = new BaseContextMenuStrip(mockServiceProvider.Object, new Component());

            Assert.NotNull(baseContextMenuStrip);
        }

        [Fact]
        public void BaseContextMenuStrip_RefreshItems()
        {
            var mockDictionary = new Dictionary<string, object>();
            mockDictionary.Add("DialogFont", new Font("Serif", 1.0f, FontStyle.Regular));

            var mockUIService = new Mock<IUIService>(MockBehavior.Default);
            mockUIService
                .Setup(s => s.Styles)
                .Returns(mockDictionary);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Default);
            mockServiceProvider
                .Setup(s => s.GetService(typeof(IUIService)))
                .Returns(null);

            BaseContextMenuStrip baseContextMenuStrip = new BaseContextMenuStrip(mockServiceProvider.Object, new Component());

            try
            {
                baseContextMenuStrip.RefreshItems();
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }
    }
}
