// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Reflection;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class EmbeddedResourceTests : IClassFixture<ThreadExceptionFixture>
    {
        // Get System.Windows.Forms.Design assembly.
        // The type is irrelevant, its for the Icon constructor.
        private readonly Assembly assembly = Assembly.GetAssembly(typeof(AnchorEditor));

        [Theory]
        [InlineData("System.ComponentModel.Design.Arrow")]
        [InlineData("System.ComponentModel.Design.ComponentEditorPage")]
        [InlineData("System.ComponentModel.Design.DateTimeFormat")]
        [InlineData("System.ComponentModel.Design.DefaultComponent")]
        [InlineData("System.ComponentModel.Design.NumericFormat")]
        [InlineData("System.ComponentModel.Design.OrderImages")]
        [InlineData("System.ComponentModel.Design.SortDown")]
        [InlineData("System.ComponentModel.Design.SortUp")]
        [InlineData("System.ComponentModel.Design.UncheckedBox")]
        [InlineData("System.Windows.Forms.Design.UserControlToolboxItem")]
        [InlineData("System.Windows.Forms.Design.InheritedGlyph")]
        [InlineData("System.Windows.Forms.Design.ImageEditor")]
        [InlineData("System.Windows.Forms.Design.ToolStripTemplateNode")]
        [InlineData("System.Windows.Forms.Design.DataPickerImages")]
        [InlineData("System.Windows.Forms.Design.AddNewDataSource")]
        [InlineData("System.Windows.Forms.Design.ChildFolder")]
        [InlineData("System.Windows.Forms.Design.Delete")]
        [InlineData("System.Windows.Forms.Design.Folder")]
        [InlineData("System.Windows.Forms.Design.default")]
        [InlineData("System.Windows.Forms.Design.Professional1")]
        [InlineData("System.Windows.Forms.Design.Professional2")]
        [InlineData("System.Windows.Forms.Design.classic")]
        [InlineData("System.Windows.Forms.Design.colorful1")]
        [InlineData("System.Windows.Forms.Design.256_1")]
        [InlineData("System.Windows.Forms.Design.256_2")]
        [InlineData("System.Windows.Forms.Design.BoundProperty")]
        [InlineData("System.Windows.Forms.Design.InsertableObject")]
        [InlineData("System.Windows.Forms.Design.Behavior.Close_left")]
        [InlineData("System.Windows.Forms.Design.Behavior.Open_left")]
        [InlineData("System.Windows.Forms.Design.Behavior.DesignerShortcutBox")]
        [InlineData("System.Windows.Forms.Design.Behavior.leftOpen")]
        [InlineData("System.Windows.Forms.Design.Behavior.leftClose")]
        [InlineData("System.Windows.Forms.Design.Behavior.MoverGlyph")]
        [InlineData("System.Windows.Forms.Design.Behavior.rightopen")]
        [InlineData("System.Windows.Forms.Design.Behavior.rightclose")]
        [InlineData("System.Windows.Forms.Design.Behavior.topopen")]
        [InlineData("System.Windows.Forms.Design.Behavior.topclose")]
        [InlineData("System.Windows.Forms.Design.Behavior.bottomopen")]
        [InlineData("System.Windows.Forms.Design.Behavior.bottomclose")]
        [InlineData("System.Windows.Forms.Design.DataGridViewColumnsDialog.delete")]
        [InlineData("System.Windows.Forms.Design.DataGridViewColumnsDialog.moveUp")]
        [InlineData("System.Windows.Forms.Design.DataGridViewColumnsDialog.moveDown")]
        [InlineData("System.Windows.Forms.Design.DataGridViewColumnsDialog.selectedColumns")]
        [InlineData("System.Windows.Forms.Design.BindingFormattingDialog.Bound")]
        [InlineData("System.Windows.Forms.Design.BindingFormattingDialog.Unbound")]
        [InlineData("System.Windows.Forms.Design.BindingFormattingDialog.Arrow")]
        public void EmbeddedResource_ResourcesExist_Icon(string resourceName)
        {
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            Assert.NotNull(stream);
            using Icon icon = new(stream);
            Assert.NotNull(icon);
        }
    }
}
