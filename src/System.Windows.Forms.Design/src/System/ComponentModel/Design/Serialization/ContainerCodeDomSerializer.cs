// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;

namespace System.ComponentModel.Design.Serialization {

    /// <summary>
    ///    This class is used to serialize things of type "IContainer".  We route all containers
    ///    to the designer host's container.
    /// </summary>
    internal class ContainerCodeDomSerializer : CodeDomSerializer {
        private const string _containerName = "components";
        private static ContainerCodeDomSerializer _defaultSerializer;
        
        /// <summary>
        ///     Retrieves a default static instance of this serializer.
        /// </summary>
        internal static ContainerCodeDomSerializer Default {
            get {
                if (_defaultSerializer == null) {
                    _defaultSerializer = new ContainerCodeDomSerializer();
                }

                return _defaultSerializer;
            }
        }

        /// <summary>
        ///     We override this so we can always provide the correct container as a reference.
        /// </summary>
        protected override object DeserializeInstance(IDesignerSerializationManager manager, Type type, object[] parameters, string name, bool addToContainer) {
            if (typeof(IContainer).IsAssignableFrom(type)) {
                object obj = manager.GetService(typeof(IContainer));

                if (obj != null) {
                    Trace("Returning IContainer service as container");
                    manager.SetName(obj, name);
                    return obj;
                }
            }

            Trace("No IContainer service, creating default container.");
            return base.DeserializeInstance(manager, type, parameters, name, addToContainer);
        }

        /// <summary>
        ///    Serializes the given object into a CodeDom object.  We serialize an IContainer by
        ///    declaring an IContainer member variable and then assigning a Container into it.
        /// </summary>
        public override object Serialize(IDesignerSerializationManager manager, object value) {

            // See if there is a type declaration on the stack. If there is, create a field representing
            // the container member variable.
            CodeTypeDeclaration typeDecl = manager.Context[typeof(CodeTypeDeclaration)] as CodeTypeDeclaration;
            RootContext rootCxt = manager.Context[typeof(RootContext)] as RootContext;
            CodeStatementCollection statements = new CodeStatementCollection();
            CodeExpression lhs;  
            
            if (typeDecl != null && rootCxt != null) {
                CodeMemberField field = new CodeMemberField(typeof(IContainer), _containerName);

                field.Attributes = MemberAttributes.Private;
                typeDecl.Members.Add(field);
                lhs = new CodeFieldReferenceExpression(rootCxt.Expression, _containerName);
            }
            else {
                CodeVariableDeclarationStatement var = new CodeVariableDeclarationStatement(typeof(IContainer), _containerName);

                statements.Add(var);
                lhs = new CodeVariableReferenceExpression(_containerName);
            }

            // Now create the container
            SetExpression(manager, value, lhs);
            CodeObjectCreateExpression objCreate = new CodeObjectCreateExpression(typeof(Container));
            CodeAssignStatement assign = new CodeAssignStatement(lhs, objCreate);

            assign.UserData["IContainer"] = "IContainer";

            statements.Add(assign);
            return statements; 
        }
    }
}

