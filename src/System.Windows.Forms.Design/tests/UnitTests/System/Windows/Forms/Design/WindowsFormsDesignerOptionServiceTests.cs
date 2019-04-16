// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class WindowsFormsDesignerOptionServiceTests
    {
        [Fact]
        public void WindowsFormsDesignerOptionService_Ctor_Default()
        {
            var service = new WindowsFormsDesignerOptionService();
            DesignerOptions options = service.CompatibilityOptions;
            Assert.True(options.EnableInSituEditing);
            Assert.Equal(new Size(8, 8), options.GridSize);
            Assert.True(options.ObjectBoundSmartTagAutoShow);
            Assert.True(options.ShowGrid);
            Assert.True(options.SnapToGrid);
            Assert.False(options.UseSmartTags);
            Assert.False(options.UseSnapLines);
            Assert.False(options.UseOptimizedCodeGeneration);
            Assert.Same(options, service.CompatibilityOptions);
        }

        [Fact]
        public void WindowsFormsDesignerOptionService_Options_Get_ReturnsExpected()
        {
            var service = new WindowsFormsDesignerOptionService();
            Assert.Empty(service.Options.Name);
            DesignerOptionService.DesignerOptionCollection childCollection = Assert.IsType<DesignerOptionService.DesignerOptionCollection>(Assert.Single(service.Options));
            Assert.Equal("DesignerOptions", childCollection.Name);
            Assert.Same(service.Options, childCollection.Parent);
            Assert.Equal(new string[] { "EnableInSituEditing", "GridSize", "ObjectBoundSmartTagAutoShow", "ShowGrid", "SnapToGrid", "UseOptimizedCodeGeneration", "UseSmartTags", "UseSnapLines" }, childCollection.Properties.Sort().Cast<PropertyDescriptor>().Select(p => p.Name));
            Assert.Empty(childCollection);
        }

        [Fact]
        public void WindowsFormsDesignerOptionService_Options_GetNullDesignerOptions_ReturnsExpected()
        {
            var service = new NullCompatibilityOptions();
            Assert.Empty(service.Options.Name);
            Assert.Empty(service.Options);
        }

        [Fact]
        public void WindowsFormsDesignerOptionService_PopulateOptionCollection_ValidOptions_Success()
        {
            var service = new SubWindowsFormsDesignerOptionService();
            var otherService = new SubWindowsFormsDesignerOptionService();
            service.PopulateOptionCollection(otherService.Options);
            Assert.Empty(service.Options);
            Assert.Empty(otherService.Options);
        }

        [Fact]
        public void WindowsFormsDesignerOptionService_PopulateOptionCollection_NullOptions_Success()
        {
            var service = new SubWindowsFormsDesignerOptionService();
            service.PopulateOptionCollection(null);
        }

        private class SubWindowsFormsDesignerOptionService : DesignerOptionService
        {
            public new void PopulateOptionCollection(DesignerOptionCollection options) => base.PopulateOptionCollection(options);
        }

        private class NullCompatibilityOptions : WindowsFormsDesignerOptionService
        {
            public override DesignerOptions CompatibilityOptions => null;
        }
    }
}
