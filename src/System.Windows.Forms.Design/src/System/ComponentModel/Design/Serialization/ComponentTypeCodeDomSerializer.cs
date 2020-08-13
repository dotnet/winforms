// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  This class performs the same tasks as a CodeDomSerializer only serializing an object through this class defines a new type.
    /// </summary>
    internal class ComponentTypeCodeDomSerializer : TypeCodeDomSerializer
    {
        private static readonly object _initMethodKey = new object();
        private const string _initMethodName = "InitializeComponent";
        private static ComponentTypeCodeDomSerializer s_default;

        internal new static ComponentTypeCodeDomSerializer Default
        {
            get
            {
                if (s_default is null)
                {
                    s_default = new ComponentTypeCodeDomSerializer();
                }

                return s_default;
            }
        }

        /// <summary>
        ///  This method returns the method to emit all of the initialization code to for the given member.
        ///  The default implementation returns an empty constructor.
        /// </summary>
        protected override CodeMemberMethod GetInitializeMethod(IDesignerSerializationManager manager, CodeTypeDeclaration typeDecl, object value)
        {
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (typeDecl is null)
            {
                throw new ArgumentNullException(nameof(typeDecl));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(typeDecl.UserData[_initMethodKey] is CodeMemberMethod method))
            {
                method = new CodeMemberMethod
                {
                    Name = _initMethodName,
                    Attributes = MemberAttributes.Private
                };
                typeDecl.UserData[_initMethodKey] = method;

                // Now create a ctor that calls this method.
                CodeConstructor ctor = new CodeConstructor();

                ctor.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), _initMethodName));
                typeDecl.Members.Add(ctor);
            }

            return method;
        }

        /// <summary>
        ///  This method returns an array of methods that need to be interpreted during deserialization.
        ///  The default implementation returns a single element array with the constructor in it.
        /// </summary>
        protected override CodeMemberMethod[] GetInitializeMethods(IDesignerSerializationManager manager, CodeTypeDeclaration typeDecl)
        {
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (typeDecl is null)
            {
                throw new ArgumentNullException(nameof(typeDecl));
            }

            foreach (CodeTypeMember member in typeDecl.Members)
            {
                // Note: the order is important here for performance!
                // method.Parameters causes OnMethodPopulateParameters callback and therefore it is much more
                // expensive than method.Name.Equals

                if (member is CodeMemberMethod method && method.Name.Equals(_initMethodName) && method.Parameters.Count == 0)
                {
                    return new CodeMemberMethod[]
                    {
                        method
                    };
                }
            }

            return Array.Empty<CodeMemberMethod>();
        }
    }
}

