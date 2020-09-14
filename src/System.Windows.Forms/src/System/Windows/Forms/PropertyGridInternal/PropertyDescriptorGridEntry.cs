// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal class PropertyDescriptorGridEntry : GridEntry
    {
        internal PropertyDescriptor _propertyInfo;

        private TypeConverter _exceptionConverter;
        private UITypeEditor _exceptionEditor;
        private bool _isSerializeContentsProp;
        private byte _parensAroundName = ParensAroundNameUnknown;
        private IPropertyValueUIService _pvSvc;
        protected IEventBindingService _eventBindings;
        private bool _pvSvcChecked;
        private PropertyValueUIItem[] _pvUIItems;
        private Rectangle[] _uiItemRects;
        private bool _readOnlyVerified;
        private bool _forceRenderReadOnly;
        private string _helpKeyword;
        private string _toolTipText;
        private readonly bool _activeXHide;
        private static int s_scaledImageSizeX = ImageSize;
        private static int s_scaledImageSizeY = ImageSize;
        private static bool s_isScalingInitialized;

        private const int ImageSize = 8;
        private const byte ParensAroundNameUnknown = 0xFF;
        private const byte ParensAroundNameNo = 0;
        private const byte ParensAroundNameYes = 1;

        private static IEventBindingService s_targetBindingService;
        private static IComponent s_targetComponent;
        private static EventDescriptor s_targetEventdesc;

        internal PropertyDescriptorGridEntry(PropertyGrid ownerGrid, GridEntry peParent, bool hide)
        : base(ownerGrid, peParent)
        {
            _activeXHide = hide;
        }

        internal PropertyDescriptorGridEntry(PropertyGrid ownerGrid, GridEntry peParent, PropertyDescriptor propInfo, bool hide)
        : base(ownerGrid, peParent)
        {
            _activeXHide = hide;
            Initialize(propInfo);
        }

        /// <summary>
        ///  specify that this grid entry should be allowed to be merged for.
        ///  multi-select.
        /// </summary>
        public override bool AllowMerge
        {
            get
            {
                MergablePropertyAttribute mpa = (MergablePropertyAttribute)_propertyInfo.Attributes[typeof(MergablePropertyAttribute)];
                return mpa is null || mpa.IsDefaultAttribute();
            }
        }

        internal override AttributeCollection Attributes
        {
            get
            {
                return _propertyInfo.Attributes;
            }
        }

        /// <summary>
        ///  Retrieves the keyword that the VS help dynamic help window will
        ///  use when this IPE is selected.
        /// </summary>
        public override string HelpKeyword
        {
            get
            {
                if (_helpKeyword is null)
                {
                    object owner = GetValueOwner();
                    if (owner is null)
                    {
                        return null; //null exception protection.
                    }

                    HelpKeywordAttribute helpAttribute = (HelpKeywordAttribute)_propertyInfo.Attributes[typeof(HelpKeywordAttribute)];

                    if (helpAttribute != null && !helpAttribute.IsDefaultAttribute())
                    {
                        return helpAttribute.HelpKeyword;
                    }
                    else if (this is ImmutablePropertyDescriptorGridEntry)
                    {
                        _helpKeyword = PropertyName;

                        GridEntry ge = this;

                        while (ge.ParentGridEntry != null)
                        {
                            ge = ge.ParentGridEntry;

                            // for value classes, the equality will never work, so
                            // just try the type equality
                            if (ge.PropertyValue == owner || (owner.GetType().IsValueType && owner.GetType() == ge.PropertyValue.GetType()))
                            {
                                _helpKeyword = ge.PropertyName + "." + _helpKeyword;
                                break;
                            }
                        }
                    }
                    else
                    {
                        string typeName = string.Empty;

                        Type componentType = _propertyInfo.ComponentType;

                        if (componentType.IsCOMObject)
                        {
                            typeName = TypeDescriptor.GetClassName(owner);
                        }
                        else
                        {
                            // make sure this property is declared on a class that
                            // is related to the component we're looking at.
                            // if it's not, it could be a shadow property so we need
                            // to try to find the real property.
                            //
                            Type ownerType = owner.GetType();
                            if (!componentType.IsPublic || !componentType.IsAssignableFrom(ownerType))
                            {
                                PropertyDescriptor componentProperty = TypeDescriptor.GetProperties(ownerType)[PropertyName];
                                if (componentProperty != null)
                                {
                                    componentType = componentProperty.ComponentType;
                                }
                                else
                                {
                                    componentType = null;
                                }
                            }

                            if (componentType is null)
                            {
                                typeName = TypeDescriptor.GetClassName(owner);
                            }
                            else
                            {
                                //

                                //if (helpAttribute != null && !helpAttribute.IsDefaultAttribute()) {
                                //    typeName = helpAttribute.HelpKeyword;
                                //}
                                //else {
                                typeName = componentType.FullName;
                                //}
                            }
                        }
                        _helpKeyword = typeName + "." + _propertyInfo.Name;
                    }
                }
                return _helpKeyword;
            }
        }

        internal override string LabelToolTipText
        {
            get
            {
                return (_toolTipText ?? base.LabelToolTipText);
            }
        }

        internal override string HelpKeywordInternal
        {
            get
            {
                return PropertyLabel;
            }
        }

        internal override bool Enumerable
        {
            get => base.Enumerable && !IsPropertyReadOnly;
        }

        internal virtual bool IsPropertyReadOnly
        {
            get
            {
                return _propertyInfo.IsReadOnly;
            }
        }

        public override bool IsValueEditable
        {
            get
            {
                return _exceptionConverter is null && !IsPropertyReadOnly && base.IsValueEditable;
            }
        }

        public override bool NeedsDropDownButton
        {
            get => base.NeedsDropDownButton && !IsPropertyReadOnly;
        }

        internal bool ParensAroundName
        {
            get
            {
                if (ParensAroundNameUnknown == _parensAroundName)
                {
                    if (((ParenthesizePropertyNameAttribute)_propertyInfo.Attributes[typeof(ParenthesizePropertyNameAttribute)]).NeedParenthesis)
                    {
                        _parensAroundName = ParensAroundNameYes;
                    }
                    else
                    {
                        _parensAroundName = ParensAroundNameNo;
                    }
                }
                return (_parensAroundName == ParensAroundNameYes);
            }
        }

        public override string PropertyCategory
        {
            get
            {
                string category = _propertyInfo.Category;
                if (category is null || category.Length == 0)
                {
                    category = base.PropertyCategory;
                }
                return category;
            }
        }

        /// <summary>
        ///  Retrieves the PropertyDescriptor that is surfacing the given object/
        /// </summary>
        public override PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return _propertyInfo;
            }
        }

        public override string PropertyDescription
        {
            get
            {
                return _propertyInfo.Description;
            }
        }

        public override string PropertyLabel
        {
            get
            {
                string label = _propertyInfo.DisplayName;
                if (ParensAroundName)
                {
                    label = "(" + label + ")";
                }
                return label;
            }
        }

        /// <summary>
        ///  Returns non-localized name of this property.
        /// </summary>
        public override string PropertyName
        {
            get
            {
                if (_propertyInfo != null)
                {
                    return _propertyInfo.Name;
                }
                return null;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return _propertyInfo.PropertyType;
            }
        }

        /// <summary>
        ///  Gets or sets the value for the property that is represented
        ///  by this GridEntry.
        /// </summary>
        public override object PropertyValue
        {
            get
            {
                try
                {
                    object objRet = GetPropertyValueCore(GetValueOwner());

                    if (_exceptionConverter != null)
                    {
                        // undo the exception converter
                        SetFlagsAndExceptionInfo(0, null, null);
                    }

                    return objRet;
                }
                catch (Exception e)
                {
                    if (_exceptionConverter is null)
                    {
                        // clear the flags
                        SetFlagsAndExceptionInfo(0, new ExceptionConverter(), new ExceptionEditor());
                    }
                    return e;
                }
            }
            set
            {
                SetPropertyValue(GetValueOwner(), value, false, null);
            }
        }

        private IPropertyValueUIService PropertyValueUIService
        {
            get
            {
                if (!_pvSvcChecked && _pvSvc is null)
                {
                    _pvSvc = (IPropertyValueUIService)GetService(typeof(IPropertyValueUIService));
                    _pvSvcChecked = true;
                }
                return _pvSvc;
            }
        }

        private void SetFlagsAndExceptionInfo(int flags, ExceptionConverter converter, ExceptionEditor editor)
        {
            Flags = flags;
            _exceptionConverter = converter;
            _exceptionEditor = editor;
        }

        public override bool ShouldRenderReadOnly
        {
            get
            {
                if (base.ForceReadOnly || _forceRenderReadOnly)
                {
                    return true;
                }

                // if read only editable is set, make sure it's valid
                //
                if (_propertyInfo.IsReadOnly && !_readOnlyVerified && GetFlagSet(GridEntry.FLAG_READONLY_EDITABLE))
                {
                    Type propType = PropertyType;

                    if (propType != null && (propType.IsArray || propType.IsValueType || propType.IsPrimitive))
                    {
                        SetFlag(FLAG_READONLY_EDITABLE, false);
                        SetFlag(FLAG_RENDER_READONLY, true);
                        _forceRenderReadOnly = true;
                    }
                }
                _readOnlyVerified = true;

                if (base.ShouldRenderReadOnly)
                {
                    if (!_isSerializeContentsProp && !base.NeedsCustomEditorButton)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        ///  Returns the type converter for this entry.
        /// </summary>
        internal override TypeConverter TypeConverter
        {
            get
            {
                if (_exceptionConverter != null)
                {
                    return _exceptionConverter;
                }

                if (converter is null)
                {
                    converter = _propertyInfo.Converter;
                }
                return base.TypeConverter;
            }
        }

        /// <summary>
        ///  Returns the type editor for this entry.  This may return null if there
        ///  is no type editor.
        /// </summary>
        internal override UITypeEditor UITypeEditor
        {
            get
            {
                if (_exceptionEditor != null)
                {
                    return _exceptionEditor;
                }

                editor = (UITypeEditor)_propertyInfo.GetEditor(typeof(UITypeEditor));

                return base.UITypeEditor;
            }
        }

        /// <summary>
        ///  Invokes the type editor for editing this item.
        /// </summary>
        internal override void EditPropertyValue(PropertyGridView iva)
        {
            base.EditPropertyValue(iva);

            if (!IsValueEditable)
            {
                RefreshPropertiesAttribute refreshAttr = (RefreshPropertiesAttribute)_propertyInfo.Attributes[typeof(RefreshPropertiesAttribute)];
                if ((refreshAttr != null && !refreshAttr.RefreshProperties.Equals(RefreshProperties.None)))
                {
                    GridEntryHost.Refresh(refreshAttr != null && refreshAttr.Equals(RefreshPropertiesAttribute.All));
                }
            }
        }

        internal override Point GetLabelToolTipLocation(int mouseX, int mouseY)
        {
            if (_pvUIItems != null)
            {
                for (int i = 0; i < _pvUIItems.Length; i++)
                {
                    if (_uiItemRects[i].Contains(mouseX, GridEntryHost.GetGridEntryHeight() / 2))
                    {
                        _toolTipText = _pvUIItems[i].ToolTip;
                        return new Point(mouseX, mouseY);
                    }
                }
            }
            _toolTipText = null;
            return base.GetLabelToolTipLocation(mouseX, mouseY);
        }

        protected object GetPropertyValueCore(object target)
        {
            if (_propertyInfo is null)
            {
                return null;
            }

            if (target is ICustomTypeDescriptor)
            {
                target = ((ICustomTypeDescriptor)target).GetPropertyOwner(_propertyInfo);
            }
            try
            {
                return _propertyInfo.GetValue(target);
            }
            catch
            {
                throw;
            }
        }

        protected void Initialize(PropertyDescriptor propInfo)
        {
            _propertyInfo = propInfo;

            _isSerializeContentsProp = (_propertyInfo.SerializationVisibility == DesignerSerializationVisibility.Content);

            Debug.Assert(propInfo != null, "Can't create propEntry because of null prop info");

            if (!_activeXHide && IsPropertyReadOnly)
            {
                SetFlag(FLAG_TEXT_EDITABLE, false);
            }

            if (_isSerializeContentsProp && TypeConverter.GetPropertiesSupported())
            {
                SetFlag(FL_EXPANDABLE, true);
            }
        }

        protected virtual void NotifyParentChange(GridEntry ge)
        {
            // now see if we need to notify the parent(s) up the chain
            while (ge != null &&
                   ge is PropertyDescriptorGridEntry &&
                   ((PropertyDescriptorGridEntry)ge)._propertyInfo.Attributes.Contains(NotifyParentPropertyAttribute.Yes))
            {
                // find the next parent property with a differnet value owner
                object owner = ge.GetValueOwner();

                // when owner is an instance of a value type,
                // we can't just use == in the following while condition testing
                bool isValueType = owner.GetType().IsValueType;

                // find the next property descriptor with a different parent
                while (!(ge is PropertyDescriptorGridEntry)
                    || isValueType ? owner.Equals(ge.GetValueOwner()) : owner == ge.GetValueOwner())
                {
                    ge = ge.ParentGridEntry;
                    if (ge is null)
                    {
                        break;
                    }
                }

                // fire the change on that owner
                if (ge != null)
                {
                    owner = ge.GetValueOwner();

                    IComponentChangeService changeService = ComponentChangeService;

                    if (changeService != null)
                    {
                        changeService.OnComponentChanging(owner, ((PropertyDescriptorGridEntry)ge)._propertyInfo);
                        changeService.OnComponentChanged(owner, ((PropertyDescriptorGridEntry)ge)._propertyInfo, null, null);
                    }

                    ge.ClearCachedValues(false); //clear the value so it paints correctly next time.
                    PropertyGridView gv = GridEntryHost;
                    if (gv != null)
                    {
                        gv.InvalidateGridEntryValue(ge);
                    }
                }
            }
        }

        internal override bool NotifyValueGivenParent(object obj, int type)
        {
            if (obj is ICustomTypeDescriptor)
            {
                obj = ((ICustomTypeDescriptor)obj).GetPropertyOwner(_propertyInfo);
            }

            switch (type)
            {
                case NOTIFY_RESET:

                    SetPropertyValue(obj, null, true, string.Format(SR.PropertyGridResetValue, PropertyName));
                    if (_pvUIItems != null)
                    {
                        for (int i = 0; i < _pvUIItems.Length; i++)
                        {
                            _pvUIItems[i].Reset();
                        }
                    }
                    _pvUIItems = null;
                    return false;
                case NOTIFY_CAN_RESET:
                    try
                    {
                        return _propertyInfo.CanResetValue(obj) || (_pvUIItems != null && _pvUIItems.Length > 0);
                    }
                    catch
                    {
                        if (_exceptionConverter is null)
                        {
                            // clear the flags
                            Flags = 0;
                            _exceptionConverter = new ExceptionConverter();
                            _exceptionEditor = new ExceptionEditor();
                        }
                        return false;
                    }
                case NOTIFY_SHOULD_PERSIST:
                    try
                    {
                        return _propertyInfo.ShouldSerializeValue(obj);
                    }
                    catch
                    {
                        if (_exceptionConverter is null)
                        {
                            // clear the flags
                            Flags = 0;
                            _exceptionConverter = new ExceptionConverter();
                            _exceptionEditor = new ExceptionEditor();
                        }
                        return false;
                    }

                case NOTIFY_DBL_CLICK:
                case NOTIFY_RETURN:
                    if (_eventBindings is null)
                    {
                        _eventBindings = (IEventBindingService)GetService(typeof(IEventBindingService));
                    }
                    if (_eventBindings != null)
                    {
                        EventDescriptor descriptor = _eventBindings.GetEvent(_propertyInfo);
                        if (descriptor != null)
                        {
                            return ViewEvent(obj, null, null, true);
                        }
                    }
                    break;
            }
            return false;
        }

        public override void OnComponentChanged()
        {
            base.OnComponentChanged();
            // If we got this it means someone called ITypeDescriptorContext.OnCompnentChanged.
            // so we need to echo that change up the inheritance change in case the owner object isn't a sited component.
            NotifyParentChange(this);
        }

        public override bool OnMouseClick(int x, int y, int count, MouseButtons button)
        {
            if (_pvUIItems != null && count == 2 && ((button & MouseButtons.Left) == MouseButtons.Left))
            {
                for (int i = 0; i < _pvUIItems.Length; i++)
                {
                    if (_uiItemRects[i].Contains(x, GridEntryHost.GetGridEntryHeight() / 2))
                    {
                        _pvUIItems[i].InvokeHandler(this, _propertyInfo, _pvUIItems[i]);
                        return true;
                    }
                }
            }
            return base.OnMouseClick(x, y, count, button);
        }

        public override void PaintLabel(Graphics g, Rectangle rect, Rectangle clipRect, bool selected, bool paintFullLabel)
        {
            base.PaintLabel(g, rect, clipRect, selected, paintFullLabel);

            IPropertyValueUIService propValSvc = PropertyValueUIService;

            if (propValSvc is null)
            {
                return;
            }

            _pvUIItems = propValSvc.GetPropertyUIValueItems(this, _propertyInfo);

            if (_pvUIItems != null)
            {
                if (_uiItemRects is null || _uiItemRects.Length != _pvUIItems.Length)
                {
                    _uiItemRects = new Rectangle[_pvUIItems.Length];
                }

                if (!s_isScalingInitialized)
                {
                    if (DpiHelper.IsScalingRequired)
                    {
                        s_scaledImageSizeX = DpiHelper.LogicalToDeviceUnitsX(ImageSize);
                        s_scaledImageSizeY = DpiHelper.LogicalToDeviceUnitsY(ImageSize);
                    }
                    s_isScalingInitialized = true;
                }

                for (int i = 0; i < _pvUIItems.Length; i++)
                {
                    _uiItemRects[i] = new Rectangle(rect.Right - ((s_scaledImageSizeX + 1) * (i + 1)), (rect.Height - s_scaledImageSizeY) / 2, s_scaledImageSizeX, s_scaledImageSizeY);
                    g.DrawImage(_pvUIItems[i].Image, _uiItemRects[i]);
                }
                GridEntryHost.LabelPaintMargin = (s_scaledImageSizeX + 1) * _pvUIItems.Length;
            }
        }

        private object SetPropertyValue(object obj, object objVal, bool reset, string undoText)
        {
            DesignerTransaction trans = null;
            try
            {
                object oldValue = GetPropertyValueCore(obj);

                if (objVal != null && objVal.Equals(oldValue))
                {
                    return objVal;
                }

                ClearCachedValues();

                IDesignerHost host = DesignerHost;

                if (host != null)
                {
                    string text = (undoText ?? string.Format(SR.PropertyGridSetValue, _propertyInfo.Name));
                    trans = host.CreateTransaction(text);
                }

                // Usually IComponent things are sited and this notification will be
                // fired automatically by the PropertyDescriptor.  However, for non-IComponent sub objects
                // or sub objects that are non-sited sub components, we need to manuall fire
                // the notification.
                //
                bool needChangeNotify = !(obj is IComponent) || ((IComponent)obj).Site is null;

                if (needChangeNotify)
                {
                    try
                    {
                        if (ComponentChangeService != null)
                        {
                            ComponentChangeService.OnComponentChanging(obj, _propertyInfo);
                        }
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

                RefreshPropertiesAttribute refreshAttr = (RefreshPropertiesAttribute)_propertyInfo.Attributes[typeof(RefreshPropertiesAttribute)];
                bool needsRefresh = wasExpanded || (refreshAttr != null && !refreshAttr.RefreshProperties.Equals(RefreshProperties.None));

                if (needsRefresh)
                {
                    DisposeChildren();
                }

                // Determine if this is an event being created, and if so, navigate to the event code
                //

                EventDescriptor eventDesc = null;

                // This is possibly an event.  Check it out.
                //
                if (obj != null && objVal is string)
                {
                    if (_eventBindings is null)
                    {
                        _eventBindings = (IEventBindingService)GetService(typeof(IEventBindingService));
                    }
                    if (_eventBindings != null)
                    {
                        eventDesc = _eventBindings.GetEvent(_propertyInfo);
                    }

                    // For a merged set of propertius, the event binding service won't
                    // find an event.  So, we ask type descriptor directly.
                    //
                    if (eventDesc is null)
                    {
                        // If we have a merged property descriptor, pull out one of
                        // the elements.
                        //
                        object eventObj = obj;

                        if (_propertyInfo is MergePropertyDescriptor && obj is Array)
                        {
                            Array objArray = obj as Array;
                            if (objArray.Length > 0)
                            {
                                eventObj = objArray.GetValue(0);
                            }
                        }
                        eventDesc = TypeDescriptor.GetEvents(eventObj)[_propertyInfo.Name];
                    }
                }

                bool setSuccessful = false;
                try
                {
                    if (reset)
                    {
                        _propertyInfo.ResetValue(obj);
                    }
                    else if (eventDesc != null)
                    {
                        ViewEvent(obj, (string)objVal, eventDesc, false);
                    }
                    else
                    { // Not an event
                        SetPropertyValueCore(obj, objVal, true);
                    }

                    setSuccessful = true;

                    // Now notify the change service that the change was successful.
                    //
                    if (needChangeNotify && ComponentChangeService != null)
                    {
                        ComponentChangeService.OnComponentChanged(obj, _propertyInfo, null, objVal);
                    }

                    NotifyParentChange(this);
                }
                finally
                {
                    // see if we need to refresh the property browser
                    // 1) if this property has the refreshpropertiesattribute, or
                    // 2) it's got expanded sub properties
                    //
                    if (needsRefresh && GridEntryHost != null)
                    {
                        RecreateChildren(childCount);
                        if (setSuccessful)
                        {
                            GridEntryHost.Refresh(refreshAttr != null && refreshAttr.Equals(RefreshPropertiesAttribute.All));
                        }
                    }
                }
            }
            catch (CheckoutException checkoutEx)
            {
                if (trans != null)
                {
                    trans.Cancel();
                    trans = null;
                }

                if (checkoutEx == CheckoutException.Canceled)
                {
                    return null;
                }
                throw;
            }
            catch
            {
                if (trans != null)
                {
                    trans.Cancel();
                    trans = null;
                }

                throw;
            }
            finally
            {
                if (trans != null)
                {
                    trans.Commit();
                }
            }
            return obj;
        }

        protected void SetPropertyValueCore(object obj, object value, bool doUndo)
        {
            if (_propertyInfo is null)
            {
                return;
            }

            // Store the current cursor and set it to the HourGlass.
            //
            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                object target = obj;

                if (target is ICustomTypeDescriptor)
                {
                    target = ((ICustomTypeDescriptor)target).GetPropertyOwner(_propertyInfo);
                }

                // check the type of the object we are modifying.  If it's a value type or an array,
                // we need to modify the object and push the value back up to the parent.
                //
                bool treatAsValueType = false;

                if (ParentGridEntry != null)
                {
                    Type propType = ParentGridEntry.PropertyType;
                    treatAsValueType = propType.IsValueType || propType.IsArray;
                }

                if (target != null)
                {
                    _propertyInfo.SetValue(target, value);

                    // Microsoft, okay, since the value that we modified may not
                    // be stored by the parent property, we need to push this
                    // value back into the parent.  An example here is Size or
                    // Location, which return Point objects that are unconnected
                    // to the object they relate to.  So we modify the Point object and
                    // push it back into the object we got it from.
                    //
                    if (treatAsValueType)
                    {
                        GridEntry parent = ParentGridEntry;
                        if (parent != null && parent.IsValueEditable)
                        {
                            parent.PropertyValue = obj;
                        }
                    }
                }
            }
            finally
            {
                // Flip back to the old cursor.
                //
                Cursor.Current = oldCursor;
            }
        }

        /// <summary>
        ///  Navigates code to the given event.
        /// </summary>
        protected bool ViewEvent(object obj, string newHandler, EventDescriptor eventdesc, bool alwaysNavigate)
        {
            object value = GetPropertyValueCore(obj);

            string handler = value as string;

            if (handler is null && value != null && TypeConverter != null && TypeConverter.CanConvertTo(typeof(string)))
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

            IComponent component = obj as IComponent;

            if (component is null && _propertyInfo is MergePropertyDescriptor)
            {
                // It's possible that multiple objects are selected, and we're trying to create an event for each of them
                //
                if (obj is Array objArray && objArray.Length > 0)
                {
                    component = objArray.GetValue(0) as IComponent;
                }
            }

            if (component is null)
            {
                return false;
            }

            if (_propertyInfo.IsReadOnly)
            {
                return false;
            }

            if (eventdesc is null)
            {
                if (_eventBindings is null)
                {
                    _eventBindings = (IEventBindingService)GetService(typeof(IEventBindingService));
                }
                if (_eventBindings != null)
                {
                    eventdesc = _eventBindings.GetEvent(_propertyInfo);
                }
            }

            IDesignerHost host = DesignerHost;
            DesignerTransaction trans = null;

            try
            {
                // This check can cause exceptions if the event has unreferenced dependencies, which we want to cath.
                // This must be done before the transaction is started or the commit/cancel will also throw.
                if (eventdesc.EventType is null)
                {
                    return false;
                }

                if (host != null)
                {
                    string compName = component.Site != null ? component.Site.Name : component.GetType().Name;
                    trans = DesignerHost.CreateTransaction(string.Format(SR.WindowsFormsSetEvent, compName + "." + PropertyName));
                }

                if (_eventBindings is null)
                {
                    ISite site = component.Site;
                    if (site != null)
                    {
                        _eventBindings = (IEventBindingService)site.GetService(typeof(IEventBindingService));
                    }
                }

                if (newHandler is null && _eventBindings != null)
                {
                    newHandler = _eventBindings.CreateUniqueMethodName(component, eventdesc);
                }

                if (newHandler != null)
                {
                    // now walk through all the matching methods to see if this one exists.
                    // if it doesn't we'll wanna show code.
                    //
                    if (_eventBindings != null)
                    {
                        bool methodExists = false;
                        foreach (string methodName in _eventBindings.GetCompatibleMethods(eventdesc))
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
                        _propertyInfo.SetValue(obj, newHandler);
                    }
                    catch (InvalidOperationException ex)
                    {
                        if (trans != null)
                        {
                            trans.Cancel();
                            trans = null;
                        }

                        if (GridEntryHost != null && GridEntryHost is PropertyGridView)
                        {
                            PropertyGridView pgv = GridEntryHost as PropertyGridView;
                            pgv.ShowInvalidMessage(newHandler, obj, ex);
                        }

                        return false;
                    }
                }

                if (alwaysNavigate && _eventBindings != null)
                {
                    s_targetBindingService = _eventBindings;
                    s_targetComponent = component;
                    s_targetEventdesc = eventdesc;
                    Application.Idle += new EventHandler(PropertyDescriptorGridEntry.ShowCodeIdle);
                }
            }
            catch
            {
                if (trans != null)
                {
                    trans.Cancel();
                    trans = null;
                }
                throw;
            }
            finally
            {
                if (trans != null)
                {
                    trans.Commit();
                }
            }
            return true;
        }

        /// <summary>
        ///  Displays the user code for the given event.  This will return true if the user
        ///  code could be displayed, or false otherwise.
        /// </summary>
        static private void ShowCodeIdle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(PropertyDescriptorGridEntry.ShowCodeIdle);
            if (s_targetBindingService != null)
            {
                s_targetBindingService.ShowCode(s_targetComponent, s_targetEventdesc);
                s_targetBindingService = null;
                s_targetComponent = null;
                s_targetEventdesc = null;
            }
        }

        /// <summary>
        ///  Creates a new AccessibleObject for this PropertyDescriptorGridEntry instance.
        ///  The AccessibleObject instance returned by this method supports IsEnabled UIA property.
        /// </summary>
        /// <returns>
        ///  AccessibleObject for this PropertyDescriptorGridEntry instance.
        /// </returns>
        protected override GridEntryAccessibleObject GetAccessibilityObject()
        {
            return new PropertyDescriptorGridEntryAccessibleObject(this);
        }

        protected class PropertyDescriptorGridEntryAccessibleObject : GridEntryAccessibleObject
        {
            private readonly PropertyDescriptorGridEntry _owningPropertyDescriptorGridEntry;

            public PropertyDescriptorGridEntryAccessibleObject(PropertyDescriptorGridEntry owner) : base(owner)
            {
                _owningPropertyDescriptorGridEntry = owner;
            }

            internal override bool IsIAccessibleExSupported() => true;

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.NextSibling:
                        var propertyGridViewAccessibleObject = (PropertyGridView.PropertyGridViewAccessibleObject)Parent;
                        var propertyGridView = propertyGridViewAccessibleObject.Owner as PropertyGridView;
                        bool currentGridEntryFound = false;
                        return propertyGridViewAccessibleObject.GetNextGridEntry(_owningPropertyDescriptorGridEntry, propertyGridView.TopLevelGridEntries, out currentGridEntryFound);
                    case UiaCore.NavigateDirection.PreviousSibling:
                        propertyGridViewAccessibleObject = (PropertyGridView.PropertyGridViewAccessibleObject)Parent;
                        propertyGridView = propertyGridViewAccessibleObject.Owner as PropertyGridView;
                        currentGridEntryFound = false;
                        return propertyGridViewAccessibleObject.GetPreviousGridEntry(_owningPropertyDescriptorGridEntry, propertyGridView.TopLevelGridEntries, out currentGridEntryFound);
                    case UiaCore.NavigateDirection.FirstChild:
                        return GetFirstChild();
                    case UiaCore.NavigateDirection.LastChild:
                        return GetLastChild();
                }

                return base.FragmentNavigate(direction);
            }

            private UiaCore.IRawElementProviderFragment GetFirstChild()
            {
                if (_owningPropertyDescriptorGridEntry is null)
                {
                    return null;
                }

                if (_owningPropertyDescriptorGridEntry.ChildCount > 0)
                {
                    return _owningPropertyDescriptorGridEntry.Children.GetEntry(0).AccessibilityObject;
                }

                var propertyGridView = GetPropertyGridView();
                if (propertyGridView is null)
                {
                    return null;
                }

                var selectedGridEntry = propertyGridView.SelectedGridEntry;
                if (_owningPropertyDescriptorGridEntry == selectedGridEntry)
                {
                    if (selectedGridEntry.Enumerable &&
                        propertyGridView.DropDownVisible &&
                        propertyGridView.DropDownControlHolder?.Component == propertyGridView.DropDownListBox)
                    {
                        return propertyGridView.DropDownListBoxAccessibleObject;
                    }

                    if (propertyGridView.DropDownVisible && propertyGridView.DropDownControlHolder != null)
                    {
                        return propertyGridView.DropDownControlHolder.AccessibilityObject;
                    }

                    return propertyGridView.EditAccessibleObject;
                }

                return null;
            }

            private UiaCore.IRawElementProviderFragment GetLastChild()
            {
                if (_owningPropertyDescriptorGridEntry is null)
                {
                    return null;
                }

                if (_owningPropertyDescriptorGridEntry.ChildCount > 0)
                {
                    return _owningPropertyDescriptorGridEntry.Children
                        .GetEntry(_owningPropertyDescriptorGridEntry.ChildCount - 1).AccessibilityObject;
                }

                var propertyGridView = GetPropertyGridView();
                if (propertyGridView is null)
                {
                    return null;
                }

                var selectedGridEntry = propertyGridView.SelectedGridEntry;
                if (_owningPropertyDescriptorGridEntry == selectedGridEntry)
                {
                    if (selectedGridEntry.Enumerable && propertyGridView.DropDownButton.Visible)
                    {
                        return propertyGridView.DropDownButton.AccessibilityObject;
                    }

                    return propertyGridView.EditAccessibleObject;
                }

                return null;
            }

            private PropertyGridView GetPropertyGridView()
            {
                var propertyGridViewAccessibleObject = Parent as PropertyGridView.PropertyGridViewAccessibleObject;
                if (propertyGridViewAccessibleObject is null)
                {
                    return null;
                }

                return propertyGridViewAccessibleObject.Owner as PropertyGridView;
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ValuePatternId ||
                    (patternId == UiaCore.UIA.ExpandCollapsePatternId && owner.Enumerable))
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void Expand()
            {
                if (ExpandCollapseState == UiaCore.ExpandCollapseState.Collapsed)
                {
                    ExpandOrCollapse();
                }
            }

            internal override void Collapse()
            {
                if (ExpandCollapseState == UiaCore.ExpandCollapseState.Expanded)
                {
                    ExpandOrCollapse();
                }
            }

            private void ExpandOrCollapse()
            {
                if (!GetPropertyGridView().IsHandleCreated)
                {
                    return;
                }

                var propertyGridView = GetPropertyGridView();
                if (propertyGridView is null)
                {
                    return;
                }

                int row = propertyGridView.GetRowFromGridEntry(_owningPropertyDescriptorGridEntry);

                if (row != -1)
                {
                    propertyGridView.PopupDialog(row);
                }
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    var propertyGridView = GetPropertyGridView();
                    if (propertyGridView is null)
                    {
                        return UiaCore.ExpandCollapseState.Collapsed;
                    }

                    if (_owningPropertyDescriptorGridEntry == propertyGridView.SelectedGridEntry &&
                        ((_owningPropertyDescriptorGridEntry != null && _owningPropertyDescriptorGridEntry.InternalExpanded)
                         || propertyGridView.DropDownVisible))
                    {
                        return UiaCore.ExpandCollapseState.Expanded;
                    }

                    return UiaCore.ExpandCollapseState.Collapsed;
                }
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.IsEnabledPropertyId)
                {
                    return !((PropertyDescriptorGridEntry)owner).IsPropertyReadOnly;
                }
                else if (propertyID == UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId)
                {
                    return string.Empty;
                }
                else if (propertyID == UiaCore.UIA.IsValuePatternAvailablePropertyId)
                {
                    return true;
                }

                return base.GetPropertyValue(propertyID);
            }
        }

        /// <summary>
        ///  The exception converter is a type converter that displays an exception to the user.
        /// </summary>
        private class ExceptionConverter : TypeConverter
        {
            /// <summary>
            ///  Converts the given object to another type.  The most common types to convert
            ///  are to and from a string object.  The default implementation will make a call
            ///  to ToString on the object if the object is valid and if the destination
            ///  type is string.  If this cannot convert to the desitnation type, this will
            ///  throw a NotSupportedException.
            /// </summary>
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    if (value is Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            ex = ex.InnerException;
                        }
                        return ex.Message;
                    }
                    return null;
                }
                throw GetConvertToException(value, destinationType);
            }
        }

        /// <summary>
        ///  The exception editor displays a message to the user.
        /// </summary>
        private class ExceptionEditor : UITypeEditor
        {
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                if (value is Exception except)
                {
                    IUIService uis = null;

                    if (context != null)
                    {
                        uis = (IUIService)context.GetService(typeof(IUIService));
                    }

                    if (uis != null)
                    {
                        uis.ShowError(except);
                    }
                    else
                    {
                        string message = except.Message;
                        if (message is null || message.Length == 0)
                        {
                            message = except.ToString();
                        }

                        RTLAwareMessageBox.Show(null, message, SR.PropertyGridExceptionInfo,
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
                    }
                }
                return value;
            }

            /// <summary>
            ///  Retrieves the editing style of the Edit method.  If the method
            ///  is not supported, this will return None.
            /// </summary>
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
        }
    }
}
