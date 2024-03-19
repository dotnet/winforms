// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal;

/// <summary>
///  Root <see cref="GridEntry"/> for the <see cref="PropertyGrid"/> when there are multiple objects
///  in <see cref="PropertyGrid.SelectedObjects"/>.
/// </summary>
internal sealed partial class MultiSelectRootGridEntry : SingleSelectRootGridEntry
{
    private static readonly PropertyDescriptorComparer s_propertyComparer = new();

    internal MultiSelectRootGridEntry(
        PropertyGridView view,
        object[] target,
        IServiceProvider baseProvider,
        IDesignerHost? host,
        PropertyTab tab,
        PropertySort sortType)
        : base(
            view,
            target,
            baseProvider,
            host,
            tab,
            sortType)
    {
    }

    internal override bool ForceReadOnly
    {
        get
        {
            if (!_forceReadOnlyChecked)
            {
                foreach (object target in (Array)Target)
                {
                    if ((TypeDescriptorHelper.TryGetAttribute(target, out ReadOnlyAttribute? readOnlyAttribute)
                        && !readOnlyAttribute.IsDefaultAttribute())
                        || TypeDescriptor.GetAttributes(target).Contains(InheritanceAttribute.InheritedReadOnly))
                    {
                        SetForceReadOnlyFlag();
                        break;
                    }
                }

                _forceReadOnlyChecked = true;
            }

            return base.ForceReadOnly;
        }
    }

    protected override bool CreateChildren(bool diffOldChildren = false)
    {
        try
        {
            object[] targets = (object[])Target;

            ChildCollection.Clear();

            var mergedProperties = PropertyMerger.GetMergedProperties(targets, this, _propertySort, OwnerTab);

            if (mergedProperties is not null)
            {
                ChildCollection.AddRange(mergedProperties);
            }

            bool expandable = Children.Count > 0;
            if (!expandable)
            {
                SetFlag(Flags.ExpandableFailed, true);
            }

            CategorizePropertyEntries();
            return expandable;
        }
        catch (Exception e) when (!e.IsCriticalException())
        {
            return false;
        }
    }
}
