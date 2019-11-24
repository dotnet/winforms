// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;
using System.Drawing;
using Xunit;
using System.ComponentModel.Design;
using Moq;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using Microsoft.VisualBasic;
using System.Collections;
using static System.ComponentModel.Design.DesignerOptionService;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design.Tests
{
    public class DesignerUtilsTests
    {
        static string imageName = "System.Drawing.Design.Tests.16x16.bmp";

        private Image LoadImageAssembly()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageName))
            {
                return Image.FromStream(stream);
            }
        }

        [Fact]
        public void DesignerUtils_BoxImage()
        {
            Assert.NotNull(DesignerUtils.BoxImage);
        }

        [Fact]
        public void DesignerUtils_HoverBrush()
        {
            Assert.NotNull(DesignerUtils.HoverBrush);
        }

        [Fact]
        public void DesignerUtils_MinDragSize()
        {
            Assert.NotEqual(DesignerUtils.MinDragSize, Size.Empty);
        }

        [Fact]
        public void DesignerUtils_LastCursorPoint()
        {
            Assert.True(DesignerUtils.LastCursorPoint.X >= 0 && DesignerUtils.LastCursorPoint.Y >= 0);
        }

        [Fact]
        public void DesignerUtils_SyncBrushes()
        {
            try
            {
                DesignerUtils.SyncBrushes();
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DesignerUtils_DrawResizeBorder()
        {
            try
            {
                DesignerUtils.DrawResizeBorder(Graphics.FromImage(LoadImageAssembly()), new Region(), Color.White);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DesignerUtils_DrawFrame()
        {
            try
            {
                DesignerUtils.DrawFrame(Graphics.FromImage(LoadImageAssembly()), new Region(), FrameStyle.Thick, Color.White);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DesignerUtils_DrawGrabHandle()
        {
            try
            {
                DesignerUtils.DrawGrabHandle(Graphics.FromImage(LoadImageAssembly()), new Rectangle(1,1,1,1), true, null);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DesignerUtils_DrawNoResizeHandle()
        {
            try
            {
                DesignerUtils.DrawNoResizeHandle(Graphics.FromImage(LoadImageAssembly()), new Rectangle(1, 1, 1, 1), true, null);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DesignerUtils_DrawLockedHandle()
        {
            try
            {
                DesignerUtils.DrawLockedHandle(Graphics.FromImage(LoadImageAssembly()), new Rectangle(1, 1, 1, 1), true, null);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DesignerUtils_DrawSelectionBorder()
        {
            try
            {
                DesignerUtils.DrawSelectionBorder(Graphics.FromImage(LoadImageAssembly()), new Rectangle(1, 1, 1, 1));
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DesignerUtils_GenerateSnapShot()
        {
            Image image = LoadImageAssembly();

            try
            {
                DesignerUtils.GenerateSnapShot(new Control(), ref image, 1, 1.0, Color.White);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DesignerUtils_GetAdornmentDimensions()
        {
            Assert.Equal(new Size(DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE), 
                DesignerUtils.GetAdornmentDimensions(AdornmentType.GrabHandle));

            Assert.Equal(new Size(DesignerUtils.CONTAINERGRABHANDLESIZE, DesignerUtils.CONTAINERGRABHANDLESIZE), 
                DesignerUtils.GetAdornmentDimensions(AdornmentType.ContainerSelector));

            Assert.Equal(new Size(DesignerUtils.CONTAINERGRABHANDLESIZE, DesignerUtils.CONTAINERGRABHANDLESIZE),
                DesignerUtils.GetAdornmentDimensions(AdornmentType.Maximum));
        }

        [Fact(Skip = "Mock assembly not working")]
        public void DesignerUtils_UseSnapLines()
        {
            var mockPropertyDescriptor = new Mock<PropertyDescriptor>(MockBehavior.Default);
            mockPropertyDescriptor
                .Setup(s => s.GetValue(null))
                .Returns(null);

            var mockDesignerOptionCollection = new Mock<DesignerOptionCollection>(MockBehavior.Default);
            mockDesignerOptionCollection
                .Setup(s => s.Properties)
                .Returns(new PropertyDescriptorCollection(new PropertyDescriptor[] { mockPropertyDescriptor.Object }));

            var mockDesignerOptionService = new Mock<DesignerOptionService>(MockBehavior.Default);
            mockDesignerOptionService
                .Setup(s => s.Options)
                .Returns(mockDesignerOptionCollection.Object);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Default);
            mockServiceProvider
                .Setup(s => s.GetService(typeof(DesignerOptionService)))
                .Returns(mockDesignerOptionService.Object);

            Assert.True(DesignerUtils.UseSnapLines(mockServiceProvider.Object));

        }

        [Fact(Skip = "Mock assembly not working")]
        public void DesignerUtils_GetOptionValue()
        {
            var mockPropertyDescriptor = new Mock<PropertyDescriptor>(MockBehavior.Default);
            mockPropertyDescriptor
                .Setup(s => s.GetValue(null))
                .Returns(null);

            var mockDesignerOptionCollection = new Mock<DesignerOptionCollection>(MockBehavior.Default);
            mockDesignerOptionCollection
                .Setup(s => s.Properties)
                .Returns(new PropertyDescriptorCollection(new PropertyDescriptor[] { mockPropertyDescriptor.Object }));

            var mockDesignerOptionService = new Mock<IDesignerOptionService>(MockBehavior.Default);
            mockDesignerOptionService
                .Setup(s => s.GetOptionValue(null, null))
                .Returns(mockDesignerOptionCollection.Object);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Default);
            mockServiceProvider
                .Setup(s => s.GetService(typeof(IDesignerOptionService)))
                .Returns(mockDesignerOptionService.Object);

            Assert.NotNull(DesignerUtils.GetOptionValue(mockServiceProvider.Object, null));
        }

        [Fact]
        public void DesignerUtils_GenerateSnapShotWithBitBltTest()
        {      
            Image image = LoadImageAssembly();

            try
            {
                DesignerUtils.GenerateSnapShotWithBitBlt(new Control(), ref image);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DesignerUtils_GenerateSnapShotWithWM_PRINT()
        {
            Image image = LoadImageAssembly();

            try
            {
                DesignerUtils.GenerateSnapShotWithWM_PRINT(new Control(), ref image);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DesignerUtils_GetBoundsForSelectionType_Border()
        {
            Rectangle mockRectangle = new Rectangle(1, 1, 1, 1);

            Assert.Equal(new Rectangle(0, 0, 3, 1),
                DesignerUtils.GetBoundsForSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Top, 1));

            Assert.Equal(new Rectangle(0, 2, 3, 1),
                DesignerUtils.GetBoundsForSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Bottom, 1));

            Assert.Equal(new Rectangle(0, 0, 1, 3),
                DesignerUtils.GetBoundsForSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Left, 1));

            Assert.Equal(new Rectangle(2, 0, 1, 3),
                DesignerUtils.GetBoundsForSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Right, 1));

            Assert.Equal(mockRectangle,
                DesignerUtils.GetBoundsForSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Body, 1));
        }

        [Fact]
        public void DesignerUtils_GetBoundsForSelectionType()
        {
            Rectangle mockRectangle = new Rectangle(1, 1, 1, 1);

            Assert.Equal(new Rectangle(-1, -1, 5, 1),
                DesignerUtils.GetBoundsForSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Top));

            Assert.Equal(new Rectangle(-1, 3, 5, 1),
                DesignerUtils.GetBoundsForSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Bottom));

            Assert.Equal(new Rectangle(-1, -1, 1, 5),
                DesignerUtils.GetBoundsForSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Left));

            Assert.Equal(new Rectangle(3, -1, 1, 5),
                DesignerUtils.GetBoundsForSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Right));

            Assert.Equal(mockRectangle,
                DesignerUtils.GetBoundsForSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Body));
        }

        [Fact]
        public void DesignerUtils_GetBoundsForNoResizeSelectionType()
        {
            Rectangle mockRectangle = new Rectangle(1, 1, 1, 1);

            Assert.Equal(new Rectangle(-2, -2, 7, 1),
                DesignerUtils.GetBoundsForNoResizeSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Top));

            Assert.Equal(new Rectangle(-2, 4, 7, 1),
                DesignerUtils.GetBoundsForNoResizeSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Bottom));

            Assert.Equal(new Rectangle(-2, -2, 1, 7),
                DesignerUtils.GetBoundsForNoResizeSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Left));

            Assert.Equal(new Rectangle(4, -2, 1, 7),
                DesignerUtils.GetBoundsForNoResizeSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Right));

            Assert.Equal(mockRectangle,
                DesignerUtils.GetBoundsForNoResizeSelectionType(mockRectangle, Behavior.SelectionBorderGlyphType.Body));
        }

        [Fact]
        public void DesignerUtils_GetTextBaseline()
        {
            Assert.Equal(13, DesignerUtils.GetTextBaseline(new Control(), ContentAlignment.TopCenter));

            Assert.Equal(6, DesignerUtils.GetTextBaseline(new Control(), ContentAlignment.MiddleCenter));

            Assert.Equal(-2, DesignerUtils.GetTextBaseline(new Control(), ContentAlignment.BottomCenter));
        }

        [Fact]
        public void DesignerUtils_GetUniqueSiteName_Null()
        {
            var name = "Button";
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Default);
            var mockNameCreationService = new Mock<INameCreationService>(MockBehavior.Default);
            mockDesignerHost
                .Setup(s => s.GetService(typeof(INameCreationService)))
                .Returns(mockNameCreationService.Object);
            mockDesignerHost
                .Setup(s => s.Container.Components)
                .Returns(new ComponentCollection(new Component[1]));        

            Assert.Null(DesignerUtils.GetUniqueSiteName(mockDesignerHost.Object, name));
            Assert.Null(DesignerUtils.GetUniqueSiteName(mockDesignerHost.Object, null));

            mockDesignerHost
                .Setup(s => s.GetService(typeof(INameCreationService)))
                .Returns(null);

            Assert.Null(DesignerUtils.GetUniqueSiteName(mockDesignerHost.Object, name));
        }

        [Fact(Skip = "Mock assembly not working")]
        public void DesignerUtils_GetBoundsFromToolboxSnapDragDropInfo()
        {
            Rectangle rectangle = new Rectangle(1, 1, 1, 1);

            var mockToolboxSnapDragDropEventArgs = new Mock<ToolboxSnapDragDropEventArgs>(MockBehavior.Default);
            mockToolboxSnapDragDropEventArgs
                .Setup(s => s.Offset)
                .Returns(new Point(1, 1));

            Assert.NotEqual(rectangle, DesignerUtils.GetBoundsFromToolboxSnapDragDropInfo(mockToolboxSnapDragDropEventArgs.Object, rectangle, false));
        }

        [Fact(Skip = "Mock assembly not working")]
        public void DesignerUtils_GetBoundsFromToolboxSnapDragDropInfo_NoOffset()
        {
            Rectangle rectangle = new Rectangle(1, 1, 1, 1);

            var mockToolboxSnapDragDropEventArgs = new Mock<ToolboxSnapDragDropEventArgs>(MockBehavior.Default);
            mockToolboxSnapDragDropEventArgs
                .Setup(s => s.Offset)
                .Returns(Point.Empty);
            mockToolboxSnapDragDropEventArgs
               .Setup(s => s.SnapDirections)
               .Returns(ToolboxSnapDragDropEventArgs.SnapDirection.Top);

            Assert.Equal(rectangle, DesignerUtils.GetBoundsFromToolboxSnapDragDropInfo(mockToolboxSnapDragDropEventArgs.Object, rectangle, false));
        }

        [Fact]
        public void DesignerUtils_GetUniqueSiteName_NotNull()
        {
            var name = "Button";
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Default);
            var mockNameCreationService = new Mock<INameCreationService>(MockBehavior.Default);
            mockNameCreationService
                .Setup(s => s.IsValidName(name))
                .Returns(true);
            mockDesignerHost
                .Setup(s => s.GetService(typeof(INameCreationService)))
                .Returns(mockNameCreationService.Object);
            mockDesignerHost
                .Setup(s => s.Container.Components)
                .Returns(new ComponentCollection(new Button[] { new Button(), new Button() }));

            Assert.NotNull(DesignerUtils.GetUniqueSiteName(mockDesignerHost.Object, name));
        }

        [Fact]
        public void DesignerUtils_FilterGenericTypes()
        {
            Collection mockCollection = new Collection();

            mockCollection.Add(typeof(Button));
            mockCollection.Add(typeof(IList));

            Assert.Equal(mockCollection, DesignerUtils.FilterGenericTypes(mockCollection));
        }

        [Fact]
        public void DesignerUtils_FilterGenericTypes_Emtpy()
        {
            Assert.Null(DesignerUtils.FilterGenericTypes(null));
            Assert.Equal(new Collection(), DesignerUtils.FilterGenericTypes(new Collection()));
        }

        [Fact]
        public void DesignerUtils_CheckForNestedContainer()
        {
            var mockSite = new Mock<ISite>(MockBehavior.Default);
            mockSite
                .Setup(s => s.Container)
                .Returns(new Container());

            Container container = new Container();
            Button button = new Button();
            button.Site = mockSite.Object;

            Container mockContainer = new Container();
            mockContainer.Add(button);

            Assert.Equal(container, DesignerUtils.CheckForNestedContainer(container));
            Assert.Equal(mockContainer, DesignerUtils.CheckForNestedContainer(new NestedContainer(button)));
        }

        [Fact]
        public void DesignerUtils_CopyDragObjects()
        {
            var mockSerializationStore = new Mock<SerializationStore>(MockBehavior.Default);
            mockSerializationStore
                .Setup(s => s.Close());

            var mockComponentSerializationService = new Mock<ComponentSerializationService>(MockBehavior.Default);
            mockComponentSerializationService
                .Setup(s => s.CreateStore())
                .Returns(mockSerializationStore.Object);
            mockComponentSerializationService
                .Setup(s => s.Serialize(mockSerializationStore.Object, new Component()));
            mockComponentSerializationService
                .Setup(s => s.Deserialize(mockSerializationStore.Object));

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockServiceProvider
                .Setup(s => s.GetService(typeof(ComponentSerializationService)))
                .Returns(mockComponentSerializationService.Object);

            Assert.Null(DesignerUtils.CopyDragObjects(new Collection(), mockServiceProvider.Object));
        }

        [Fact]
        public void DesignerUtils_ApplyTreeViewThemeStyles()
        {

            Assert.Throws<ArgumentNullException>(() => DesignerUtils.ApplyTreeViewThemeStyles(null));

            try
            {
                DesignerUtils.ApplyTreeViewThemeStyles(new TreeView());
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }

        [Fact]
        public void DesignerUtils_ApplyListViewThemeStyles()
        {

            Assert.Throws<ArgumentNullException>(() => DesignerUtils.ApplyListViewThemeStyles(null));

            try
            {
                DesignerUtils.ApplyListViewThemeStyles(new ListView());
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
        }
    }
}
