// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Tests.TestResources;
using Windows.Win32.Graphics.GdiPlus;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using WMPLib;

namespace System.Windows.Forms.ComponentModel.Com2Interop.Tests;

public unsafe class ComNativeDescriptorTests
{
    [StaFact]
    public void ComNativeDescriptor_GetProperties_FromIPictureDisp_ComInterop()
    {
        using Bitmap bitmap = new(10, 20);
        object iPictureDisp = bitmap.CreateIPictureDispRCW();
        Assert.NotNull(iPictureDisp);

        ValidateIPictureDispProperties(iPictureDisp, TypeDescriptor.GetProperties(iPictureDisp));
    }

    [StaFact]
    public void ComNativeDescriptor_GetProperties_FromIPictureDisp_ComWrappers()
    {
        using Bitmap bitmap = new(10, 20);
        using var iPictureDisp = bitmap.CreateIPictureDisp();
        Assert.False(iPictureDisp.IsNull);

        // The runtime needs to be updated to allow ComWrappers through
        ComNativeDescriptor descriptor = new();
        object comWrapper = UnknownComWrappers.Instance.GetOrCreateObjectForComInstance(iPictureDisp, CreateObjectFlags.None);
        ValidateIPictureDispProperties(comWrapper, descriptor.GetProperties(comWrapper));
    }

    private static void ValidateIPictureDispProperties(object iPictureDisp, PropertyDescriptorCollection properties)
    {
        Assert.NotNull(properties);
        Assert.Equal(5, properties.Count);

        var handleProperty = properties["Handle"];
        Assert.IsType<Com2PropertyDescriptor>(handleProperty);
        Assert.True(handleProperty.IsReadOnly);
        Assert.Equal("Misc", handleProperty.Category);
        Assert.False(handleProperty.IsLocalizable);
        Assert.Equal("Windows.Win32.System.Com.IDispatch+Interface", handleProperty.ComponentType.FullName);

        // OLE_HANDLE
        Assert.Equal("System.Int32", handleProperty.PropertyType.FullName);

        Assert.Equal(DesignerSerializationVisibility.Visible, handleProperty.SerializationVisibility);
        Assert.False(handleProperty.CanResetValue(iPictureDisp));
        Assert.False(handleProperty.ShouldSerializeValue(iPictureDisp));
        Assert.NotEqual(0, handleProperty.GetValue(iPictureDisp));

        var converter = (Com2ExtendedTypeConverter)handleProperty.Converter;
        Assert.IsAssignableFrom<Int32Converter>(converter.InnerConverter);

        var typeProperty = properties["Type"];
        Assert.NotNull(typeProperty);
        Assert.True(typeProperty.IsReadOnly);
        Assert.Equal("Misc", typeProperty.Category);
        Assert.False(typeProperty.IsLocalizable);
        Assert.Equal("Windows.Win32.System.Com.IDispatch+Interface", typeProperty.ComponentType.FullName);

        Assert.Equal("System.Int16", typeProperty.PropertyType.FullName);
        Assert.Equal(PICTYPE.PICTYPE_BITMAP, (PICTYPE)(short)typeProperty.GetValue(iPictureDisp));
    }

    [StaFact]
    public void ComNativeDescriptor_GetProperties_FromIPicture()
    {
        // While we ask for IPicture, the underlying native class also supports IDispatch, so we get support
        // in the type descriptor.
        using Bitmap bitmap = new(10, 20);
        object iPicture = bitmap.CreateIPictureDispRCW();
        Assert.NotNull(iPicture);

        var properties = TypeDescriptor.GetProperties(iPicture);
        Assert.Equal(PICTYPE.PICTYPE_BITMAP, (PICTYPE)(short)properties["Type"].GetValue(iPicture));
    }

    [StaFact]
    public void ComNativeDescriptor_GetProperties_FromActiveXMediaPlayerControl_ComInterop()
    {
        Guid guid = typeof(WindowsMediaPlayerClass).GUID;
        HRESULT hr = PInvokeCore.CoCreateInstance(
            in guid,
            pUnkOuter: null,
            CLSCTX.CLSCTX_INPROC_SERVER,
            out IUnknown* mediaPlayerPtr);

        Assert.Equal(HRESULT.S_OK, hr);

        ComNativeDescriptor descriptor = new();
        object mediaPlayer = ComHelpers.GetObjectForIUnknown(mediaPlayerPtr);
        ValidateMediaPlayerProperties(mediaPlayer, descriptor.GetProperties(mediaPlayer));
    }

    [StaFact]
    public void ComNativeDescriptor_GetProperties_FromActiveXMediaPlayerControl_ComWrappers()
    {
        Guid guid = typeof(WindowsMediaPlayerClass).GUID;
        ComScope<IUnknown> unknown = new(null);
        HRESULT hr = PInvokeCore.CoCreateInstance(
            &guid,
            null,
            CLSCTX.CLSCTX_INPROC_SERVER,
            IID.Get<IUnknown>(),
            (void**)unknown);

        Assert.Equal(HRESULT.S_OK, hr);

        object mediaPlayer = UnknownComWrappers.Instance.GetOrCreateObjectForComInstance(unknown, CreateObjectFlags.None);

        ComNativeDescriptor descriptor = new();
        ValidateMediaPlayerProperties(mediaPlayer, descriptor.GetProperties(mediaPlayer));
    }

    private static void ValidateMediaPlayerProperties(object mediaPlayer, PropertyDescriptorCollection properties)
    {
        Assert.Equal(25, properties.Count);

        var urlProperty = properties["URL"];
        Assert.IsType<Com2PropertyDescriptor>(urlProperty);
        Assert.False(urlProperty.IsReadOnly);
        Assert.Equal("Misc", urlProperty.Category);
        Assert.False(urlProperty.IsLocalizable);
        Assert.Equal("Windows.Win32.System.Com.IDispatch+Interface", urlProperty.ComponentType.FullName);

        Assert.Equal("System.String", urlProperty.PropertyType.FullName);

        Assert.Equal(DesignerSerializationVisibility.Visible, urlProperty.SerializationVisibility);
        Assert.False(urlProperty.CanResetValue(mediaPlayer));
        Assert.False(urlProperty.ShouldSerializeValue(mediaPlayer));
        Assert.Equal(string.Empty, urlProperty.GetValue(mediaPlayer));

        urlProperty.SetValue(mediaPlayer, "Movie.mpg");

        // This will be the fully qualified path name, which is based on the current directory.
        Assert.EndsWith("Movie.mpg", (string)urlProperty.GetValue(mediaPlayer));

        var converter = (Com2ExtendedTypeConverter)urlProperty.Converter;
        Assert.IsAssignableFrom<StringConverter>(converter.InnerConverter);
    }

    [WinFormsFact]
    public void ComNativeDescriptor_GetProperties_StdAccessible()
    {
        using Control control = new();
        control.CreateControl();

        using ComScope<IAccessible> accessible = new(null);
        HRESULT hr = PInvoke.CreateStdAccessibleObject(
            control.HWND,
            (int)OBJECT_IDENTIFIER.OBJID_CLIENT,
            IID.Get<IAccessible>(),
            accessible);

        object comWrapper = UnknownComWrappers.Instance.GetOrCreateObjectForComInstance(accessible, CreateObjectFlags.None);

        ComNativeDescriptor descriptor = new();
        var properties = descriptor.GetProperties(comWrapper);
        Assert.Equal(4, properties.Count);

        var accChildCount = properties["accChildCount"];
        Assert.True(accChildCount.IsReadOnly);
        Assert.Equal(0, accChildCount.GetValue(comWrapper));

        var accFocus = properties["accFocus"];
        Assert.True(accFocus.IsReadOnly);
        // VT_EMPTY - Nothing has focus
        Assert.Null(accFocus.GetValue(comWrapper));
    }

    [StaFact]
    public void ComNativeDescriptor_GetProperties_StandardDispatchAccessible_MultiClassInfo()
    {
        // Hitting the IProvideMultipleClassInfo with our stub object created over a system created IAccessible TypeInfo.

        StandardAccessibleObjectWithMultipleClassInfo value = new();
        using var accessible = ComHelpers.GetComScope<IAccessible>(value);

        object comWrapper = UnknownComWrappers.Instance.GetOrCreateObjectForComInstance(accessible, CreateObjectFlags.None);
        ComNativeDescriptor descriptor = new();
        var properties = descriptor.GetProperties(comWrapper);

        Assert.Equal(4, properties.Count);

        var accChildCount = properties["accChildCount"];
        Assert.True(accChildCount.IsReadOnly);
        Assert.Equal(42, accChildCount.GetValue(comWrapper));

        var accFocus = properties["accFocus"];
        Assert.True(accFocus.IsReadOnly);
        Assert.Equal((int)PInvoke.CHILDID_SELF, accFocus.GetValue(comWrapper));
    }

    [WinFormsFact(Skip = "Causes test run to abort, must be run manually.")]
    public void ComNativeDescriptor_GetProperties_FromSimpleVBControl()
    {
        if (RuntimeInformation.ProcessArchitecture != Architecture.X86)
        {
            return;
        }

        // Not much to see with this control, but it does exercise a fair amount of code.
        ComClasses.VisualBasicSimpleControl.CreateInstance(out object vbcontrol).ThrowOnFailure();

        var properties = TypeDescriptor.GetProperties(vbcontrol);
        Assert.Empty(properties);

        var events = TypeDescriptor.GetEvents(vbcontrol);
        Assert.Empty(events);

        var attributes = TypeDescriptor.GetAttributes(vbcontrol);
        Assert.Equal(2, attributes.Count);
        BrowsableAttribute browsable = (BrowsableAttribute)attributes[0];
        Assert.True(browsable.Browsable);
        DesignTimeVisibleAttribute visible = (DesignTimeVisibleAttribute)attributes[1];
        Assert.False(visible.Visible);
    }

    /// <summary>
    ///  A simple stub that implements IDispatch for IAccessible and also implements IProvideMultipleClassInfo that
    ///  just forwards to the IDispatch ITypeInfo to exercise the IProvideMultipleClassInfo code path.
    /// </summary>
    private unsafe class StandardAccessibleObjectWithMultipleClassInfo :
        AccessibleDispatch,
        IProvideMultipleClassInfo.Interface,
        IAccessible.Interface,
        IManagedWrapper<IAccessible, IDispatch, IProvideMultipleClassInfo>
    {
        HRESULT IProvideMultipleClassInfo.Interface.GetClassInfo(ITypeInfo** ppTI) => HRESULT.E_NOTIMPL;

        HRESULT IProvideMultipleClassInfo.Interface.GetGUID(uint dwGuidKind, Guid* pGUID) => HRESULT.E_NOTIMPL;

        HRESULT IProvideMultipleClassInfo.Interface.GetMultiTypeInfoCount(uint* pcti)
        {
            if (pcti is null)
            {
                return HRESULT.E_POINTER;
            }

            *pcti = 1;
            return HRESULT.S_OK;
        }

        HRESULT IProvideMultipleClassInfo.Interface.GetInfoOfIndex(
            uint iti,
            MULTICLASSINFO_FLAGS dwFlags,
            ITypeInfo** pptiCoClass,
            uint* pdwTIFlags,
            uint* pcdispidReserved,
            Guid* piidPrimary,
            Guid* piidSource)
        {
            if (pptiCoClass is null)
            {
                return HRESULT.E_POINTER;
            }

            if (dwFlags != MULTICLASSINFO_FLAGS.MULTICLASSINFO_GETTYPEINFO)
            {
                return HRESULT.E_NOTIMPL;
            }

            return ((IDispatch.Interface)this).GetTypeInfo(iti, 0, pptiCoClass);
        }

        HRESULT IAccessible.Interface.get_accChildCount(int* pcountChildren)
        {
            if (pcountChildren is null)
            {
                return HRESULT.E_POINTER;
            }

            *pcountChildren = 42;
            return HRESULT.S_OK;
        }

        HRESULT IAccessible.Interface.get_accFocus(VARIANT* pvarChild)
        {
            if (pvarChild is null)
            {
                return HRESULT.E_POINTER;
            }

            *pvarChild = (VARIANT)(int)PInvoke.CHILDID_SELF;
            return HRESULT.S_OK;
        }

        HRESULT IProvideClassInfo2.Interface.GetClassInfo(ITypeInfo** ppTI) => HRESULT.E_NOTIMPL;
        HRESULT IProvideClassInfo2.Interface.GetGUID(uint dwGuidKind, Guid* pGUID) => HRESULT.E_NOTIMPL;
        HRESULT IProvideClassInfo.Interface.GetClassInfo(ITypeInfo** ppTI) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accParent(IDispatch** ppdispParent) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accChild(VARIANT varChild, IDispatch** ppdispChild) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accName(VARIANT varChild, BSTR* pszName) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accValue(VARIANT varChild, BSTR* pszValue) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accDescription(VARIANT varChild, BSTR* pszDescription) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accRole(VARIANT varChild, VARIANT* pvarRole) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accState(VARIANT varChild, VARIANT* pvarState) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accHelp(VARIANT varChild, BSTR* pszHelp) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accHelpTopic(BSTR* pszHelpFile, VARIANT varChild, int* pidTopic) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accKeyboardShortcut(VARIANT varChild, BSTR* pszKeyboardShortcut) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accSelection(VARIANT* pvarChildren) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accDefaultAction(VARIANT varChild, BSTR* pszDefaultAction) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.accSelect(int flagsSelect, VARIANT varChild) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.accLocation(int* pxLeft, int* pyTop, int* pcxWidth, int* pcyHeight, VARIANT varChild) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.accNavigate(int navDir, VARIANT varStart, VARIANT* pvarEndUpAt) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.accHitTest(int xLeft, int yTop, VARIANT* pvarChild) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.accDoDefaultAction(VARIANT varChild) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.put_accName(VARIANT varChild, BSTR szName) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.put_accValue(VARIANT varChild, BSTR szValue) => HRESULT.E_NOTIMPL;
    }

    private class UnknownComWrappers : ComWrappers
    {
        private UnknownComWrappers() { }
        public static ComWrappers Instance { get; } = new UnknownComWrappers();

        protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
        {
            throw new NotImplementedException();
        }

        protected override object CreateObject(nint externalComObject, CreateObjectFlags flags)
        {
            return new UnknownClass() { Unknown = externalComObject };
        }

        protected override void ReleaseObjects(IEnumerable objects)
        {
            throw new NotImplementedException();
        }

        private class UnknownClass
        {
            public nint Unknown { get; set; }
        }
    }
}
