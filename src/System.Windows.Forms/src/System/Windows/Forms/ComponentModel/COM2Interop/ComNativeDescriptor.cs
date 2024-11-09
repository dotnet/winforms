// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Shell;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  Top level mapping layer between COM objects and TypeDescriptor.
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
[RequiresUnreferencedCode($"{ComTypeDescriptorsMessage} Uses Com2IManagedPerPropertyBrowsingHandler which is not trim-compatible.")]
internal sealed unsafe partial class ComNativeDescriptor : TypeDescriptionProvider
{
    internal const string ComTypeDescriptorsMessage = "COM type descriptors are not trim-compatible.";

    private static readonly Attribute[] s_staticAttributes = [BrowsableAttribute.Yes, DesignTimeVisibleAttribute.No];

    // Our collection of Object managers (Com2Properties) for native properties
    private readonly ConditionalWeakTable<object, Com2Properties> _nativeProperties = [];

    // Our collection of browsing handlers, which are stateless and shared across objects.
    //
    // These used to be created as needed for each type descriptor and shared among the properties that supported them.
    // Lazily creating a single instance of these for each ComNativeDescriptor was needlessly complex and used reflection.
    // These objects have no state so the memory impact of always creating all of them is trivial.
    //
    // Making these static isn't really a good option as they register callbacks that we currently don't unhook.
    private readonly ICom2ExtendedBrowsingHandler[] _extendedBrowsingHandlers =
    [
        new Com2ICategorizePropertiesHandler(),
        new Com2IProvidePropertyBuilderHandler(),
        new Com2IPerPropertyBrowsingHandler(),
        new Com2IVsPerPropertyBrowsingHandler(),
        new Com2IManagedPerPropertyBrowsingHandler()
    ];

    // We increment this every time we look at an Object, then at specified intervals, we run through the
    // properties list to see if we should delete any.
    private int _clearCount;
    private const int ClearInterval = 25;

    // Called via reflection for AutomationExtender stuff. Don't delete!
    public static object? GetNativePropertyValue(object component, string propertyName, ref bool succeeded)
    {
        using var dispatch = ComHelpers.TryGetComScope<IDispatch>(component, out HRESULT hr);
        object? value = null;
        succeeded = hr.Succeeded && GetPropertyValue(dispatch, propertyName, out value).Succeeded;
        return value;
    }

    public override ICustomTypeDescriptor? GetTypeDescriptor(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type objectType,
        object? instance)
        => instance is null ? new NullTypeDescriptor() : new ComTypeDescriptor(this, instance);

    internal static string GetClassName(object component)
    {
        // Check IVsPerPropertyBrowsing for a name first.
        using var propertyBrowsing = ComHelpers.TryGetComScope<IVsPerPropertyBrowsing>(component, out HRESULT hr);
        if (hr.Succeeded)
        {
            using BSTR className = default;
            if (propertyBrowsing.Value->GetClassName(&className).Succeeded && !className.IsNull)
            {
                return className.ToString();
            }
        }

        using ComScope<ITypeInfo> typeInfo = new(Com2TypeInfoProcessor.FindTypeInfo(component, preferIProvideClassInfo: true));
        if (typeInfo.IsNull)
        {
            return string.Empty;
        }

        using BSTR typeInfoName = default;
        typeInfo.Value->GetDocumentation(
            PInvoke.MEMBERID_NIL,
            &typeInfoName,
            pBstrDocString: null,
            pdwHelpContext: null,
            pBstrHelpFile: null);

        return typeInfoName.AsSpan().TrimStart('_').ToString();
    }

    internal static TypeConverter GetIComponentConverter() => TypeDescriptor.GetConverter(typeof(IComponent));

    [RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming.")]
    internal static object? GetEditor(object component, Type baseEditorType)
        => TypeDescriptor.GetEditor(component.GetType(), baseEditorType);

    internal static string GetName(IDispatch* dispatch)
    {
        int dispid = Com2TypeInfoProcessor.GetNameDispId(dispatch);
        if (dispid != PInvokeCore.DISPID_UNKNOWN)
        {
            HRESULT hr = GetPropertyValue(dispatch, dispid, out object? value);

            if (hr.Succeeded && value is not null)
            {
                return value.ToString() ?? string.Empty;
            }
        }

        return string.Empty;
    }

    internal static HRESULT GetPropertyValue(IDispatch* dispatch, string propertyName, out object? value)
    {
        value = null;

        fixed (char* n = propertyName)
        {
            Guid guid = Guid.Empty;
            int dispid = PInvokeCore.DISPID_UNKNOWN;

            HRESULT hr = dispatch->GetIDsOfNames(&guid, (PWSTR*)&n, 1, PInvokeCore.GetThreadLocale(), &dispid);
            if (hr.Failed)
            {
                return hr;
            }

            return dispid == PInvokeCore.DISPID_UNKNOWN
                ? HRESULT.DISP_E_MEMBERNOTFOUND
                : GetPropertyValue(dispatch, dispid, out value);
        }
    }

    internal static HRESULT GetPropertyValue(IDispatch* dispatch, int dispid, out object? value)
    {
        value = null;

        using VARIANT result = default;
        HRESULT hr = dispatch->TryGetProperty(dispid, &result, PInvokeCore.GetThreadLocale());

        if (hr.Succeeded)
        {
            try
            {
                value = result.ToObject();
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
                hr = (HRESULT)ex.HResult;
            }
        }

        return hr;
    }

    /// <summary>
    ///  Checks if the given dispid matches the dispid that the Object would like to specify
    ///  as its identification property (Name, ID, etc).
    /// </summary>
    internal static bool IsNameDispId(object? @object, int dispid)
    {
        using var dispatch = ComHelpers.TryGetComScope<IDispatch>(@object, out HRESULT hr);
        return !hr.Failed && dispid == Com2TypeInfoProcessor.GetNameDispId(dispatch);
    }

    /// <summary>
    ///  Checks all our property managers to see if any have become invalid.
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

            List<object> disposeKeys = [];

            // First walk the list looking for items that need to be cleaned out.
            foreach (var entry in _nativeProperties)
            {
                if (entry.Value is Com2Properties { NeedsRefreshed: true })
                {
                    disposeKeys.Add(entry.Key);
                }
            }

            // Now run through the ones that are dead and dispose them.
            // There's going to be a very small number of these.
            foreach (object key in disposeKeys)
            {
                if (_nativeProperties.TryGetValue(key, out Com2Properties? properties))
                {
                    properties.Disposed -= OnPropsInfoDisposed;
                    properties.Dispose();
                    _nativeProperties.Remove(key);
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

        // If we don't have one, create one and set it up.
        if (!_nativeProperties.TryGetValue(component, out Com2Properties? properties)
            || properties.CheckAndGetTarget(checkVersions: false, callDispose: true) is null)
        {
            properties = Com2TypeInfoProcessor.GetProperties(component);
            if (properties is not null)
            {
                properties.Disposed += OnPropsInfoDisposed;
                _nativeProperties.AddOrUpdate(component, properties);
                properties.RegisterPropertyEvents(_extendedBrowsingHandlers);
            }
        }

        return properties;
    }

    internal static AttributeCollection GetAttributes(object component)
    {
        List<Attribute> attributes = [];

        using var browsing = ComHelpers.TryGetComScope<IVSMDPerPropertyBrowsing>(component, out HRESULT hr);
        if (hr.Succeeded)
        {
            attributes.AddRange(Com2IManagedPerPropertyBrowsingHandler.GetComponentAttributes(browsing, PInvoke.MEMBERID_NIL));
        }

        if (Com2ComponentEditor.NeedsComponentEditor(component))
        {
            attributes.Add(new EditorAttribute(typeof(Com2ComponentEditor), typeof(ComponentEditor)));
        }

        return attributes.Count == 0 ? new(s_staticAttributes) : new(attributes.ToArray());
    }

    internal PropertyDescriptor? GetDefaultProperty(object component)
    {
        CheckClear();
        return GetPropertiesInfo(component)?.DefaultProperty;
    }

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
        if (sender is not Com2Properties properties)
        {
            return;
        }

        properties.Disposed -= OnPropsInfoDisposed;

        lock (_nativeProperties)
        {
            // Find the key.
            object? key = properties.TargetObject;

            if (key is null)
            {
                // Need to find it - the target object has probably been cleaned out of the Com2Properties object
                // already, so we run through the hashtable looking for the value, so we know what key to remove.
                foreach (var entry in _nativeProperties)
                {
                    if (entry.Value == properties)
                    {
                        key = entry.Key;
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
    ///  Looks at value's type and creates an editor based on that. We use this to decide which editor to use
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
