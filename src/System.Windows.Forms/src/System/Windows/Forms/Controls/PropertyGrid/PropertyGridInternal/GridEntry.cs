// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.PropertyGridInternal;

/// <summary>
///  Base entry for properties to be displayed in the <see cref="PropertyGridView"/>.
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

    private EventEntry? _eventList;
    private CacheItems? _cacheItems;

    protected TypeConverter? _typeConverter;

    protected UITypeEditor? Editor { get; set; }

    private GridEntry? _parent;
    private GridEntryCollection? _children;
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

    private GridEntryAccessibleObject? _accessibleObject;

    private bool _lastPaintWithExplorerStyle;

    private readonly Lock _lock = new();

    private static Color InvertColor(Color color)
        => Color.FromArgb(color.A, (byte)~color.R, (byte)~color.G, (byte)~color.B);

    protected GridEntry(PropertyGrid ownerGrid, GridEntry? parent)
    {
        _parent = parent;
        OwnerGrid = ownerGrid;

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
    ///  Outline Icon padding.
    /// </summary>
    protected int OutlineIconPadding
    {
        get
        {
            const int LogicalOutlineIconPadding = 5;
            return OwnerGridView?.LogicalToDeviceUnits(LogicalOutlineIconPadding) ?? LogicalOutlineIconPadding;
        }
    }

    private bool ColorInversionNeededInHighContrast
        => SystemInformation.HighContrast && !OwnerGrid.HasCustomLineColor;

    /// <summary>
    ///  Gets the <see cref="AccessibleObject"/> for this instance.
    /// </summary>
    public AccessibleObject AccessibilityObject => _accessibleObject ??= GetAccessibilityObject();

    /// <summary>
    ///  Creates a new <see cref="AccessibleObject"/> for this instance.
    /// </summary>
    protected virtual GridEntryAccessibleObject GetAccessibilityObject() => new(this);

    /// <summary>
    ///  Specify that this grid entry should be allowed to be merged for multi-select.
    /// </summary>
    public virtual bool AllowMerge => true;

    protected virtual AttributeCollection Attributes => TypeDescriptor.GetAttributes(PropertyType!);

    /// <summary>
    ///  Gets the value of the background brush to use. Override this member to cause the entry to paint it's
    ///  background in a different color. The base implementation returns null.
    /// </summary>
    protected virtual Color BackgroundColor => OwnerGridView?.BackColor ?? default;

    protected virtual Color LabelTextColor
    {
        get
        {
            if (OwnerGridView is null)
            {
                return default;
            }

            return ShouldRenderReadOnly
                ? OwnerGridView.GrayTextColor
                : OwnerGridView.TextColor;
        }
    }

    /// <summary>
    ///  The set of attributes that will be used for browse filtering.
    /// </summary>
    public virtual AttributeCollection? BrowsableAttributes
    {
        get => _parent?.BrowsableAttributes;
        set
        {
            if (_parent is not null)
            {
                _parent.BrowsableAttributes = value;
            }
        }
    }

    /// <summary>
    ///  Retrieves the component that is invoking the method on the formatter object. This may
    ///  return null if there is no component responsible for the call.
    /// </summary>
    public virtual IComponent? Component
        => GetValueOwner() is IComponent component ? component : _parent?.Component;

    protected virtual IComponentChangeService? ComponentChangeService => _parent?.ComponentChangeService;

    /// <summary>
    ///  Retrieves the container that contains the set of objects this formatter may work
    ///  with. It may return null if there is no container, or of the formatter should not
    ///  use any outside objects.
    /// </summary>
    public virtual IContainer? Container => Component?.Site?.Container;

    [AllowNull]
    protected GridEntryCollection ChildCollection
    {
        get => _children ??= [];
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

    public int ChildCount => Children.Count;

    public virtual GridEntryCollection Children
    {
        get
        {
            if (_children is null && !Disposed)
            {
                CreateChildren();
            }

            return _children ??= [];
        }
    }

    /// <summary>
    ///  The <see cref="PropertyTab"/> that the <see cref="GridEntry"/> belongs to.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The root grid entry <see cref="SingleSelectRootGridEntry"/> maintains this value.
    ///  </para>
    /// </remarks>
    public virtual PropertyTab? OwnerTab => _parent?.OwnerTab;

    /// <summary>
    ///  Returns the default child <see cref="GridEntry"/> of this item.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The root grid entry <see cref="SingleSelectRootGridEntry"/> maintains this value.
    ///  </para>
    /// </remarks>
    internal virtual GridEntry? DefaultChild
    {
        get => null;
        set { }
    }

    /// <summary>
    ///  The currently active <see cref="IDesignerHost"/>, if any.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The root grid entry <see cref="SingleSelectRootGridEntry"/> maintains this value. The owning
    ///   <see cref="PropertyGrid"/> will update this when <see cref="PropertyGrid.ActiveDesigner"/> is set.
    ///  </para>
    /// </remarks>
    internal virtual IDesignerHost? DesignerHost
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

    /// <summary>
    ///  Returns true if there is a standard set of values that can be selected from.
    ///  <see cref="GetPropertyValueList"/> should return said values when this is true.
    /// </summary>
    internal virtual bool Enumerable => EntryFlags.HasFlag(Flags.StandardValuesSupported);

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

            if (expandable && _cacheItems?.LastValue is null && PropertyValue is null)
            {
                expandable = false;
            }

            return expandable;
        }
    }

    public override bool Expanded
    {
        get => InternalExpanded;
        set => OwnerGridView?.SetExpand(this, value);
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
                    bool childrenExpandable = CreateChildren();
                    SetFlag(Flags.Expand, childrenExpandable);
                }
            }

            // Notify accessibility clients of expanded state change. StateChange requires NameChange as well.
            // Accessible clients won't see this unless both events are raised.

            // Root item is hidden and should not raise events
            if (OwnerGridView is { } ownerGridView
                && ownerGridView.IsAccessibilityObjectCreated
                && GridItemType != GridItemType.Root)
            {
                int id = OwnerGridView.AccessibilityGetGridEntryChildID(this);
                if (id >= 0)
                {
                    var accessibleObject = (PropertyGridView.PropertyGridViewAccessibleObject)OwnerGridView.AccessibilityObject;
                    accessibleObject.NotifyClients(AccessibleEvents.StateChange, id);
                    accessibleObject.NotifyClients(AccessibleEvents.NameChange, id);
                }
            }
        }
    }

    public Flags EntryFlags
    {
        get
        {
            if (_flags.HasFlag(Flags.Checked))
            {
                return _flags;
            }

            _flags |= Flags.Checked;

            TypeConverter converter = TypeConverter;
            UITypeEditor? editor = UITypeEditor;
            object? value = Instance;
            bool forceReadOnly = ForceReadOnly;

            if (value is not null)
            {
                forceReadOnly |= TypeDescriptor.GetAttributes(value).Contains(InheritanceAttribute.InheritedReadOnly);
            }

            if (converter.GetStandardValuesSupported(this))
            {
                _flags |= Flags.StandardValuesSupported;
            }

            if (!forceReadOnly && converter.CanConvertFrom(this, typeof(string)) &&
                !converter.GetStandardValuesExclusive(this))
            {
                _flags |= Flags.TextEditable;
            }

            bool hasImmutableAttribute = TypeDescriptor.GetAttributes(PropertyType!)[typeof(ImmutableObjectAttribute)]!
                .Equals(ImmutableObjectAttribute.Yes);
            bool isImmutable = hasImmutableAttribute || converter.GetCreateInstanceSupported(this);

            if (isImmutable)
            {
                _flags |= Flags.Immutable;
            }

            if (converter.GetPropertiesSupported(this))
            {
                _flags |= Flags.Expandable;

                // If we're expandable, but we don't support editing,
                // make us read only editable so we don't paint grey.

                if (!forceReadOnly && !_flags.HasFlag(Flags.TextEditable) && !hasImmutableAttribute)
                {
                    _flags |= Flags.ReadOnlyEditable;
                }
            }

            if (Attributes.Contains(PasswordPropertyTextAttribute.Yes))
            {
                _flags |= Flags.RenderPassword;
            }

            if (editor is null)
            {
                return _flags;
            }

            if (editor.GetPaintValueSupported(this))
            {
                _flags |= Flags.CustomPaint;
            }

            // We only allow drop-downs if the object is NOT being inherited.

            bool allowButtons = !forceReadOnly;

            if (allowButtons)
            {
                switch (editor.GetEditStyle(this))
                {
                    case UITypeEditorEditStyle.Modal:
                        _flags |= Flags.ModalEditable;
                        if (!isImmutable && !(PropertyType?.IsValueType ?? false))
                        {
                            _flags |= Flags.ReadOnlyEditable;
                        }

                        break;
                    case UITypeEditorEditStyle.DropDown:
                        _flags |= Flags.DropDownEditable;
                        break;
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
                if (OwnerGridView is { } ownerGridView
                    && ownerGridView.IsAccessibilityObjectCreated
                    && value)
                {
                    int id = OwnerGridView.AccessibilityGetGridEntryChildID(this);
                    if (id >= 0)
                    {
                        var gridAccObj = (PropertyGridView.PropertyGridViewAccessibleObject)OwnerGridView.AccessibilityObject;
                        gridAccObj.NotifyClients(AccessibleEvents.Focus, id);
                        gridAccObj.NotifyClients(AccessibleEvents.Selection, id);

                        AccessibilityObject.SetFocus();
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Returns the label including the object name, and properties. For example, the value
    ///  of the Font size property on a Button called Button1 would be "Button1.Font.Size"
    /// </summary>
    public string? FullLabel
    {
        get
        {
            string? label = _parent?.FullLabel;

            if (label is not null)
            {
                label = $"{label}.{PropertyLabel}";
            }
            else
            {
                label = PropertyLabel;
            }

            return label;
        }
    }

    public override GridItemCollection GridItems
    {
        get
        {
            ObjectDisposedException.ThrowIf(Disposed, typeof(GridItem));
            if (IsExpandable && _children is not null && _children.Count == 0)
            {
                CreateChildren();
            }

            return new GridItemCollection(Children);
        }
    }

    /// <summary>
    ///  The <see cref="PropertyGridView"/> that this <see cref="GridEntry"/> belongs to.
    /// </summary>
    [DisallowNull]
    internal virtual PropertyGridView? OwnerGridView
    {
        get => _parent?.OwnerGridView;
        set => throw new NotSupportedException();
    }

    public override GridItemType GridItemType => GridItemType.Property;

    /// <summary>
    ///  Returns true if this GridEntry has a value field in the right hand column.
    /// </summary>
    internal virtual bool HasValue => true;

    /// <summary>
    ///  Retrieves the keyword that Visual Studio dynamic help window will use when this entry is selected.
    /// </summary>
    public virtual string? HelpKeyword => _parent?.HelpKeyword ?? string.Empty;

    /// <summary>
    ///  Returns true when the entry has an <see cref="UITypeEditor"/> that custom paints a value.
    /// </summary>
    public bool IsCustomPaint
    {
        get
        {
            // Prevent full flag population if possible by not hitting EntryFlags if flags have not been checked yet.
            if (!_flags.HasFlag(Flags.Checked))
            {
                UITypeEditor? editor = UITypeEditor;
                if (editor is not null)
                {
                    if (_flags.HasFlag(Flags.CustomPaint) || _flags.HasFlag(Flags.NoCustomPaint))
                    {
                        return _flags.HasFlag(Flags.CustomPaint);
                    }

                    if (editor.GetPaintValueSupported(this))
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

            return EntryFlags.HasFlag(Flags.CustomPaint);
        }
    }

    public bool IsExpandable
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

    public bool IsTextEditable => IsValueEditable && EntryFlags.HasFlag(Flags.TextEditable);

    public virtual bool IsValueEditable
        => !ForceReadOnly
        && GetFlagSet(Flags.DropDownEditable | Flags.TextEditable | Flags.ModalEditable | Flags.StandardValuesSupported);

    /// <summary>
    ///  Retrieves the component that is invoking the method on the formatter object. This may
    ///  return null if there is no component responsible for the call.
    /// </summary>
    public object? Instance => GetValueOwner() ?? _parent?.Instance;

    public override string? Label => PropertyLabel;

    public override PropertyDescriptor? PropertyDescriptor => null;

    /// <summary>
    ///  Returns the pixel indent of the current GridEntry's label.
    /// </summary>
    internal virtual int PropertyLabelIndent
    {
        get
        {
            int borderWidth = (OwnerGridView?.OutlineIconSize ?? 0) + OutlineIconPadding;
            return ((_propertyDepth + 1) * borderWidth) + 1;
        }
    }

    internal virtual Point GetLabelToolTipLocation(int mouseX, int mouseY) => _labelTipPoint;

    internal virtual string? LabelToolTipText => PropertyLabel;

    /// <summary>
    ///  The entry needs a drop down button to invoke its editor.
    /// </summary>
    public virtual bool NeedsDropDownButton => EntryFlags.HasFlag(Flags.DropDownEditable);

    /// <summary>
    ///  The entry needs a modal editor button ("...") to invoke its editor.
    /// </summary>
    public bool NeedsModalEditorButton
        => EntryFlags.HasFlag(Flags.ModalEditable) && (IsValueEditable || EntryFlags.HasFlag(Flags.ReadOnlyEditable));

    public PropertyGrid OwnerGrid { get; }

    /// <summary>
    ///  Returns the rectangle that the outline icon (+ or - or arrow) will be drawn into, relative
    ///  to the upper left corner of the <see cref="GridEntry"/>.
    /// </summary>
    public Rectangle OutlineRectangle
    {
        get
        {
            if (!_outlineRect.IsEmpty || OwnerGridView is null)
            {
                return _outlineRect;
            }

            int outlineSize = OwnerGridView.OutlineIconSize;
            int borderWidth = outlineSize + OutlineIconPadding;
            _outlineRect = new Rectangle(
                (_propertyDepth * borderWidth) + OutlineIconPadding / 2,
                (OwnerGridView.GridEntryHeight - outlineSize) / 2,
                outlineSize,
                outlineSize);

            return _outlineRect;
        }
    }

    /// <summary>
    ///  Recursively resets outline rectangles for this <see cref="GridEntry"/> and it's children.
    /// </summary>
    public void ResetOutlineRectangle()
    {
        _outlineRect = Rectangle.Empty;
        if (ChildCount > 0)
        {
            foreach (GridEntry child in Children)
            {
                child.ResetOutlineRectangle();
            }
        }
    }

    public GridEntry? ParentGridEntry
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
                        _children[i].ParentGridEntry = this;
                    }
                }
            }
        }
    }

    public override GridItem? Parent
    {
        get
        {
            ObjectDisposedException.ThrowIf(Disposed, typeof(GridItem));
            return ParentGridEntry;
        }
    }

    /// <summary>
    ///  Returns the category name of the current property.
    /// </summary>
    public virtual string PropertyCategory => CategoryAttribute.Default.Category;

    /// <summary>
    ///  Returns "depth" of this property. That is, how many parents between
    ///  this property and the root property. The root property has a depth of -1.
    /// </summary>
    public virtual int PropertyDepth => _propertyDepth;

    /// <summary>
    ///  Returns the description helpstring for this GridEntry.
    /// </summary>
    public virtual string? PropertyDescription => null;

    /// <summary>
    ///  Returns the label of this property. Usually this is the property name.
    /// </summary>
    public virtual string? PropertyLabel => null;

    /// <summary>
    ///  Returns non-localized name of this property.
    /// </summary>
    public virtual string? PropertyName => PropertyLabel;

    /// <summary>
    ///  Returns the Type of the value of this <see cref="GridEntry"/>, or null if the value is null.
    /// </summary>
    public virtual Type? PropertyType => PropertyValue?.GetType();

    /// <summary>
    ///  Gets or sets the value for the property that is represented by this <see cref="GridEntry"/>.
    /// </summary>
    public virtual object? PropertyValue
    {
        get => _cacheItems?.LastValue;
        set { }
    }

    public bool ShouldRenderPassword => EntryFlags.HasFlag(Flags.RenderPassword);

    public virtual bool ShouldRenderReadOnly
         => ForceReadOnly
            || EntryFlags.HasFlag(Flags.RenderReadOnly)
            || (!IsValueEditable && !EntryFlags.HasFlag(Flags.ReadOnlyEditable));

    /// <summary>
    ///  Returns the type converter for this entry.
    /// </summary>
    internal virtual TypeConverter TypeConverter
        => _typeConverter ??= TypeDescriptor.GetConverter((PropertyValue ?? PropertyType)!);

    /// <summary>
    ///  Returns the type editor for this entry. This may return null if there is no type editor.
    /// </summary>
    internal virtual UITypeEditor? UITypeEditor
    {
        get
        {
            if (Editor is null && PropertyType is not null)
            {
                Editor = (UITypeEditor?)TypeDescriptor.GetEditor(PropertyType, typeof(UITypeEditor));
            }

            return Editor;
        }
    }

    // Note: we don't do set because of the value class semantics, etc.

    public sealed override object? Value => PropertyValue;

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
                totalCount += ChildCollection[i].VisibleChildCount;
            }

            return totalCount;
        }
    }

    /// <summary>
    ///  Add an event handler to be invoked when the label portion of the property entry is clicked.
    /// </summary>
    public void AddOnLabelClick(EventHandler h) => AddEventHandler(s_labelClickEvent, h);

    /// <summary>
    ///  Add an event handler to be invoked when the label portion of the property entry is double-clicked.
    /// </summary>
    public void AddOnLabelDoubleClick(EventHandler h) => AddEventHandler(s_labelDoubleClickEvent, h);

    /// <summary>
    ///  Add an event handler to be invoked when the value portion of the property entry is clicked.
    /// </summary>
    public void AddOnValueClick(EventHandler h) => AddEventHandler(s_valueClickEvent, h);

    /// <summary>
    ///  Add an event handler to be invoked when the value portion of the prop entry is double-clicked.
    /// </summary>
    public void AddOnValueDoubleClick(EventHandler h) => AddEventHandler(s_valueDoubleClickEvent, h);

    /// <summary>
    ///  Add an event handler to be invoked when the outline icon portion of the prop entry is clicked
    /// </summary>
    public void AddOnOutlineClick(EventHandler h) => AddEventHandler(s_outlineClickEvent, h);

    /// <summary>
    ///  Add an event handler to be invoked when the outline icon portion of the prop entry is double clicked.
    /// </summary>
    public void AddOnOutlineDoubleClick(EventHandler h) => AddEventHandler(s_outlineDoubleClickEvent, h);

    /// <summary>
    ///  Add an event handler to be invoked when the children grid entries are re-created.
    /// </summary>
    public void AddOnRecreateChildren(GridEntryRecreateChildrenEventHandler h) => AddEventHandler(s_recreateChildrenEvent, h);

    internal void ClearCachedValues(bool clearChildren = true)
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
                ChildCollection[i].ClearCachedValues();
            }
        }
    }

    /// <summary>
    ///  Converts the given string of text to a value.
    /// </summary>
    public object? ConvertTextToValue(string? text)
    {
        if (TypeConverter.CanConvertFrom(this, typeof(string)))
        {
            // We will return an empty string when text is null.
            return TypeConverter.ConvertFromString(this, text!);
        }

        return text;
    }

    /// <summary>
    ///  Create the root grid entry given an object or set of objects.
    /// </summary>
    /// <param name="objects">The objects to build the root entry on.</param>
    internal static GridEntry? CreateRootGridEntry(
        PropertyGridView view,
        object[] objects,
        IServiceProvider baseProvider,
        IDesignerHost? currentHost,
        PropertyTab tab,
        PropertySort initialSortType)
    {
        if (objects is null || objects.Length == 0)
        {
            return null;
        }

        return objects.Length == 1
            ? new SingleSelectRootGridEntry(
                view,
                objects[0],
                baseProvider,
                currentHost,
                tab,
                initialSortType)
            : new MultiSelectRootGridEntry(
                view,
                objects,
                baseProvider,
                currentHost,
                tab,
                initialSortType);
    }

    /// <summary>
    ///  Populates the children of this grid entry.
    /// </summary>
    /// <param name="useExistingChildren">
    ///  When set to true, will check existing children to see if they need to be recreated. If they
    ///  haven't changed, the existing children will be used.
    /// </param>
    /// <returns>True if the children are expandable.</returns>
    protected virtual bool CreateChildren(bool useExistingChildren = false)
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
                _children = [];
            }

            return false;
        }

        if (!useExistingChildren && _children is not null && _children.Count > 0)
        {
            return true;
        }

        GridEntry[]? childProperties = GetChildEntries();

        bool expandable = childProperties is not null && childProperties.Length > 0;

        if (useExistingChildren && _children is not null && _children.Count > 0)
        {
            bool same = true;
            if (childProperties is not null && childProperties.Length == _children.Count)
            {
                for (int i = 0; i < childProperties.Length; i++)
                {
                    if (!childProperties[i].EqualsIgnoreParent(_children[i]))
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
                _children = [];
            }

            if (InternalExpanded)
            {
                InternalExpanded = false;
            }
        }
        else
        {
            if (_children is not null && childProperties is not null)
            {
                _children.Clear();
                _children.AddRange(childProperties);
            }
            else
            {
                _children = new GridEntryCollection(childProperties);
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
        _typeConverter = null;
        Editor = null;
        ReleaseUiaProvider();

        if (disposing)
        {
            DisposeChildren();
        }
    }

    internal void ReleaseUiaProvider()
    {
        if (_children?.Count > 0)
        {
            foreach (GridEntry gridEntry in _children)
            {
                gridEntry.ReleaseUiaProvider();
            }
        }

        PInvoke.UiaDisconnectProvider(_accessibleObject);

        _accessibleObject = null;
    }

    public virtual void DisposeChildren()
    {
        _children?.Dispose();
        _children = null;
    }

    ~GridEntry() => Dispose(disposing: false);

    /// <summary>
    ///  Invokes the type editor for this item.
    /// </summary>
    internal virtual void EditPropertyValue(PropertyGridView gridView)
    {
        if (UITypeEditor is null)
        {
            return;
        }

        try
        {
            object? originalValue = PropertyValue;
            object? value = UITypeEditor.EditValue(this, this, originalValue);

            // Since edit value can push a modal loop there is a chance that this gridentry will be zombied
            // before it returns. Make sure we're not disposed.
            if (Disposed)
            {
                return;
            }

            // Push the new value back into the property.
            if (value != originalValue && IsValueEditable)
            {
                gridView.CommitValue(this, value);
            }

            if (InternalExpanded && OwnerGridView is not null)
            {
                // If the edited property is expanded to show sub-properties, then we want to
                // preserve the expanded states of it and all of its descendants. RecreateChildren()
                // has logic that is supposed to do this, but it doesn't do so correctly.
                PropertyGridView.GridPositionData positionData = OwnerGridView.CaptureGridPositionData();
                InternalExpanded = false;
                RecreateChildren();
                positionData.Restore(OwnerGridView);
            }
            else
            {
                // If edited property has no children or is collapsed, we don't need to preserve expanded states.
                RecreateChildren();
            }
        }
        catch (Exception e)
        {
            if (this.TryGetService(out IUIService? uiService))
            {
                uiService.ShowError(e);
            }
            else
            {
                RTLAwareMessageBox.Show(
                    OwnerGridView,
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
    ///  Compares equality, ignoring the parent.
    /// </summary>
    internal bool EqualsIgnoreParent(GridEntry entry)
    {
        if (entry == this)
        {
            return true;
        }

        return entry is not null
            && entry.PropertyLabel is not null
            && entry.PropertyLabel.Equals(PropertyLabel)
            && entry.PropertyType is not null
            && entry.PropertyType.Equals(PropertyType)
            && entry.PropertyDepth == PropertyDepth;
    }

    public override bool Equals(object? obj)
    {
        if (obj is GridEntry entry && EqualsIgnoreParent(entry))
        {
            return entry.ParentGridEntry == ParentGridEntry;
        }

        return false;
    }

    /// <summary>
    ///  Searches for a value of a given property for a value editor user.
    /// </summary>
    private object? FindPropertyValue(string propertyName, Type propertyType)
    {
        object? owner = GetValueOwner();
        if (owner is not null)
        {
            PropertyDescriptor? property = TypeDescriptor.GetProperties(owner)[propertyName];
            if (property is not null && property.PropertyType == propertyType)
            {
                return property.GetValue(owner);
            }
        }

        return _parent?.FindPropertyValue(propertyName, propertyType);
    }

    /// <summary>
    ///  Returns the index of a child <see cref="GridEntry"/>.
    /// </summary>
    internal int GetChildIndex(GridEntry entry) => Children.IndexOf(entry);

    /// <summary>
    ///  Gets the components that own the current value. This is usually the value of the root entry, which is the
    ///  object being browsed. Walks up the <see cref="GridEntry"/> tree looking for an owner that is an
    ///  <see cref="IComponent"/>.
    /// </summary>
    public virtual IComponent[]? GetComponents()
    {
        IComponent? component = Component;
        if (component is not null)
        {
            return [component];
        }

        return null;
    }

    protected int GetLabelTextWidth(string? text, Graphics graphics, Font font)
    {
        if (_cacheItems is null)
        {
            _cacheItems = new CacheItems();
        }
        else if (_cacheItems.UseCompatTextRendering == OwnerGrid.UseCompatibleTextRendering
            && _cacheItems.LastLabel == text
            && font.Equals(_cacheItems.LastLabelFont))
        {
            return _cacheItems.LastLabelWidth;
        }

        SizeF textSize = PropertyGrid.MeasureTextHelper.MeasureText(OwnerGrid, graphics, text, font);

        _cacheItems.LastLabelWidth = (int)textSize.Width;
        _cacheItems.LastLabel = text;
        _cacheItems.LastLabelFont = font;
        _cacheItems.UseCompatTextRendering = OwnerGrid.UseCompatibleTextRendering;

        return _cacheItems.LastLabelWidth;
    }

    public int GetValueTextWidth(string text, Graphics graphics, Font font)
    {
        if (_cacheItems is null)
        {
            _cacheItems = new CacheItems();
        }
        else if (_cacheItems.LastValueTextWidth != -1
            && _cacheItems.LastValueString == text
            && font.Equals(_cacheItems.LastValueFont))
        {
            return _cacheItems.LastValueTextWidth;
        }

        // Value text is rendered using GDI directly but always measured/adjusted using GDI+,
        // so don't use MeasureTextHelper.
        _cacheItems.LastValueTextWidth = (int)graphics.MeasureString(text, font).Width;
        _cacheItems.LastValueString = text;
        _cacheItems.LastValueFont = font;
        return _cacheItems.LastValueTextWidth;
    }

    /// <summary>
    ///  Gets the owner of the current value. This is usually the value of the root entry,
    ///  which is the object being browsed.
    /// </summary>
    public object? GetValueOwner() => _parent is null ? PropertyValue : _parent.GetValueOwnerInternal();

    /// <summary>
    ///  Gets the owner of the current value. This is usually the value of the root entry,
    ///  which is the object being browsed.
    /// </summary>
    /// <devdoc>
    ///  This internal override allows <see cref="CategoryGridEntry"/> to skip to its parent <see cref="PropertyValue"/>
    ///  and <see cref="MultiPropertyDescriptorGridEntry"/> to return it's set of owners.
    /// </devdoc>
    internal virtual object? GetValueOwnerInternal() => PropertyValue;

    /// <summary>
    ///  Returns a string with info about the currently selected <see cref="GridEntry"/>.
    /// </summary>
    public virtual string GetTestingInfo()
        => $@"object = ({FullLabel}), property = ({PropertyLabel},{(PropertyType ?? typeof(object)).AssemblyQualifiedName})
                , value = [{(GetPropertyTextValue()?.Replace('\0', ' ') ?? "null")}], expandable = {Expandable}
                , readOnly = {ShouldRenderReadOnly}";

    /// <summary>
    ///  Returns the child <see cref="GridEntry"/> items for this <see cref="GridEntry"/>.
    /// </summary>
    private GridEntry[]? GetChildEntries()
    {
        object? value = PropertyValue;
        Type? objectType = PropertyType;

        // We don't want to create child entries for null objects.
        if (value is null)
        {
            return null;
        }

        GridEntry[]? entries = null;

        AttributeCollection? browsableAttributes = BrowsableAttributes;
        Attribute[]? attributes = null;
        if (browsableAttributes is not null)
        {
            attributes = new Attribute[browsableAttributes.Count];
            browsableAttributes.CopyTo(attributes, 0);
        }

        PropertyTab? ownerTab = OwnerTab;
        Debug.Assert(ownerTab is not null, "No current tab!");

        try
        {
            bool forceReadOnly = ForceReadOnly;

            if (!forceReadOnly)
            {
                forceReadOnly = TypeDescriptorHelper.TryGetAttribute(value, out ReadOnlyAttribute? readOnlyAttribute)
                    && !readOnlyAttribute.IsDefaultAttribute();
            }

            if (this is not IRootGridEntry && !TypeConverter.GetPropertiesSupported(this))
            {
                // We can't get properties on this sub entry.
                return null;
            }

            // Ask the owning tab for properties if we have one.
            PropertyDescriptorCollection? properties = null;
            PropertyDescriptor? defaultProperty = null;
            if (ownerTab is not null)
            {
                properties = ownerTab.GetProperties(this, value, attributes);
                defaultProperty = ownerTab.GetDefaultProperty(value);
            }
            else
            {
                properties = TypeConverter.GetProperties(this, value, attributes);
                defaultProperty = TypeDescriptor.GetDefaultProperty(value);
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

            if ((properties is null || properties.Count == 0) && objectType is not null && objectType.IsArray && value is not null)
            {
                var objArray = (Array)value;

                entries = new GridEntry[objArray.Length];

                for (int i = 0; i < entries.Length; i++)
                {
                    entries[i] = new ArrayElementGridEntry(OwnerGrid, this, i);
                }
            }
            else
            {
                // Otherwise, create the proper GridEntries.
                bool createInstanceSupported = TypeConverter.GetCreateInstanceSupported(this);
                if (properties is null)
                {
                    return entries;
                }

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
                        object? owner = value;
                        if (value is ICustomTypeDescriptor descriptor)
                        {
                            owner = descriptor.GetPropertyOwner(property);
                        }

                        property.GetValue(owner);
                    }
                    catch (Exception w)
                    {
                        Debug.Assert(!s_pbrsAssertPropsSwitch.Enabled, $"Bad property '{PropertyLabel}.{property.Name}': {w}");

                        hide = true;
                    }

                    newEntry = createInstanceSupported
                        ? new ImmutablePropertyDescriptorGridEntry(OwnerGrid, this, property, hide)
                        : new PropertyDescriptorGridEntry(OwnerGrid, this, property, hide);

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
            Debug.Fail($"Failed to get properties: {e.GetType().Name},{e.Message}\n{e.StackTrace}");
        }

        return entries;
    }

    /// <summary>
    ///  Resets the current item.
    /// </summary>
    public virtual void ResetPropertyValue()
    {
        SendNotificationToParent(Notify.Reset);
        Refresh();
    }

    /// <summary>
    ///  Returns if the property can be reset
    /// </summary>
    public virtual bool CanResetPropertyValue() => SendNotificationToParent(Notify.CanReset);

    /// <summary>
    ///  Called when the item is double clicked.
    /// </summary>
    public virtual bool DoubleClickPropertyValue() => SendNotificationToParent(Notify.DoubleClick);

    /// <summary>
    ///  Returns the text value of this property.
    /// </summary>
    public virtual string GetPropertyTextValue() => GetPropertyTextValue(PropertyValue);

    /// <summary>
    ///  Returns the text value of this property.
    /// </summary>
    public virtual string GetPropertyTextValue(object? value)
    {
        string? textValue = null;

        TypeConverter converter = TypeConverter;
        try
        {
            textValue = converter.ConvertToString(this, value);
        }
        catch (Exception t)
        {
            Debug.Fail($"Bad Type Converter! {t.GetType().Name}, {t.Message},{converter}", t.ToString());
        }

        textValue ??= string.Empty;

        return textValue;
    }

    /// <summary>
    ///  Returns the standard text values of this property.
    /// </summary>
    public virtual object[] GetPropertyValueList()
    {
        if (TypeConverter.GetStandardValues(this) is { } values)
        {
            object[] valueArray = new object[values.Count];
            values.CopyTo(valueArray, 0);
            return valueArray;
        }

        return [];
    }

    public override int GetHashCode() => HashCode.Combine(PropertyLabel, PropertyType, GetType());

    /// <summary>
    ///  Checks if any given flags are set.
    /// </summary>
    protected bool GetFlagSet(Flags flags) => (flags & EntryFlags) != 0;

    protected Font GetFont(bool boldFont)
    {
        if (OwnerGridView is null)
        {
            return Control.DefaultFont;
        }

        return boldFont
            ? OwnerGridView.GetBoldFont()
            : OwnerGridView.GetBaseFont();
    }

    /// <summary>
    ///  Retrieves the requested service. This may return null if the requested service is not available.
    /// </summary>
    public virtual object? GetService(Type serviceType)
        => serviceType == typeof(GridItem) ? this : (_parent?.GetService(serviceType));

    /// <summary>
    ///  Paints the label portion of this <see cref="GridEntry"/> into the given <see cref="Graphics"/> object.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is called by the <see cref="GridEntry"/> host (the <see cref="PropertyGridView"/>) when this
    ///   <see cref="GridEntry"/> needs to be painted.
    ///  </para>
    /// </remarks>
    public virtual void PaintLabel(
        Graphics g,
        Rectangle rect,
        Rectangle clipRect,
        bool selected,
        bool paintFullLabel)
    {
        if (OwnerGridView is not PropertyGridView ownerGrid)
        {
            throw new InvalidOperationException();
        }

        string? label = PropertyLabel;
        int borderWidth = ownerGrid.OutlineIconSize + OutlineIconPadding;

        // Fill the background if necessary.
        Color backColor = selected ? ownerGrid.SelectedItemWithFocusBackColor : BackgroundColor;

        // If we don't have focus, paint with the line color.
        if (selected && !_hasFocus)
        {
            backColor = ownerGrid.LineColor;
        }

        bool bold = EntryFlags.HasFlag(Flags.LabelBold);
        Font font = GetFont(boldFont: bold);

        int labelWidth = GetLabelTextWidth(label, g, font);

        int neededWidth = paintFullLabel ? labelWidth : 0;
        int stringX = rect.X + PropertyLabelIndent;

        using var backBrush = backColor.GetCachedSolidBrushScope();
        if (paintFullLabel && (neededWidth >= (rect.Width - (stringX + 2))))
        {
            // GdiPlusSpace = extra needed to ensure text draws completely and isn't clipped.
            int totalWidth = stringX + neededWidth + PropertyGridView.GdiPlusSpace;

            // Blank out the space we're going to use.
            g.FillRectangle(backBrush, borderWidth - 1, rect.Y, totalWidth - borderWidth + 3, rect.Height);

            // Draw an end line.
            using var linePen = ownerGrid.LineColor.GetCachedPenScope();
            g.DrawLine(linePen, totalWidth, rect.Y, totalWidth, rect.Height);

            // Set the new width that we can draw into.
            rect.Width = totalWidth - rect.X;
        }
        else
        {
            // Normal case -- no pseudo-tooltip for the label.
            g.FillRectangle(backBrush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        // Draw the border stripe on the left.
        using var stripeBrush = ownerGrid.LineColor.GetCachedSolidBrushScope();
        g.FillRectangle(stripeBrush, rect.X, rect.Y, borderWidth, rect.Height);

        if (selected && _hasFocus)
        {
            using var focusBrush = ownerGrid.SelectedItemWithFocusBackColor.GetCachedSolidBrushScope();
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

            // We need to invert the color only if in Highcontrast mode, targeting 4.7.1 and above, Gridcategory and
            // not a developer override. This is required to achieve required contrast ratio.
            bool shouldInvert = ColorInversionNeededInHighContrast && (bold || (selected && !_hasFocus));

            // Do actual drawing.

            // A brush is needed if using GDI+ only (UseCompatibleTextRendering); if using GDI, only the color is needed.
            Color textColor = selected && _hasFocus
                ? ownerGrid.SelectedItemWithFocusForeColor
                : shouldInvert
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
            oldClip.Dispose();   // SetClip copies the passed in Region.

            _labelTipPoint = maxSpace <= labelWidth ? new Point(stringX + 2, rect.Y + 1) : InvalidPoint;
        }

        rect.Y -= 1;
        rect.Height += 2;

        if (Expandable)
        {
            PaintOutlineGlyph(g, rect);
        }
    }

    /// <summary>
    ///  Paints the outline portion (the expand / collapse area to the left) of this <see cref="GridEntry"/> into
    ///  the given <see cref="Graphics"/> object.
    /// </summary>
    private void PaintOutlineGlyph(Graphics g, Rectangle r)
    {
        if (OwnerGridView is { } owner && owner.IsExplorerTreeSupported)
        {
            // Draw tree-view glyphs with the current ExplorerTreeView UxTheme

            if (!_lastPaintWithExplorerStyle)
            {
                // Size of Explorer Tree style glyph (triangle) is different from +/- glyph, so when we change the
                // visual style (such as changing the Windows theme), we need to recalculate outlineRect.

                _outlineRect = Rectangle.Empty;
                _lastPaintWithExplorerStyle = true;
            }

            PaintOutlineWithExplorerTreeStyle(g, r, OwnerGridView.HWNDInternal);
        }
        else
        {
            // Draw tree-view glyphs as +/-

            if (_lastPaintWithExplorerStyle)
            {
                // Size of Explorer Tree style glyph (triangle) is different from +/- glyph, so when we change the
                // visual style (such as changing the Windows theme), we need to recalculate outlineRect.

                _outlineRect = Rectangle.Empty;
                _lastPaintWithExplorerStyle = false;
            }

            PaintOutlineWithClassicStyle(g, r);
        }

        // Draw the expansion glyph with the Explorer tree style.
        void PaintOutlineWithExplorerTreeStyle(Graphics g, Rectangle r, HWND hwnd)
        {
            // Make sure we're in our bounds.
            Rectangle outline = Rectangle.Intersect(r, OutlineRectangle);
            if (outline.IsEmpty)
            {
                return;
            }

            bool expanded = InternalExpanded;

            // Invert color if it is not overridden by developer.
            if (g is not null && ColorInversionNeededInHighContrast)
            {
                Color textColor = InvertColor(OwnerGrid.LineColor);
                using var brush = textColor.GetCachedSolidBrushScope();
                g.FillRectangle(brush, outline);
            }

            if (g is null)
            {
                throw new InvalidOperationException();
            }

            if (ColorInversionNeededInHighContrast || !expanded)
            {
                VisualStyleElement element = expanded
                    ? VisualStyleElement.ExplorerTreeView.Glyph.Opened
                    : VisualStyleElement.ExplorerTreeView.Glyph.Closed;

                VisualStyleRenderer explorerTreeRenderer = new(element);
                RedrawExplorerTreeViewClosedGlyph(g, explorerTreeRenderer, outline, hwnd);
            }
            else
            {
                using DeviceContextHdcScope hdc = new(g);
                VisualStyleRenderer explorerTreeRenderer = new(VisualStyleElement.ExplorerTreeView.Glyph.Opened);
                explorerTreeRenderer.DrawBackground(hdc, outline, hwnd);
            }

            unsafe void RedrawExplorerTreeViewClosedGlyph(
                Graphics graphics,
                VisualStyleRenderer explorerTreeRenderer,
                Rectangle rectangle,
                HWND hwnd)
            {
                Color backgroundColor = ColorInversionNeededInHighContrast ? InvertColor(OwnerGrid.LineColor) : OwnerGrid.LineColor;
                using CreateDcScope compatibleDC = new(default);

                int planes = PInvokeCore.GetDeviceCaps(compatibleDC, GET_DEVICE_CAPS_INDEX.PLANES);
                int bitsPixel = PInvokeCore.GetDeviceCaps(compatibleDC, GET_DEVICE_CAPS_INDEX.BITSPIXEL);
                using HBITMAP compatibleBitmap = PInvokeCore.CreateBitmap(rectangle.Width, rectangle.Height, (uint)planes, (uint)bitsPixel, lpBits: null);
                using SelectObjectScope targetBitmapSelection = new(compatibleDC, compatibleBitmap);

                using CreateBrushScope brush = new(backgroundColor);
                compatibleDC.HDC.FillRectangle(new Rectangle(0, 0, rectangle.Width, rectangle.Height), brush);
                explorerTreeRenderer.DrawBackground(compatibleDC, new Rectangle(0, 0, rectangle.Width, rectangle.Height), hwnd);

                using Bitmap bitmap = Image.FromHbitmap(compatibleBitmap);
                ControlPaint.InvertForeColorIfNeeded(bitmap, backgroundColor);
                graphics.DrawImage(bitmap, rectangle, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
            }
        }

        // Draw the expansion glyph as a plus/minus.
        void PaintOutlineWithClassicStyle(Graphics g, Rectangle r)
        {
            // Make sure we're in our bounds.
            Rectangle outline = Rectangle.Intersect(r, OutlineRectangle);
            if (outline.IsEmpty)
            {
                return;
            }

            bool expanded = InternalExpanded;

            Color penColor = OwnerGridView?.TextColor ?? default;

            if (ColorInversionNeededInHighContrast)
            {
                // Inverting text color to background to get required contrast ratio.
                penColor = InvertColor(OwnerGrid.LineColor);
            }
            else
            {
                // Fill the background when not inverting.
                using var brush = BackgroundColor.GetCachedSolidBrushScope();
                g.FillRectangle(brush, outline);
            }

            // Draw the border.
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
            if (!expanded)
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
    ///  Paints the value portion of this <see cref="GridEntry"/> into the given <see cref="Graphics"/> object.
    ///  This is called by the <see cref="GridEntry"/> host (the <see cref="PropertyGridView"/>) when this
    ///  <see cref="GridEntry"/> is to be painted.
    /// </summary>
    /// <param name="text">
    ///  Optional text representation of the value. If not specified, will use the <see cref="PropertyValue"/> directly.
    /// </param>
    public virtual void PaintValue(
        Graphics g,
        Rectangle rect,
        Rectangle clipRect,
        PaintValueFlags paintFlags,
        string? text = null)
    {
        if (OwnerGridView is not PropertyGridView ownerGrid)
        {
            throw new InvalidOperationException();
        }

        Color textColor = ShouldRenderReadOnly ? ownerGrid.GrayTextColor : ownerGrid.TextColor;
        object? value;

        if (text is null)
        {
            if (_cacheItems is not null && _cacheItems.UseValueString)
            {
                text = _cacheItems.LastValueString;
                value = _cacheItems.LastValue;
            }
            else
            {
                value = PropertyValue;
                text = GetPropertyTextValue(value);

                _cacheItems ??= new CacheItems();
                _cacheItems.LastValueString = text;
                _cacheItems.UseValueString = true;
                _cacheItems.LastValueTextWidth = -1;
                _cacheItems.LastValueFont = null;
                _cacheItems.LastValue = value;
            }
        }
        else
        {
            value = ConvertTextToValue(text);
            text = GetPropertyTextValue(value);
        }

        // Paint out the main rect using the appropriate brush.
        Color backColor = BackgroundColor;

        if (paintFlags.HasFlag(PaintValueFlags.DrawSelected))
        {
            backColor = ownerGrid.SelectedItemWithFocusBackColor;
            textColor = ownerGrid.SelectedItemWithFocusForeColor;
        }

        using var backBrush = backColor.GetCachedSolidBrushScope();
        g.FillRectangle(backBrush, clipRect);

        int paintIndent = 0;
        if (IsCustomPaint)
        {
            paintIndent = ownerGrid.ValuePaintIndent;
            Rectangle rectPaint = new(
                rect.X + 1,
                rect.Y + 1,
                ownerGrid.ValuePaintWidth,
                ownerGrid.GridEntryHeight - 2);

            if (!Rectangle.Intersect(rectPaint, clipRect).IsEmpty)
            {
                UITypeEditor?.PaintValue(new PaintValueEventArgs(this, value, g, rectPaint));

                // Paint a border around the area
                rectPaint.Width--;
                rectPaint.Height--;
                g.DrawRectangle(SystemPens.WindowText, rectPaint);
            }
        }

        rect.X += paintIndent + PropertyGridView.ValueStringIndent;
        rect.Width -= paintIndent + 2 * PropertyGridView.ValueStringIndent;

        // Bold the property if we need to persist it (e.g. it's not the default value).
        bool valueModified = paintFlags.HasFlag(PaintValueFlags.CheckShouldSerialize) && ShouldSerializePropertyValue();

        // If we have text to paint, paint it.
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (text.Length > MaximumLengthOfPropertyString)
        {
            text = text[..MaximumLengthOfPropertyString];
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

        backColor = OwnerGridView is not { } owner
            ? default
            : paintFlags.HasFlag(PaintValueFlags.DrawSelected)
                ? owner.SelectedItemWithFocusBackColor
                : owner.BackColor;

        DRAW_TEXT_FORMAT format = DRAW_TEXT_FORMAT.DT_EDITCONTROL | DRAW_TEXT_FORMAT.DT_EXPANDTABS | DRAW_TEXT_FORMAT.DT_NOCLIP
            | DRAW_TEXT_FORMAT.DT_SINGLELINE | DRAW_TEXT_FORMAT.DT_NOPREFIX;

        if (ownerGrid.DrawValuesRightToLeft)
        {
            format |= DRAW_TEXT_FORMAT.DT_RIGHT | DRAW_TEXT_FORMAT.DT_RTLREADING;
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
            object? owner = GetValueOwner();
            if (owner is not null)
            {
                ComponentChangeService?.OnComponentChanging(owner, PropertyDescriptor);
                return true;
            }

            return false;
        }
        catch (CheckoutException e) when (e == CheckoutException.Canceled)
        {
            return false;
        }
    }

    public virtual void OnComponentChanged()
    {
        object? owner = GetValueOwner();
        if (owner is not null)
        {
            ComponentChangeService?.OnComponentChanged(owner, PropertyDescriptor);
        }
    }

    /// <summary>
    ///  Called when the GridEntry is clicked.
    /// </summary>
    public virtual bool OnMouseClick(int x, int y, int count, MouseButtons button)
    {
        // Where are we at?
        PropertyGridView gridHost = OwnerGridView!;
        Debug.Assert(gridHost is not null, "No prop entry host!");

        // Make sure it's the left button.
        if ((button & MouseButtons.Left) != MouseButtons.Left)
        {
            return false;
        }

        int labelWidth = gridHost.LabelWidth;

        // Are we in the label?
        if (x >= 0 && x <= labelWidth)
        {
            // Are we on the outline?
            if (Expandable)
            {
                Rectangle outlineRect = OutlineRectangle;
                if (outlineRect.Contains(x, y))
                {
                    if (count % 2 == 0)
                    {
                        RaiseEvent(s_outlineDoubleClickEvent, EventArgs.Empty);
                    }
                    else
                    {
                        OnOutlineClick(EventArgs.Empty);
                    }

                    return true;
                }
            }

            RaiseEvent(count % 2 == 0 ? s_labelDoubleClickEvent : s_labelClickEvent, EventArgs.Empty);
            return true;
        }

        // Are we in the value?
        labelWidth += PropertyGridView.SplitterWidth;
        if (x >= labelWidth && x <= labelWidth + gridHost.ValueWidth)
        {
            RaiseEvent(count % 2 == 0 ? s_valueDoubleClickEvent : s_valueClickEvent, EventArgs.Empty);
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Called when the outline icon portion of this <see cref="GridEntry"/> is clicked.
    /// </summary>
    protected void OnOutlineClick(EventArgs e) => RaiseEvent(s_outlineClickEvent, e);

    internal bool OnValueReturnKey() => SendNotificationToParent(Notify.Return);

    /// <summary>
    ///  Sets the specified flag.
    /// </summary>
    protected void SetFlag(Flags flag, bool value)
        => _flags = (EntryFlags & ~flag) | (value ? flag : 0);

    public override bool Select()
    {
        if (Disposed)
        {
            return false;
        }

        try
        {
            if (OwnerGridView is not PropertyGridView propertyGridView)
            {
                return false;
            }

            propertyGridView.SelectedGridEntry = this;

            return true;
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
        }

        return false;
    }

    /// <summary>
    ///  Checks if this value should be persisted.
    /// </summary>
    internal bool ShouldSerializePropertyValue()
    {
        if (_cacheItems is not null)
        {
            if (_cacheItems.UseShouldSerialize)
            {
                return _cacheItems.LastShouldSerialize;
            }
            else
            {
                _cacheItems.LastShouldSerialize = SendNotificationToParent(Notify.ShouldPersist);
                _cacheItems.UseShouldSerialize = true;
            }
        }
        else
        {
            _cacheItems = new CacheItems
            {
                LastShouldSerialize = SendNotificationToParent(Notify.ShouldPersist),
                UseShouldSerialize = true
            };
        }

        return _cacheItems.LastShouldSerialize;
    }

    private static PropertyDescriptor[] SortParenProperties(PropertyDescriptor[] props)
    {
        PropertyDescriptor[]? newProperties = null;
        int newPosition = 0;

        // First scan the list and move any parenthesized properties to the front.
        for (int i = 0; i < props.Length; i++)
        {
            if (props[i].GetAttribute<ParenthesizePropertyNameAttribute>()?.NeedParenthesis ?? false)
            {
                newProperties ??= new PropertyDescriptor[props.Length];
                newProperties[newPosition++] = props[i];
                props[i] = null!;
            }
        }

        // Second pass, copy any that didn't have the parenthesis.
        if (newProperties is not null)
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
    ///  Sends a notification to the given owner.
    /// </summary>
    /// <param name="owner">
    ///  The owner of the <see cref="GridItem"/>.
    /// </param>
    /// <returns>
    ///  The result of the notification.
    /// </returns>
    protected virtual bool SendNotification(object? owner, Notify notification) => false;

    /// <summary>
    ///  Sends a notification to the owner of the given <paramref name="entry"/>.
    /// </summary>
    /// <returns>
    ///  The result of the notification.
    /// </returns>
    internal virtual bool SendNotification(GridEntry entry, Notify notification)
        => entry.SendNotification(entry.GetValueOwner(), notification);

    /// <summary>
    ///  Sends a notification to the owner of the <see cref="ParentGridEntry"/> if it exists.
    /// </summary>
    /// <returns>
    ///  The result of the notification. Returns true if there is no parent.
    /// </returns>
    internal bool SendNotificationToParent(Notify type)
        => _parent is null || _parent.SendNotification(this, type);

    protected void RecreateChildren() => RecreateChildren(-1);

    protected void RecreateChildren(int oldCount)
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

        if (GetEventHandler(s_recreateChildrenEvent) is GridEntryRecreateChildrenEventHandler handler)
        {
            handler(this, new(oldCount, VisibleChildCount));
        }
    }

    /// <summary>
    ///  Refresh the current GridEntry's value and it's children.
    /// </summary>
    public void Refresh()
    {
        Type? type = PropertyType;
        if (type is not null && type.IsArray)
        {
            CreateChildren(useExistingChildren: true);
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

    public void RemoveOnLabelClick(EventHandler handler) => RemoveEventHandler(s_labelClickEvent, handler);

    public void RemoveOnLabelDoubleClick(EventHandler handler) => RemoveEventHandler(s_labelDoubleClickEvent, handler);

    public void RemoveOnValueClick(EventHandler handler) => RemoveEventHandler(s_valueClickEvent, handler);

    public void RemoveOnValueDoubleClick(EventHandler handler) => RemoveEventHandler(s_valueDoubleClickEvent, handler);

    public void RemoveOnOutlineClick(EventHandler handler) => RemoveEventHandler(s_outlineClickEvent, handler);

    public void RemoveOnOutlineDoubleClick(EventHandler handler) => RemoveEventHandler(s_outlineDoubleClickEvent, handler);

    public void RemoveOnRecreateChildren(GridEntryRecreateChildrenEventHandler handler)
        => RemoveEventHandler(s_recreateChildrenEvent, handler);

    private void ResetState()
    {
        ClearFlags();
        ClearCachedValues();
    }

    /// <summary>
    ///  Sets the value of this <see cref="GridEntry"/> from text.
    /// </summary>
    public bool SetPropertyTextValue(string? text)
    {
        bool childrenPrior = _children is not null && _children.Count > 0;
        PropertyValue = ConvertTextToValue(text);
        CreateChildren();
        bool childrenAfter = _children is not null && _children.Count > 0;
        return childrenPrior != childrenAfter;
    }

    public override string ToString() => $"{GetType().FullName} {PropertyLabel}";

    protected virtual void AddEventHandler(object key, Delegate handler)
    {
        lock (_lock)
        {
            if (handler is null)
            {
                return;
            }

            for (EventEntry? e = _eventList; e is not null; e = e.Next)
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
        Delegate? handler = GetEventHandler(key);
        if (handler is not null)
        {
            ((EventHandler)handler)(this, e);
        }
    }

    protected virtual Delegate? GetEventHandler(object key)
    {
        lock (_lock)
        {
            for (EventEntry? e = _eventList; e is not null; e = e.Next)
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
        lock (_lock)
        {
            if (handler is null)
            {
                return;
            }

            for (EventEntry? entry = _eventList, previous = null; entry is not null; previous = entry, entry = entry.Next)
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
