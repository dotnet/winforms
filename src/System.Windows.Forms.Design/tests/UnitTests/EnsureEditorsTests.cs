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
    public class EnsureEditorsTests
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
        [InlineData(typeof(GridColumnStylesCollection), typeof(DataGridColumnCollectionEditor))]
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
        [InlineData(typeof(ButtonBase), "Text", typeof(MultilineStringEditor))]
        //[InlineData(typeof(CheckedListBox), "Items", typeof(ListControlStringCollectionEditor))]
        //[InlineData(typeof(ComboBox), "AutoCompleteCustomSource", typeof(ListControlStringCollectionEditor))]
        //[InlineData(typeof(ComboBox), "Items", typeof(ListControlStringCollectionEditor))]
        //[InlineData(typeof(DataGrid), "DataMember", typeof(DataMemberListEditor))]
        //[InlineData(typeof(DataGridColumnStyle), "MappingName", typeof(DataGridColumnStyleMappingNameEditor))]
        //[InlineData(typeof(DataGridTableStyle), "MappingName", typeof(DataGridTableStyleMappingNameEditor))]
        //[InlineData(typeof(DataGridTextBoxColumn), "Format", typeof(DataGridColumnStyleFormatEditor))]
        //[InlineData(typeof(DataGridView), "Columns", typeof(DataGridViewColumnCollectionEditor))]
        //[InlineData(typeof(DataGridView), "DataMember", typeof(DataMemberListEditor))]
        //[InlineData(typeof(DataGridViewCellStyle), "Format", typeof(FormatStringEditor))]
        //[InlineData(typeof(DataGridViewColumn), "DataPropertyName", typeof(DataGridViewColumnDataPropertyNameEditor))]
        //[InlineData(typeof(DataGridViewComboBoxColumn), "DisplayMember", typeof(DataMemberFieldEditor))]
        [InlineData(typeof(DataGridViewComboBoxColumn), "Items", typeof(StringCollectionEditor))]
        //[InlineData(typeof(DataGridViewComboBoxColumn), "ValueMember", typeof(DataMemberFieldEditor))]
        [InlineData(typeof(DateTimePicker), "MaxDate", typeof(DateTimeEditor))]
        [InlineData(typeof(DateTimePicker), "MinDate", typeof(DateTimeEditor))]
        [InlineData(typeof(DateTimePicker), "Value", typeof(DateTimeEditor))]
        [InlineData(typeof(DomainUpDown), "Items", typeof(StringCollectionEditor))]
        //[InlineData(typeof(ErrorProvider), "DataMember", typeof(DataMemberListEditor))]
        //[InlineData(typeof(FolderBrowserDialog), "SelectedPath", typeof(SelectedPathEditor))]
        //[InlineData(typeof(HelpProvider), "HelpNamespace", typeof(HelpNamespaceEditor))]
        [InlineData(typeof(Label), "Text", typeof(MultilineStringEditor))]
        //[InlineData(typeof(LinkLabel), "LinkArea", typeof(LinkAreaEditor))]
        //[InlineData(typeof(ListBox), "Items", typeof(ListControlStringCollectionEditor))]
        //[InlineData(typeof(ListControl), "DisplayMember", typeof(DataMemberFieldEditor))]
        //[InlineData(typeof(ListControl), "FormatString", typeof(FormatStringEditor))]
        //[InlineData(typeof(ListControl), "ValueMember", typeof(DataMemberFieldEditor))]
        //[InlineData(typeof(ListView), "Columns", typeof(ColumnHeaderCollectionEditor))]
        [InlineData(typeof(ListView), "Groups", typeof(ListViewGroupCollectionEditor))]
        [InlineData(typeof(ListView), "Items", typeof(ListViewItemCollectionEditor))]
        [InlineData(typeof(ListViewItem), "SubItems", typeof(ListViewSubItemCollectionEditor))]
        //[InlineData(typeof(MaskedTextBox), "Mask", typeof(MaskPropertyEditor))]
        //[InlineData(typeof(MaskedTextBox), "Text", typeof(MaskedTextBoxTextEditor))]
        [InlineData(typeof(MonthCalendar), "MaxDate", typeof(DateTimeEditor))]
        [InlineData(typeof(MonthCalendar), "MinDate", typeof(DateTimeEditor))]
        [InlineData(typeof(MonthCalendar), "SelectionEnd", typeof(DateTimeEditor))]
        [InlineData(typeof(MonthCalendar), "SelectionStart", typeof(DateTimeEditor))]
        [InlineData(typeof(MonthCalendar), "TodayDate", typeof(DateTimeEditor))]
        [InlineData(typeof(NotifyIcon), "BalloonTipText", typeof(MultilineStringEditor))]
        [InlineData(typeof(NotifyIcon), "Text", typeof(MultilineStringEditor))]
        [InlineData(typeof(TabControl), "TabPages", typeof(TabPageCollectionEditor))]
        //[InlineData(typeof(TextBox), "AutoCompleteCustomSource", typeof(ListControlStringCollectionEditor))]
        [InlineData(typeof(TextBoxBase), "Lines", typeof(StringArrayEditor))]
        [InlineData(typeof(TextBoxBase), "Text", typeof(MultilineStringEditor))]
        //[InlineData(typeof(ToolStripComboBox), "AutoCompleteCustomSource", typeof(ListControlStringCollectionEditor))]
        //[InlineData(typeof(ToolStripComboBox), "Items", typeof(ListControlStringCollectionEditor))]
        //[InlineData(typeof(ToolStripItem), "ImageIndex", typeof(ToolStripImageIndexEditor))]
        //[InlineData(typeof(ToolStripItem), "ImageKey", typeof(ToolStripImageIndexEditor))]
        [InlineData(typeof(ToolStripItem), "ToolTipText", typeof(MultilineStringEditor))]
        //[InlineData(typeof(ToolStripTextBox), "AutoCompleteCustomSource", typeof(ListControlStringCollectionEditor))]
        [InlineData(typeof(ToolStripTextBox), "Lines", typeof(StringArrayEditor))]
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
