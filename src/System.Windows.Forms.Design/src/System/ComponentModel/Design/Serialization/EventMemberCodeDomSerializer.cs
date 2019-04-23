// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.CodeDom;
using System.ComponentModel.Design;
using System.Design;
using System.Diagnostics;
using System.Reflection;

namespace System.ComponentModel.Design.Serialization {

    /// <devdoc>
    ///    A MemberCodeDomSerializer for events.
    /// </devdoc>
    internal sealed class EventMemberCodeDomSerializer : MemberCodeDomSerializer {

        private static CodeThisReferenceExpression _thisRef = new CodeThisReferenceExpression();
        private static EventMemberCodeDomSerializer _default;

        internal static EventMemberCodeDomSerializer Default {
            get {
                if (_default == null) _default = new EventMemberCodeDomSerializer();

                return _default;
            }
        }

        /// <devdoc>
        ///    This method actually performs the serialization.  When the member is serialized
        ///    the necessary statements will be added to the statements collection.
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2102:CatchNonClsCompliantExceptionsInGeneralHandlers")]
        public override void Serialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor, CodeStatementCollection statements) {
            EventDescriptor eventToSerialize = descriptor as EventDescriptor;

            if (manager == null)              throw new ArgumentNullException("manager");
            if (value == null)                throw new ArgumentNullException("value");
            if (eventToSerialize == null) throw new ArgumentNullException("descriptor"); 
            if (statements == null) throw new ArgumentNullException("statements");

            try {
                IEventBindingService eventBindings = (IEventBindingService)manager.GetService(typeof(IEventBindingService));

                // VSWhidbey 204574: If the IEventBindingService is not available, we don't throw - we just don't do 
                // anything.
                if (eventBindings != null) {
                    PropertyDescriptor prop = eventBindings.GetEventProperty(eventToSerialize);
                    string methodName = (string)prop.GetValue(value);
    
                    if (methodName != null) {
                        CodeDomSerializer.Trace("Event {0} bound to {1}", eventToSerialize.Name, methodName);
                        CodeExpression eventTarget = SerializeToExpression(manager, value);
                        CodeDomSerializer.TraceWarningIf(eventTarget == null, "Object has no name and no propery ref in context so we cannot serialize events: {0}", value);
                        if (eventTarget != null) {
                            CodeTypeReference delegateTypeRef = new CodeTypeReference(eventToSerialize.EventType);
                            CodeDelegateCreateExpression delegateCreate = new CodeDelegateCreateExpression(delegateTypeRef, _thisRef, methodName);
                            CodeEventReferenceExpression eventRef = new CodeEventReferenceExpression(eventTarget, eventToSerialize.Name);
                            CodeAttachEventStatement attach = new CodeAttachEventStatement(eventRef, delegateCreate);
    
                            attach.UserData[typeof(Delegate)] = eventToSerialize.EventType;
                            statements.Add(attach);
                        }
                    }
                }
            }
            catch (Exception e) {
                // Since we usually go through reflection, don't 
                // show what our engine does, show what caused 
                // the problem.
                //
                if (e is TargetInvocationException) {
                    e = e.InnerException;
                }

                manager.ReportError(SR.GetString(SR.SerializerPropertyGenFailed, eventToSerialize.Name, e.Message));
            }
        }

        /// <devdoc>
        ///    This method returns true if the given member descriptor should be serialized,
        ///    or false if there is no need to serialize the member.
        /// </devdoc>
        public override bool ShouldSerialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor) {
            return true;
        }
	}
}
