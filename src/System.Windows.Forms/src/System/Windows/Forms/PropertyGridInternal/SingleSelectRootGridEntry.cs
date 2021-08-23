// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal class SingleSelectRootGridEntry : GridEntry, IRootGridEntry
    {
        protected object _value;
        private string _valueClassName;
        private GridEntry _defaultEntry;
        private IDesignerHost _host;
        private IServiceProvider _baseProvider;
        private PropertyTab _tab;
        private PropertyGridView _gridEntryHost;
        private AttributeCollection _browsableAttributes;
        private IComponentChangeService _changeService;
        protected bool _forceReadOnlyChecked;

        internal SingleSelectRootGridEntry(
            PropertyGridView gridEntryHost,
            object value,
            GridEntry parent,
            IServiceProvider baseProvider,
            IDesignerHost host,
            PropertyTab tab,
            PropertySort sortType)
            : base(gridEntryHost.OwnerGrid, parent)
        {
            Debug.Assert(value is not null, "Can't browse a null object!");
            _host = host;
            _gridEntryHost = gridEntryHost;
            _baseProvider = baseProvider;
            _tab = tab;
            _value = value;
            _valueClassName = TypeDescriptor.GetClassName(_value);

            IsExpandable = true;

            // Default to categories.
            _propertySort = sortType;
            InternalExpanded = true;
        }

        internal SingleSelectRootGridEntry(
            PropertyGridView view,
            object value,
            IServiceProvider baseProvider,
            IDesignerHost host,
            PropertyTab tab,
            PropertySort sortType) : this(view, value, null, baseProvider, host, tab, sortType)
        {
        }

        /// <summary>
        ///  The set of attributes that will be used for browse filtering.
        /// </summary>
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

                if (!same && Children is not null && Children.Count > 0)
                {
                    DisposeChildren();
                }
            }
        }

        protected override IComponentChangeService ComponentChangeService
            => _changeService ?? this.GetService<IComponentChangeService>();

        internal override bool AlwaysAllowExpand => true;

        public override PropertyTab CurrentTab
        {
            get => _tab;
            set => _tab = value;
        }

        internal override GridEntry DefaultChild
        {
            get => _defaultEntry;
            set => _defaultEntry = value;
        }

        internal override IDesignerHost DesignerHost
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
                    var readOnlyAttribute = (ReadOnlyAttribute)TypeDescriptor.GetAttributes(_value)[typeof(ReadOnlyAttribute)];
                    if ((readOnlyAttribute is not null && !readOnlyAttribute.IsDefaultAttribute())
                        || TypeDescriptor.GetAttributes(_value).Contains(InheritanceAttribute.InheritedReadOnly))
                    {
                        SetForceReadOnlyFlag();
                    }

                    _forceReadOnlyChecked = true;
                }

                return base.ForceReadOnly || (GridEntryHost is not null && !GridEntryHost.Enabled);
            }
        }

        internal override PropertyGridView GridEntryHost
        {
            get => _gridEntryHost;
            set => _gridEntryHost = value;
        }

        public override GridItemType GridItemType => GridItemType.Root;

        /// <summary>
        ///  Retrieves the keyword that the VS help dynamic help window will use when this IPE is selected.
        /// </summary>
        public override string HelpKeyword
        {
            get
            {
                var helpAttribute = (HelpKeywordAttribute)TypeDescriptor.GetAttributes(_value)[typeof(HelpKeywordAttribute)];

                if (helpAttribute is not null && !helpAttribute.IsDefaultAttribute())
                {
                    return helpAttribute.HelpKeyword;
                }

                return _valueClassName;
            }
        }

        public override string PropertyLabel
        {
            get
            {
                if (_value is IComponent component)
                {
                    ISite site = component.Site;
                    if (site is null)
                    {
                        return _value.GetType().Name;
                    }

                    return site.Name;
                }

                return _value?.ToString();
            }
        }

        /// <summary>
        ///  Gets or sets the value for the property that is represented by this GridEntry.
        /// </summary>
        public override object PropertyValue
        {
            get => _value;
            set
            {
                object old = _value;
                _value = value;
                _valueClassName = TypeDescriptor.GetClassName(_value);
                OwnerGrid.ReplaceSelectedObject(old, value);
            }
        }

        protected override bool CreateChildren()
        {
            bool expandable = base.CreateChildren();
            CategorizePropEntries();
            return expandable;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _host = null;
                _baseProvider = null;
                _tab = null;
                _gridEntryHost = null;
                _changeService = null;
            }

            _value = null;
            _valueClassName = null;
            _defaultEntry = null;
            base.Dispose(disposing);
        }

        public override object GetService(Type serviceType)
            => _host?.GetService(serviceType) ?? _baseProvider?.GetService(serviceType);

        /// <summary>
        ///  Reset the Browsable attributes to the default (<see cref="BrowsableAttribute.Yes"/>).
        /// </summary>
        public void ResetBrowsableAttributes() => _browsableAttributes = new(BrowsableAttribute.Yes);

        /// <summary>
        ///  Sets the value of this GridEntry from text.
        /// </summary>
        public virtual void ShowCategories(bool sortByCategories)
        {
            if ((_propertySort &= PropertySort.Categorized) != 0 != sortByCategories)
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

        internal void CategorizePropEntries()
        {
            if (Children.Count <= 0)
            {
                return;
            }

            var childEntries = new GridEntry[Children.Count];
            Children.CopyTo(childEntries, 0);

            if ((_propertySort & PropertySort.Categorized) == 0)
            {
                return;
            }

            // First, walk through all the entries and group them by their category.

            Hashtable categories = new();
            for (int i = 0; i < childEntries.Length; i++)
            {
                GridEntry child = childEntries[i];
                Debug.Assert(child is not null);
                if (child is not null)
                {
                    string category = child.PropertyCategory;
                    var gridEntries = (ArrayList)categories[category];
                    if (gridEntries is null)
                    {
                        gridEntries = new ArrayList();
                        categories[category] = gridEntries;
                    }

                    gridEntries.Add(child);
                }
            }

            // Now walk through and create a CategoryGridEntry that holds all the properties for each category.

            ArrayList categoryGridEntries = new();
            IDictionaryEnumerator enumerator = categories.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var gridEntries = (ArrayList)enumerator.Value;
                if (gridEntries is not null)
                {
                    string category = (string)enumerator.Key;
                    if (gridEntries.Count > 0)
                    {
                        var rgpes = new GridEntry[gridEntries.Count];
                        gridEntries.CopyTo(rgpes, 0);
                        try
                        {
                            categoryGridEntries.Add(new CategoryGridEntry(OwnerGrid, this, category, rgpes));
                        }
                        catch
                        {
                        }
                    }
                }
            }

            childEntries = new GridEntry[categoryGridEntries.Count];
            categoryGridEntries.CopyTo(childEntries, 0);
            Array.Sort(childEntries, GridEntryComparer.Default);

            ChildCollection.Clear();
            ChildCollection.AddRange(childEntries);
        }
    }
}
