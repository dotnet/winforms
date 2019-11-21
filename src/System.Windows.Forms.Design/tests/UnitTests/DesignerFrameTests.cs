// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Xunit;
using Moq;

namespace System.Windows.Forms.Design.Tests
{
    public class DesignerFrameTests
    {
        static private DesignerFrame CreateDesignerFrame()
        {
            var mockDictionary = new Dictionary<string, Color>();
            mockDictionary.Add("ArtboardBackground", Color.White);

            var mockIUIService = new Mock<IUIService>(MockBehavior.Default);
            mockIUIService
                .Setup(s => s.Styles)
                .Returns(mockDictionary);

            var mockSite = new Mock<ISite>(MockBehavior.Default);
            mockSite
                .Setup(s => s.GetService(typeof(IUIService)))
                .Returns(mockIUIService.Object);

            return new DesignerFrame(mockSite.Object);
        }

        [WinFormsFact]
        public void DesignerFrame_Ctor_Default()
        {
            DesignerFrame designerFrame = CreateDesignerFrame();

            Assert.NotNull(designerFrame);

            designerFrame.Dispose();
        }

        [WinFormsFact]
        public void DesignerFrame_Initialize()
        {
            DesignerFrame designerFrame = CreateDesignerFrame();

            try
            {
                designerFrame.Initialize(new Control());
            }
            catch(Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }

            Assert.NotNull(designerFrame);

            designerFrame.Dispose();
        }
    }
}
