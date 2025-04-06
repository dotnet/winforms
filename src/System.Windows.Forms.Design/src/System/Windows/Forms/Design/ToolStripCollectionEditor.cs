// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    internal partial class ToolStripCollectionEditor : CollectionEditor
    {
        public ToolStripCollectionEditor() : base(typeof(ToolStripItemCollection))
        {
        }

        /// <summary>
        ///  Overridden to reaction our editor form instead of the standard collection editor form.
        /// </summary>
        /// <returns>An instance of a ToolStripItemEditorForm</returns>
        protected override CollectionForm CreateCollectionForm() => new ToolStripItemEditorForm(this);

        /// <summary>
        ///  Gets the help topic to display for the dialog help button or pressing F1. Override to
        ///  display a different help topic.
        /// </summary>
        protected override string HelpTopic => "net.ComponentModel.ToolStripCollectionEditor";

        /// <summary>
        ///  Check the owner.
        /// </summary>
        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            if (provider is null)
            {
                return null;
            }

            // get ahold of the designer for the component that is launching this editor.
            // If it is a win bar, then we want to let it know this editor is up.
            ToolStripDesigner? designer = null;

            // see if the selected component is a windows bar or windows bar toolStripDropDownItem that is directly on the form.
            ISelectionService? selectionService = provider.GetService<ISelectionService>();
            if (selectionService is not null)
            {
                object? primarySelection = selectionService.PrimarySelection;

                // if it's a drop down toolStripDropDownItem, just pop up to it's owner.
                if (primarySelection is ToolStripDropDownItem toolStripDropDownItem)
                {
                    primarySelection = toolStripDropDownItem.Owner;
                }

                // Now get the designer.
                if (primarySelection is ToolStrip)
                {
                    IDesignerHost? host = provider.GetService<IDesignerHost>();
                    if (host is not null)
                    {
                        designer = host.GetDesigner((IComponent)primarySelection) as ToolStripDesigner;
                    }
                }
            }

            try
            {
                if (designer is not null)
                {
                    designer.EditingCollection = true;
                }

                using (ScaleHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
                {
                    return base.EditValue(context, provider, value);
                }
            }
            finally
            {
                if (designer is not null)
                {
                    designer.EditingCollection = false;
                }
            }
        }
    }
}
