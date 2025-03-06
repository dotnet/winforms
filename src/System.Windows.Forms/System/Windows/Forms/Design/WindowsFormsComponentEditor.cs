// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides a base class for editors that support any type of
/// <see cref="IComponent"/> objects.
/// </summary>
public abstract class WindowsFormsComponentEditor : ComponentEditor
{
    /// <summary>
    ///  Activates a UI used to edit the component.
    /// </summary>
    public override bool EditComponent(ITypeDescriptorContext? context, object component)
    {
        return EditComponent(context, component, null);
    }

    /// <summary>
    ///  Activates the advanced UI used to edit the component.
    /// </summary>
    public bool EditComponent(object component, IWin32Window? owner)
    {
        return EditComponent(null, component, owner);
    }

    /// <summary>
    ///  Activates the advanced UI used to edit the component.
    /// </summary>
    public virtual bool EditComponent(ITypeDescriptorContext? context, object component, IWin32Window? owner)
    {
        Type[]? pageControlTypes = GetComponentEditorPages();
        if (pageControlTypes is null || pageControlTypes.Length == 0)
        {
            return false;
        }

        ComponentEditorForm form = new(component, pageControlTypes);
        return form.ShowForm(owner, GetInitialComponentEditorPageIndex()) == DialogResult.OK;
    }

    /// <summary>
    ///  Gets the set of <see cref="ComponentEditorPage"/> pages to be used.
    /// </summary>
    protected virtual Type[]? GetComponentEditorPages() => null;

    /// <summary>
    ///  Gets the index of the <see cref="ComponentEditorPage"/>
    ///  to be shown by default as the first active page.
    /// </summary>
    protected virtual int GetInitialComponentEditorPageIndex() => 0;
}
