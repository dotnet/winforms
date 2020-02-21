// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace System.Windows.Forms.Design.Editors.Tests
{
    public class EnsureEditorsTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        // In Table
        [InlineData(typeof(Array), typeof(ArrayEditor))]
        [InlineData(typeof(IList), typeof(CollectionEditor))]
        [InlineData(typeof(ICollection), typeof(CollectionEditor))]
        [InlineData(typeof(byte[]), typeof(BinaryEditor))]
        [InlineData(typeof(Stream), typeof(BinaryEditor))]
        [InlineData(typeof(string[]), typeof(StringArrayEditor))]
        [InlineData(typeof(Bitmap), typeof(BitmapEditor))]
        [InlineData(typeof(Color), typeof(ColorEditor))]
        [InlineData(typeof(ContentAlignment), typeof(ContentAlignmentEditor))]
        [InlineData(typeof(Font), typeof(FontEditor))]
        [InlineData(typeof(Icon), typeof(IconEditor))]
        [InlineData(typeof(Image), typeof(ImageEditor))]
        [InlineData(typeof(Metafile), typeof(MetafileEditor))]
        [InlineData(typeof(AnchorStyles), typeof(AnchorEditor))]
        [InlineData(typeof(DockStyle), typeof(DockEditor))]
        [InlineData(typeof(ImageListImage), typeof(ImageListImageEditor))]
        [InlineData(typeof(DateTime), typeof(DateTimeEditor))]
        // With Editor Attribute
        [InlineData(typeof(Cursor), typeof(CursorEditor))]
        //[InlineData(typeof(GridColumnStylesCollection), typeof(DataGridColumnCollectionEditor))]
        //[InlineData(typeof(DataGridView), typeof(DataGridViewComponentEditor))]
        //[InlineData(typeof(DataGridViewCellStyle), typeof(DataGridViewCellStyleEditor))]
        [InlineData(typeof(ImageList.ImageCollection), typeof(ImageCollectionEditor))]
        [InlineData(typeof(Keys), typeof(ShortcutKeysEditor))]
        //[InlineData(typeof(TableLayoutStyleCollection), typeof(StyleCollectionEditor))]
        //[InlineData(typeof(ToolStripItemCollection), typeof(ToolStripCollectionEditor))]
        [InlineData(typeof(ToolStripStatusLabelBorderSides), typeof(BorderSidesEditor))]
        //[InlineData(typeof(TreeNodeCollection), typeof(TreeNodeCollectionEditor))]
        public void EnsureUITypeEditorForType(Type type, Type expectedEditorType)
        {
            var editor = TypeDescriptor.GetEditor(type, typeof(UITypeEditor));

            Assert.NotNull(editor);
            Assert.Equal(expectedEditorType, editor.GetType());
        }

        [WinFormsTheory]
        //[InlineData(typeof(BindingSource), "DataMember", typeof(DataMemberListEditor))]
        [InlineData(typeof(ButtonBase), "ImageIndex", typeof(ImageIndexEditor))]
        [InlineData(typeof(ButtonBase), "ImageKey", typeof(ImageIndexEditor))]
        [InlineData(typeof(ButtonBase), "Text", typeof(MultilineStringEditor))]
        [InlineData(typeof(CheckedListBox), "Items", typeof(ListControlStringCollectionEditor))]
        [InlineData(typeof(ColumnHeader), "ImageIndex", typeof(ImageIndexEditor))]
        [InlineData(typeof(ColumnHeader), "ImageKey", typeof(ImageIndexEditor))]
        [InlineData(typeof(ComboBox), "AutoCompleteCustomSource", typeof(ListControlStringCollectionEditor))]
        [InlineData(typeof(ComboBox), "Items", typeof(ListControlStringCollectionEditor))]
        //[InlineData(typeof(DataGridView), "Columns", typeof(DataGridViewColumnCollectionEditor))]
        //[InlineData(typeof(DataGridView), "DataMember", typeof(DataMemberListEditor))]
        [InlineData(typeof(DataGridViewCellStyle), "Format", typeof(FormatStringEditor))]
        //[InlineData(typeof(DataGridViewColumn), "DataPropertyName", typeof(DataGridViewColumnDataPropertyNameEditor))]
        //[InlineData(typeof(DataGridViewComboBoxColumn), "DisplayMember", typeof(DataMemberFieldEditor))]
        [InlineData(typeof(DataGridViewComboBoxColumn), "Items", typeof(StringCollectionEditor))]
        //[InlineData(typeof(DataGridViewComboBoxColumn), "ValueMember", typeof(DataMemberFieldEditor))]
        [InlineData(typeof(DateTimePicker), "MaxDate", typeof(DateTimeEditor))]
        [InlineData(typeof(DateTimePicker), "MinDate", typeof(DateTimeEditor))]
        [InlineData(typeof(DateTimePicker), "Value", typeof(DateTimeEditor))]
        [InlineData(typeof(DomainUpDown), "Items", typeof(StringCollectionEditor))]
        //[InlineData(typeof(ErrorProvider), "DataMember", typeof(DataMemberListEditor))]
        [InlineData(typeof(FolderBrowserDialog), "SelectedPath", typeof(SelectedPathEditor))]
        [InlineData(typeof(HelpProvider), "HelpNamespace", typeof(HelpNamespaceEditor))]
        [InlineData(typeof(Label), "ImageIndex", typeof(ImageIndexEditor))]
        [InlineData(typeof(Label), "ImageKey", typeof(ImageIndexEditor))]
        [InlineData(typeof(Label), "Text", typeof(MultilineStringEditor))]
        [InlineData(typeof(LinkLabel), "LinkArea", typeof(LinkAreaEditor))]
        [InlineData(typeof(ListBox), "Items", typeof(ListControlStringCollectionEditor))]
        //[InlineData(typeof(ListControl), "DisplayMember", typeof(DataMemberFieldEditor))]
        [InlineData(typeof(ListControl), "FormatString", typeof(FormatStringEditor))]
        //[InlineData(typeof(ListControl), "ValueMember", typeof(DataMemberFieldEditor))]
        //[InlineData(typeof(ListView), "Columns", typeof(ColumnHeaderCollectionEditor))]
        [InlineData(typeof(ListView), "Groups", typeof(ListViewGroupCollectionEditor))]
        [InlineData(typeof(ListView), "Items", typeof(ListViewItemCollectionEditor))]
        [InlineData(typeof(ListViewItem), "ImageIndex", typeof(ImageIndexEditor))]
        [InlineData(typeof(ListViewItem), "ImageKey", typeof(ImageIndexEditor))]
        [InlineData(typeof(ListViewItem), "StateImageIndex", typeof(ImageIndexEditor))]
        [InlineData(typeof(ListViewItem), "SubItems", typeof(ListViewSubItemCollectionEditor))]
        [InlineData(typeof(MaskedTextBox), "Mask", typeof(MaskPropertyEditor))]
        [InlineData(typeof(MaskedTextBox), "Text", typeof(MaskedTextBoxTextEditor))]
        [InlineData(typeof(MonthCalendar), "MaxDate", typeof(DateTimeEditor))]
        [InlineData(typeof(MonthCalendar), "MinDate", typeof(DateTimeEditor))]
        [InlineData(typeof(MonthCalendar), "SelectionEnd", typeof(DateTimeEditor))]
        [InlineData(typeof(MonthCalendar), "SelectionStart", typeof(DateTimeEditor))]
        [InlineData(typeof(MonthCalendar), "TodayDate", typeof(DateTimeEditor))]
        [InlineData(typeof(NotifyIcon), "BalloonTipText", typeof(MultilineStringEditor))]
        [InlineData(typeof(NotifyIcon), "Text", typeof(MultilineStringEditor))]
        [InlineData(typeof(TabControl), "TabPages", typeof(TabPageCollectionEditor))]
        [InlineData(typeof(TabPage), "ImageIndex", typeof(ImageIndexEditor))]
        [InlineData(typeof(TabPage), "ImageKey", typeof(ImageIndexEditor))]
        [InlineData(typeof(TextBox), "AutoCompleteCustomSource", typeof(ListControlStringCollectionEditor))]
        [InlineData(typeof(TextBoxBase), "Lines", typeof(StringArrayEditor))]
        [InlineData(typeof(TextBoxBase), "Text", typeof(MultilineStringEditor))]
        [InlineData(typeof(ToolStripComboBox), "AutoCompleteCustomSource", typeof(ListControlStringCollectionEditor))]
        [InlineData(typeof(ToolStripComboBox), "Items", typeof(ListControlStringCollectionEditor))]
        //[InlineData(typeof(ToolStripItem), "ImageIndex", typeof(ToolStripImageIndexEditor))]
        //[InlineData(typeof(ToolStripItem), "ImageKey", typeof(ToolStripImageIndexEditor))]
        [InlineData(typeof(ToolStripItem), "ToolTipText", typeof(MultilineStringEditor))]
        [InlineData(typeof(ToolStripTextBox), "AutoCompleteCustomSource", typeof(ListControlStringCollectionEditor))]
        [InlineData(typeof(ToolStripTextBox), "Lines", typeof(StringArrayEditor))]
        [InlineData(typeof(TreeNode), "ImageIndex", typeof(ImageIndexEditor))]
        [InlineData(typeof(TreeNode), "ImageKey", typeof(ImageIndexEditor))]
        [InlineData(typeof(TreeNode), "SelectedImageKey", typeof(ImageIndexEditor))]
        [InlineData(typeof(TreeNode), "StateImageKey", typeof(ImageIndexEditor))]
        [InlineData(typeof(TreeNode), "StateImageIndex", typeof(ImageIndexEditor))]
        [InlineData(typeof(TreeView), "ImageIndex", typeof(ImageIndexEditor))]
        [InlineData(typeof(TreeView), "ImageKey", typeof(ImageIndexEditor))]
        [InlineData(typeof(TreeView), "SelectedImageIndex", typeof(ImageIndexEditor))]
        [InlineData(typeof(TreeView), "SelectedImageKey", typeof(ImageIndexEditor))]
        public void EnsureUITypeEditorForProperty(Type type, string propertyName, Type expectedEditorType)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            Assert.NotNull(properties);
            Assert.NotEmpty(properties);

            PropertyDescriptor propertyDescriptor = properties.Find(propertyName, true);
            Assert.NotNull(propertyDescriptor);

            var editor = propertyDescriptor.GetEditor(typeof(UITypeEditor));
            Assert.NotNull(editor);
            Assert.Equal(expectedEditorType, editor.GetType());
        }

        [WinFormsTheory]
        [InlineData(typeof(ToolTip), "GetToolTip", typeof(MultilineStringEditor))]
        public void EnsureUITypeEditorForMethod(Type type, string methodName, Type expectedEditorType)
        {
            Type reflectType = TypeDescriptor.GetReflectionType(type);
            Assert.NotNull(reflectType);

            MethodInfo method = reflectType.GetMethod(methodName);
            Assert.NotNull(method);

            IEnumerable<EditorAttribute> attributes = method.GetCustomAttributes(typeof(EditorAttribute), false).Cast<EditorAttribute>();
            Assert.NotNull(attributes);
            Assert.NotEmpty(attributes);
            Assert.Contains(attributes, editor => editor.EditorTypeName.StartsWith(expectedEditorType.FullName + ", "));
        }
    }
}
