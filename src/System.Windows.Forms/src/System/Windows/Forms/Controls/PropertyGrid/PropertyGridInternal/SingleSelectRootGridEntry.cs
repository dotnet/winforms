// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal;

/// <summary>
///  Root <see cref="GridEntry"/> for the <see cref="PropertyGrid"/> when there is only one object
///  in <see cref="PropertyGrid.SelectedObjects"/>.
/// </summary>
internal class SingleSelectRootGridEntry : GridEntry, IRootGridEntry
{
    private string? _valueClassName;
    private GridEntry? _defaultEntry;
    private IDesignerHost? _host;
    private IServiceProvider _baseProvider;
    private PropertyTab _ownerTab;
    private PropertyGridView _ownerGridView;
    private AttributeCollection? _browsableAttributes;
    private IComponentChangeService? _changeService;
    protected bool _forceReadOnlyChecked;

    internal SingleSelectRootGridEntry(
        PropertyGridView ownerGridView,
        object target,
        IServiceProvider baseProvider,
        IDesignerHost? host,
        PropertyTab ownerTab,
        PropertySort sortType)
        : base(ownerGridView.OwnerGrid, parent: null)
    {
        Debug.Assert(target is not null, "Can't browse a null object!");
        _host = host;
        _ownerGridView = ownerGridView;
        _baseProvider = baseProvider;
        _ownerTab = ownerTab;
        Target = target;
        _valueClassName = TypeDescriptor.GetClassName(Target);

        IsExpandable = true;

        // Default to categories.
        _propertySort = sortType;
        InternalExpanded = true;
    }

    /// <summary>
    ///  The target object for this root entry. This is either a single object or an array of objects from
    ///  <see cref="PropertyGrid.SelectedObjects" />
    /// </summary>
    protected object Target { get; private set; }

    [AllowNull]
    public override AttributeCollection BrowsableAttributes
    {
        get => _browsableAttributes ??= new(BrowsableAttribute.Yes);
        set
        {
            if (value is null)
            {
                ResetBrowsableAttributes();
                return;
            }

            bool same = true;

            if (_browsableAttributes is not null && value is not null && _browsableAttributes.Count == value.Count)
            {
                var currentAttributes = new Attribute[_browsableAttributes.Count];
                var newAttributes = new Attribute[value.Count];

                _browsableAttributes.CopyTo(currentAttributes, 0);
                value.CopyTo(newAttributes, 0);

                Array.Sort(currentAttributes, AttributeTypeSorter);
                Array.Sort(newAttributes, AttributeTypeSorter);
                for (int i = 0; i < currentAttributes.Length; i++)
                {
                    if (!currentAttributes[i].Equals(newAttributes[i]))
                    {
                        same = false;
                        break;
                    }
                }
            }
            else
            {
                same = false;
            }

            _browsableAttributes = value;

            if (!same && ChildCount > 0)
            {
                DisposeChildren();
            }
        }
    }

    protected override IComponentChangeService? ComponentChangeService
        => _changeService ?? this.GetService<IComponentChangeService>();

    public override PropertyTab OwnerTab => _ownerTab;

    internal sealed override GridEntry? DefaultChild
    {
        get => _defaultEntry;
        set => _defaultEntry = value;
    }

    internal sealed override IDesignerHost? DesignerHost
    {
        get => _host;
        set => _host = value;
    }

    internal override bool ForceReadOnly
    {
        get
        {
            if (!_forceReadOnlyChecked)
            {
                if ((TypeDescriptorHelper.TryGetAttribute(Target, out ReadOnlyAttribute? readOnlyAttribute)
                    && !readOnlyAttribute.IsDefaultAttribute())
                    || TypeDescriptor.GetAttributes(Target).Contains(InheritanceAttribute.InheritedReadOnly))
                {
                    SetForceReadOnlyFlag();
                }

                _forceReadOnlyChecked = true;
            }

            return base.ForceReadOnly || (OwnerGridView is not null && !OwnerGridView.Enabled);
        }
    }

    internal override PropertyGridView OwnerGridView
    {
        get => _ownerGridView;
        set => _ownerGridView = value;
    }

    public override GridItemType GridItemType => GridItemType.Root;

    public override string? HelpKeyword
    {
        get
        {
            if (TypeDescriptorHelper.TryGetAttribute(Target, out HelpKeywordAttribute? helpAttribute)
                && !helpAttribute.IsDefaultAttribute())
            {
                return helpAttribute.HelpKeyword;
            }

            return _valueClassName;
        }
    }

    public override string? PropertyLabel
    {
        get
        {
            if (Target is IComponent component)
            {
                return component.Site?.Name ?? Target.GetType().Name;
            }

            return Target?.ToString();
        }
    }

    [AllowNull]
    public override object PropertyValue
    {
        get => Target;
        set
        {
            object old = Target;
            if (value is not null)
            {
                Target = value;
                _valueClassName = TypeDescriptor.GetClassName(Target);
                OwnerGrid.ReplaceSelectedObject(old, value);
            }
        }
    }

    protected override bool CreateChildren(bool diffOldChildren = false)
    {
        bool expandable = base.CreateChildren(diffOldChildren);
        CategorizePropertyEntries();
        return expandable;
    }

    protected override void Dispose(bool disposing)
    {
        _host = null;
        _baseProvider = null!;
        _ownerTab = null!;
        _ownerGridView = null!;
        _changeService = null;

        Target = null!;
        _valueClassName = null;
        _defaultEntry = null;
        base.Dispose(disposing);
    }

    public override object? GetService(Type serviceType)
        => _host?.GetService(serviceType) ?? _baseProvider?.GetService(serviceType);

    public void ResetBrowsableAttributes() => _browsableAttributes = new(BrowsableAttribute.Yes);

    public virtual void ShowCategories(bool sortByCategories)
    {
        if (((_propertySort &= PropertySort.Categorized) != 0) != sortByCategories)
        {
            if (sortByCategories)
            {
                _propertySort |= PropertySort.Categorized;
            }
            else
            {
                _propertySort &= ~PropertySort.Categorized;
            }

            // Recreate the children.
            if (Expandable && ChildCollection is not null)
            {
                CreateChildren();
            }
        }
    }

    /// <summary>
    ///  Groups all children under category grid entries.
    /// </summary>
    protected void CategorizePropertyEntries()
    {
        if (Children.Count == 0 || (_propertySort & PropertySort.Categorized) == 0)
        {
            return;
        }

        // First, walk through all the entries and group them by their category.

        Dictionary<string, List<GridEntry>> categories = [];
        foreach (var child in Children)
        {
            string category = child.PropertyCategory;
            if (!categories.TryGetValue(category, out var gridEntries))
            {
                gridEntries = [];
                categories[category] = gridEntries;
            }

            gridEntries.Add(child);
        }

        // Now walk through and create a CategoryGridEntry that holds all the properties for each category.

        List<GridEntry> categoryGridEntries = [];
        foreach (var entry in categories)
        {
            var gridEntries = entry.Value;
            if (gridEntries.Count > 0)
            {
                categoryGridEntries.Add(new CategoryGridEntry(OwnerGrid, this, entry.Key, entry.Value));
            }
        }

        categoryGridEntries.Sort(GridEntryComparer.Default);
        ChildCollection.Clear();
        ChildCollection.AddRange(categoryGridEntries);
    }
}
