// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design {
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.ComponentModel.Design;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using System.Collections;
    using Microsoft.Win32;


    /// <include file='doc\EventsTab.uex' path='docs/doc[@for="EventsTab"]/*' />
    /// <devdoc>
    ///    <para>Provides a tab on the property browser to display events for selection and linking.</para>
    /// </devdoc>
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name="FullTrust")]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Name="FullTrust")]
    public class EventsTab : PropertyTab {
        private IServiceProvider sp;
        private IDesignerHost currentHost;
        private bool          sunkEvent;

        /// <include file='doc\EventsTab.uex' path='docs/doc[@for="EventsTab.EventsTab"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.Design.EventsTab'/> class.</para>
        /// </devdoc>
        public EventsTab(IServiceProvider sp){
            this.sp = sp;
        }

        /// <include file='doc\EventsTab.uex' path='docs/doc[@for="EventsTab.TabName"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the name of the tab.</para>
        /// </devdoc>
        public override string TabName {
            get {
                return SR.PBRSToolTipEvents;
            }
        }

        /// <include file='doc\EventsTab.uex' path='docs/doc[@for="EventsTab.HelpKeyword"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the help keyword for the tab.</para>
        /// </devdoc>
        public override string HelpKeyword {
            get {
                return "Events"; // do not localize.
            }
        }
        
             // override this to reject components you don't want to support.
        /// <include file='doc\EventsTab.uex' path='docs/doc[@for="EventsTab.CanExtend"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether the specified object can be extended.</para>
        /// </devdoc>
        public override bool CanExtend(Object extendee) {
            return !Marshal.IsComObject(extendee);
        }

        private void OnActiveDesignerChanged(object sender, ActiveDesignerEventArgs adevent){
            currentHost = adevent.NewDesigner;
        }

        /// <include file='doc\EventsTab.uex' path='docs/doc[@for="EventsTab.GetDefaultProperty"]/*' />
        /// <devdoc>
        ///    <para>Gets the default property from the specified object.</para>
        /// </devdoc>
        public override PropertyDescriptor GetDefaultProperty(object obj) {

            IEventBindingService eventPropertySvc = GetEventPropertyService(obj, null);

            if (eventPropertySvc == null) {
                return null;
            }

            // Find the default event.  Note that we rely on GetEventProperties caching
            // the property to event match, so we can == on the default event.
            // We assert that this always works.
            //
            EventDescriptor defEvent = TypeDescriptor.GetDefaultEvent(obj);
            if (defEvent != null) {
                return eventPropertySvc.GetEventProperty(defEvent);
            }
            return null;
        }

        private IEventBindingService GetEventPropertyService(object obj, ITypeDescriptorContext context) {

            IEventBindingService eventPropertySvc = null;

            if (!sunkEvent){
               IDesignerEventService des = (IDesignerEventService)sp.GetService(typeof(IDesignerEventService));

               if (des != null){
                   des.ActiveDesignerChanged += new ActiveDesignerEventHandler(this.OnActiveDesignerChanged);
               }
               sunkEvent = true;
            }

            if (eventPropertySvc == null && currentHost != null) {
               eventPropertySvc = (IEventBindingService)currentHost.GetService(typeof(IEventBindingService));
            }

            if (eventPropertySvc == null && obj is IComponent){
                  ISite site = ((IComponent)obj).Site;

                  if (site != null) {
                      eventPropertySvc = (IEventBindingService)site.GetService(typeof(IEventBindingService));
                  }
            }

            if (eventPropertySvc == null && context != null) {
                eventPropertySvc = (IEventBindingService)context.GetService(typeof(IEventBindingService));
            }
            return eventPropertySvc;
        }

        /// <include file='doc\EventsTab.uex' path='docs/doc[@for="EventsTab.GetProperties"]/*' />
        /// <devdoc>
        ///    <para> Gets all the properties of the tab.</para>
        /// </devdoc>
        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes) {
            return GetProperties(null, component, attributes);
        }

        /// <include file='doc\EventsTab.uex' path='docs/doc[@for="EventsTab.GetProperties2"]/*' />
        /// <devdoc>
        ///    <para>Gets the properties of the specified component...</para>
        /// </devdoc>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes) {
            //Debug.Assert(component != null, "Can't get properties for a null item!");

            IEventBindingService eventPropertySvc = GetEventPropertyService(component, context);

            if (eventPropertySvc == null) {
                return new PropertyDescriptorCollection(null);
            }
            EventDescriptorCollection events = TypeDescriptor.GetEvents(component, attributes);
            PropertyDescriptorCollection realEvents = eventPropertySvc.GetEventProperties(events);


            // Add DesignerSerializationVisibilityAttribute.Content to attributes to see if we have any.
            Attribute[] attributesPlusNamespace = new Attribute[attributes.Length + 1];
            Array.Copy(attributes, 0, attributesPlusNamespace, 0, attributes.Length);
            attributesPlusNamespace[attributes.Length] = DesignerSerializationVisibilityAttribute.Content;

            // If we do, then we traverse them to see if they have any events under the current attributes,
            // and if so, we'll show them as top-level properties so they can be drilled down into to get events.
            PropertyDescriptorCollection namespaceProperties = TypeDescriptor.GetProperties(component, attributesPlusNamespace);
            if (namespaceProperties.Count > 0) {
                ArrayList list = null;
                for (int i = 0; i < namespaceProperties.Count; i++) {
                    PropertyDescriptor nsProp = namespaceProperties[i];

                    TypeConverter tc = nsProp.Converter;

                    if (!tc.GetPropertiesSupported()) {
                         continue;
                    }

                    Object namespaceValue = nsProp.GetValue(component);
                    EventDescriptorCollection namespaceEvents = TypeDescriptor.GetEvents(namespaceValue, attributes);
                    if (namespaceEvents.Count > 0) {
                        if (list == null) {
                            list = new ArrayList();
                        }

                        // make this non-mergable
                        //
                        nsProp = TypeDescriptor.CreateProperty(nsProp.ComponentType, nsProp, MergablePropertyAttribute.No);
                        list.Add(nsProp);
                    }
                }

                // we've found some, so add them to the event list.
                if (list != null) {
                    PropertyDescriptor[] realNamespaceProperties = new PropertyDescriptor[list.Count];
                    list.CopyTo(realNamespaceProperties, 0);
                    PropertyDescriptor[] finalEvents = new PropertyDescriptor[realEvents.Count + realNamespaceProperties.Length];
                    realEvents.CopyTo(finalEvents, 0);
                    Array.Copy(realNamespaceProperties, 0, finalEvents, realEvents.Count, realNamespaceProperties.Length);
                    realEvents = new PropertyDescriptorCollection(finalEvents);
                }
            }

            return realEvents;
        }
    }
}

