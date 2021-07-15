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
        protected object _objValue;
        private string _objValueClassName;
        private GridEntry _propDefault;
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
            _objValue = value;
            _objValueClassName = TypeDescriptor.GetClassName(_objValue);

            IsExpandable = true;

            // Default to categories.
            PropertySort = sortType;
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
        ///  The set of attributes that will be used for browse filtering
        /// </summary>
        public override AttributeCollection BrowsableAttributes
        {
            get
            {
                if (_browsableAttributes is null)
                {
                    _browsableAttributes = new AttributeCollection(new Attribute[] { BrowsableAttribute.Yes });
                }

                return _browsableAttributes;
            }
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
                    var attr1 = new Attribute[_browsableAttributes.Count];
                    var attr2 = new Attribute[value.Count];

                    _browsableAttributes.CopyTo(attr1, 0);
                    value.CopyTo(attr2, 0);

                    Array.Sort(attr1, AttributeTypeSorter);
                    Array.Sort(attr2, AttributeTypeSorter);
                    for (int i = 0; i < attr1.Length; i++)
                    {
                        if (!attr1[i].Equals(attr2[i]))
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
        {
            get
            {
                if (_changeService is null)
                {
                    _changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                }

                return _changeService;
            }
        }

        internal override bool AlwaysAllowExpand => true;

        public override PropertyTab CurrentTab
        {
            get => _tab;
            set => _tab = value;
        }

        internal override GridEntry DefaultChild
        {
            get => _propDefault;
            set => _propDefault = value;
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
                    var readOnlyAttr = (ReadOnlyAttribute)TypeDescriptor.GetAttributes(_objValue)[typeof(ReadOnlyAttribute)];
                    if ((readOnlyAttr is not null && !readOnlyAttr.IsDefaultAttribute()) || TypeDescriptor.GetAttributes(_objValue).Contains(InheritanceAttribute.InheritedReadOnly))
                    {
                        _flags |= FLAG_FORCE_READONLY;
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
                var helpAttribute = (HelpKeywordAttribute)TypeDescriptor.GetAttributes(_objValue)[typeof(HelpKeywordAttribute)];

                if (helpAttribute is not null && !helpAttribute.IsDefaultAttribute())
                {
                    return helpAttribute.HelpKeyword;
                }

                return _objValueClassName;
            }
        }

        public override string PropertyLabel
        {
            get
            {
                if (_objValue is IComponent component)
                {
                    ISite site = component.Site;
                    if (site is null)
                    {
                        return _objValue.GetType().Name;
                    }

                    return site.Name;
                }

                if (_objValue is not null)
                {
                    return _objValue.ToString();
                }

                return null;
            }
        }

        /// <summary>
        ///  Gets or sets the value for the property that is represented by this GridEntry.
        /// </summary>
        public override object PropertyValue
        {
            get => _objValue;
            set
            {
                object old = _objValue;
                _objValue = value;
                _objValueClassName = TypeDescriptor.GetClassName(_objValue);
                OwnerGrid.ReplaceSelectedObject(old, value);
            }
        }

        protected override bool CreateChildren()
        {
            bool fReturn = base.CreateChildren();
            CategorizePropEntries();
            return fReturn;
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

            _objValue = null;
            _objValueClassName = null;
            _propDefault = null;
            base.Dispose(disposing);
        }

        public override object GetService(Type serviceType)
        {
            object service = null;

            if (_host is not null)
            {
                service = _host.GetService(serviceType);
            }

            if (service is null && _baseProvider is not null)
            {
                service = _baseProvider.GetService(serviceType);
            }

            return service;
        }

        /// <summary>
        ///  Reset the Browsable attributes to the default (BrowsableAttribute.Yes).
        /// </summary>
        public void ResetBrowsableAttributes()
        {
            _browsableAttributes = new AttributeCollection(new Attribute[] { BrowsableAttribute.Yes });
        }

        /// <summary>
        ///  Sets the value of this GridEntry from text.
        /// </summary>
        public virtual void ShowCategories(bool fCategories)
        {
            if ((PropertySort &= PropertySort.Categorized) != 0 != fCategories)
            {
                if (fCategories)
                {
                    PropertySort |= PropertySort.Categorized;
                }
                else
                {
                    PropertySort &= ~PropertySort.Categorized;
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

            if ((PropertySort & PropertySort.Categorized) == 0)
            {
                return;
            }

            // First, walk through all the entries and group them by their category by adding
            // them to a hashtable of arraylists.

            Hashtable bins = new();
            for (int i = 0; i < childEntries.Length; i++)
            {
                GridEntry pe = childEntries[i];
                Debug.Assert(pe is not null);
                if (pe is not null)
                {
                    string category = pe.PropertyCategory;
                    var bin = (ArrayList)bins[category];
                    if (bin is null)
                    {
                        bin = new ArrayList();
                        bins[category] = bin;
                    }

                    bin.Add(pe);
                }
            }

            // Now walk through the hashtable and create a categorygridentry for each
            // category that holds all the properties of that category.

            ArrayList propList = new();
            IDictionaryEnumerator enumBins = bins.GetEnumerator();
            while (enumBins.MoveNext())
            {
                var bin = (ArrayList)enumBins.Value;
                if (bin is not null)
                {
                    string category = (string)enumBins.Key;
                    if (bin.Count > 0)
                    {
                        var rgpes = new GridEntry[bin.Count];
                        bin.CopyTo(rgpes, 0);
                        try
                        {
                            propList.Add(new CategoryGridEntry(OwnerGrid, this, category, rgpes));
                        }
                        catch
                        {
                        }
                    }
                }
            }

            childEntries = new GridEntry[propList.Count];
            propList.CopyTo(childEntries, 0);
            Array.Sort(childEntries, GridEntryComparer.Default);

            ChildCollection.Clear();
            ChildCollection.AddRange(childEntries);
        }
    }
}
