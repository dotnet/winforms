// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel.Design;
using System.Runtime.Serialization;

namespace System.Resources
{
    // This class implements a partial type resolver for the BinaryFormatter.
    // This is needed to be able to read binary serialized content from older
    // NDP types and map them to newer versions.

    internal class ResXSerializationBinder : SerializationBinder
    {
        private readonly ITypeResolutionService _typeResolver;
        private readonly Func<Type, string> _typeNameConverter;

        internal ResXSerializationBinder(ITypeResolutionService typeResolver)
        {
            _typeResolver = typeResolver;
        }

        internal ResXSerializationBinder(Func<Type, string> typeNameConverter)
        {
            _typeNameConverter = typeNameConverter;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            if (_typeResolver is null)
            {
                return null;
            }

            typeName = typeName + ", " + assemblyName;

            Type type = _typeResolver.GetType(typeName);
            if (type is null)
            {
                string[] typeParts = typeName.Split(',');

                // Break up the assembly name from the rest of the assembly strong name.
                // we try 1) FQN 2) FQN without a version 3) just the short name
                if (typeParts != null && typeParts.Length > 2)
                {
                    string partialName = typeParts[0].Trim();

                    for (int i = 1; i < typeParts.Length; ++i)
                    {
                        string typePart = typeParts[i].Trim();
                        if (!typePart.StartsWith("Version=") && !typePart.StartsWith("version="))
                        {
                            partialName = partialName + ", " + typePart;
                        }
                    }

                    type = _typeResolver.GetType(partialName);
                    if (type is null)
                    {
                        type = _typeResolver.GetType(typeParts[0].Trim());
                    }
                }
            }

            // Binder couldn't handle it, let the default loader take over.
            return type;
        }

        // Get the multitarget-aware string representation for the give type.
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            // Normally we don't change typeName when changing the target framework,
            // only assembly version or assembly name might change, thus we are setting
            // typeName only if it changed with the framework version.
            // If binder passes in a null, BinaryFormatter will use the original value or
            // for un-serializable types will redirect to another type.
            // For example:
            //
            // Encoding = Encoding.GetEncoding("shift_jis");
            // public Encoding Encoding { get; set; }
            // property type (Encoding) is abstract, but the value is instantiated to a specific class,
            // and should be serialized as a specific class in order to be able to instantiate the result.
            //
            // another example are singleton objects like DBNull.Value which are serialized by System.UnitySerializationHolder
            typeName = null;
            if (_typeNameConverter != null)
            {
                string assemblyQualifiedTypeName = MultitargetUtil.GetAssemblyQualifiedName(serializedType, _typeNameConverter);
                if (!string.IsNullOrEmpty(assemblyQualifiedTypeName))
                {
                    int pos = assemblyQualifiedTypeName.IndexOf(',');
                    if (pos > 0 && pos < assemblyQualifiedTypeName.Length - 1)
                    {
                        assemblyName = assemblyQualifiedTypeName.Substring(pos + 1).TrimStart();
                        string newTypeName = assemblyQualifiedTypeName.Substring(0, pos);
                        if (!string.Equals(newTypeName, serializedType.FullName, StringComparison.InvariantCulture))
                        {
                            typeName = newTypeName;
                        }

                        return;
                    }
                }
            }

            base.BindToName(serializedType, out assemblyName, out typeName);
        }
    }
}
