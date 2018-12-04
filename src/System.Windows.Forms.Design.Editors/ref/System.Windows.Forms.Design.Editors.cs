// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Drawing.Design
{
    [System.CLSCompliantAttribute(false)]
    public partial class BitmapEditor : System.Drawing.Design.ImageEditor
    {
        public BitmapEditor() { }
        protected override string[] GetExtensions() { throw null; }
        protected override string GetFileDialogDescription() { throw null; }
        protected override System.Drawing.Image LoadFromStream(System.IO.Stream stream) { throw null; }
    }
    [System.CLSCompliantAttribute(false)]
    public partial class ColorEditor : System.Drawing.Design.UITypeEditor
    {
        public ColorEditor() { }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) { throw null; }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
        public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
        public override void PaintValue(System.Drawing.Design.PaintValueEventArgs e) { }
    }
    [System.CLSCompliantAttribute(false)]
    public partial class CursorEditor : System.Drawing.Design.UITypeEditor
    {
        public CursorEditor() { }
        public override bool IsDropDownResizable { get { throw null; } }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) { throw null; }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
    }
    [System.CLSCompliantAttribute(false)]
    public partial class FontEditor : System.Drawing.Design.UITypeEditor
    {
        public FontEditor() { }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) { throw null; }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
    }
    [System.CLSCompliantAttribute(false)]
    public partial class ImageEditor : System.Drawing.Design.UITypeEditor
    {
        public ImageEditor() { }
        protected static string CreateExtensionsString(string[] extensions, string sep) { throw null; }
        protected static string CreateFilterEntry(System.Drawing.Design.ImageEditor e) { throw null; }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) { throw null; }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
        protected virtual string[] GetExtensions() { throw null; }
        protected virtual string GetFileDialogDescription() { throw null; }
        protected virtual System.Type[] GetImageExtenders() { throw null; }
        public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
        protected virtual System.Drawing.Image LoadFromStream(System.IO.Stream stream) { throw null; }
        public override void PaintValue(System.Drawing.Design.PaintValueEventArgs e) { }
    }
    [System.CLSCompliantAttribute(false)]
    public partial class MetafileEditor : System.Drawing.Design.ImageEditor
    {
        public MetafileEditor() { }
        protected override string[] GetExtensions() { throw null; }
        protected override string GetFileDialogDescription() { throw null; }
        protected override System.Drawing.Image LoadFromStream(System.IO.Stream stream) { throw null; }
    }
}
namespace System.Windows.Forms.Design
{
    [System.CLSCompliantAttribute(false)]
    public sealed partial class AnchorEditor : System.Drawing.Design.UITypeEditor
    {
        public AnchorEditor() { }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) { throw null; }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
    }
    [System.CLSCompliantAttribute(false)]
    public partial class BorderSidesEditor : System.Drawing.Design.UITypeEditor
    {
        public BorderSidesEditor() { }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) { throw null; }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
    }
    [System.CLSCompliantAttribute(false)]
    public sealed partial class DockEditor : System.Drawing.Design.UITypeEditor
    {
        public DockEditor() { }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) { throw null; }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
    }
    [System.CLSCompliantAttribute(false)]
    public partial class FileNameEditor : System.Drawing.Design.UITypeEditor
    {
        public FileNameEditor() { }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) { throw null; }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
        protected virtual void InitializeDialog(System.Windows.Forms.OpenFileDialog openFileDialog) { }
    }
    [System.CLSCompliantAttribute(false)]
    public partial class FolderNameEditor : System.Drawing.Design.UITypeEditor
    {
        public FolderNameEditor() { }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) { throw null; }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
        protected virtual void InitializeDialog(System.Windows.Forms.Design.FolderNameEditor.FolderBrowser folderBrowser) { }
        protected sealed partial class FolderBrowser : System.ComponentModel.Component
        {
            public FolderBrowser() { }
            public string Description { get { throw null; } set { } }
            public string DirectoryPath { get { throw null; } }
            public System.Windows.Forms.Design.FolderNameEditor.FolderBrowserFolder StartLocation { get { throw null; } set { } }
            public System.Windows.Forms.Design.FolderNameEditor.FolderBrowserStyles Style { get { throw null; } set { } }
            public System.Windows.Forms.DialogResult ShowDialog() { throw null; }
            public System.Windows.Forms.DialogResult ShowDialog(System.Windows.Forms.IWin32Window owner) { throw null; }
        }
        protected enum FolderBrowserFolder
        {
            Desktop = 0,
            Favorites = 6,
            MyComputer = 17,
            MyDocuments = 5,
            MyPictures = 39,
            NetAndDialUpConnections = 49,
            NetworkNeighborhood = 18,
            Printers = 4,
            Recent = 8,
            SendTo = 9,
            StartMenu = 11,
            Templates = 21,
        }
        [System.FlagsAttribute]
        protected enum FolderBrowserStyles
        {
            BrowseForComputer = 4096,
            BrowseForEverything = 16384,
            BrowseForPrinter = 8192,
            RestrictToDomain = 2,
            RestrictToFilesystem = 1,
            RestrictToSubfolders = 8,
            ShowTextBox = 16,
        }
    }
    [System.CLSCompliantAttribute(false)]
    public partial class ShortcutKeysEditor : System.Drawing.Design.UITypeEditor
    {
        public ShortcutKeysEditor() { }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) { throw null; }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) { throw null; }
    }
}
