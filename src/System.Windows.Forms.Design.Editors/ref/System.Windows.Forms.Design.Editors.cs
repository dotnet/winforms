// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

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
namespace System.Windows.Forms.Design.Editors
{
    [System.CLSCompliantAttribute(false)]
    public partial class CollectionEditor : System.Drawing.Design.UITypeEditor
    {
        public CollectionEditor() { }
        public CollectionEditor(Type type) { }
        protected Type CollectionItemType { get { throw null; } }
        protected Type CollectionType { get { throw null; } }
        protected ITypeDescriptorContext Context { get { throw null; } }
        protected Type[] NewItemTypes { get { throw null; } }
        protected virtual string HelpTopic { get { throw null; } }
        protected virtual bool CanRemoveInstance(object value) { throw null; }
        protected virtual bool CanSelectMultipleInstances() { throw null; }
        protected virtual CollectionForm CreateCollectionForm() { throw null; }
        protected virtual object CreateInstance(Type itemType) { throw null; }
        protected virtual IList GetObjectsFromInstance(object instance) { throw null; }
        protected virtual string GetDisplayText(object value) { throw null; }
        protected virtual Type CreateCollectionItemType() { throw null; }
        protected virtual Type[] CreateNewItemTypes() { throw null; }
        protected virtual void DestroyInstance(object instance) { throw null; }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) { throw null; }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { throw null; }
        protected virtual object[] GetItems(object editValue) { throw null; }
        protected object GetService(Type serviceType) { throw null; }
        protected virtual object SetItems(object editValue, object[] value) { throw null; }
        protected virtual void ShowHelp() { }
        protected abstract class CollectionForm : Form
        {
            public CollectionForm(CollectionEditor editor) { }
            protected Type CollectionItemType { get { throw null; } }
            protected Type CollectionType { get { throw null; } }
            protected ITypeDescriptorContext Context { get { throw null; } }
            public object EditValue { get { throw null; } set { } }
            protected object[] Items { get { throw null; } set { } }
            protected Type[] NewItemTypes { get { throw null; } }
            protected bool CanRemoveInstance(object value) { throw null; }
            protected virtual bool CanSelectMultipleInstances() { throw null; }
            protected object CreateInstance(Type itemType) { throw null; }
            protected void DestroyInstance(object instance) { }
            protected virtual void DisplayError(Exception e) { }
            protected override object GetService(Type serviceType) { throw null; }
            protected internal virtual DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc) { throw null; }
            protected abstract void OnEditValueChanged();
        }
    }

    [System.CLSCompliantAttribute(false)]
    public partial class ObjectSelectorEditor : System.Drawing.Design.UITypeEditor
    {
        public ObjectSelectorEditor() { }
        public ObjectSelectorEditor(bool subObjectSelector) { }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) { throw null; }
        public static void ApplyTreeViewThemeStyles(TreeView treeView) { }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { throw null; }
        public bool EqualsToValue(object value) { throw null; }
        protected virtual void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider) { }
        public virtual void SetValue(object value) { }
        public class Selector : TreeView
        {
            public Selector(ObjectSelectorEditor editor) { }
            public SelectorNode AddNode(string label, object value, SelectorNode parent) { throw null; }
            public void Clear() { }
            protected void OnAfterSelect(object sender, TreeViewEventArgs e) { }
            protected override void OnKeyDown(KeyEventArgs e) { }
            protected override void OnKeyPress(KeyPressEventArgs e) { }
            protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e) { }
            public bool SetSelection(object value, TreeNodeCollection nodes) { throw null; }
            public void Start(IWindowsFormsEditorService edSvc, object value) { }
            public void Stop() { }
            protected override void WndProc(ref Message m) { }
        }
        public class SelectorNode : TreeNode
        {
            public SelectorNode(string label, object value) : base(label) { }

        }
    }
}
