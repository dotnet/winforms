// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.ComponentModel.Com2Interop.Tests;

public unsafe class Com2IPerPropertyBrowsingHandlerTests
{
    private const int TestDispId = 42;

    private enum Level
    {
        Low = 1,
        High = 2
    }

    // Scenario: An object that exposes IPerPropertyBrowsing is detected by the handler.
    [StaFact]
    public void ObjectSupportsInterface_FixtureImplementingInterface_ReturnsTrue()
    {
        Com2IPerPropertyBrowsingHandler handler = new();
        Assert.True(handler.ObjectSupportsInterface(new PerPropertyBrowsingFixture()));
    }

    // Scenario: A plain managed object that does not expose IPerPropertyBrowsing is not detected by the handler.
    [StaFact]
    public void ObjectSupportsInterface_ObjectWithoutInterface_ReturnsFalse()
    {
        Com2IPerPropertyBrowsingHandler handler = new();
        Assert.False(handler.ObjectSupportsInterface(new object()));
    }

    // Scenario: Registering the handler with no properties is a no-op and does not throw.
    [StaFact]
    public void RegisterEvents_NullProperties_DoesNotThrow()
    {
        Com2IPerPropertyBrowsingHandler handler = new();
        handler.RegisterEvents(null);
        handler.RegisterEvents([]);
    }

    // Scenario: A property with a property page is explicitly marked browsable by the handler.
    [StaFact]
    public void Attributes_PropertyWithPropertyPage_IsMarkedBrowsable()
    {
        PerPropertyBrowsingFixture fixture = new() { PageGuid = Guid.NewGuid() };
        FixturePropertyDescriptor descriptor = new(fixture);
        FixturePropertyDescriptor withoutHandler = new(fixture);

        Assert.False(withoutHandler.HandlerForcedBrowsable);

        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.True(descriptor.HandlerForcedBrowsable);
        Assert.Equal(TestDispId, fixture.MapPropertyToPageDispIds.Single());
    }

    // Scenario: A property without a property page (empty GUID) is not forced to be browsable.
    [StaFact]
    public void Attributes_PropertyWithEmptyPageGuid_IsNotMarkedBrowsable()
    {
        PerPropertyBrowsingFixture fixture = new() { PageGuid = Guid.Empty };
        FixturePropertyDescriptor descriptor = new(fixture);

        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.False(descriptor.HandlerForcedBrowsable);
        Assert.Single(fixture.MapPropertyToPageDispIds);
    }

    // Scenario: A native failure while mapping the property page does not mark the property browsable.
    [StaFact]
    public void Attributes_WhenMapPropertyToPageFails_IsNotMarkedBrowsable()
    {
        PerPropertyBrowsingFixture fixture = new()
        {
            PageGuid = Guid.NewGuid(),
            MapPropertyToPageResult = HRESULT.E_FAIL
        };

        FixturePropertyDescriptor descriptor = new(fixture);
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.False(descriptor.HandlerForcedBrowsable);
    }

    // Scenario: A property explicitly marked as not browsable in metadata is not made browsable by a property page.
    [StaFact]
    public void Attributes_PropertyExplicitlyHidden_IsNotMadeBrowsable()
    {
        PerPropertyBrowsingFixture fixture = new() { PageGuid = Guid.NewGuid() };
        FixturePropertyDescriptor descriptor = new(
            fixture,
            propertyType: typeof(string),
            attributes: [BrowsableAttribute.No]);

        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.False(descriptor.CanShow);
        Assert.False(descriptor.HandlerForcedBrowsable);
        Assert.False(descriptor.IsBrowsable);
    }

    // Scenario: When the target does not implement IPerPropertyBrowsing the handler leaves attributes untouched.
    [StaFact]
    public void Attributes_TargetWithoutInterface_IsNotMarkedBrowsable()
    {
        FixturePropertyDescriptor descriptor = new(new object());
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.False(descriptor.HandlerForcedBrowsable);
    }

    // Scenario: The display string provided by the object is shown for the property value.
    [StaFact]
    public void GetDisplayValue_ObjectProvidesDisplayString_UsesIt()
    {
        PerPropertyBrowsingFixture fixture = new() { DisplayString = "Custom display" };
        FixturePropertyDescriptor descriptor = new(fixture);
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.Equal("Custom display", descriptor.GetDisplayValue("default"));
        Assert.Equal(TestDispId, fixture.GetDisplayStringDispIds.Single());
    }

    // Scenario: If the object fails to provide a display string the default text is kept.
    [StaFact]
    public void GetDisplayValue_GetDisplayStringFails_KeepsDefault()
    {
        PerPropertyBrowsingFixture fixture = new()
        {
            DisplayString = "ignored",
            GetDisplayStringResult = HRESULT.E_FAIL
        };

        FixturePropertyDescriptor descriptor = new(fixture);
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.Equal("default", descriptor.GetDisplayValue("default"));
        Assert.Single(fixture.GetDisplayStringDispIds);
    }

    // Scenario: When the target does not implement IPerPropertyBrowsing the display value is left unchanged.
    [StaFact]
    public void GetDisplayValue_TargetWithoutInterface_KeepsDefault()
    {
        FixturePropertyDescriptor descriptor = new(new object());
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.Equal("default", descriptor.GetDisplayValue("default"));
    }

    // Scenario: A property whose native value is converted to a managed type (a color) does not ask the object for a
    // display string, because that would override the converted value.
    [StaFact]
    public void GetDisplayValue_PropertyConvertsNativeType_DoesNotCallGetDisplayString()
    {
        PerPropertyBrowsingFixture fixture = new() { DisplayString = "ignored" };
        FixturePropertyDescriptor descriptor = new(
            fixture,
            propertyType: typeof(Drawing.Color),
            typeData: new Guid("{66504301-BE0F-101A-8BBB-00AA00300CAB}"));

        Assert.True(descriptor.ConvertingNativeType);
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.Equal("default", descriptor.GetDisplayValue("default"));
        Assert.Empty(fixture.GetDisplayStringDispIds);
    }

    // Scenario: Predefined strings and cookies from the object produce a dropdown converter, and the actual values
    // are only requested from the object once the list is needed.
    [StaFact]
    public void Converter_PredefinedStrings_ProvidesStandardValuesFetchedLazily()
    {
        PerPropertyBrowsingFixture fixture = new()
        {
            Names = ["Low", "High"],
            Cookies = [10, 20],
            PredefinedValues = { [10] = 100, [20] = 200 }
        };

        FixturePropertyDescriptor descriptor = new(fixture, propertyType: typeof(int));
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        TypeConverter converter = descriptor.Converter;

        Assert.Equal(1, fixture.GetPredefinedStringsCalls);
        Assert.Empty(fixture.GetPredefinedValueCookies);
        Assert.True(converter.GetStandardValuesSupported());
        Assert.False(converter.GetStandardValuesExclusive());

        ICollection values = converter.GetStandardValues()!;
        Assert.Equal([100, 200], values.Cast<object>().ToArray());
        Assert.Equal(new uint[] { 20, 10 }, fixture.GetPredefinedValueCookies.Order().Reverse().ToArray());

        Assert.Equal(100, converter.ConvertFromString("Low"));
        Assert.Equal(200, converter.ConvertFromString("High"));
        Assert.Equal("High", converter.ConvertToString(200));
    }

    // Scenario: Predefined values reported as integers are converted to the managed enum type of the property.
    [StaFact]
    public void Converter_EnumPropertyType_ConvertsNativeValuesToEnum()
    {
        PerPropertyBrowsingFixture fixture = new()
        {
            Names = ["Low", "High"],
            Cookies = [1, 2],
            PredefinedValues = { [1] = 1, [2] = 2 }
        };

        FixturePropertyDescriptor descriptor = new(fixture, propertyType: typeof(Level));
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        TypeConverter converter = descriptor.Converter;
        Assert.Equal([Level.Low, Level.High], converter.GetStandardValues()!.Cast<object>().ToArray());
        Assert.Equal(Level.High, converter.ConvertFromString("High"));
    }

    // Scenario: Predefined values of a different native type are converted to the property's managed type.
    [StaFact]
    public void Converter_NativeValueTypeDiffers_ConvertsToPropertyType()
    {
        PerPropertyBrowsingFixture fixture = new()
        {
            Names = ["First", "Second"],
            Cookies = [1, 2],
            PredefinedValues = { [1] = (short)7, [2] = (short)9 }
        };

        FixturePropertyDescriptor descriptor = new(fixture, propertyType: typeof(int));
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        object[] values = descriptor.Converter.GetStandardValues()!.Cast<object>().ToArray();
        Assert.Equal([7, 9], values);
        Assert.All(values, value => Assert.IsType<int>(value));
    }

    // Scenario: A property whose predefined strings are empty does not get a dropdown converter.
    [StaFact]
    public void Converter_EmptyPredefinedStrings_NoStandardValues()
    {
        PerPropertyBrowsingFixture fixture = new();
        FixturePropertyDescriptor descriptor = new(fixture, propertyType: typeof(int));
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.False(descriptor.Converter.GetStandardValuesSupported());
        Assert.Empty(fixture.GetPredefinedValueCookies);
    }

    // Scenario: Converting the current value to text uses the object's display string without fetching the whole
    // list of predefined values.
    [StaFact]
    public void Converter_ConvertToStringForCurrentValue_UsesDisplayStringWithoutFetchingValues()
    {
        PerPropertyBrowsingFixture fixture = new()
        {
            DisplayString = "Current item",
            Names = ["Low"],
            Cookies = [1],
            PredefinedValues = { [1] = 100 }
        };

        FixturePropertyDescriptor descriptor = new(fixture, propertyType: typeof(int));
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        // The fixture is not an IDispatch, so the property's current value is null.
        Assert.Equal("Current item", descriptor.Converter.ConvertToString(null));
        Assert.Empty(fixture.GetPredefinedValueCookies);
    }

    // Scenario: When the object cannot supply predefined strings but has a property page, a property page editor is
    // provided for the property.
    [StaFact]
    public void GetEditor_PredefinedStringsFailWithPropertyPage_ReturnsPropertyPageEditor()
    {
        using NoAssertContext noAssert = new();

        PerPropertyBrowsingFixture fixture = new()
        {
            PageGuid = Guid.NewGuid(),
            GetPredefinedStringsResult = HRESULT.E_FAIL
        };

        FixturePropertyDescriptor descriptor = new(fixture, propertyType: typeof(string));
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        object? editor = descriptor.GetEditor(typeof(UITypeEditor));

        Assert.IsType<Com2PropertyPageUITypeEditor>(editor);
        Assert.Equal(UITypeEditorEditStyle.Modal, ((UITypeEditor)editor).GetEditStyle());
        Assert.False(descriptor.Converter.GetStandardValuesSupported());
    }

    // Scenario: When the object has neither predefined strings nor a property page, no editor is added.
    [StaFact]
    public void GetEditor_PredefinedStringsFailWithoutPropertyPage_NoPropertyPageEditor()
    {
        using NoAssertContext noAssert = new();

        PerPropertyBrowsingFixture fixture = new()
        {
            PageGuid = Guid.Empty,
            GetPredefinedStringsResult = HRESULT.E_FAIL
        };

        FixturePropertyDescriptor descriptor = new(fixture, propertyType: typeof(string));
        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.IsNotType<Com2PropertyPageUITypeEditor>(descriptor.GetEditor(typeof(UITypeEditor)));
    }

    // Scenario: A property that converts its native type keeps its own editing behavior instead of getting the
    // property page editor.
    [StaFact]
    public void GetEditor_PropertyConvertsNativeType_DoesNotUsePropertyPageEditor()
    {
        using NoAssertContext noAssert = new();

        PerPropertyBrowsingFixture fixture = new()
        {
            PageGuid = Guid.NewGuid(),
            GetPredefinedStringsResult = HRESULT.E_FAIL
        };

        FixturePropertyDescriptor descriptor = new(
            fixture,
            propertyType: typeof(Drawing.Color),
            typeData: new Guid("{66504301-BE0F-101A-8BBB-00AA00300CAB}"));

        new Com2IPerPropertyBrowsingHandler().RegisterEvents([descriptor]);

        Assert.IsNotType<Com2PropertyPageUITypeEditor>(descriptor.GetEditor(typeof(UITypeEditor)));
    }

    // Scenario: Using the handler against a COM object releases every native reference it acquired, so the object
    // can be collected afterwards.
    [StaFact]
    public void Handler_AfterBrowsing_ReleasesNativeReferences()
    {
        WeakReference weakFixture = BrowseAndReleaseFixture();

        for (int i = 0; i < 5 && weakFixture.IsAlive; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        Assert.False(weakFixture.IsAlive);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference BrowseAndReleaseFixture()
    {
        PerPropertyBrowsingFixture fixture = new()
        {
            PageGuid = Guid.NewGuid(),
            DisplayString = "text",
            Names = ["Low"],
            Cookies = [1],
            PredefinedValues = { [1] = 100 }
        };

        FixturePropertyDescriptor descriptor = new(fixture, propertyType: typeof(int));
        Com2IPerPropertyBrowsingHandler handler = new();
        Assert.True(handler.ObjectSupportsInterface(fixture));
        handler.RegisterEvents([descriptor]);

        _ = descriptor.Attributes;
        _ = descriptor.GetDisplayValue("default");
        _ = descriptor.Converter.GetStandardValues();
        _ = descriptor.Converter.ConvertToString(null);

        WeakReference weakFixture = new(fixture);
        descriptor.Target = null;
        return weakFixture;
    }

    private sealed class PerPropertyBrowsingFixture : IPerPropertyBrowsing.Interface, IManagedWrapper<IPerPropertyBrowsing>
    {
        // CALPOLESTR.ConvertAndFree releases the strings but not the pointer table, so the fixture frees the tables.
        private readonly List<nint> _stringTables = [];

        ~PerPropertyBrowsingFixture()
        {
            foreach (nint table in _stringTables)
            {
                Marshal.FreeCoTaskMem(table);
            }
        }

        public Guid PageGuid { get; set; }
        public HRESULT MapPropertyToPageResult { get; set; } = HRESULT.S_OK;
        public string? DisplayString { get; set; }
        public HRESULT GetDisplayStringResult { get; set; } = HRESULT.S_OK;
        public string[] Names { get; set; } = [];
        public uint[] Cookies { get; set; } = [];
        public HRESULT GetPredefinedStringsResult { get; set; } = HRESULT.S_OK;
        public Dictionary<uint, object> PredefinedValues { get; } = [];

        public List<int> MapPropertyToPageDispIds { get; } = [];
        public List<int> GetDisplayStringDispIds { get; } = [];
        public int GetPredefinedStringsCalls { get; private set; }
        public List<uint> GetPredefinedValueCookies { get; } = [];

        public HRESULT MapPropertyToPage(int dispID, Guid* pClsid)
        {
            MapPropertyToPageDispIds.Add(dispID);
            // The output is written even on failure so a failed HRESULT must be what hides the page.
            *pClsid = PageGuid;
            return MapPropertyToPageResult;
        }

        public HRESULT GetDisplayString(int dispID, BSTR* pBstr)
        {
            GetDisplayStringDispIds.Add(dispID);
            // Ownership of the allocated string transfers to the caller, even when the call reports failure.
            *pBstr = new BSTR(DisplayString);
            return GetDisplayStringResult;
        }

        public HRESULT GetPredefinedStrings(int dispID, CALPOLESTR* pCaStringsOut, CADWORD* pCaCookiesOut)
        {
            GetPredefinedStringsCalls++;
            if (GetPredefinedStringsResult.Failed)
            {
                return GetPredefinedStringsResult;
            }

            pCaStringsOut->cElems = (uint)Names.Length;
            pCaStringsOut->pElems = (PWSTR*)Marshal.AllocCoTaskMem(Names.Length * sizeof(nint));
            _stringTables.Add((nint)pCaStringsOut->pElems);
            for (int i = 0; i < Names.Length; i++)
            {
                pCaStringsOut->pElems[i] = new PWSTR((char*)Marshal.StringToCoTaskMemUni(Names[i]));
            }

            pCaCookiesOut->cElems = (uint)Cookies.Length;
            pCaCookiesOut->pElems = (uint*)Marshal.AllocCoTaskMem(Cookies.Length * sizeof(uint));
            for (int i = 0; i < Cookies.Length; i++)
            {
                pCaCookiesOut->pElems[i] = Cookies[i];
            }

            return HRESULT.S_OK;
        }

        public HRESULT GetPredefinedValue(int dispID, uint dwCookie, VARIANT* pVarOut)
        {
            GetPredefinedValueCookies.Add(dwCookie);
            if (!PredefinedValues.TryGetValue(dwCookie, out object? value))
            {
                return HRESULT.E_FAIL;
            }

            *pVarOut = VARIANT.FromObject(value);
            return HRESULT.S_OK;
        }
    }
}
