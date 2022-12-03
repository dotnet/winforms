// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using static Interop;
using static Interop.Ole32;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  Top level mapping layer between COM Object and TypeDescriptor.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   .NET uses this class for COM object type browsing. <see cref="PropertyGrid"/> is an indirect consumer of this
    ///   through the <see cref="TypeDescriptor"/> support in .NET.
    ///  </para>
    ///  <para>
    ///   The instance of this type is found via reflection in <see cref="TypeConverter"/> and exposed as
    ///   <see cref="TypeDescriptor.ComObjectType"/>.
    ///  </para>
    /// </remarks>
    internal partial class ComNativeDescriptor : TypeDescriptionProvider
    {
        private readonly AttributeCollection _staticAttributes = new(new Attribute[] { BrowsableAttribute.Yes, DesignTimeVisibleAttribute.No });

        // Our collection of Object managers (Com2Properties) for native properties
        private readonly WeakHashtable _nativeProperties = new();

        // Our collection of browsing handlers, which are stateless and shared across objects.
        private readonly Hashtable extendedBrowsingHandlers = new();

        // We increment this every time we look at an Object, then at specified intervals, we run through the
        // properties list to see if we should delete any.
        private int _clearCount;
        private const int ClearInterval = 25;

        // Called via reflection for AutomationExtender stuff. Don't delete!
        public static object? GetNativePropertyValue(object component, string propertyName, ref bool succeeded)
            => GetPropertyValue(component, propertyName, ref succeeded);

        public override ICustomTypeDescriptor? GetTypeDescriptor(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type objectType,
            object? instance)
            => new ComTypeDescriptor(this, instance);

        internal static unsafe string GetClassName(object component)
        {
            // Check IVsPerPropertyBrowsing for a name.
            if (component is VSSDK.IVsPerPropertyBrowsing browsing)
            {
                string? name = null;
                if (browsing.GetClassName(ref name).Succeeded && name is not null)
                {
                    return name;
                }
            }

            if (Com2TypeInfoProcessor.FindTypeInfo(component, preferIProvideClassInfo: true) is not Oleaut32.ITypeInfo typeInfo)
            {
                return string.Empty;
            }

            using BSTR nameBstr = default;
            typeInfo.GetDocumentation(
                DispatchID.MEMBERID_NIL,
                &nameBstr,
                pBstrDocString: null,
                pdwHelpContext: null,
                pBstrHelpFile: null);
            return nameBstr.AsSpan().TrimStart('_').ToString();
        }

        internal static TypeConverter GetConverter() => TypeDescriptor.GetConverter(typeof(IComponent));

        internal static object? GetEditor(object component, Type baseEditorType)
            => TypeDescriptor.GetEditor(component.GetType(), baseEditorType);

        internal static string GetName(object component)
        {
            if (component is not Oleaut32.IDispatch)
            {
                return string.Empty;
            }

            DispatchID dispid = Com2TypeInfoProcessor.GetNameDispId((Oleaut32.IDispatch)component);
            if (dispid != DispatchID.UNKNOWN)
            {
                bool success = false;
                object? value = GetPropertyValue(component, dispid, ref success);

                if (success && value is not null)
                {
                    return value.ToString() ?? string.Empty;
                }
            }

            return string.Empty;
        }

        internal static unsafe object? GetPropertyValue(object component, string propertyName, ref bool succeeded)
        {
            if (component is not Oleaut32.IDispatch dispatch)
            {
                return null;
            }

            string[] names = new string[] { propertyName };
            DispatchID dispid = DispatchID.UNKNOWN;
            Guid guid = Guid.Empty;
            try
            {
                HRESULT result = dispatch.GetIDsOfNames(&guid, names, 1, PInvoke.GetThreadLocale(), &dispid);
                return result.Failed || dispid == DispatchID.UNKNOWN
                    ? null
                    : GetPropertyValue(component, dispid, ref succeeded);
            }
            catch
            {
                return null;
            }
        }

        internal static object? GetPropertyValue(object component, DispatchID dispid, ref bool succeeded)
        {
            if (component is not Oleaut32.IDispatch)
            {
                return null;
            }

            object[] pVarResult = new object[1];
            if (GetPropertyValue(component, dispid, pVarResult) == HRESULT.S_OK)
            {
                succeeded = true;
                return pVarResult[0];
            }
            else
            {
                succeeded = false;
                return null;
            }
        }

        internal static unsafe HRESULT GetPropertyValue(object component, DispatchID dispid, object[] retval)
        {
            if (component is not Oleaut32.IDispatch dispatch)
            {
                return HRESULT.E_NOINTERFACE;
            }

            try
            {
                Guid guid = Guid.Empty;
                EXCEPINFO pExcepInfo = default;
                DISPPARAMS dispParams = default;
                try
                {
                    HRESULT hr = dispatch.Invoke(
                        dispid,
                        &guid,
                        PInvoke.GetThreadLocale(),
                        DISPATCH_FLAGS.DISPATCH_PROPERTYGET,
                        &dispParams,
                        retval,
                        &pExcepInfo,
                        null);

                    return hr == HRESULT.DISP_E_EXCEPTION ? (HRESULT)pExcepInfo.scode : hr;
                }
                catch (ExternalException ex)
                {
                    return (HRESULT)ex.ErrorCode;
                }
            }
            catch
            {
            }

            return HRESULT.E_FAIL;
        }

        /// <summary>
        ///  Checks if the given dispid matches the dispid that the Object would like to specify
        ///  as its identification property (Name, ID, etc).
        /// </summary>
        internal static bool IsNameDispId(object obj, DispatchID dispid)
            => obj is not null
                && obj.GetType().IsCOMObject
                && dispid == Com2TypeInfoProcessor.GetNameDispId((Oleaut32.IDispatch)obj);

        /// <summary>
        ///  Checks all our property manages to see if any have become invalid.
        /// </summary>
        private void CheckClear()
        {
            // Walk the list every so many calls.
            if ((++_clearCount % ClearInterval) != 0)
            {
                return;
            }

            lock (_nativeProperties)
            {
                _clearCount = 0;

                List<object>? disposeList = null;
                Com2Properties? entry;

                // First walk the list looking for items that need to be cleaned out.
                foreach (DictionaryEntry de in _nativeProperties)
                {
                    entry = de.Value as Com2Properties;

                    if (entry is not null && entry.NeedsRefreshed)
                    {
                        disposeList ??= new List<object>(3);
                        disposeList.Add(de.Key);
                    }
                }

                // Now run through the ones that are dead and dispose them.
                // There's going to be a very small number of these.
                if (disposeList is not null)
                {
                    object oldKey;
                    for (int i = disposeList.Count - 1; i >= 0; i--)
                    {
                        oldKey = disposeList[i];
                        entry = _nativeProperties[oldKey] as Com2Properties;

                        if (entry is not null)
                        {
                            entry.Disposed -= new EventHandler(OnPropsInfoDisposed);
                            entry.Dispose();
                            _nativeProperties.Remove(oldKey);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Gets the properties manager for an Object.
        /// </summary>
        private Com2Properties? GetPropertiesInfo(object component)
        {
            // Check caches if necessary.
            CheckClear();

            // Get the property info Object.
            Com2Properties? properties = (Com2Properties?)_nativeProperties[component];

            // If we don't have one, create one and set it up.
            if (properties is null || !properties.CheckValidity())
            {
                properties = Com2TypeInfoProcessor.GetProperties(component);
                if (properties is not null)
                {
                    properties.Disposed += OnPropsInfoDisposed;
                    _nativeProperties.SetWeak(component, properties);
                    properties.AddExtendedBrowsingHandlers(extendedBrowsingHandlers);
                }
            }

            return properties;
        }

        internal AttributeCollection GetAttributes(object? component)
        {
            if (component is null)
            {
                return _staticAttributes;
            }

            List<Attribute> attributes = new();

            if (component is VSSDK.IVSMDPerPropertyBrowsing browsing)
            {
                attributes.AddRange(Com2IManagedPerPropertyBrowsingHandler.GetComponentAttributes(browsing, DispatchID.MEMBERID_NIL));
            }

            if (Com2ComponentEditor.NeedsComponentEditor(component))
            {
                attributes.Add(new EditorAttribute(typeof(Com2ComponentEditor), typeof(ComponentEditor)));
            }

            return attributes.Count == 0 ? _staticAttributes : new(attributes.ToArray());
        }

        internal PropertyDescriptor? GetDefaultProperty(object component)
        {
            CheckClear();
            return GetPropertiesInfo(component)?.DefaultProperty;
        }

        internal static EventDescriptorCollection GetEvents() => new EventDescriptorCollection(null);

        internal static EventDescriptor? GetDefaultEvent() => null;

        internal PropertyDescriptorCollection GetProperties(object component)
        {
            Com2Properties? properties = GetPropertiesInfo(component);

            if (properties is null)
            {
                return PropertyDescriptorCollection.Empty;
            }

            try
            {
                properties.AlwaysValid = true;
                return new PropertyDescriptorCollection(properties.Properties);
            }
            finally
            {
                properties.AlwaysValid = false;
            }
        }

        /// <summary>
        ///  Fired when the property info gets disposed.
        /// </summary>
        private void OnPropsInfoDisposed(object? sender, EventArgs e)
        {
            if (sender is not Com2Properties propsInfo)
            {
                return;
            }

            propsInfo.Disposed -= OnPropsInfoDisposed;

            lock (_nativeProperties)
            {
                // Find the key.
                object? key = propsInfo.TargetObject;

                if (key is null && _nativeProperties.ContainsValue(propsInfo))
                {
                    // Need to find it - the target object has probably been cleaned out of the Com2Properties object
                    // already, so we run through the hashtable looking for the value, so we know what key to remove.
                    foreach (DictionaryEntry de in _nativeProperties)
                    {
                        if (de.Value == propsInfo)
                        {
                            key = de.Key;
                            break;
                        }
                    }

                    if (key is null)
                    {
                        Debug.Fail("Failed to find Com2 properties key on dispose.");
                        return;
                    }
                }

                if (key is not null)
                {
                    _nativeProperties.Remove(key);
                }
            }
        }

        /// <summary>
        ///  Looks at at value's type and creates an editor based on that.  We use this to decide which editor to use
        ///  for a generic variant.
        /// </summary>
        internal static void ResolveVariantTypeConverterAndTypeEditor(
            object? propertyValue,
            ref TypeConverter currentConverter,
            Type editorType,
            ref object? currentEditor)
        {
            if (propertyValue is not null && !Convert.IsDBNull(propertyValue))
            {
                Type type = propertyValue.GetType();
                TypeConverter subConverter = TypeDescriptor.GetConverter(type);
                if (subConverter is not null && subConverter.GetType() != typeof(TypeConverter))
                {
                    currentConverter = subConverter;
                }

                object? subEditor = TypeDescriptor.GetEditor(type, editorType);
                if (subEditor is not null)
                {
                    currentEditor = subEditor;
                }
            }
        }
    }
}
