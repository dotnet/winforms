//------------------------------------------------------------------------------
// <copyright file="ComponentTypeCodeDomSerializer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.ComponentModel.Design.Serialization {

    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    
    /// <include file='doc\ComponentTypeCodeDomSerializer.uex' path='docs/doc[@for="ComponentTypeCodeDomSerializer"]/*' />
    /// <devdoc>
    ///     This class performs the same tasks as a CodeDomSerializer only serializing an object through this class defines a new type.
    /// </devdoc>
    internal class ComponentTypeCodeDomSerializer : TypeCodeDomSerializer {
        private static object _initMethodKey = new object();
        private const string _initMethodName = "InitializeComponent";
        private static ComponentTypeCodeDomSerializer _default;

        internal new static ComponentTypeCodeDomSerializer Default {
            get {
                if (_default == null) {
                    _default = new ComponentTypeCodeDomSerializer();
                }

                return _default;
            }
        }

        /// <include file='doc\ComponentTypeCodeDomSerializer.uex' path='docs/doc[@for="ComponentTypeCodeDomSerializer.GetInitializeMethod"]/*' />
        /// <devdoc>
        ///    This method returns the method to emit all of the initialization code to for the given member.  
        ///    The default implementation returns an empty constructor.
        /// </devdoc>
        protected override CodeMemberMethod GetInitializeMethod(IDesignerSerializationManager manager, CodeTypeDeclaration typeDecl, object value) {
            if (manager == null) {
                throw new ArgumentNullException("manager");
            }

            if (typeDecl == null) {
                throw new ArgumentNullException("typeDecl");
            }

            if (value == null) {
                throw new ArgumentNullException("value");
            }

            CodeMemberMethod method = typeDecl.UserData[_initMethodKey] as CodeMemberMethod;

            if (method == null) {
                method = new CodeMemberMethod();
                method.Name = _initMethodName;
                method.Attributes = MemberAttributes.Private;
                typeDecl.UserData[_initMethodKey] = method;

                // Now create a ctor that calls this method.
                CodeConstructor ctor = new CodeConstructor();

                ctor.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), _initMethodName));
                typeDecl.Members.Add(ctor);
            }

            return method;
        }

        /// <include file='doc\ComponentTypeCodeDomSerializer.uex' path='docs/doc[@for="ComponentTypeCodeDomSerializer.GetInitializeMethods"]/*' />
        /// <devdoc>
        ///    This method returns an array of methods that need to be interpreted during deserialization.  
        ///    The default implementation returns a single element array with the constructor in it.
        /// </devdoc>
        protected override CodeMemberMethod[] GetInitializeMethods(IDesignerSerializationManager manager, CodeTypeDeclaration typeDecl) {
            if (manager == null) {
                throw new ArgumentNullException("manager");
            }

            if (typeDecl == null) {
                throw new ArgumentNullException("typeDecl");
            }

            foreach (CodeTypeMember member in typeDecl.Members) {
                CodeMemberMethod method = member as CodeMemberMethod;

                // Note: the order is important here for performance! 
                // method.Parameters causes OnMethodPopulateParameters callback and therefore it is much more 
                // expensive than method.Name.Equals

                if (method != null && method.Name.Equals(_initMethodName) && method.Parameters.Count == 0 ) {
                    return new CodeMemberMethod[] { method };
                }
            }

            return new CodeMemberMethod[0];
        }
    }
}

