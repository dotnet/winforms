// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides a tab on the property browser to display events for selection and linking.
    /// </summary>
    public class EventsTab : PropertyTab
    {
        private readonly IServiceProvider _sp;
        private IDesignerHost _currentHost;
        private bool _sunkEvent;

        /// <summary>
        ///  Initializes a new instance of the <see cref='EventsTab'/> class.
        /// </summary>
        public EventsTab(IServiceProvider sp)
        {
            _sp = sp;
        }

        /// <summary>
        ///  Gets or sets the name of the tab.
        /// </summary>
        public override string TabName => SR.PBRSToolTipEvents;

        /// <summary>
        ///  Gets or sets the help keyword for the tab.
        /// </summary>
        public override string HelpKeyword => "Events";

        /// <summary>
        ///  Gets a value indicating whether the specified object can be extended.
        /// </summary>
        public override bool CanExtend(object extendee) => extendee is null || !Marshal.IsComObject(extendee);

        private void OnActiveDesignerChanged(object sender, ActiveDesignerEventArgs adevent)
        {
            _currentHost = adevent?.NewDesigner;
        }

        /// <summary>
        ///  Gets the default property from the specified object.
        /// </summary>
        public override PropertyDescriptor GetDefaultProperty(object obj)
        {
            IEventBindingService eventPropertySvc = GetEventPropertyService(obj, null);
            if (eventPropertySvc is null)
            {
                return null;
            }

            // Find the default event.  Note that we rely on GetEventProperties caching
            // the property to event match, so we can == on the default event.
            // We assert that this always works.
            EventDescriptor defEvent = TypeDescriptor.GetDefaultEvent(obj);
            if (defEvent is null)
            {
                return null;
            }

            return eventPropertySvc.GetEventProperty(defEvent);
        }

        private IEventBindingService GetEventPropertyService(object obj, ITypeDescriptorContext context)
        {
            if (!_sunkEvent)
            {
                if (_sp?.GetService(typeof(IDesignerEventService)) is IDesignerEventService des)
                {
                    des.ActiveDesignerChanged += new ActiveDesignerEventHandler(OnActiveDesignerChanged);
                }

                _sunkEvent = true;
            }

            if (_currentHost != null)
            {
                if (_currentHost.GetService(typeof(IEventBindingService)) is IEventBindingService service)
                {
                    return service;
                }
            }

            if (obj is IComponent component)
            {
                if (component.Site?.GetService(typeof(IEventBindingService)) is IEventBindingService service)
                {
                    return service;
                }
            }

            if (context != null)
            {
                if (context.GetService(typeof(IEventBindingService)) is IEventBindingService service)
                {
                    return service;
                }
            }

            return null;
        }

        /// <summary>
        ///  Gets all the properties of the tab.
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            return GetProperties(null, component, attributes);
        }

        /// <summary>
        ///  Gets the properties of the specified component.
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
        {
            IEventBindingService eventPropertySvc = GetEventPropertyService(component, context);
            if (eventPropertySvc is null)
            {
                return new PropertyDescriptorCollection(null);
            }

            EventDescriptorCollection events = TypeDescriptor.GetEvents(component, attributes);
            PropertyDescriptorCollection realEvents = eventPropertySvc.GetEventProperties(events);

            // Add DesignerSerializationVisibilityAttribute.Content to attributes to see if we have any.
            var attributesPlusNamespace = new Attribute[attributes.Length + 1];
            Array.Copy(attributes, 0, attributesPlusNamespace, 0, attributes.Length);
            attributesPlusNamespace[attributes.Length] = DesignerSerializationVisibilityAttribute.Content;

            // If we do, then we traverse them to see if they have any events under the current attributes,
            // and if so, we'll show them as top-level properties so they can be drilled down into to get events.
            PropertyDescriptorCollection namespaceProperties = TypeDescriptor.GetProperties(component, attributesPlusNamespace);
            if (namespaceProperties.Count > 0)
            {
                ArrayList list = null;
                for (int i = 0; i < namespaceProperties.Count; i++)
                {
                    PropertyDescriptor nsProp = namespaceProperties[i];
                    TypeConverter tc = nsProp.Converter;
                    if (!tc.GetPropertiesSupported())
                    {
                        continue;
                    }

                    object namespaceValue = nsProp.GetValue(component);
                    EventDescriptorCollection namespaceEvents = TypeDescriptor.GetEvents(namespaceValue, attributes);
                    if (namespaceEvents.Count > 0)
                    {
                        if (list is null)
                        {
                            list = new ArrayList();
                        }

                        // Make this non-mergable
                        nsProp = TypeDescriptor.CreateProperty(nsProp.ComponentType, nsProp, MergablePropertyAttribute.No);
                        list.Add(nsProp);
                    }
                }

                // We've found some, so add them to the event list.
                if (list != null)
                {
                    var realNamespaceProperties = new PropertyDescriptor[list.Count];
                    list.CopyTo(realNamespaceProperties, 0);
                    var finalEvents = new PropertyDescriptor[realEvents.Count + realNamespaceProperties.Length];
                    realEvents.CopyTo(finalEvents, 0);
                    Array.Copy(realNamespaceProperties, 0, finalEvents, realEvents.Count, realNamespaceProperties.Length);
                    realEvents = new PropertyDescriptorCollection(finalEvents);
                }
            }

            return realEvents;
        }
    }
}
