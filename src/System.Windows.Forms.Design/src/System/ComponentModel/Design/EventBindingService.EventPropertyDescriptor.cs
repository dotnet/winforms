// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Globalization;

namespace System.ComponentModel.Design;

public abstract partial class EventBindingService
{
    /// <summary>
    ///  This is an EventDescriptor cleverly wrapped in a PropertyDescriptor
    ///  of type String. Note that we now handle subobjects by storing their
    ///  event information in their base component's site's dictionary.
    ///  Note also that when a value is set for this property we will code-gen
    ///  the event method. If the property is set to a new value we will
    ///  remove the old event method ONLY if it is empty.
    /// </summary>
    private class EventPropertyDescriptor : PropertyDescriptor
    {
        private readonly EventBindingService _eventService;
        private TypeConverter? _converter;

        /// <summary>
        ///  Creates a new EventPropertyDescriptor.
        /// </summary>
        internal EventPropertyDescriptor(EventDescriptor eventDesc, EventBindingService eventSvc) : base(eventDesc, null)
        {
            Event = eventDesc;
            _eventService = eventSvc;
        }

        /// <summary>
        ///  Indicates whether reset will change the value of the component. If there
        ///  is a DefaultValueAttribute, then this will return true if getValue returns
        ///  something different than the default value. If there is a reset method and
        ///  a shouldPersist method, this will return what shouldPersist returns.
        ///  If there is just a reset method, this always returns true. If none of these
        ///  cases apply, this returns false.
        /// </summary>
        public override bool CanResetValue(object component) => GetValue(component) is not null;

        /// <summary>
        ///  Retrieves the type of the component this PropertyDescriptor is bound to.
        /// </summary>
        public override Type ComponentType => Event.ComponentType;

        /// <summary>
        ///  Retrieves the type converter for this property.
        /// </summary>
        public override TypeConverter Converter
        {
            [RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
            get => _converter ??= new EventConverter(Event);
        }

        /// <summary>
        ///  Retrieves the event descriptor we are representing.
        /// </summary>
        internal EventDescriptor Event { get; }

        /// <summary>
        ///  Indicates whether this property is read only.
        /// </summary>
        public override bool IsReadOnly => Attributes[typeof(ReadOnlyAttribute)]!.Equals(ReadOnlyAttribute.Yes);

        /// <summary>
        ///  Retrieves the type of the property.
        /// </summary>
        public override Type PropertyType => Event.EventType;

        /// <summary>
        ///  Retrieves the current value of the property on component,
        ///  invoking the getXXX method. An exception in the getXXX
        ///  method will pass through.
        /// </summary>
        public override object? GetValue(object? component)
        {
            ArgumentNullException.ThrowIfNull(component);

            // We must locate the sited component, because we store data on the dictionary
            // service for the component.
            ISite? site = (component as IComponent)?.Site;

            if (site is null)
            {
                IReferenceService? rs = _eventService._provider.GetService<IReferenceService>();
                IComponent? baseComponent = rs?.GetComponent(component);

                if (baseComponent is not null)
                {
                    site = baseComponent.Site;
                }
            }

            if (site is null)
            {
                // Object not sited, so we weren't able to set a value on it. Setting a value will fail.
                return null;
            }

            IDictionaryService? ds = site.GetService<IDictionaryService>();

            if (ds is null)
            {
                // No dictionary service, so we weren't able to set a value on it. Setting a value will fail.
                return null;
            }

            return (string?)ds.GetValue(new ReferenceEventClosure(component, this));
        }

        /// <summary>
        ///  Will reset the default value for this property on the component. If
        ///  there was a default value passed in as a DefaultValueAttribute, that
        ///  value will be set as the value of the property on the component. If
        ///  there was no default value passed in, a ResetXXX method will be looked
        ///  for. If one is found, it will be invoked. If one is not found, this
        ///  is a nop.
        /// </summary>
        public override void ResetValue(object component) => SetValue(component, null);

        /// <summary>
        ///  This will set value to be the new value of this property on the
        ///  component by invoking the setXXX method on the component. If the
        ///  value specified is invalid, the component should throw an exception
        ///  which will be passed up. The component designer should design the
        ///  property so that getXXX following a setXXX should return the value
        ///  passed in if no exception was thrown in the setXXX call.
        /// </summary>
        public override void SetValue(object? component, object? value)
        {
            // Argument, state checking. Is it ok to set this event?
            if (IsReadOnly)
            {
                throw new InvalidOperationException(string.Format(SR.EventBindingServiceEventReadOnly, Name))
                {
                    HelpLink = SR.EventBindingServiceEventReadOnly
                };
            }

            string? name = value as string;

            if (value is not null && name is null)
            {
                throw new ArgumentException(string.Format(SR.EventBindingServiceBadArgType, Name, nameof(String)))
                {
                    HelpLink = SR.EventBindingServiceBadArgType
                };
            }

            if (name is not null && name.Length == 0)
            {
                name = null;
            }

            // Obtain the site for the component. Note that this can be a site
            // to a parent component if we can get to the reference service.
            ISite? site = (component as IComponent)?.Site;

            if (site is null && _eventService._provider.TryGetService(out IReferenceService? referenceService))
            {
                IComponent? baseComponent = referenceService.GetComponent(component!);

                if (baseComponent is not null)
                {
                    site = baseComponent.Site;
                }
            }

            if (site is null)
            {
                throw new InvalidOperationException(SR.EventBindingServiceNoSite)
                {
                    HelpLink = SR.EventBindingServiceNoSite
                };
            }

            // The dictionary service is where we store the actual event method name.
            if (!site.TryGetService(out IDictionaryService? dictionaryService))
            {
                throw new InvalidOperationException(string.Format(SR.EventBindingServiceMissingService, nameof(IDictionaryService)))
                {
                    HelpLink = SR.EventBindingServiceMissingService
                };
            }

            // Get the old method name, ensure that they are different, and then continue.
            ReferenceEventClosure key = new(component!, this);
            string? oldName = (string?)dictionaryService.GetValue(key);

            if (ReferenceEquals(oldName, name))
            {
                return;
            }

            if (oldName is not null && name is not null && oldName.Equals(name))
            {
                return;
            }

            // Before we continue our work, ensure that the name is actually valid.
            if (name is not null)
            {
                _eventService.ValidateMethodName(name);
            }

            // If there is a designer host, create a transaction so there is a
            // nice name for this change. We don't want a name like
            // "Change property 'Click', because to users, this isn't a property.
            DesignerTransaction? transaction = null;

            if (site.TryGetService(out IDesignerHost? host))
            {
                transaction = host.CreateTransaction(string.Format(SR.EventBindingServiceSetValue, site.Name, name));
            }

            try
            {
                // The names are different. Fire a changing event to make
                // sure it's OK to perform the change.

                if (site.TryGetService(out IComponentChangeService? changeService))
                {
                    try
                    {
                        changeService.OnComponentChanging(component!, this);
                        changeService.OnComponentChanging(component!, Event);
                    }
                    catch (CheckoutException coEx) when (coEx == CheckoutException.Canceled)
                    {
                        return;
                    }
                }

                // Less chance of success of adding a new method name, so
                // don't release the old name until we verify that adding
                // the new one actually succeeded.
                if (name is not null)
                {
                    _eventService.UseMethod((IComponent)component!, Event, name);
                }

                if (oldName is not null)
                {
                    _eventService.FreeMethod((IComponent)component!, Event, oldName);
                }

                dictionaryService.SetValue(key, name);

                if (changeService is not null)
                {
                    changeService.OnComponentChanged(component!, Event);
                    changeService.OnComponentChanged(component!, this, oldName, name);
                }

                OnValueChanged(component, EventArgs.Empty);

                transaction?.Commit();
            }
            finally
            {
                ((IDisposable?)transaction)?.Dispose();
            }
        }

        /// <summary>
        ///  Indicates whether the value of this property needs to be persisted. In
        ///  other words, it indicates whether the state of the property is distinct
        ///  from when the component is first instantiated. If there is a default
        ///  value specified in this PropertyDescriptor, it will be compared against the
        ///  property's current value to determine this. If there isn't, the
        ///  shouldPersistXXX method is looked for and invoked if found. If both
        ///  these routes fail, true will be returned.
        ///  If this returns false, a tool should not persist this property's value.
        /// </summary>
        public override bool ShouldSerializeValue(object component) => CanResetValue(component);

        /// <summary>
        ///  Implements a type converter for event objects.
        /// </summary>
        private class EventConverter : TypeConverter
        {
            private readonly EventDescriptor _evt;

            /// <summary>
            ///  Creates a new EventConverter.
            /// </summary>
            internal EventConverter(EventDescriptor evt)
            {
                _evt = evt;
            }

            /// <summary>
            ///  Determines if this converter can convert an object in the given source
            ///  type to the native type of the converter.
            /// </summary>
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }

                return base.CanConvertFrom(context, sourceType);
            }

            /// <summary>
            ///  Determines if this converter can convert an object to the given destination type.
            /// </summary>
            public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return true;
                }

                return base.CanConvertTo(context, destinationType);
            }

            /// <summary>
            ///  Converts the given object to the converter's native type.
            /// </summary>
            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
            {
                return value switch
                {
                    null or string { Length: 0 } => null,
                    string => value,
                    _ => base.ConvertFrom(context, culture, value)
                };
            }

            /// <summary>
            ///  Converts the given object to another type. The most common types to convert
            ///  are to and from a string object. The default implementation will make a call
            ///  to ToString on the object if the object is valid and if the destination
            ///  type is string. If this cannot convert to the destination type, this will
            ///  throw a NotSupportedException.
            /// </summary>
            public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return value ?? string.Empty;
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            /// <summary>
            ///  Retrieves a collection containing a set of standard values
            ///  for the data type this validator is designed for. This
            ///  will return null if the data type does not support a
            ///  standard set of values.
            /// </summary>
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
            {
                // We cannot cache this because it depends on the contents of the source file.
                string[]? eventMethods = null;

                if (context.TryGetService(out IEventBindingService? ebs))
                {
                    ICollection methods = ebs.GetCompatibleMethods(_evt);
                    eventMethods = new string[methods.Count];
                    methods.CopyTo(eventMethods, 0);
                }

                return new StandardValuesCollection(eventMethods);
            }

            /// <summary>
            ///  Determines if the list of standard values returned from
            ///  GetStandardValues is an exclusive list. If the list
            ///  is exclusive, then no other values are valid, such as
            ///  in an enum data type. If the list is not exclusive,
            ///  then there are other valid values besides the list of
            ///  standard values GetStandardValues provides.
            /// </summary>
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => false;

            /// <summary>
            ///  Determines if this object supports a standard set of values
            ///  that can be picked from a list.
            /// </summary>
            public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;
        }

        /// <summary>
        ///  This is a combination of a reference and a property, so that it can be used
        ///  as the key of a hashtable. This is because we may have subobjects that share
        ///  the same property.
        /// </summary>
        private class ReferenceEventClosure
        {
            private readonly object _reference;
            private readonly EventPropertyDescriptor _propertyDescriptor;

            public ReferenceEventClosure(object reference, EventPropertyDescriptor prop)
            {
                _reference = reference;
                _propertyDescriptor = prop;
            }

            public override int GetHashCode()
            {
                return _reference.GetHashCode() * _propertyDescriptor.GetHashCode();
            }

            public override bool Equals(object? otherClosure)
            {
                if (otherClosure is ReferenceEventClosure typedClosure)
                {
                    return typedClosure._reference == _reference &&
                           typedClosure._propertyDescriptor.Equals(_propertyDescriptor);
                }

                return false;
            }
        }
    }
}
