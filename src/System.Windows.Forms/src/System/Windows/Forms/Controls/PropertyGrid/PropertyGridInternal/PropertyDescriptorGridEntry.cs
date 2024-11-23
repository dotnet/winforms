// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyDescriptorGridEntry : GridEntry
{
    private TypeConverter? _exceptionConverter;
    private UITypeEditor? _exceptionEditor;
    private bool _isSerializeContentsProperty;
    private bool? _parensAroundName;
    private IPropertyValueUIService? _propertyValueUIService;
    protected IEventBindingService? _eventBindings;
    private bool _propertyValueUIServiceChecked;
    private PropertyValueUIItem[]? _propertyValueUIItems;
    private Rectangle[]? _uiItemRects;
    private bool _readOnlyVerified;
    private bool _forceRenderReadOnly;
    private string? _helpKeyword;
    private string? _toolTipText;
    private readonly bool _hide;

    private const int LogicalImageSize = 8;
    private static int s_imageSize = LogicalImageSize;
    private static bool s_isScalingInitialized;

    private static IEventBindingService? s_targetBindingService;
    private static IComponent? s_targetComponent;
    private static EventDescriptor? s_targetEventdesc;

    internal PropertyDescriptorGridEntry(
        PropertyGrid ownerGrid,
        GridEntry parent,
        PropertyDescriptor propertyDescriptor,
        bool hide)
        : base(ownerGrid, parent)
    {
        _hide = hide;
        PropertyDescriptor = propertyDescriptor;
        Initialize(propertyDescriptor);
    }

    public override bool AllowMerge
        => PropertyDescriptor.GetAttribute<MergablePropertyAttribute>()?.IsDefaultAttribute() ?? true;

    protected override AttributeCollection Attributes => PropertyDescriptor.Attributes;

    public override string? HelpKeyword
    {
        get
        {
            if (_helpKeyword is not null)
            {
                return _helpKeyword;
            }

            object? owner = GetValueOwner();
            if (owner is null)
            {
                return null;
            }

            if (PropertyDescriptor.TryGetAttribute(out HelpKeywordAttribute? helpAttribute) &&
                !helpAttribute.IsDefaultAttribute())
            {
                return helpAttribute.HelpKeyword;
            }
            else if (this is ImmutablePropertyDescriptorGridEntry)
            {
                _helpKeyword = PropertyName;

                GridEntry entry = this;

                while (entry.ParentGridEntry is not null)
                {
                    entry = entry.ParentGridEntry;

                    // For value classes, the equality will never work, so just try the type equality.
                    if (entry.PropertyValue == owner
                        || (owner.GetType().IsValueType && owner.GetType() == entry.PropertyValue?.GetType()))
                    {
                        _helpKeyword = $"{entry.PropertyName}.{_helpKeyword}";
                        break;
                    }
                }
            }
            else
            {
                string? typeName;

                Type? componentType = PropertyDescriptor.ComponentType;

                if (componentType is not null && componentType.IsCOMObject)
                {
                    typeName = TypeDescriptor.GetClassName(owner);
                }
                else
                {
                    // Make sure this property is declared on a class that is related to the component we're
                    // looking at. If it's not, it could be a shadow property so we need to try to find the
                    // real property.
                    Type ownerType = owner.GetType();
                    if (componentType is not null && (!componentType.IsPublic || !componentType.IsAssignableFrom(ownerType)))
                    {
                        PropertyDescriptor? componentProperty = TypeDescriptor.GetProperties(ownerType)[PropertyName!];
                        componentType = componentProperty?.ComponentType;
                    }

                    typeName = componentType is null ? TypeDescriptor.GetClassName(owner) : componentType.FullName;
                }

                _helpKeyword = $"{typeName}.{PropertyDescriptor.Name}";
            }

            return _helpKeyword;
        }
    }

    internal override string? LabelToolTipText => _toolTipText ?? base.LabelToolTipText;

    internal override bool Enumerable => base.Enumerable && !IsPropertyReadOnly;

    internal virtual bool IsPropertyReadOnly => PropertyDescriptor.IsReadOnly;

    public override bool IsValueEditable => _exceptionConverter is null && !IsPropertyReadOnly && base.IsValueEditable;

    public override bool NeedsDropDownButton => base.NeedsDropDownButton && !IsPropertyReadOnly;

    internal bool ParensAroundName
    {
        get
        {
            if (!_parensAroundName.HasValue)
            {
                _parensAroundName = PropertyDescriptor.GetAttribute<ParenthesizePropertyNameAttribute>()?.NeedParenthesis ?? false;
            }

            return _parensAroundName.Value;
        }
    }

    public override string PropertyCategory
    {
        get
        {
            string? category = PropertyDescriptor.Category;
            if (category is null || category.Length == 0)
            {
                category = base.PropertyCategory;
            }

            return category;
        }
    }

    public sealed override PropertyDescriptor PropertyDescriptor { get; }

    public override string? PropertyDescription => PropertyDescriptor.Description;

    public override string? PropertyLabel
    {
        get
        {
            string? label = PropertyDescriptor.DisplayName;
            if (ParensAroundName)
            {
                label = $"({label})";
            }

            return label;
        }
    }

    /// <summary>
    ///  Returns non-localized name of this property.
    /// </summary>
    public override string? PropertyName => PropertyDescriptor.Name;

    public override Type? PropertyType => PropertyDescriptor.PropertyType;

    /// <summary>
    ///  Gets or sets the value for the property that is represented by this GridEntry.
    /// </summary>
    public override object? PropertyValue
    {
        get
        {
            try
            {
                object? value = GetPropertyValue(GetValueOwner());

                if (_exceptionConverter is not null)
                {
                    // Undo the exception converter.
                    ClearFlags();
                    _exceptionConverter = null;
                    _exceptionEditor = null;
                }

                return value;
            }
            catch (Exception e)
            {
                if (_exceptionConverter is null)
                {
                    // Clear the flags.
                    ClearFlags();
                    _exceptionConverter = new ExceptionConverter();
                    _exceptionEditor = new ExceptionEditor();
                }

                return e;
            }
        }
        set
        {
            SetPropertyValue(GetValueOwner(), value, reset: false, undoText: null);
        }
    }

    private IPropertyValueUIService? PropertyValueUIService
    {
        get
        {
            if (!_propertyValueUIServiceChecked && _propertyValueUIService is null)
            {
                _propertyValueUIService = (IPropertyValueUIService?)GetService(typeof(IPropertyValueUIService));
                _propertyValueUIServiceChecked = true;
            }

            return _propertyValueUIService;
        }
    }

    public override bool ShouldRenderReadOnly
    {
        get
        {
            if (base.ForceReadOnly || _forceRenderReadOnly)
            {
                return true;
            }

            // If read only editable is set, make sure it's valid.
            if (PropertyDescriptor.IsReadOnly && !_readOnlyVerified && GetFlagSet(Flags.ReadOnlyEditable))
            {
                Type? propertyType = PropertyType;

                if (propertyType is not null && (propertyType.IsArray || propertyType.IsValueType || propertyType.IsPrimitive))
                {
                    SetFlag(Flags.ReadOnlyEditable, false);
                    SetFlag(Flags.RenderReadOnly, true);
                    _forceRenderReadOnly = true;
                }
            }

            _readOnlyVerified = true;

            return base.ShouldRenderReadOnly && !_isSerializeContentsProperty && !NeedsModalEditorButton;
        }
    }

    internal override TypeConverter TypeConverter
    {
        get
        {
            if (_exceptionConverter is not null)
            {
                return _exceptionConverter;
            }

            _typeConverter ??= PropertyDescriptor.Converter;

            return base.TypeConverter;
        }
    }

    internal override UITypeEditor? UITypeEditor
    {
        get
        {
            if (_exceptionEditor is not null)
            {
                return _exceptionEditor;
            }

            Editor = (UITypeEditor?)PropertyDescriptor.GetEditor(typeof(UITypeEditor));

            return base.UITypeEditor;
        }
    }

    internal override void EditPropertyValue(PropertyGridView gridView)
    {
        base.EditPropertyValue(gridView);

        if (!IsValueEditable)
        {
            if (PropertyDescriptor.TryGetAttribute(out RefreshPropertiesAttribute? refreshAttribute) &&
                !refreshAttribute.RefreshProperties.Equals(RefreshProperties.None))
            {
                OwnerGridView?.Refresh(fullRefresh: refreshAttribute.Equals(RefreshPropertiesAttribute.All));
            }
        }
    }

    internal override Point GetLabelToolTipLocation(int mouseX, int mouseY)
    {
        if (_propertyValueUIItems is not null
            && _uiItemRects is not null
            && OwnerGridView is not null)
        {
            for (int i = 0; i < _propertyValueUIItems.Length; i++)
            {
                if (_uiItemRects[i].Contains(mouseX, OwnerGridView.GridEntryHeight / 2))
                {
                    _toolTipText = _propertyValueUIItems[i].ToolTip;
                    return new Point(mouseX, mouseY);
                }
            }
        }

        _toolTipText = null;
        return base.GetLabelToolTipLocation(mouseX, mouseY);
    }

    private object? GetPropertyValue(object? owner)
    {
        if (owner is ICustomTypeDescriptor descriptor)
        {
            owner = descriptor.GetPropertyOwner(PropertyDescriptor);
        }

        return PropertyDescriptor.GetValue(owner);
    }

    protected void Initialize(PropertyDescriptor propertyDescriptor)
    {
        _isSerializeContentsProperty = propertyDescriptor.SerializationVisibility == DesignerSerializationVisibility.Content;

        if (!_hide && IsPropertyReadOnly)
        {
            SetFlag(Flags.TextEditable, false);
        }

        if (_isSerializeContentsProperty && TypeConverter.GetPropertiesSupported())
        {
            SetFlag(Flags.Expandable, true);
        }
    }

    protected virtual void NotifyParentsOfChanges(GridEntry? entry)
    {
        // See if we need to notify the parent(s) up the chain.
        while (entry is PropertyDescriptorGridEntry propertyEntry &&
            propertyEntry.PropertyDescriptor.Attributes.Contains(NotifyParentPropertyAttribute.Yes))
        {
            // Find the next parent property with a different value owner.
            object? owner = entry.GetValueOwner();

            // When owner is an instance of a value type we can't use == in the following while condition.
            bool isValueType = owner?.GetType().IsValueType ?? false;

            // Find the next property descriptor with a different parent.
            while (entry is not PropertyDescriptorGridEntry
                || isValueType ? owner!.Equals(entry.GetValueOwner()) : owner == entry.GetValueOwner())
            {
                entry = entry.ParentGridEntry;
                if (entry is null)
                {
                    break;
                }
            }

            // Fire the change on the owner.
            if (entry is not null)
            {
                owner = entry.GetValueOwner();

                if (owner is not null)
                {
                    ComponentChangeService?.OnComponentChanging(owner, entry.PropertyDescriptor);
                    ComponentChangeService?.OnComponentChanged(owner, entry.PropertyDescriptor);
                }

                // Clear the value so it paints correctly next time.
                entry.ClearCachedValues(clearChildren: false);
                OwnerGridView?.InvalidateGridEntryValue(entry);
            }
        }
    }

    protected override bool SendNotification(object? owner, Notify type)
    {
        if (owner is ICustomTypeDescriptor descriptor)
        {
            owner = descriptor.GetPropertyOwner(PropertyDescriptor);
        }

        switch (type)
        {
            case Notify.Reset:

                SetPropertyValue(owner, value: null, reset: true, undoText: string.Format(SR.PropertyGridResetValue, PropertyName));
                if (_propertyValueUIItems is not null)
                {
                    for (int i = 0; i < _propertyValueUIItems.Length; i++)
                    {
                        _propertyValueUIItems[i].Reset();
                    }
                }

                _propertyValueUIItems = null;
                return false;
            case Notify.CanReset:
                try
                {
                    return PropertyDescriptor.CanResetValue(owner!)
                        || (_propertyValueUIItems is not null && _propertyValueUIItems.Length > 0);
                }
                catch
                {
                    if (_exceptionConverter is null)
                    {
                        ClearFlags();
                        _exceptionConverter = new ExceptionConverter();
                        _exceptionEditor = new ExceptionEditor();
                    }

                    return false;
                }

            case Notify.ShouldPersist:
                try
                {
                    return PropertyDescriptor.ShouldSerializeValue(owner!);
                }
                catch
                {
                    if (_exceptionConverter is null)
                    {
                        ClearFlags();
                        _exceptionConverter = new ExceptionConverter();
                        _exceptionEditor = new ExceptionEditor();
                    }

                    return false;
                }

            case Notify.DoubleClick:
            case Notify.Return:
                _eventBindings ??= this.GetService<IEventBindingService>();

                if (_eventBindings?.GetEvent(PropertyDescriptor) is not null)
                {
                    return ViewEvent(owner, newHandler: null, eventDescriptor: null, alwaysNavigate: true);
                }

                break;
        }

        return false;
    }

    public override void OnComponentChanged()
    {
        base.OnComponentChanged();

        // If we got this it means someone called ITypeDescriptorContext.OnComponentChanged.
        // We need to echo that change up the inheritance in case the owner object isn't a sited component.
        NotifyParentsOfChanges(this);
    }

    public override bool OnMouseClick(int x, int y, int count, MouseButtons button)
    {
        if (_propertyValueUIItems is not null
            && _uiItemRects is not null
            && count == 2
            && ((button & MouseButtons.Left) == MouseButtons.Left)
            && OwnerGridView is not null)
        {
            for (int i = 0; i < _propertyValueUIItems.Length; i++)
            {
                if (_uiItemRects[i].Contains(x, OwnerGridView.GridEntryHeight / 2))
                {
                    _propertyValueUIItems[i].InvokeHandler(this, PropertyDescriptor, _propertyValueUIItems[i]);
                    return true;
                }
            }
        }

        return base.OnMouseClick(x, y, count, button);
    }

    public override void PaintLabel(
        Graphics g,
        Rectangle rect,
        Rectangle clipRect,
        bool selected,
        bool paintFullLabel)
    {
        base.PaintLabel(g, rect, clipRect, selected, paintFullLabel);

        IPropertyValueUIService? uiService = PropertyValueUIService;

        if (uiService is null)
        {
            return;
        }

        _propertyValueUIItems = uiService.GetPropertyUIValueItems(this, PropertyDescriptor);

        if (_propertyValueUIItems is null)
        {
            return;
        }

        if (_uiItemRects is null || _uiItemRects.Length != _propertyValueUIItems.Length)
        {
            _uiItemRects = new Rectangle[_propertyValueUIItems.Length];
        }

        if (!s_isScalingInitialized)
        {
            s_imageSize = ScaleHelper.ScaleToInitialSystemDpi(LogicalImageSize);
            s_isScalingInitialized = true;
        }

        for (int i = 0; i < _propertyValueUIItems.Length; i++)
        {
            _uiItemRects[i] = new Rectangle(
                rect.Right - ((s_imageSize + 1) * (i + 1)),
                (rect.Height - s_imageSize) / 2,
                s_imageSize,
                s_imageSize);
            g.DrawImage(_propertyValueUIItems[i].Image, _uiItemRects[i]);
        }

        if (OwnerGridView is { } ownerGridView)
        {
            ownerGridView.LabelPaintMargin = (s_imageSize + 1) * _propertyValueUIItems.Length;
        }
    }

    private object? SetPropertyValue(
        object? owner,
        object? value,
        bool reset,
        string? undoText)
    {
        DesignerTransaction? transaction = null;
        try
        {
            object? oldValue = GetPropertyValue(owner);

            if (value is not null && value.Equals(oldValue))
            {
                return value;
            }

            ClearCachedValues();

            IDesignerHost? host = DesignerHost;

            transaction = host?.CreateTransaction(undoText ?? string.Format(SR.PropertyGridSetValue, PropertyDescriptor.Name));

            // Usually IComponent things are sited and this notification will be fired automatically by
            // the PropertyDescriptor. However, for non-IComponent sub objects or sub objects that are
            // non-sited sub components, we need to manually fire the notification.

            bool needChangeNotify = owner is not IComponent component || component.Site is null;

            if (needChangeNotify)
            {
                try
                {
                    if (owner is null)
                    {
                        throw new InvalidOperationException();
                    }

                    ComponentChangeService?.OnComponentChanging(owner, PropertyDescriptor);
                }
                catch (CheckoutException coEx)
                {
                    if (coEx == CheckoutException.Canceled)
                    {
                        return oldValue;
                    }

                    throw;
                }
            }

            bool wasExpanded = InternalExpanded;
            int childCount = -1;
            if (wasExpanded)
            {
                childCount = ChildCount;
            }

            // See if we need to refresh the property browser.
            var refresh = PropertyDescriptor.GetAttribute<RefreshPropertiesAttribute>();
            bool needsRefresh = wasExpanded || (refresh is not null && !refresh.RefreshProperties.Equals(RefreshProperties.None));

            if (needsRefresh)
            {
                DisposeChildren();
            }

            // Determine if this is an event being created, and if so, navigate to the event code

            EventDescriptor? eventDescriptor = null;

            // This is possibly an event. Check it out.
            if (owner is not null && value is string)
            {
                _eventBindings ??= this.GetService<IEventBindingService>();
                eventDescriptor = _eventBindings?.GetEvent(PropertyDescriptor);

                // For a merged set of properties, the event binding service won't
                // find an event. So, we ask the type descriptor directly.
                if (eventDescriptor is null)
                {
                    // If we have a merged property descriptor, pull out one of the elements.
                    object? eventObj = owner;

                    if (PropertyDescriptor is MergePropertyDescriptor && owner is Array objArray)
                    {
                        if (objArray.Length > 0)
                        {
                            eventObj = objArray.GetValue(0);
                        }
                    }

                    // TypeDescriptor will return an empty collection if component is null.
                    eventDescriptor = TypeDescriptor.GetEvents(eventObj!)[PropertyDescriptor.Name];
                }
            }

            bool setSuccessful = false;
            try
            {
                if (reset)
                {
                    if (owner is null)
                    {
                        throw new InvalidOperationException();
                    }

                    PropertyDescriptor.ResetValue(owner);
                }
                else if (eventDescriptor is not null)
                {
                    ViewEvent(owner, (string?)value, eventDescriptor, false);
                }
                else
                {
                    // Not an event
                    SetPropertyValueCore(owner, value);
                }

                setSuccessful = true;

                // Now notify the change service that the change was successful.
                if (needChangeNotify)
                {
                    // We have already checked if owner is null.
                    ComponentChangeService?.OnComponentChanged(owner!, PropertyDescriptor, oldValue: null, value);
                }

                NotifyParentsOfChanges(this);
            }
            finally
            {
                if (needsRefresh && OwnerGridView is not null)
                {
                    RecreateChildren(childCount);
                    if (setSuccessful)
                    {
                        OwnerGridView.Refresh(refresh is not null && refresh.Equals(RefreshPropertiesAttribute.All));
                    }
                }
            }
        }
        catch (CheckoutException checkoutEx)
        {
            transaction?.Cancel();
            transaction = null;

            if (checkoutEx == CheckoutException.Canceled)
            {
                return null;
            }

            throw;
        }
        catch
        {
            transaction?.Cancel();
            transaction = null;

            throw;
        }
        finally
        {
            transaction?.Commit();
        }

        return owner;
    }

    protected void SetPropertyValueCore(object? owner, object? newValue)
    {
        // Store the current cursor and set it to the HourGlass.
        Cursor? oldCursor = Cursor.Current;
        try
        {
            Cursor.Current = Cursors.WaitCursor;

            object? realOwner = owner;

            if (realOwner is ICustomTypeDescriptor descriptor)
            {
                realOwner = descriptor.GetPropertyOwner(PropertyDescriptor);
            }

            // Check the type of the object we are modifying. If it's a value type or an array,
            // we need to modify the object and push the value back up to the parent.

            bool treatAsValueType = false;

            if (ParentGridEntry is not null)
            {
                Type? propertyType = ParentGridEntry.PropertyType;
                if (propertyType is not null)
                {
                    treatAsValueType = propertyType.IsValueType || propertyType.IsArray;
                }
            }

            if (realOwner is not null)
            {
                PropertyDescriptor.SetValue(realOwner, newValue);

                // Since the value that we modified may not be stored by the parent property, we need to push this
                // value back into the parent. An example here is Size or Location, which return Point objects that
                // are unconnected to the object they relate to. So we modify the Point object and push it back
                // into the object we got it from.

                if (treatAsValueType)
                {
                    GridEntry? parent = ParentGridEntry;
                    if (parent is not null && parent.IsValueEditable)
                    {
                        parent.PropertyValue = owner;
                    }
                }
            }
        }
        finally
        {
            // Flip back to the old cursor.
            Cursor.Current = oldCursor;
        }
    }

    /// <summary>
    ///  Navigates code to the given event.
    /// </summary>
    protected bool ViewEvent(
        object? owner,
        string? newHandler,
        EventDescriptor? eventDescriptor,
        bool alwaysNavigate)
    {
        object? value = GetPropertyValue(owner);

        string? handler = value as string;

        if (handler is null && value is not null && TypeConverter is not null && TypeConverter.CanConvertTo(typeof(string)))
        {
            handler = TypeConverter.ConvertToString(value);
        }

        if (newHandler is null && !string.IsNullOrEmpty(handler))
        {
            newHandler = handler;
        }
        else if (handler == newHandler && !string.IsNullOrEmpty(newHandler))
        {
            return true;
        }

        var component = owner as IComponent;

        if (component is null && PropertyDescriptor is MergePropertyDescriptor)
        {
            // It's possible that multiple objects are selected, and we're trying to create an event for each of them.
            if (owner is Array array && array.Length > 0)
            {
                component = array.GetValue(0) as IComponent;
            }
        }

        if (component is null)
        {
            return false;
        }

        if (PropertyDescriptor.IsReadOnly)
        {
            return false;
        }

        if (eventDescriptor is null)
        {
            _eventBindings ??= this.GetService<IEventBindingService>();
            eventDescriptor = _eventBindings?.GetEvent(PropertyDescriptor);
        }

        IDesignerHost? host = DesignerHost;
        DesignerTransaction? transaction = null;

        try
        {
            // This check can cause exceptions if the event has unreferenced dependencies, which we want to catch.
            // This must be done before the transaction is started or the commit/cancel will also throw.
            if (eventDescriptor is null)
            {
                throw new InvalidOperationException();
            }

            if (eventDescriptor.EventType is null)
            {
                return false;
            }

            if (host is not null)
            {
                string componentName = component.Site?.Name ?? component.GetType().Name;
                transaction = host.CreateTransaction(string.Format(
                    SR.WindowsFormsSetEvent,
                    $"{componentName}.{PropertyName}"));
            }

            _eventBindings ??= component.Site?.GetService<IEventBindingService>();

            newHandler ??= _eventBindings?.CreateUniqueMethodName(component, eventDescriptor);

            if (newHandler is not null)
            {
                // Now walk through all the matching methods to see if this one exists.
                // If it doesn't we'll want to show code.
                if (_eventBindings is not null)
                {
                    bool methodExists = false;
                    foreach (string methodName in _eventBindings.GetCompatibleMethods(eventDescriptor))
                    {
                        if (newHandler == methodName)
                        {
                            methodExists = true;
                            break;
                        }
                    }

                    if (!methodExists)
                    {
                        alwaysNavigate = true;
                    }
                }

                try
                {
                    PropertyDescriptor.SetValue(owner, newHandler);
                }
                catch (InvalidOperationException ex)
                {
                    transaction?.Cancel();
                    transaction = null;

                    OwnerGridView?.ShowInvalidMessage(ex);

                    return false;
                }
            }

            if (alwaysNavigate && _eventBindings is not null)
            {
                s_targetBindingService = _eventBindings;
                s_targetComponent = component;
                s_targetEventdesc = eventDescriptor;
                Application.Idle += ShowCodeIdle;
            }
        }
        catch
        {
            transaction?.Cancel();
            transaction = null;
            throw;
        }
        finally
        {
            transaction?.Commit();
        }

        return true;
    }

    /// <summary>
    ///  Displays the user code for the given event. This will return true if the user
    ///  code could be displayed, or false otherwise.
    /// </summary>
    private static void ShowCodeIdle(object? sender, EventArgs e)
    {
        Application.Idle -= ShowCodeIdle;
        if (s_targetBindingService is not null)
        {
            // We set s_targetComponent and s_targetEventdesc with s_targetBindingService,
            // so we don't expect them to be null here.
            s_targetBindingService.ShowCode(s_targetComponent!, s_targetEventdesc!);
            s_targetBindingService = null;
            s_targetComponent = null;
            s_targetEventdesc = null;
        }
    }

    protected override GridEntryAccessibleObject GetAccessibilityObject() =>
        new PropertyDescriptorGridEntryAccessibleObject(this);
}
