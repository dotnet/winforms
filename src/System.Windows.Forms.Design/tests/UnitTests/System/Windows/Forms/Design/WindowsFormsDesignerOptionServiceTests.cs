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
        public void WindowsFormsDesignerOptionService_PopulateOptionCollection_ValidOptionsFromSameClass_Success()
        {
            var service = new SubWindowsFormsDesignerOptionService();
            service.PopulateOptionCollectionEntry(service.Options);
            Assert.Equal(1, service.PopulateOptionCollectionCallCount);

            // Should not retrieve again when accessing Options.
            DesignerOptionService.DesignerOptionCollection childCollection = Assert.IsType<DesignerOptionService.DesignerOptionCollection>(Assert.Single(service.Options));
            Assert.Equal(1, service.PopulateOptionCollectionCallCount);
            Assert.Equal("DesignerOptions", childCollection.Name);
            Assert.Same(service.Options, childCollection.Parent);
            Assert.Equal(new string[] { "EnableInSituEditing", "GridSize", "ObjectBoundSmartTagAutoShow", "ShowGrid", "SnapToGrid", "UseOptimizedCodeGeneration", "UseSmartTags", "UseSnapLines" }, childCollection.Properties.Sort().Cast<PropertyDescriptor>().Select(p => p.Name));
            Assert.Empty(childCollection);
        }

        [Fact]
        public void WindowsFormsDesignerOptionService_PopulateOptionCollection_ValidOptionsFromOtherClass_Success()
        {
            var service = new SubWindowsFormsDesignerOptionService();
            var otherService = new SubWindowsFormsDesignerOptionService();
            service.PopulateOptionCollectionEntry(otherService.Options);
            Assert.Equal(1, service.PopulateOptionCollectionCallCount);

            // Should retrieve again when accessing Options.
            DesignerOptionService.DesignerOptionCollection childCollection = Assert.IsType<DesignerOptionService.DesignerOptionCollection>(Assert.Single(service.Options));
            Assert.Equal(2, service.PopulateOptionCollectionCallCount);
            Assert.Equal("DesignerOptions", childCollection.Name);
            Assert.Same(service.Options, childCollection.Parent);
            Assert.Equal(new string[] { "EnableInSituEditing", "GridSize", "ObjectBoundSmartTagAutoShow", "ShowGrid", "SnapToGrid", "UseOptimizedCodeGeneration", "UseSmartTags", "UseSnapLines" }, childCollection.Properties.Sort().Cast<PropertyDescriptor>().Select(p => p.Name));
            Assert.Empty(childCollection);

            DesignerOptionService.DesignerOptionCollection otherChildCollection = Assert.IsType<DesignerOptionService.DesignerOptionCollection>(Assert.Single(service.Options));
            Assert.Equal(0, otherService.PopulateOptionCollectionCallCount);
            Assert.Equal("DesignerOptions", otherChildCollection.Name);
            Assert.Same(service.Options, otherChildCollection.Parent);
            Assert.Equal(new string[] { "EnableInSituEditing", "GridSize", "ObjectBoundSmartTagAutoShow", "ShowGrid", "SnapToGrid", "UseOptimizedCodeGeneration", "UseSmartTags", "UseSnapLines" }, childCollection.Properties.Sort().Cast<PropertyDescriptor>().Select(p => p.Name));
            Assert.Empty(otherChildCollection);
        }

        [Fact]
        public void WindowsFormsDesignerOptionService_PopulateOptionCollection_NullOptions_Success()
        {
            var service = new SubWindowsFormsDesignerOptionService();
            service.PopulateOptionCollectionEntry(null);
            Assert.Equal(1, service.PopulateOptionCollectionCallCount);

            // Should retrieve again when accessing Options.
            DesignerOptionService.DesignerOptionCollection childCollection = Assert.IsType<DesignerOptionService.DesignerOptionCollection>(Assert.Single(service.Options));
            Assert.Equal(2, service.PopulateOptionCollectionCallCount);
            Assert.Equal("DesignerOptions", childCollection.Name);
            Assert.Same(service.Options, childCollection.Parent);
            Assert.Equal(new string[] { "EnableInSituEditing", "GridSize", "ObjectBoundSmartTagAutoShow", "ShowGrid", "SnapToGrid", "UseOptimizedCodeGeneration", "UseSmartTags", "UseSnapLines" }, childCollection.Properties.Sort().Cast<PropertyDescriptor>().Select(p => p.Name));
            Assert.Empty(childCollection);
            Assert.Equal(3, service.PopulateOptionCollectionCallCount);
        }

        private class SubWindowsFormsDesignerOptionService : WindowsFormsDesignerOptionService
        {
            public int PopulateOptionCollectionCallCount { get; set; }

            public void PopulateOptionCollectionEntry(DesignerOptionCollection options)
            {
                PopulateOptionCollection(options);
            }

            protected override void PopulateOptionCollection(DesignerOptionCollection options)
            {
                PopulateOptionCollectionCallCount++;
                base.PopulateOptionCollection(options);
            }
        }

        private class NullCompatibilityOptions : WindowsFormsDesignerOptionService
        {
            public override DesignerOptions CompatibilityOptions => null;
        }
    }
}
