// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  Browsing handler for <see cref="IVSMDPerPropertyBrowsing"/>.
/// </summary>
[RequiresUnreferencedCode(ComNativeDescriptor.ComTypeDescriptorsMessage + " Uses reflection to inspect types whose names are not statically known.")]
internal sealed unsafe class Com2IManagedPerPropertyBrowsingHandler : Com2ExtendedBrowsingHandler<IVSMDPerPropertyBrowsing>
{
    public override void RegisterEvents(Com2PropertyDescriptor[]? properties)
    {
        if (properties is null)
        {
            return;
        }

        for (int i = 0; i < properties.Length; i++)
        {
            properties[i].QueryGetDynamicAttributes += OnGetAttributes;
        }
    }

    /// <summary>
    ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing.
    ///  Hide properties such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
    /// </summary>
    private void OnGetAttributes(Com2PropertyDescriptor sender, GetAttributesEvent e)
    {
        using var propertyBrowsing = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Succeeded)
        {
            Attribute[] attributes = GetComponentAttributes(propertyBrowsing.Value, sender.DISPID);
            for (int i = 0; i < attributes.Length; i++)
            {
                e.Add(attributes[i]);
            }
        }
    }

    internal static Attribute[] GetComponentAttributes(IVSMDPerPropertyBrowsing* propertyBrowsing, int dispid)
    {
        uint attributeCount;
        BSTR* nativeTypeNames;
        VARIANT* nativeValues;

        HRESULT hr = propertyBrowsing->GetPropertyAttributes(dispid, &attributeCount, &nativeTypeNames, &nativeValues);
        if (hr != HRESULT.S_OK || attributeCount == 0 || nativeValues is null)
        {
            return [];
        }

        List<Attribute> attributes = [];

        string[] typeNames = GetStringsFromPtr(nativeTypeNames, attributeCount);
        object?[] values = GetVariantsFromPtr(nativeValues, attributeCount);

        Debug.Assert(typeNames.Length == values.Length, "Mismatched parameter and attribute name length");
        if (typeNames.Length != values.Length)
        {
            return [];
        }

        // Get the types.
        for (int i = 0; i < typeNames.Length; i++)
        {
            string typeName = typeNames[i];

            // Try the name first.
            Type? type = null;
            if (typeName.Length > 0)
            {
                type = Type.GetType(typeName);
            }

            Assembly? assembly = type?.Assembly;

            if (type is null)
            {
                // Couldn't find the type by name, try to parse it as a fully qualified field name
                // and look at that field for the type.

                string assemblyName = string.Empty;

                int comma = typeName.LastIndexOf(',');

                if (comma != -1)
                {
                    assemblyName = typeName[comma..];
                    typeName = typeName[..comma];
                }

                string fieldName;
                int lastDot = typeName.LastIndexOf('.');
                if (lastDot != -1)
                {
                    fieldName = typeName[(lastDot + 1)..];
                }
                else
                {
                    Debug.Fail("No dot in class name?");
                    continue;
                }

                // Try to get the field value
                type = assembly is null
                    ? Type.GetType(string.Concat(typeName.AsSpan(0, lastDot), assemblyName))
                    : assembly.GetType(string.Concat(typeName.AsSpan(0, lastDot), assemblyName));

                if (type is null)
                {
                    Debug.Fail($"Failed load attribute '{typeName}{assemblyName}'. It's Type could not be found.");
                    continue;
                }

                Debug.Assert(typeof(Attribute).IsAssignableFrom(type), $"Attribute type {type.FullName} does not derive from Attribute");
                if (!typeof(Attribute).IsAssignableFrom(type))
                {
                    continue;
                }

                if (type.GetField(fieldName) is { } field && field.IsStatic)
                {
                    if (field.GetValue(null) is Attribute attribute)
                    {
                        attributes.Add(attribute);
                        continue;
                    }
                }
                else
                {
                    Debug.Fail($"Couldn't load field '{fieldName}' from type '{typeName[..lastDot]}'. It does not exist or is not static");
                }
            }

            Debug.Assert(typeof(Attribute).IsAssignableFrom(type), $"Attribute type {type.FullName} does not derive from Attribute");
            if (!typeof(Attribute).IsAssignableFrom(type))
            {
                continue;
            }

            // If we got here, we need to build the attribute.

            object? value = values[i];
            if (!Convert.IsDBNull(value) && value is not null)
            {
                // We have an initializer value for a one item constructor.

                foreach (var constructor in type.GetConstructors())
                {
                    ParameterInfo[] parameters = constructor.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(value.GetType()))
                    {
                        // Found a one-parameter ctor, use it to try to construct a default one.
                        try
                        {
                            if (Activator.CreateInstance(type, [value]) is Attribute attribute)
                            {
                                attributes.Add(attribute);
                            }
                        }
                        catch
                        {
                            Debug.Fail($"Attribute {type.FullName} did not have a initializer specified and has no default constructor");
                            continue;
                        }
                    }
                }
            }
            else
            {
                // Try to construct a default one.
                try
                {
                    if (Activator.CreateInstance(type) is Attribute attribute)
                    {
                        attributes.Add(attribute);
                    }
                }
                catch
                {
                    Debug.Fail($"Attribute {type.FullName} did not have a initializer specified and has no default constructor");
                    continue;
                }
            }
        }

        return [.. attributes];
    }

    private static string[] GetStringsFromPtr(BSTR* values, uint count)
    {
        if (values is null)
        {
            return [];
        }

        string[] strings = new string[count];
        for (int i = 0; i < count; i++)
        {
            strings[i] = values[i].ToStringAndFree();
        }

        try
        {
            Marshal.FreeCoTaskMem((nint)(void*)values);
        }
        catch (Exception ex)
        {
            Debug.Fail("Failed to free BSTR array memory", ex.ToString());
        }

        return strings;
    }

    private static unsafe object?[] GetVariantsFromPtr(VARIANT* values, uint count)
    {
        object?[] objects = new object?[count];
        for (int i = 0; i < count; i++)
        {
            try
            {
                using VARIANT variant = values[i];
                objects[i] = variant.ToObject();
            }
            catch (Exception ex)
            {
                Debug.Fail($"Failed to marshal component attribute VARIANT {i}", ex.ToString());
            }
        }

        try
        {
            Marshal.FreeCoTaskMem((IntPtr)values);
        }
        catch (Exception ex)
        {
            Debug.Fail("Failed to free VARIANT array memory", ex.ToString());
        }

        return objects;
    }
}
