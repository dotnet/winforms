// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
#if DEBUG
using System.Globalization;
#endif
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    /// <summary>
    ///  Base Entry for properties to be displayed in properties window.
    /// </summary>
    internal abstract partial class GridEntry : GridItem, ITypeDescriptorContext
    {
        protected static Point InvalidPoint { get; } = new(int.MinValue, int.MinValue);

        private static readonly BooleanSwitch s_pbrsAssertPropsSwitch
            = new("PbrsAssertProps", "PropertyBrowser : Assert on broken properties");

        internal static AttributeTypeSorter AttributeTypeSorter { get; } = new();

        protected static IComparer DisplayNameComparer { get; } = new DisplayNameSortComparer();

        private static char s_passwordReplaceChar;

        // Maximum number of characters we'll show in the property grid.
        // Too many characters leads to bad performance.
        private const int MaximumLengthOfPropertyString = 1000;

        private EventEntry _eventList;
        private CacheItems _cacheItems;

        protected TypeConverter Converter { get; set; }
        protected UITypeEditor Editor { get; set; }

        private GridEntry _parent;
        private GridEntryCollection _children;
        private int _propertyDepth;
        private bool _hasFocus;
        private Rectangle _outlineRect = Rectangle.Empty;

        private Flags _flags;
        protected PropertySort _propertySort;

        private Point _labelTipPoint = InvalidPoint;
        private Point _valueTipPoint = InvalidPoint;

        private static readonly object s_valueClickEvent = new();
        private static readonly object s_labelClickEvent = new();
        private static readonly object s_outlineClickEvent = new();
        private static readonly object s_valueDoubleClickEvent = new();
        private static readonly object s_labelDoubleClickEvent = new();
        private static readonly object s_outlineDoubleClickEvent = new();
        private static readonly object s_recreateChildrenEvent = new();

        private GridEntryAccessibleObject _accessibleObject;

        private bool _lastPaintWithExplorerStyle;

        private static Color InvertColor(Color color)
            => Color.FromArgb(color.A, (byte)~color.R, (byte)~color.G, (byte)~color.B);

        protected GridEntry(PropertyGrid owner, GridEntry parent)
        {
            _parent = parent;
            OwnerGrid = owner;

            Debug.Assert(OwnerGrid is not null, "GridEntry w/o PropertyGrid owner, text rendering will fail.");

            if (parent is not null)
            {
                _propertyDepth = parent.PropertyDepth + 1;
                _propertySort = parent._propertySort;

                if (parent.ForceReadOnly)
                {
                    SetForceReadOnlyFlag();
                }
            }
            else
            {
                _propertyDepth = -1;
            }
        }

        /// <summary>
        ///  Outline Icon padding
        /// </summary>
        internal int OutlineIconPadding
        {
            get
            {
                const int OutlineIconPaddingDefault = 5;

                if (DpiHelper.IsScalingRequirementMet)
                {
                    if (GridEntryHost is not null)
                    {
                        return GridEntryHost.LogicalToDeviceUnits(OutlineIconPaddingDefault);
                    }
                }

                return OutlineIconPaddingDefault;
            }
        }

        private bool ColorInversionNeededInHC => SystemInformation.HighContrast && !OwnerGrid._developerOverride;

        public AccessibleObject AccessibilityObject => _accessibleObject ??= GetAccessibilityObject();

        protected virtual GridEntryAccessibleObject GetAccessibilityObject() => new(this);

        /// <summary>
        ///  Specify that this grid entry should be allowed to be merged for multi-select.
        /// </summary>
        public virtual bool AllowMerge => true;

        internal virtual bool AlwaysAllowExpand => false;

        internal virtual AttributeCollection Attributes => TypeDescriptor.GetAttributes(PropertyType);

        /// <summary>
        ///  Gets the value of the background brush to use. Override this member to cause the entry to paint it's
        ///  background in a different color. The base implementation returns null.
        /// </summary>
        protected virtual Color GetBackgroundColor() => GridEntryHost.BackColor;

        protected virtual Color LabelTextColor
            => ShouldRenderReadOnly ? GridEntryHost.GrayTextColor : GridEntryHost.GetTextColor();

        /// <summary>
        ///  The set of attributes that will be used for browse filtering
        /// </summary>
        public virtual AttributeCollection BrowsableAttributes
        {
            get => _parent?.BrowsableAttributes;
            set => _parent.BrowsableAttributes = value;
        }

        /// <summary>
        ///  Retrieves the component that is invoking the method on the formatter object.  This may
        ///  return null if there is no component responsible for the call.
        /// </summary>
        public virtual IComponent Component
            => GetValueOwner() is IComponent component ? component : (_parent?.Component);

        protected virtual IComponentChangeService ComponentChangeService => _parent.ComponentChangeService;

        /// <summary>
        ///  Retrieves the container that contains the set of objects this formatter may work
        ///  with.  It may return null if there is no container, or of the formatter should not
        ///  use any outside objects.
        /// </summary>
        public virtual IContainer Container => Component?.Site?.Container;

        protected GridEntryCollection ChildCollection
        {
            get => _children ??= new GridEntryCollection(this, entries: null);
            set
            {
                Debug.Assert(value is null || !Disposed, "Why are we putting new children in after we are disposed?");
                if (_children != value)
                {
                    if (_children is not null)
                    {
                        _children.Dispose();
                        _children = null;
                    }

                    _children = value;
                }
            }
        }

        public int ChildCount => Children?.Count ?? 0;

        public virtual GridEntryCollection Children
        {
            get
            {
                if (_children is null && !Disposed)
                {
                    CreateChildren();
                }

                return _children;
            }
        }

        public virtual PropertyTab CurrentTab
        {
            get => _parent?.CurrentTab;
            set
            {
                if (_parent is not null)
                {
                    _parent.CurrentTab = value;
                }
            }
        }

        /// <summary>
        ///  Returns the default child GridEntry of this item.  Usually the default property
        ///  of the target object.
        /// </summary>
        internal virtual GridEntry DefaultChild
        {
            get => null;
            set { }
        }

        internal virtual IDesignerHost DesignerHost
        {
            get => _parent?.DesignerHost;
            set
            {
                if (_parent is not null)
                {
                    _parent.DesignerHost = value;
                }
            }
        }

        internal bool Disposed => GetFlagSet(Flags.Disposed);

        internal virtual bool Enumerable => (EntryFlags & Flags.Enumerable) != 0;

        public override bool Expandable
        {
            get
            {
                bool expandable = GetFlagSet(Flags.Expandable);

                if (expandable && _children is not null && _children.Count > 0)
                {
                    return true;
                }

                if (GetFlagSet(Flags.ExpandableFailed))
                {
                    return false;
                }

                if (expandable && (_cacheItems is null || _cacheItems.LastValue is null) && PropertyValue is null)
                {
                    expandable = false;
                }

                return expandable;
            }
        }

        public override bool Expanded
        {
            get => InternalExpanded;
            set => GridEntryHost.SetExpand(this, value);
        }

        internal virtual bool ForceReadOnly => (_flags & Flags.ForceReadOnly) != 0;

        protected void SetForceReadOnlyFlag() => _flags |= Flags.ForceReadOnly;

        internal virtual bool InternalExpanded
        {
            get
            {
                // Short circuit if we don't have children.
                if (_children is null || _children.Count == 0)
                {
                    return false;
                }

                return GetFlagSet(Flags.Expand);
            }
            set
            {
                if (!Expandable || value == InternalExpanded)
                {
                    return;
                }

                if (_children is not null && _children.Count > 0)
                {
                    SetFlag(Flags.Expand, value);
                }
                else
                {
                    SetFlag(Flags.Expand, false);
                    if (value)
                    {
                        bool fMakeSure = CreateChildren();
                        SetFlag(Flags.Expand, fMakeSure);
                    }
                }

                // Notify accessibility clients of expanded state change
                // StateChange requires NameChange, too - accessible clients won't see this, unless both events are raised

                // Root item is hidden and should not raise events
                if (GridItemType != GridItemType.Root)
                {
                    int id = GridEntryHost.AccessibilityGetGridEntryChildID(this);
                    if (id >= 0)
                    {
                        var gridAccObj = (PropertyGridView.PropertyGridViewAccessibleObject)GridEntryHost.AccessibilityObject;
                        gridAccObj.NotifyClients(AccessibleEvents.StateChange, id);
                        gridAccObj.NotifyClients(AccessibleEvents.NameChange, id);
                    }
                }
            }
        }

        public virtual Flags EntryFlags
        {
            get
            {
                if ((_flags & Flags.Checked) != 0)
                {
                    return _flags;
                }

                _flags |= Flags.Checked;

                TypeConverter converter = TypeConverter;
                UITypeEditor uiEditor = UITypeEditor;
                object value = Instance;
                bool forceReadOnly = ForceReadOnly;

                if (value is not null)
                {
                    forceReadOnly |= TypeDescriptor.GetAttributes(value).Contains(InheritanceAttribute.InheritedReadOnly);
                }

                if (converter.GetStandardValuesSupported(this))
                {
                    _flags |= Flags.Enumerable;
                }

                if (!forceReadOnly && converter.CanConvertFrom(this, typeof(string)) &&
                    !converter.GetStandardValuesExclusive(this))
                {
                    _flags |= Flags.TextEditable;
                }

                bool isImmutableReadOnly = TypeDescriptor.GetAttributes(PropertyType)[typeof(ImmutableObjectAttribute)]
                    .Equals(ImmutableObjectAttribute.Yes);
                bool isImmutable = isImmutableReadOnly || converter.GetCreateInstanceSupported(this);

                if (isImmutable)
                {
                    _flags |= Flags.Immutable;
                }

                if (converter.GetPropertiesSupported(this))
                {
                    _flags |= Flags.Expandable;

                    // If we're expandable, but we don't support editing,
                    // make us read only editable so we don't paint grey.

                    if (!forceReadOnly && (_flags & Flags.TextEditable) == 0 && !isImmutableReadOnly)
                    {
                        _flags |= Flags.ReadOnlyEditable;
                    }
                }

                if (Attributes.Contains(PasswordPropertyTextAttribute.Yes))
                {
                    _flags |= Flags.RenderPassword;
                }

                if (uiEditor is not null)
                {
                    if (uiEditor.GetPaintValueSupported(this))
                    {
                        _flags |= Flags.CustomPaint;
                    }

                    // We only allow drop-downs if the object is NOT being inherited.

                    bool allowButtons = !forceReadOnly;

                    if (allowButtons)
                    {
                        switch (uiEditor.GetEditStyle(this))
                        {
                            case UITypeEditorEditStyle.Modal:
                                _flags |= Flags.CustomEditable;
                                if (!isImmutable && !PropertyType.IsValueType)
                                {
                                    _flags |= Flags.ReadOnlyEditable;
                                }

                                break;
                            case UITypeEditorEditStyle.DropDown:
                                _flags |= Flags.DropdownEditable;
                                break;
                        }
                    }
                }

                return _flags;
            }
        }

        protected void ClearFlags() => _flags = 0;

        /// <summary>
        ///  Checks if the entry is currently expanded.
        /// </summary>
        public bool HasFocus
        {
            get => _hasFocus;
            set
            {
                if (Disposed)
                {
                    return;
                }

                if (_cacheItems is not null)
                {
                    _cacheItems.LastValueString = null;
                    _cacheItems.UseValueString = false;
                    _cacheItems.UseShouldSerialize = false;
                }

                if (_hasFocus != value)
                {
                    _hasFocus = value;

                    // Notify accessibility applications that keyboard focus has changed.
                    if (value == true)
                    {
                        int id = GridEntryHost.AccessibilityGetGridEntryChildID(this);
                        if (id >= 0)
                        {
                            var gridAccObj = (PropertyGridView.PropertyGridViewAccessibleObject)GridEntryHost.AccessibilityObject;
                            gridAccObj.NotifyClients(AccessibleEvents.Focus, id);
                            gridAccObj.NotifyClients(AccessibleEvents.Selection, id);

                            AccessibilityObject.SetFocus();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Returns the label including the object name, and properties.  For example, the value
        ///  of the Font size property on a Button called Button1 would be "Button1.Font.Size"
        /// </summary>
        public string FullLabel
        {
            get
            {
                string label = _parent?.FullLabel;

                if (label is not null)
                {
                    label += ".";
                }
                else
                {
                    label = string.Empty;
                }

                label += PropertyLabel;

                return label;
            }
        }

        public override GridItemCollection GridItems
        {
            get
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException(SR.GridItemDisposed);
                }

                if (IsExpandable && _children is not null && _children.Count == 0)
                {
                    CreateChildren();
                }

                return Children;
            }
        }

        internal virtual PropertyGridView GridEntryHost
        {
            get => _parent?.GridEntryHost;
            set => throw new NotSupportedException();
        }

        public override GridItemType GridItemType => GridItemType.Property;

        /// <summary>
        ///  Returns true if this GridEntry has a value field in the right hand column.
        /// </summary>
        internal virtual bool HasValue => true;

        /// <summary>
        ///  Retrieves the keyword that the VS help dynamic help window will
        ///  use when this IPE is selected.
        /// </summary>
        public virtual string HelpKeyword
        {
            get
            {
                string keyWord = null;

                if (_parent is not null)
                {
                    keyWord = _parent.HelpKeyword;
                }

                if (keyWord is null)
                {
                    keyWord = string.Empty;
                }

                return keyWord;
            }
        }

        internal virtual string HelpKeywordInternal => HelpKeyword;

        public virtual bool IsCustomPaint
        {
            get
            {
                // Prevent full flag population if possible.
                if ((_flags & Flags.Checked) == 0)
                {
                    UITypeEditor typeEd = UITypeEditor;
                    if (typeEd is not null)
                    {
                        if ((_flags & Flags.CustomPaint) != 0 || (_flags & Flags.NoCustomPaint) != 0)
                        {
                            return (_flags & Flags.CustomPaint) != 0;
                        }

                        if (typeEd.GetPaintValueSupported(this))
                        {
                            _flags |= Flags.CustomPaint;
                            return true;
                        }
                        else
                        {
                            _flags |= Flags.NoCustomPaint;
                            return false;
                        }
                    }
                }

                return (EntryFlags & Flags.CustomPaint) != 0;
            }
        }

        public virtual bool IsExpandable
        {
            get => Expandable;
            set
            {
                if (value != GetFlagSet(Flags.Expandable))
                {
                    SetFlag(Flags.ExpandableFailed, false);
                    SetFlag(Flags.Expandable, value);
                }
            }
        }

        public virtual bool IsTextEditable => IsValueEditable && (EntryFlags & Flags.TextEditable) != 0;

        public virtual bool IsValueEditable
            => !ForceReadOnly
            && 0 != (EntryFlags & (Flags.DropdownEditable | Flags.TextEditable | Flags.CustomEditable | Flags.Enumerable));

        public bool IsImmediatelyEditable => (EntryFlags & Flags.ImmediatelyEditable) != 0;

        /// <summary>
        ///  Retrieves the component that is invoking the method on the formatter object.  This may
        ///  return null if there is no component responsible for the call.
        /// </summary>
        public virtual object Instance
        {
            get
            {
                object owner = GetValueOwner();

                if (_parent is not null && owner is null)
                {
                    return _parent.Instance;
                }

                return owner;
            }
        }

        public override string Label => PropertyLabel;

        /// <summary>
        ///  Retrieves the PropertyDescriptor that is surfacing the given object/
        /// </summary>
        public override PropertyDescriptor PropertyDescriptor => null;

        /// <summary>
        ///  Returns the pixel indent of the current GridEntry's label.
        /// </summary>
        internal virtual int PropertyLabelIndent
        {
            get
            {
                int borderWidth = GridEntryHost.GetOutlineIconSize() + OutlineIconPadding;
                return ((_propertyDepth + 1) * borderWidth) + 1;
            }
        }

        internal virtual Point GetLabelToolTipLocation(int mouseX, int mouseY) => _labelTipPoint;

        internal virtual string LabelToolTipText => PropertyLabel;

        public virtual bool NeedsDropDownButton => (EntryFlags & Flags.DropdownEditable) != 0;

        public virtual bool NeedsCustomEditorButton
            => (EntryFlags & Flags.CustomEditable) != 0
                && (IsValueEditable || (EntryFlags & Flags.ReadOnlyEditable) != 0);

        public PropertyGrid OwnerGrid { get; }

        /// <summary>
        ///  Returns rect that the outline icon (+ or - or arrow) will be drawn into, relative
        ///  to the upper left corner of the GridEntry.
        /// </summary>
        public Rectangle OutlineRect
        {
            get
            {
                if (!_outlineRect.IsEmpty)
                {
                    return _outlineRect;
                }

                PropertyGridView gridHost = GridEntryHost;
                Debug.Assert(gridHost is not null, "No propEntryHost!");
                int outlineSize = gridHost.GetOutlineIconSize();
                int borderWidth = outlineSize + OutlineIconPadding;
                int left = (_propertyDepth * borderWidth) + OutlineIconPadding / 2;
                int top = (gridHost.GetGridEntryHeight() - outlineSize) / 2;
                _outlineRect = new Rectangle(left, top, outlineSize, outlineSize);
                return _outlineRect;
            }
            set
            {
                // Set property is required to reset cached value when dpi changed.
                if (value != _outlineRect)
                {
                    _outlineRect = value;
                }
            }
        }

        public virtual GridEntry ParentGridEntry
        {
            get => _parent;
            set
            {
                Debug.Assert(value != this, "how can we be our own parent?");
                _parent = value;
                if (value is not null)
                {
                    _propertyDepth = value.PropertyDepth + 1;

                    if (_children is not null)
                    {
                        for (int i = 0; i < _children.Count; i++)
                        {
                            _children.GetEntry(i).ParentGridEntry = this;
                        }
                    }
                }
            }
        }

        public override GridItem Parent
        {
            get
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException(SR.GridItemDisposed);
                }

                return ParentGridEntry;
            }
        }

        /// <summary>
        ///  Returns category name of the current property
        /// </summary>
        public virtual string PropertyCategory => CategoryAttribute.Default.Category;

        /// <summary>
        ///  Returns "depth" of this property.  That is, how many parents between
        ///  this property and the root property.  The root property has a depth of -1.
        /// </summary>
        public virtual int PropertyDepth => _propertyDepth;

        /// <summary>
        ///  Returns the description helpstring for this GridEntry.
        /// </summary>
        public virtual string PropertyDescription => null;

        /// <summary>
        ///  Returns the label of this property. Usually this is the property name.
        /// </summary>
        public virtual string PropertyLabel => null;

        /// <summary>
        ///  Returns non-localized name of this property.
        /// </summary>
        public virtual string PropertyName => PropertyLabel;

        /// <summary>
        ///  Returns the Type of the value of this GridEntry, or null if the value is null.
        /// </summary>
        public virtual Type PropertyType => PropertyValue?.GetType();

        /// <summary>
        ///  Gets or sets the value for the property that is represented by this GridEntry.
        /// </summary>
        public virtual object PropertyValue
        {
            get => _cacheItems?.LastValue;
            set { }
        }

        public virtual bool ShouldRenderPassword => (EntryFlags & Flags.RenderPassword) != 0;

        public virtual bool ShouldRenderReadOnly
             => ForceReadOnly
                || 0 != (EntryFlags & Flags.RenderReadOnly)
                || (!IsValueEditable && (0 == (EntryFlags & Flags.ReadOnlyEditable)));

        /// <summary>
        ///  Returns the type converter for this entry.
        /// </summary>
        internal virtual TypeConverter TypeConverter
        {
            get
            {
                if (Converter is null)
                {
                    object value = PropertyValue;
                    if (value is null)
                    {
                        Converter = TypeDescriptor.GetConverter(PropertyType);
                    }
                    else
                    {
                        Converter = TypeDescriptor.GetConverter(value);
                    }
                }

                return Converter;
            }
        }

        /// <summary>
        ///  Returns the type editor for this entry.  This may return null if there is no type editor.
        /// </summary>
        internal virtual UITypeEditor UITypeEditor
        {
            get
            {
                if (Editor is null && PropertyType is not null)
                {
                    Editor = (UITypeEditor)TypeDescriptor.GetEditor(PropertyType, typeof(UITypeEditor));
                }

                return Editor;
            }
        }

        // note: we don't do set because of the value class semantics, etc.

        public override object Value => PropertyValue;

        internal Point ValueToolTipLocation
        {
            get => ShouldRenderPassword ? InvalidPoint : _valueTipPoint;
            set => _valueTipPoint = value;
        }

        internal int VisibleChildCount
        {
            get
            {
                if (!Expanded)
                {
                    return 0;
                }

                int count = ChildCount;
                int totalCount = count;
                for (int i = 0; i < count; i++)
                {
                    totalCount += ChildCollection.GetEntry(i).VisibleChildCount;
                }

                return totalCount;
            }
        }

        /// <summary>
        ///  Add an event handler to be invoked when the label portion of the property entry is clicked.
        /// </summary>
        public virtual void AddOnLabelClick(EventHandler h) => AddEventHandler(s_labelClickEvent, h);

        /// <summary>
        ///  Add an event handler to be invoked when the label portion of the property entry is double-clicked.
        /// </summary>
        public virtual void AddOnLabelDoubleClick(EventHandler h) => AddEventHandler(s_labelDoubleClickEvent, h);

        /// <summary>
        ///  Add an event handler to be invoked when the value portion of the property entry is clicked.
        /// </summary>
        public virtual void AddOnValueClick(EventHandler h) => AddEventHandler(s_valueClickEvent, h);

        /// <summary>
        ///  Add an event handler to be invoked when the value portion of the prop entry is double-clicked.
        /// </summary>
        public virtual void AddOnValueDoubleClick(EventHandler h) => AddEventHandler(s_valueDoubleClickEvent, h);

        /// <summary>
        ///  Add an event handler to be invoked when the outline icon portion of the prop entry is clicked
        /// </summary>
        public virtual void AddOnOutlineClick(EventHandler h) => AddEventHandler(s_outlineClickEvent, h);

        /// <summary>
        ///  Add an event handler to be invoked when the outline icon portion of the prop entry is double clicked.
        /// </summary>
        public virtual void AddOnOutlineDoubleClick(EventHandler h) => AddEventHandler(s_outlineDoubleClickEvent, h);

        /// <summary>
        ///  Add an event handler to be invoked when the children grid entries are re-created.
        /// </summary>
        public virtual void AddOnRecreateChildren(GridEntryRecreateChildrenEventHandler h) => AddEventHandler(s_recreateChildrenEvent, h);

        internal void ClearCachedValues() => ClearCachedValues(clearChildren: true);

        internal void ClearCachedValues(bool clearChildren)
        {
            if (_cacheItems is not null)
            {
                _cacheItems.UseValueString = false;
                _cacheItems.LastValue = null;
                _cacheItems.UseShouldSerialize = false;
            }

            if (clearChildren)
            {
                for (int i = 0; i < ChildCollection.Count; i++)
                {
                    ChildCollection.GetEntry(i).ClearCachedValues();
                }
            }
        }

        /// <summary>
        ///  Converts the given string of text to a value.
        /// </summary>
        public object ConvertTextToValue(string text)
        {
            if (TypeConverter.CanConvertFrom(this, typeof(string)))
            {
                return TypeConverter.ConvertFromString(this, text);
            }

            return text;
        }

        /// <summary>
        ///  Create the base prop entries given an object or set of objects
        /// </summary>
        internal static IRootGridEntry Create(
            PropertyGridView view,
            object[] objects,
            IServiceProvider baseProvider,
            IDesignerHost currentHost,
            PropertyTab tab,
            PropertySort initialSortType)
        {
            IRootGridEntry entry;

            if (objects is null || objects.Length == 0)
            {
                return null;
            }

            try
            {
                if (objects.Length == 1)
                {
                    entry = new SingleSelectRootGridEntry(view, objects[0], baseProvider, currentHost, tab, initialSortType);
                }
                else
                {
                    entry = new MultiSelectRootGridEntry(view, objects, baseProvider, currentHost, tab, initialSortType);
                }
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString());
                throw;
            }

            return entry;
        }

        /// <summary>
        ///  Populates the children of this grid entry.
        /// </summary>
        /// <returns>True if the children are expandable.</returns>
        protected virtual bool CreateChildren() => CreateChildren(diffOldChildren: false);

        /// <summary>
        ///  Populates the children of this grid entry.
        /// </summary>
        /// <returns>True if the children are expandable.</returns>
        protected virtual bool CreateChildren(bool diffOldChildren)
        {
            Debug.Assert(!Disposed, "Why are we creating children after we are disposed?");

            if (!GetFlagSet(Flags.Expandable))
            {
                if (_children is not null)
                {
                    _children.Clear();
                }
                else
                {
                    _children = new GridEntryCollection(this, Array.Empty<GridEntry>());
                }

                return false;
            }

            if (!diffOldChildren && _children is not null && _children.Count > 0)
            {
                return true;
            }

            GridEntry[] childProperties = GetPropEntries(this, PropertyValue, PropertyType);

            bool expandable = childProperties is not null && childProperties.Length > 0;

            if (diffOldChildren && _children is not null && _children.Count > 0)
            {
                bool same = true;
                if (childProperties.Length == _children.Count)
                {
                    for (int i = 0; i < childProperties.Length; i++)
                    {
                        if (!childProperties[i].NonParentEquals(_children[i]))
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

                if (same)
                {
                    return true;
                }
            }

            if (!expandable)
            {
                SetFlag(Flags.ExpandableFailed, true);
                if (_children is not null)
                {
                    _children.Clear();
                }
                else
                {
                    _children = new GridEntryCollection(this, Array.Empty<GridEntry>());
                }

                if (InternalExpanded)
                {
                    InternalExpanded = false;
                }
            }
            else
            {
                if (_children is not null)
                {
                    _children.Clear();
                    _children.AddRange(childProperties);
                }
                else
                {
                    _children = new GridEntryCollection(this, childProperties);
                }
            }

            return expandable;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Make sure we don't accidentally check flags while disposing.
            _flags |= Flags.Checked;

            SetFlag(Flags.Disposed, true);

            _cacheItems = null;
            Converter = null;
            Editor = null;
            _accessibleObject = null;

            if (disposing)
            {
                DisposeChildren();
            }
        }

        /// <summary>
        ///  Disposes the array of children.
        /// </summary>
        public virtual void DisposeChildren()
        {
            _children?.Dispose();
            _children = null;
        }

        ~GridEntry() => Dispose(disposing: false);

        /// <summary>
        ///  Invokes the type editor for editing this item.
        /// </summary>
        internal virtual void EditPropertyValue(PropertyGridView gridView)
        {
            if (UITypeEditor is null)
            {
                return;
            }

            try
            {
                // Since edit value can push a modal loop there is a chance that this gridentry will be zombied
                // before it returns.  Make sure we're not disposed.

                object originalValue = PropertyValue;
                object value = UITypeEditor.EditValue(this, this, originalValue);

                if (Disposed)
                {
                    return;
                }

                // Push the new value back into the property
                if (value != originalValue && IsValueEditable)
                {
                    gridView.CommitValue(this, value);
                }

                if (InternalExpanded)
                {
                    // If the edited property is expanded to show sub-properties, then we want to
                    // preserve the expanded states of it and all of its descendants. RecreateChildren()
                    // has logic that is supposed to do this, but it doesn't do so correctly.
                    PropertyGridView.GridPositionData positionData = GridEntryHost.CaptureGridPositionData();
                    InternalExpanded = false;
                    RecreateChildren();
                    positionData.Restore(GridEntryHost);
                }
                else
                {
                    // If edited property has no children or is collapsed, we don't need to preserve expanded states.
                    RecreateChildren();
                }
            }
            catch (Exception e)
            {
                if (this.TryGetService(out IUIService uiService))
                {
                    uiService.ShowError(e);
                }
                else
                {
                    RTLAwareMessageBox.Show(
                        GridEntryHost,
                        e.Message,
                        SR.PBRSErrorTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        0);
                }
            }
        }

        /// <summary>
        ///  Tests two GridEntries for equality.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (NonParentEquals(obj))
            {
                return ((GridEntry)obj).ParentGridEntry == ParentGridEntry;
            }

            return false;
        }

        /// <summary>
        ///  Searches for a value of a given property for a value editor user.
        /// </summary>
        public virtual object FindPropertyValue(string propertyName, Type propertyType)
        {
            object owner = GetValueOwner();
            PropertyDescriptor property = TypeDescriptor.GetProperties(owner)[propertyName];
            if (property is not null && property.PropertyType == propertyType)
            {
                return property.GetValue(owner);
            }

            return _parent?.FindPropertyValue(propertyName, propertyType);
        }

        /// <summary>
        ///  Returns the index of a child GridEntry.
        /// </summary>
        internal virtual int GetChildIndex(GridEntry entry) => Children.GetEntry(entry);

        /// <summary>
        ///  Gets the components that own the current value.  This is usually the value of the
        ///  root entry, which is the object being browsed.  Walks up the GridEntry tree
        ///  looking for an owner that is an IComponent
        /// </summary>
        public virtual IComponent[] GetComponents()
        {
            IComponent component = Component;
            if (component is not null)
            {
                return new IComponent[] { component };
            }

            return null;
        }

        protected int GetLabelTextWidth(string labelText, Graphics graphics, Font font)
        {
            if (_cacheItems is null)
            {
                _cacheItems = new CacheItems();
            }
            else if (_cacheItems.UseCompatTextRendering == OwnerGrid.UseCompatibleTextRendering
                && _cacheItems.LastLabel == labelText
                && font.Equals(_cacheItems.LastLabelFont))
            {
                return _cacheItems.LastLabelWidth;
            }

            SizeF textSize = PropertyGrid.MeasureTextHelper.MeasureText(OwnerGrid, graphics, labelText, font);

            _cacheItems.LastLabelWidth = (int)textSize.Width;
            _cacheItems.LastLabel = labelText;
            _cacheItems.LastLabelFont = font;
            _cacheItems.UseCompatTextRendering = OwnerGrid.UseCompatibleTextRendering;

            return _cacheItems.LastLabelWidth;
        }

        internal int GetValueTextWidth(string valueString, Graphics graphics, Font font)
        {
            if (_cacheItems is null)
            {
                _cacheItems = new CacheItems();
            }
            else if (_cacheItems.LastValueTextWidth != -1
                && _cacheItems.LastValueString == valueString
                && font.Equals(_cacheItems.LastValueFont))
            {
                return _cacheItems.LastValueTextWidth;
            }

            // Value text is rendered using GDI directly (No TextRenderer) but measured/adjusted using GDI+
            // (since previous releases), so don't use MeasureTextHelper.
            _cacheItems.LastValueTextWidth = (int)graphics.MeasureString(valueString, font).Width;
            _cacheItems.LastValueString = valueString;
            _cacheItems.LastValueFont = font;
            return _cacheItems.LastValueTextWidth;
        }

        /// <summary>
        ///  Gets the owner of the current value.  This is usually the value of the
        ///  root entry, which is the object being browsed.
        /// </summary>
        public virtual object GetValueOwner() => _parent is null ? PropertyValue : _parent.GetChildValueOwner(this);

        /// <summary>
        ///  Gets the owners of the current value.  This is usually the value of the
        ///  root entry, which is the objects being browsed for a multiselect item.
        /// </summary>
        public virtual object[] GetValueOwners()
        {
            object owner = GetValueOwner();
            if (owner is not null)
            {
                return new object[] { owner };
            }

            return null;
        }

        /// <summary>
        ///  Gets the owner of the current value.  This is usually the value of the
        ///  root entry, which is the object being browsed.
        /// </summary>
        public virtual object GetChildValueOwner(GridEntry childEntry) => PropertyValue;

        /// <summary>
        ///  Returns a string with info about the currently selected GridEntry
        /// </summary>
        public virtual string GetTestingInfo()
        {
            string info = "object = (";
            string textValue = GetPropertyTextValue();
            if (textValue is null)
            {
                textValue = "(null)";
            }
            else
            {
                // make sure we clear any embedded nulls
                textValue = textValue.Replace((char)0, ' ');
            }

            Type type = PropertyType;
            if (type is null)
            {
                type = typeof(object);
            }

            info += $"{FullLabel}), property = ({PropertyLabel},{type.AssemblyQualifiedName}), value = [{textValue}], expandable = {Expandable}, readOnly = {ShouldRenderReadOnly}";

            return info;
        }

        /// <summary>
        ///  Retrieves the type of the value for this GridEntry.
        /// </summary>
        public virtual Type GetValueType() => PropertyType;

        /// <summary>
        ///  Returns the child GridEntries for this item.
        /// </summary>
        protected virtual GridEntry[] GetPropEntries(GridEntry parentEntry, object @object, Type objectType)
        {
            // We don't want to create subprops for null objects.
            if (@object is null)
            {
                return null;
            }

            GridEntry[] entries = null;

            var attributes = new Attribute[BrowsableAttributes.Count];
            BrowsableAttributes.CopyTo(attributes, 0);

            PropertyTab tab = CurrentTab;
            Debug.Assert(tab is not null, "No current tab!");

            try
            {
                bool forceReadOnly = ForceReadOnly;

                if (!forceReadOnly)
                {
                    var readOnlyAttribute = (ReadOnlyAttribute)TypeDescriptor.GetAttributes(@object)[typeof(ReadOnlyAttribute)];
                    forceReadOnly = readOnlyAttribute is not null && !readOnlyAttribute.IsDefaultAttribute();
                }

                // Do we want to expose sub properties?
                if (!TypeConverter.GetPropertiesSupported(this) && !AlwaysAllowExpand)
                {
                    return entries;
                }

                // Ask the tab if we have one.
                PropertyDescriptorCollection properties = null;
                PropertyDescriptor defaultProperty = null;
                if (tab is not null)
                {
                    properties = tab.GetProperties(this, @object, attributes);
                    defaultProperty = tab.GetDefaultProperty(@object);
                }
                else
                {
                    properties = TypeConverter.GetProperties(this, @object, attributes);
                    defaultProperty = TypeDescriptor.GetDefaultProperty(@object);
                }

                if (properties is null)
                {
                    return null;
                }

                if ((_propertySort & PropertySort.Alphabetical) != 0)
                {
                    if (objectType is null || !objectType.IsArray)
                    {
                        properties = properties.Sort(DisplayNameComparer);
                    }

                    var propertyDescriptors = new PropertyDescriptor[properties.Count];
                    properties.CopyTo(propertyDescriptors, 0);

                    properties = new PropertyDescriptorCollection(SortParenProperties(propertyDescriptors));
                }

                if (defaultProperty is null && properties.Count > 0)
                {
                    defaultProperty = properties[0];
                }

                // If the target object is an array and nothing else has provided a set of properties to use,
                // then expand the array.

                if ((properties is null || properties.Count == 0) && objectType is not null && objectType.IsArray && @object is not null)
                {
                    var objArray = (Array)@object;

                    entries = new GridEntry[objArray.Length];

                    for (int i = 0; i < entries.Length; i++)
                    {
                        entries[i] = new ArrayElementGridEntry(OwnerGrid, parentEntry, i);
                    }
                }
                else
                {
                    // Otherwise, create the proper GridEntries.
                    bool createInstanceSupported = TypeConverter.GetCreateInstanceSupported(this);
                    entries = new GridEntry[properties.Count];
                    int index = 0;

                    // Loop through all the props we got and create property descriptors.
                    foreach (PropertyDescriptor property in properties)
                    {
                        GridEntry newEntry;

                        // Make sure we've got a valid property, otherwise hide it.
                        bool hide = false;
                        try
                        {
                            object owner = @object;
                            if (@object is ICustomTypeDescriptor descriptor)
                            {
                                owner = descriptor.GetPropertyOwner(property);
                            }

                            property.GetValue(owner);
                        }
                        catch (Exception w)
                        {
                            if (s_pbrsAssertPropsSwitch.Enabled)
                            {
                                Debug.Fail($"Bad property '{parentEntry.PropertyLabel}.{property.Name}': {w}");
                            }

                            hide = true;
                        }

                        newEntry = createInstanceSupported
                            ? new ImmutablePropertyDescriptorGridEntry(OwnerGrid, parentEntry, property, hide)
                            : new PropertyDescriptorGridEntry(OwnerGrid, parentEntry, property, hide);

                        if (forceReadOnly)
                        {
                            newEntry._flags |= Flags.ForceReadOnly;
                        }

                        // Check to see if we've come across the default item.
                        if (property.Equals(defaultProperty))
                        {
                            DefaultChild = newEntry;
                        }

                        // Add it to the array.
                        entries[index++] = newEntry;
                    }
                }

                return entries;
            }
            catch (Exception e)
            {
#if DEBUG
                if (s_pbrsAssertPropsSwitch.Enabled)
                {
                    // Checked builds are not giving us enough information here.  So, output as much stuff as we can.
                    Text.StringBuilder b = new();
                    b.Append(string.Format(CultureInfo.CurrentCulture, "********* Debug log written on {0} ************\r\n", DateTime.Now));
                    b.Append(string.Format(CultureInfo.CurrentCulture, "Exception '{0}' reading properties for object {1}.\r\n", e.GetType().Name, @object));
                    b.Append(string.Format(CultureInfo.CurrentCulture, "Exception Text: \r\n{0}", e.ToString()));
                    b.Append(string.Format(CultureInfo.CurrentCulture, "Exception stack: \r\n{0}", e.StackTrace));
                    string path = string.Format(CultureInfo.CurrentCulture, "{0}\\PropertyGrid.log", Environment.GetEnvironmentVariable("SYSTEMDRIVE"));
                    IO.FileStream s = new(path, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.None);
                    IO.StreamWriter w = new(s);
                    w.Write(b.ToString());
                    w.Close();
                    s.Close();
                    RTLAwareMessageBox.Show(null, b.ToString(), string.Format(SR.PropertyGridInternalNoProp, path),
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
                }
#endif
                Debug.Fail($"Failed to get properties: {e.GetType().Name},{e.Message}\n{e.StackTrace}");
            }

            return entries;
        }

        /// <summary>
        ///  Resets the current item.
        /// </summary>
        public virtual void ResetPropertyValue()
        {
            NotifyValue(Notify.Reset);
            Refresh();
        }

        /// <summary>
        ///  Returns if the property can be reset
        /// </summary>
        public virtual bool CanResetPropertyValue() => NotifyValue(Notify.CanReset);

        /// <summary>
        ///  Called when the item is double clicked.
        /// </summary>
        public virtual bool DoubleClickPropertyValue() => NotifyValue(Notify.DoubleClick);

        /// <summary>
        ///  Returns the text value of this property.
        /// </summary>
        public virtual string GetPropertyTextValue() => GetPropertyTextValue(PropertyValue);

        /// <summary>
        ///  Returns the text value of this property.
        /// </summary>
        public virtual string GetPropertyTextValue(object value)
        {
            string textValue = null;

            TypeConverter converter = TypeConverter;
            try
            {
                textValue = converter.ConvertToString(this, value);
            }
            catch (Exception t)
            {
                Debug.Fail($"Bad Type Converter! {t.GetType().Name}, {t.Message},{converter}", t.ToString());
            }

            if (textValue is null)
            {
                textValue = string.Empty;
            }

            return textValue;
        }

        /// <summary>
        ///  Returns the text values of this property.
        /// </summary>
        public virtual object[] GetPropertyValueList()
        {
            ICollection values = TypeConverter.GetStandardValues(this);
            if (values is not null)
            {
                object[] valueArray = new object[values.Count];
                values.CopyTo(valueArray, 0);
                return valueArray;
            }

            return Array.Empty<object>();
        }

        public override int GetHashCode() => HashCode.Combine(PropertyLabel, PropertyType, GetType());

        /// <summary>
        ///  Checks if a given flag is set
        /// </summary>
        protected virtual bool GetFlagSet(Flags flag) => (flag & EntryFlags) != 0;

        protected Font GetFont(bool boldFont) => boldFont ? GridEntryHost.GetBoldFont() : GridEntryHost.GetBaseFont();

        /// <summary>
        ///  Retrieves the requested service.  This may return null if the requested service is not available.
        /// </summary>
        public virtual object GetService(Type serviceType)
        {
            if (serviceType == typeof(GridItem))
            {
                return this;
            }

            if (_parent is not null)
            {
                return _parent.GetService(serviceType);
            }

            return null;
        }

        internal virtual bool NonParentEquals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            if (obj is not GridEntry entry)
            {
                return false;
            }

            return entry.PropertyLabel.Equals(PropertyLabel) &&
                   entry.PropertyType.Equals(PropertyType) && entry.PropertyDepth == PropertyDepth;
        }

        /// <summary>
        ///  Paints the label portion of this GridEntry into the given Graphics object. This is called by the GridEntry
        ///  host (the PropertyGridView) when this GridEntry is to be painted.
        /// </summary>
        public virtual void PaintLabel(Graphics g, Rectangle rect, Rectangle clipRect, bool selected, bool paintFullLabel)
        {
            PropertyGridView gridHost = GridEntryHost;
            Debug.Assert(gridHost is not null, "No propEntryHost");
            string label = PropertyLabel;

            int borderWidth = gridHost.GetOutlineIconSize() + OutlineIconPadding;

            // Fill the background if necessary.
            Color backColor = selected ? gridHost.GetSelectedItemWithFocusBackColor() : GetBackgroundColor();

            // If we don't have focus, paint with the line color.
            if (selected && !_hasFocus)
            {
                backColor = gridHost.GetLineColor();
            }

            bool bold = (EntryFlags & Flags.LabelBold) != 0;
            Font font = GetFont(bold);

            int labelWidth = GetLabelTextWidth(label, g, font);

            int neededWidth = paintFullLabel ? labelWidth : 0;
            int stringX = rect.X + PropertyLabelIndent;

            using var backBrush = backColor.GetCachedSolidBrushScope();
            if (paintFullLabel && (neededWidth >= (rect.Width - (stringX + 2))))
            {
                // GDIPLUS_SPACE = extra needed to ensure text draws completely and isn't clipped.
                int totalWidth = stringX + neededWidth + PropertyGridView.GdiPlusSpace;

                // Blank out the space we're going to use.
                g.FillRectangle(backBrush, borderWidth - 1, rect.Y, totalWidth - borderWidth + 3, rect.Height);

                // Draw an end line.
                using var linePen = gridHost.GetLineColor().GetCachedPenScope();
                g.DrawLine(linePen, totalWidth, rect.Y, totalWidth, rect.Height);

                // Set the new width that we can draw into.
                rect.Width = totalWidth - rect.X;
            }
            else
            {
                // Normal case -- no pseudo-tooltip for the label
                g.FillRectangle(backBrush, rect.X, rect.Y, rect.Width, rect.Height);
            }

            // Draw the border stripe on the left
            using var stripeBrush = gridHost.GetLineColor().GetCachedSolidBrushScope();
            g.FillRectangle(stripeBrush, rect.X, rect.Y, borderWidth, rect.Height);

            if (selected && _hasFocus)
            {
                using var focusBrush = gridHost.GetSelectedItemWithFocusBackColor().GetCachedSolidBrushScope();
                g.FillRectangle(
                    focusBrush,
                    stringX, rect.Y, rect.Width - stringX - 1, rect.Height);
            }

            int maxSpace = Math.Min(rect.Width - stringX - 1, labelWidth + PropertyGridView.GdiPlusSpace);
            Rectangle textRect = new(stringX, rect.Y + 1, maxSpace, rect.Height - 1);

            if (!Rectangle.Intersect(textRect, clipRect).IsEmpty)
            {
                Region oldClip = g.Clip;
                g.SetClip(textRect);

                // We need to Invert color only if in Highcontrast mode, targeting 4.7.1 and above, Gridcategory and
                // not a developer override. This is required to achieve required contrast ratio.
                bool shouldInvertForHC = ColorInversionNeededInHC && (bold || (selected && !_hasFocus));

                // Do actual drawing
                // A brush is needed if using GDI+ only (UseCompatibleTextRendering); if using GDI, only the color is needed.
                Color textColor = selected && _hasFocus
                    ? gridHost.GetSelectedItemWithFocusForeColor()
                    : shouldInvertForHC
                        ? InvertColor(OwnerGrid.LineColor)
                        : g.FindNearestColor(LabelTextColor);

                if (OwnerGrid.UseCompatibleTextRendering)
                {
                    using var textBrush = textColor.GetCachedSolidBrushScope();
                    StringFormat stringFormat = new(StringFormatFlags.NoWrap)
                    {
                        Trimming = StringTrimming.None
                    };
                    g.DrawString(label, font, textBrush, textRect, stringFormat);
                }
                else
                {
                    TextRenderer.DrawText(g, label, font, textRect, textColor, PropertyGrid.MeasureTextHelper.GetTextRendererFlags());
                }

                g.SetClip(oldClip, CombineMode.Replace);
                oldClip.Dispose();   // clip is actually copied out.

                if (maxSpace <= labelWidth)
                {
                    _labelTipPoint = new Point(stringX + 2, rect.Y + 1);
                }
                else
                {
                    _labelTipPoint = InvalidPoint;
                }
            }

            rect.Y -= 1;
            rect.Height += 2;

            PaintOutline(g, rect);
        }

        /// <summary>
        ///  Paints the outline portion of this GridEntry into the given Graphics object.  This
        ///  is called by the GridEntry host (the PropertyGridView) when this GridEntry is
        ///  to be painted.
        /// </summary>
        public virtual void PaintOutline(Graphics g, Rectangle r)
        {
            // Draw tree-view glyphs as triangles on Vista and Windows afterword
            // when Visual style is enabled
            if (GridEntryHost.IsExplorerTreeSupported)
            {
                if (!_lastPaintWithExplorerStyle)
                {
                    // Size of Explorer Tree style glyph (triangle) is different from +/- glyph, so when we change the
                    // visual style (such as changing Windows theme), we need to recalculate outlineRect.

                    _outlineRect = Rectangle.Empty;
                    _lastPaintWithExplorerStyle = true;
                }

                PaintOutlineWithExplorerTreeStyle(g, r, (GridEntryHost is not null) ? GridEntryHost.HandleInternal : IntPtr.Zero);
            }
            else
            {
                // Draw tree-view glyphs as +/-

                if (_lastPaintWithExplorerStyle)
                {
                    // Size of Explorer Tree style glyph (triangle) is different from +/- glyph, so when we change the
                    // visual style (such as changing Windows theme), we need to recalculate outlineRect.

                    _outlineRect = Rectangle.Empty;
                    _lastPaintWithExplorerStyle = false;
                }

                PaintOutlineWithClassicStyle(g, r);
            }
        }

        private void PaintOutlineWithExplorerTreeStyle(Graphics g, Rectangle r, IntPtr handle)
        {
            if (Expandable)
            {
                bool expanded = InternalExpanded;
                Rectangle outline = OutlineRect;

                // Make sure we're in our bounds.
                outline = Rectangle.Intersect(r, outline);
                if (outline.IsEmpty)
                {
                    return;
                }

                VisualStyleElement element = expanded
                    ? VisualStyleElement.ExplorerTreeView.Glyph.Opened
                    : VisualStyleElement.ExplorerTreeView.Glyph.Closed;

                // Invert color if it is not overriden by developer.
                if (ColorInversionNeededInHC)
                {
                    Color textColor = InvertColor(OwnerGrid.LineColor);
                    if (g is not null)
                    {
                        using var brush = textColor.GetCachedSolidBrushScope();
                        g.FillRectangle(brush, outline);
                    }
                }

                VisualStyleRenderer explorerTreeRenderer = new(element);

                using var hdc = new DeviceContextHdcScope(g);
                explorerTreeRenderer.DrawBackground(hdc, outline, handle);
            }
        }

        private void PaintOutlineWithClassicStyle(Graphics g, Rectangle r)
        {
            // Draw outline box.
            if (Expandable)
            {
                bool fExpanded = InternalExpanded;
                Rectangle outline = OutlineRect;

                // Make sure we're in our bounds.
                outline = Rectangle.Intersect(r, outline);
                if (outline.IsEmpty)
                {
                    return;
                }

                // Draw border area box.
                Color penColor = GridEntryHost.GetTextColor();

                // Inverting text color to back ground to get required contrast ratio.
                if (ColorInversionNeededInHC)
                {
                    penColor = InvertColor(OwnerGrid.LineColor);
                }
                else
                {
                    // Filling rectangle as it was in all cases where we do not invert colors.
                    Color brushColor = GetBackgroundColor();
                    using var brush = brushColor.GetCachedSolidBrushScope();
                    g.FillRectangle(brush, outline);
                }

                using var pen = penColor.GetCachedPenScope();

                g.DrawRectangle(pen, outline.X, outline.Y, outline.Width - 1, outline.Height - 1);

                // Draw horizontal line for +/-
                int indent = 2;
                g.DrawLine(
                    pen,
                    outline.X + indent,
                    outline.Y + outline.Height / 2,
                    outline.X + outline.Width - indent - 1,
                    outline.Y + outline.Height / 2);

                // Draw vertical line to make a +
                if (!fExpanded)
                {
                    g.DrawLine(
                        pen,
                        outline.X + outline.Width / 2,
                        outline.Y + indent,
                        outline.X + outline.Width / 2,
                        outline.Y + outline.Height - indent - 1);
                }
            }
        }

        /// <summary>
        ///  Paints the value portion of this GridEntry into the given Graphics object. This is called by the GridEntry
        ///  host (the PropertyGridView) when this GridEntry is to be painted.
        /// </summary>
        public virtual void PaintValue(object val, Graphics g, Rectangle rect, Rectangle clipRect, PaintValueFlags paintFlags)
        {
            PropertyGridView gridHost = GridEntryHost;
            Debug.Assert(gridHost is not null);

            Color textColor = ShouldRenderReadOnly ? GridEntryHost.GrayTextColor : gridHost.GetTextColor();

            string text;

            if (paintFlags.HasFlag(PaintValueFlags.FetchValue))
            {
                if (_cacheItems is not null && _cacheItems.UseValueString)
                {
                    text = _cacheItems.LastValueString;
                    val = _cacheItems.LastValue;
                }
                else
                {
                    val = PropertyValue;
                    text = GetPropertyTextValue(val);

                    if (_cacheItems is null)
                    {
                        _cacheItems = new CacheItems();
                    }

                    _cacheItems.LastValueString = text;
                    _cacheItems.UseValueString = true;
                    _cacheItems.LastValueTextWidth = -1;
                    _cacheItems.LastValueFont = null;
                    _cacheItems.LastValue = val;
                }
            }
            else
            {
                text = GetPropertyTextValue(val);
            }

            // Paint out the main rect using the appropriate brush.
            Color backColor = GetBackgroundColor();

            if (paintFlags.HasFlag(PaintValueFlags.DrawSelected))
            {
                backColor = gridHost.GetSelectedItemWithFocusBackColor();
                textColor = gridHost.GetSelectedItemWithFocusForeColor();
            }

            using var backBrush = backColor.GetCachedSolidBrushScope();
            g.FillRectangle(backBrush, clipRect);

            int paintIndent = 0;
            if (IsCustomPaint)
            {
                paintIndent = gridHost.GetValuePaintIndent();
                Rectangle rectPaint = new(
                    rect.X + 1,
                    rect.Y + 1,
                    gridHost.GetValuePaintWidth(),
                    gridHost.GetGridEntryHeight() - 2);

                if (!Rectangle.Intersect(rectPaint, clipRect).IsEmpty)
                {
                    UITypeEditor?.PaintValue(new PaintValueEventArgs(this, val, g, rectPaint));

                    // Paint a border around the area
                    rectPaint.Width--;
                    rectPaint.Height--;
                    g.DrawRectangle(SystemPens.WindowText, rectPaint);
                }
            }

            rect.X += paintIndent + gridHost.GetValueStringIndent();
            rect.Width -= paintIndent + 2 * gridHost.GetValueStringIndent();

            // Bold the property if we need to persist it (e.g. it's not the default value).
            bool valueModified = paintFlags.HasFlag(PaintValueFlags.CheckShouldSerialize) && ShouldSerializePropertyValue();

            // If we have text to paint, paint it.
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (text.Length > MaximumLengthOfPropertyString)
            {
                text = text.Substring(0, MaximumLengthOfPropertyString);
            }

            int textWidth = GetValueTextWidth(text, g, GetFont(valueModified));
            bool doToolTip = false;

            // Check if text contains multiple lines.
            if (textWidth >= rect.Width || HasMultipleLines(text))
            {
                doToolTip = true;
            }

            if (Rectangle.Intersect(rect, clipRect).IsEmpty)
            {
                return;
            }

            // Do actual drawing, shifting to match the PropertyGridView.GridViewListbox content alignment.

            if (paintFlags.HasFlag(PaintValueFlags.PaintInPlace))
            {
                rect.Offset(1, 2);
            }
            else
            {
                // Only go down one pixel when we're painting in the listbox.
                rect.Offset(1, 1);
            }

            Rectangle textRectangle = new(
                rect.X - 1,
                rect.Y - 1,
                rect.Width - 4,
                rect.Height);

            backColor = paintFlags.HasFlag(PaintValueFlags.DrawSelected)
                ? GridEntryHost.GetSelectedItemWithFocusBackColor()
                : GridEntryHost.BackColor;

            User32.DT format = User32.DT.EDITCONTROL | User32.DT.EXPANDTABS | User32.DT.NOCLIP
                | User32.DT.SINGLELINE | User32.DT.NOPREFIX;

            if (gridHost.DrawValuesRightToLeft)
            {
                format |= User32.DT.RIGHT | User32.DT.RTLREADING;
            }

            // For password mode, replace the string value with a bullet.
            if (ShouldRenderPassword)
            {
                if (s_passwordReplaceChar == '\0')
                {
                    // Bullet is 2022, but edit box uses round circle 25CF
                    s_passwordReplaceChar = '\u25CF';
                }

                text = new string(s_passwordReplaceChar, text.Length);
            }

            TextRenderer.DrawTextInternal(
                g,
                text,
                GetFont(boldFont: valueModified),
                textRectangle,
                textColor,
                backColor,
                (TextFormatFlags)format | PropertyGrid.MeasureTextHelper.GetTextRendererFlags());

            ValueToolTipLocation = doToolTip ? new Point(rect.X + 2, rect.Y - 1) : InvalidPoint;

            static bool HasMultipleLines(string value) => value.IndexOf('\n') > 0 || value.IndexOf('\r') > 0;
        }

        public virtual bool OnComponentChanging()
        {
            try
            {
                ComponentChangeService?.OnComponentChanging(GetValueOwner(), PropertyDescriptor);
                return true;
            }
            catch (CheckoutException e) when (e == CheckoutException.Canceled)
            {
                return false;
            }
        }

        public virtual void OnComponentChanged()
            => ComponentChangeService?.OnComponentChanged(GetValueOwner(), PropertyDescriptor);

        /// <summary>
        ///  Called when the label portion of this GridEntry is clicked.
        ///  Default implementation fired the event to any listeners, so be sure
        ///  to call base.OnLabelClick(e) if this is overridden.
        /// </summary>
        protected virtual void OnLabelClick(EventArgs e) => RaiseEvent(s_labelClickEvent, e);

        /// <summary>
        ///  Called when the label portion of this GridEntry is double-clicked.
        ///  Default implementation fired the event to any listeners, so be sure
        ///  to call base.OnLabelDoubleClick(e) if this is overridden.
        /// </summary>
        protected virtual void OnLabelDoubleClick(EventArgs e) => RaiseEvent(s_labelDoubleClickEvent, e);

        /// <summary>
        ///  Called when the GridEntry is clicked.
        /// </summary>
        public virtual bool OnMouseClick(int x, int y, int count, MouseButtons button)
        {
            // Where are we at?
            PropertyGridView gridHost = GridEntryHost;
            Debug.Assert(gridHost is not null, "No prop entry host!");

            // Make sure it's the left button.
            if ((button & MouseButtons.Left) != MouseButtons.Left)
            {
                return false;
            }

            int labelWidth = gridHost.GetLabelWidth();

            // Are we in the label?
            if (x >= 0 && x <= labelWidth)
            {
                // Are we on the outline?
                if (Expandable)
                {
                    Rectangle outlineRect = OutlineRect;
                    if (outlineRect.Contains(x, y))
                    {
                        if (count % 2 == 0)
                        {
                            OnOutlineDoubleClick(EventArgs.Empty);
                        }
                        else
                        {
                            OnOutlineClick(EventArgs.Empty);
                        }

                        return true;
                    }
                }

                if (count % 2 == 0)
                {
                    OnLabelDoubleClick(EventArgs.Empty);
                }
                else
                {
                    OnLabelClick(EventArgs.Empty);
                }

                return true;
            }

            // Are we in the value?
            labelWidth += gridHost.GetSplitterWidth();
            if (x >= labelWidth && x <= labelWidth + gridHost.GetValueWidth())
            {
                if (count % 2 == 0)
                {
                    OnValueDoubleClick(EventArgs.Empty);
                }
                else
                {
                    OnValueClick(EventArgs.Empty);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///  Called when the outline icon portion of this GridEntry is clicked.
        ///  Default implementation fired the event to any listeners, so be sure
        ///  to call base.OnOutlineClick(e) if this is overridden.
        /// </summary>
        protected virtual void OnOutlineClick(EventArgs e) => RaiseEvent(s_outlineClickEvent, e);

        /// <summary>
        ///  Called when the outline icon portion of this GridEntry is double-clicked.
        ///  Default implementation fired the event to any listeners, so be sure
        ///  to call base.OnOutlineDoubleClick(e) if this is overridden.
        /// </summary>
        protected virtual void OnOutlineDoubleClick(EventArgs e) => RaiseEvent(s_outlineDoubleClickEvent, e);

        /// <summary>
        ///  Called when RecreateChildren is called.
        ///  Default implementation fired the event to any listeners, so be sure
        ///  to call base.OnOutlineDoubleClick(e) if this is overridden.
        /// </summary>
        protected virtual void OnRecreateChildren(GridEntryRecreateChildrenEventArgs e)
        {
            Delegate handler = GetEventHandler(s_recreateChildrenEvent);
            if (handler is not null)
            {
                ((GridEntryRecreateChildrenEventHandler)handler)(this, e);
            }
        }

        /// <summary>
        ///  Called when the value portion of this GridEntry is clicked.
        ///  Default implementation fired the event to any listeners, so be sure
        ///  to call base.OnValueClick(e) if this is overridden.
        /// </summary>
        protected virtual void OnValueClick(EventArgs e) => RaiseEvent(s_valueClickEvent, e);

        /// <summary>
        ///  Called when the value portion of this GridEntry is clicked.
        ///  Default implementation fired the event to any listeners, so be sure
        ///  to call base.OnValueDoubleClick(e) if this is overridden.
        /// </summary>
        protected virtual void OnValueDoubleClick(EventArgs e) => RaiseEvent(s_valueDoubleClickEvent, e);

        internal bool OnValueReturnKey() => NotifyValue(Notify.Return);

        /// <summary>
        ///  Sets the specified flag
        /// </summary>
        protected virtual void SetFlag(Flags flag, bool value) => SetFlag(flag, value ? flag : 0);

        /// <summary>
        ///  Sets the default child of this entry, given a valid value mask.
        /// </summary>
        protected virtual void SetFlag(Flags validFlags, Flags flag, bool value)
            => SetFlag(validFlags | flag, validFlags | (value ? flag : 0));

        /// <summary>
        ///  Sets the value of a flag
        /// </summary>
        protected virtual void SetFlag(Flags flag, Flags value) => _flags = (EntryFlags & ~flag) | value;

        public override bool Select()
        {
            if (Disposed)
            {
                return false;
            }

            try
            {
                GridEntryHost.SelectedGridEntry = this;
                return true;
            }
            catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
            {
            }

            return false;
        }

        /// <summary>
        ///  Checks if this value should be persisted.
        /// </summary>
        internal virtual bool ShouldSerializePropertyValue()
        {
            if (_cacheItems is not null)
            {
                if (_cacheItems.UseShouldSerialize)
                {
                    return _cacheItems.LastShouldSerialize;
                }
                else
                {
                    _cacheItems.LastShouldSerialize = NotifyValue(Notify.ShouldPersist);
                    _cacheItems.UseShouldSerialize = true;
                }
            }
            else
            {
                _cacheItems = new CacheItems
                {
                    LastShouldSerialize = NotifyValue(Notify.ShouldPersist),
                    UseShouldSerialize = true
                };
            }

            return _cacheItems.LastShouldSerialize;
        }

        private PropertyDescriptor[] SortParenProperties(PropertyDescriptor[] props)
        {
            PropertyDescriptor[] newProperties = null;
            int newPosition = 0;

            // First scan the list and move any parenthesized properties to the front.
            for (int i = 0; i < props.Length; i++)
            {
                if (((ParenthesizePropertyNameAttribute)props[i].Attributes[typeof(ParenthesizePropertyNameAttribute)]).NeedParenthesis)
                {
                    newProperties ??= new PropertyDescriptor[props.Length];
                    newProperties[newPosition++] = props[i];
                    props[i] = null;
                }
            }

            // Second pass, copy any that didn't have the parenthesis.
            if (newPosition > 0)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    if (props[i] is not null)
                    {
                        newProperties[newPosition++] = props[i];
                    }
                }

                props = newProperties;
            }

            return props;
        }

        /// <summary>
        ///  Sends a notify message to this GridEntry, and returns the success result
        /// </summary>
        protected internal virtual bool NotifyValueGivenParent(object value, Notify type) => false;

        /// <summary>
        ///  Sends a notify message to the child GridEntry, and returns the success result.
        /// </summary>
        protected internal virtual bool NotifyChildValue(GridEntry entry, Notify type)
            => entry.NotifyValueGivenParent(entry.GetValueOwner(), type);

        protected internal virtual bool NotifyValue(Notify type) => _parent is null || _parent.NotifyChildValue(this, type);

        internal void RecreateChildren() => RecreateChildren(-1);

        internal void RecreateChildren(int oldCount)
        {
            // Cause the flags to be rebuilt as well.
            bool wasExpanded = InternalExpanded || oldCount > 0;

            if (oldCount == -1)
            {
                oldCount = VisibleChildCount;
            }

            ResetState();
            if (oldCount == 0)
            {
                return;
            }

            foreach (GridEntry child in ChildCollection)
            {
                child.RecreateChildren();
            }

            DisposeChildren();
            InternalExpanded = wasExpanded;
            OnRecreateChildren(new GridEntryRecreateChildrenEventArgs(oldCount, VisibleChildCount));
        }

        /// <summary>
        ///  Refresh the current GridEntry's value and it's children.
        /// </summary>
        public virtual void Refresh()
        {
            Type type = PropertyType;
            if (type is not null && type.IsArray)
            {
                CreateChildren(true);
            }

            if (_children is not null)
            {
                // Check to see if the value has changed.
                if (InternalExpanded && _cacheItems?.LastValue is not null && _cacheItems.LastValue != PropertyValue)
                {
                    ClearCachedValues();
                    RecreateChildren();
                    return;
                }
                else if (InternalExpanded)
                {
                    // Otherwise just do a refresh.
                    IEnumerator childEnum = _children.GetEnumerator();
                    while (childEnum.MoveNext())
                    {
                        object o = childEnum.Current;
                        Debug.Assert(o is not null, "Collection contains a null element.");
                        ((GridEntry)o).Refresh();
                    }
                }
                else
                {
                    DisposeChildren();
                }
            }

            ClearCachedValues();
        }

        public virtual void RemoveOnLabelClick(EventHandler h) => RemoveEventHandler(s_labelClickEvent, h);

        public virtual void RemoveOnLabelDoubleClick(EventHandler h) => RemoveEventHandler(s_labelDoubleClickEvent, h);

        public virtual void RemoveOnValueClick(EventHandler h) => RemoveEventHandler(s_valueClickEvent, h);

        public virtual void RemoveOnValueDoubleClick(EventHandler h) => RemoveEventHandler(s_valueDoubleClickEvent, h);

        public virtual void RemoveOnOutlineClick(EventHandler h) => RemoveEventHandler(s_outlineClickEvent, h);

        public virtual void RemoveOnOutlineDoubleClick(EventHandler h) => RemoveEventHandler(s_outlineDoubleClickEvent, h);

        public virtual void RemoveOnRecreateChildren(GridEntryRecreateChildrenEventHandler h)
            => RemoveEventHandler(s_recreateChildrenEvent, h);

        protected void ResetState()
        {
            ClearFlags();
            ClearCachedValues();
        }

        /// <summary>
        ///  Sets the value of this GridEntry from text
        /// </summary>
        public virtual bool SetPropertyTextValue(string str)
        {
            bool childrenPrior = _children is not null && _children.Count > 0;
            PropertyValue = ConvertTextToValue(str);
            CreateChildren();
            bool childrenAfter = _children is not null && _children.Count > 0;
            return childrenPrior != childrenAfter;
        }

        public override string ToString() => $"{GetType().FullName} {PropertyLabel}";

        protected virtual void AddEventHandler(object key, Delegate handler)
        {
            // Locking 'this' here is ok since this is an internal class.
            lock (this)
            {
                if (handler is null)
                {
                    return;
                }

                for (EventEntry e = _eventList; e is not null; e = e.Next)
                {
                    if (e.Key == key)
                    {
                        e.Handler = Delegate.Combine(e.Handler, handler);
                        return;
                    }
                }

                _eventList = new EventEntry(_eventList, key, handler);
            }
        }

        protected virtual void RaiseEvent(object key, EventArgs e)
        {
            Delegate handler = GetEventHandler(key);
            if (handler is not null)
            {
                ((EventHandler)handler)(this, e);
            }
        }

        protected virtual Delegate GetEventHandler(object key)
        {
            // Locking 'this' here is ok since this is an internal class.
            lock (this)
            {
                for (EventEntry e = _eventList; e is not null; e = e.Next)
                {
                    if (e.Key == key)
                    {
                        return e.Handler;
                    }
                }

                return null;
            }
        }

        protected virtual void RemoveEventHandler(object key, Delegate handler)
        {
            // Locking this here is ok since this is an internal class.
            lock (this)
            {
                if (handler is null)
                {
                    return;
                }

                for (EventEntry entry = _eventList, previous = null; entry is not null; previous = entry, entry = entry.Next)
                {
                    if (entry.Key == key)
                    {
                        entry.Handler = Delegate.Remove(entry.Handler, handler);
                        if (entry.Handler is null)
                        {
                            if (previous is null)
                            {
                                _eventList = entry.Next;
                            }
                            else
                            {
                                previous.Next = entry.Next;
                            }
                        }

                        return;
                    }
                }
            }
        }

        protected virtual void RemoveEventHandlers() => _eventList = null;
    }

    internal delegate void GridEntryRecreateChildrenEventHandler(object sender, GridEntryRecreateChildrenEventArgs rce);
}
