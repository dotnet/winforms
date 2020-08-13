// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This internal Class is used by all TOPLEVEL ToolStripItems to show the InSitu Editor. When the ToolStripItem receives the MouseDown on its Glyph it calls the "ActivateEditor" Function on this EditorManager. The ActivateEditor Function checks for any existing "EDITOR" active, closes that down and now opens the new editor on the "AdornerWindow". This class is also responsible for "hookingup" to the F2 Command on VS.
    /// </summary>
    internal class ToolStripEditorManager
    {
        // Local copy of BehaviorService so that we can add the Insitu Editor to the AdornerWindow.
        private readonly BehaviorService _behaviorService;
        // Component for this InSitu Editor... (this is a ToolStripItem) that wants to go into InSitu
        private readonly IDesignerHost _designerHost;
        // This is always ToolStrip
        private readonly IComponent _comp;
        // The current Bounds of the Insitu Editor on Adorner Window.. These are required for invalidation.
        private Rectangle _lastKnownEditorBounds = Rectangle.Empty;
        // The encapsulated Editor.
        private ToolStripEditorControl _editor;
        // Actual ToolStripEditor for the current ToolStripItem.
        private ToolStripTemplateNode _editorUI;
        // The Current ToolStripItem that needs to go into the InSitu Mode. We keep a local copy so that when a new item comes in, we can "ROLLBACK" the existing edit.
        private ToolStripItem _currentItem;
        // The designer for current ToolStripItem.
        private ToolStripItemDesigner _itemDesigner;

        public ToolStripEditorManager(IComponent comp)
        {
            // get the parent
            _comp = comp;
            _behaviorService = (BehaviorService)comp.Site.GetService(typeof(BehaviorService));
            _designerHost = (IDesignerHost)comp.Site.GetService(typeof(IDesignerHost));
        }

        /// <summary>
        ///  Activates the editor for the given item.If there's still an editor around  for the previous-edited item, it is deactivated. Pass in 'null' to deactivate and remove the current editor, if any.
        /// </summary>
        internal void ActivateEditor(ToolStripItem item, bool clicked)
        {
            if (item != _currentItem)
            {
                // Remove old editor
                if (_editor != null)
                {
                    _behaviorService.AdornerWindowControl.Controls.Remove(_editor);
                    _behaviorService.Invalidate(_editor.Bounds);
                    _editorUI = null;
                    _editor = null;
                    _currentItem = null;
                    _itemDesigner.IsEditorActive = false;

                    // Show the previously edited glyph
                    if (_currentItem != null)
                    {
                        _currentItem = null;
                    }
                }
                if (item != null)
                {
                    // Add new editor from the item...
                    _currentItem = item;
                    if (_designerHost != null)
                    {
                        _itemDesigner = (ToolStripItemDesigner)_designerHost.GetDesigner(_currentItem);
                    }
                    _editorUI = (ToolStripTemplateNode)_itemDesigner.Editor;
                    // If we got an editor, position and focus it.
                    if (_editorUI != null)
                    {
                        // Hide this glyph while it's being edited
                        _itemDesigner.IsEditorActive = true;
                        _editor = new ToolStripEditorControl(_editorUI.EditorToolStrip, _editorUI.Bounds);
                        _behaviorService.AdornerWindowControl.Controls.Add(_editor);
                        _lastKnownEditorBounds = _editor.Bounds;
                        _editor.BringToFront();
                        // this is important since the ToolStripEditorControl listens    to textchanged messages from TextBox.
                        _editorUI.ignoreFirstKeyUp = true;
                        // Select the Editor... Put Text and Select it ...
                        _editorUI.FocusEditor(_currentItem);
                    }
                }
            }
        }

        /// <summary>
        ///  This will remove the Command for F2.
        /// </summary>
        internal void CloseManager()
        {
        }

        /// <summary>
        ///  This LISTENs to the Editor Resize for resizing the Insitu edit on  the Adorner Window ... CURRENTLY DISABLED.
        /// </summary>
        private void OnEditorResize(object sender, EventArgs e)
        {
            // THIS IS CURRENTLY DISABLE !!!!! TO DO !! SHOULD WE SUPPORT AUTOSIZED INSITU ?????
            _behaviorService.Invalidate(_lastKnownEditorBounds);
            if (_editor != null)
            {
                _lastKnownEditorBounds = _editor.Bounds;
            }
        }

        //  Private Class Implemented for InSitu Editor. This class just Wraps the ToolStripEditor from the TemplateNode and puts it in  a Panel.
        private class ToolStripEditorControl : Panel
        {
            private readonly Control _wrappedEditor;
            private Rectangle _bounds;

            public ToolStripEditorControl(Control editorToolStrip, Rectangle bounds)
            {
                _wrappedEditor = editorToolStrip;
                Bounds1 = bounds;
                _wrappedEditor.Resize += new EventHandler(OnWrappedEditorResize);
                Controls.Add(editorToolStrip);
                Location = new Point(bounds.X, bounds.Y);
                Text = "InSituEditorWrapper";
                UpdateSize();
            }

            public Rectangle Bounds1 { get => _bounds; set => _bounds = value; }

            private void OnWrappedEditorResize(object sender, EventArgs e)
            {
            }

            private void UpdateSize() => Size = new Size(_wrappedEditor.Size.Width, _wrappedEditor.Size.Height);
        }
    }
}
