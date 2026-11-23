// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Shell;
using Moq;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.ComponentModel.Com2Interop.Tests;

public unsafe class Com2IProvidePropertyBuilderHandlerTests
{
    private const int TestDispId = 42;
    private const string BuilderGuid = "{8D1B3E5A-6D2C-4F6E-A4C5-3B7C1F2E9A10}";

    private const CTLBLDTYPE InternalBuilder = CTLBLDTYPE.CTLBLDTYPE_FINTERNALBUILDER;

    // Scenario: An object that exposes IProvidePropertyBuilder is detected by the handler.
    [StaFact]
    public void ObjectSupportsInterface_FixtureImplementingInterface_ReturnsTrue()
    {
        Com2IProvidePropertyBuilderHandler handler = new();
        Assert.True(handler.ObjectSupportsInterface(new PropertyBuilderFixture()));
    }

    // Scenario: A plain managed object that does not expose IProvidePropertyBuilder is not detected by the handler.
    [StaFact]
    public void ObjectSupportsInterface_ObjectWithoutInterface_ReturnsFalse()
    {
        Com2IProvidePropertyBuilderHandler handler = new();
        Assert.False(handler.ObjectSupportsInterface(new object()));
    }

    // Scenario: Registering the handler with no properties is a no-op and does not throw.
    [StaFact]
    public void RegisterEvents_NullProperties_DoesNotThrow()
    {
        Com2IProvidePropertyBuilderHandler handler = new();
        handler.RegisterEvents(null);
        handler.RegisterEvents([]);
    }

    // Scenario: A property with an internal builder is explicitly marked browsable by the handler.
    [StaFact]
    public void Attributes_PropertyWithBuilder_IsMarkedBrowsable()
    {
        PropertyBuilderFixture fixture = new() { BuilderType = InternalBuilder, BuilderAvailable = true };
        FixturePropertyDescriptor descriptor = new(fixture, TestDispId);
        FixturePropertyDescriptor withoutHandler = new(fixture, TestDispId);

        Assert.False(withoutHandler.HandlerForcedBrowsable);

        new Com2IProvidePropertyBuilderHandler().RegisterEvents([descriptor]);

        Assert.True(descriptor.HandlerForcedBrowsable);
        Assert.Equal(TestDispId, fixture.MapPropertyToBuilderDispIds.First());
    }

    // Scenario: A native failure while mapping the builder does not mark the property browsable and adds no builder editor.
    [StaFact]
    public void Attributes_MapPropertyToBuilderFails_IsNotMarkedBrowsableAndHasNoBuilderEditor()
    {
        PropertyBuilderFixture fixture = new() { MapPropertyToBuilderResult = HRESULT.E_FAIL };
        FixturePropertyDescriptor descriptor = new(fixture, TestDispId);

        new Com2IProvidePropertyBuilderHandler().RegisterEvents([descriptor]);

        Assert.False(descriptor.HandlerForcedBrowsable);
        Assert.IsNotType<Com2PropertyBuilderUITypeEditor>(descriptor.GetEditor(typeof(UITypeEditor)));
    }

    // Scenario: A property explicitly marked as not browsable in metadata is not made browsable by a builder.
    [StaFact]
    public void Attributes_PropertyExplicitlyHidden_IsNotMadeBrowsable()
    {
        PropertyBuilderFixture fixture = new() { BuilderType = InternalBuilder, BuilderAvailable = true };
        FixturePropertyDescriptor descriptor = new(
            fixture,
            TestDispId,
            propertyType: typeof(string),
            attributes: [BrowsableAttribute.No]);

        new Com2IProvidePropertyBuilderHandler().RegisterEvents([descriptor]);

        Assert.False(descriptor.CanShow);
        Assert.False(descriptor.HandlerForcedBrowsable);
        Assert.False(descriptor.IsBrowsable);
    }

    // Scenario: A standard (non-internal) builder is not supported by the property browser, so the property is not
    // marked browsable and no builder editor is provided.
    [StaFact]
    public void Attributes_StandardBuilder_IsNotSupported()
    {
        using NoAssertContext noAssert = new();

        PropertyBuilderFixture fixture = new()
        {
            BuilderType = CTLBLDTYPE.CTLBLDTYPE_FSTDPROPBUILDER,
            BuilderAvailable = true
        };

        FixturePropertyDescriptor descriptor = new(fixture, TestDispId);
        new Com2IProvidePropertyBuilderHandler().RegisterEvents([descriptor]);

        Assert.False(descriptor.HandlerForcedBrowsable);
        Assert.IsNotType<Com2PropertyBuilderUITypeEditor>(descriptor.GetEditor(typeof(UITypeEditor)));
    }

    // Scenario: A property with an internal builder gets a modal builder editor.
    [StaFact]
    public void GetEditor_PropertyWithBuilder_ReturnsModalBuilderEditor()
    {
        PropertyBuilderFixture fixture = new() { BuilderType = InternalBuilder, BuilderAvailable = true };
        FixturePropertyDescriptor descriptor = new(fixture, TestDispId, propertyType: typeof(string));
        new Com2IProvidePropertyBuilderHandler().RegisterEvents([descriptor]);

        object? editor = descriptor.GetEditor(typeof(UITypeEditor));

        Assert.IsType<Com2PropertyBuilderUITypeEditor>(editor);
        Assert.Equal(UITypeEditorEditStyle.Modal, ((UITypeEditor)editor).GetEditStyle());
    }

    // Scenario: When the target does not implement IProvidePropertyBuilder no builder editor is provided.
    [StaFact]
    public void GetEditor_TargetWithoutInterface_ReturnsNoBuilderEditor()
    {
        FixturePropertyDescriptor descriptor = new(new object(), TestDispId, propertyType: typeof(string));
        new Com2IProvidePropertyBuilderHandler().RegisterEvents([descriptor]);

        Assert.IsNotType<Com2PropertyBuilderUITypeEditor>(descriptor.GetEditor(typeof(UITypeEditor)));
    }

    // Scenario: When the builder commits, the editor returns the new value supplied by the builder and passes the
    // property's DISPID, builder GUID and the dialog owner window to the object.
    [StaFact]
    public void EditValue_BuilderCommits_ReturnsNewValue()
    {
        PropertyBuilderFixture fixture = new()
        {
            BuilderType = InternalBuilder,
            BuilderAvailable = true,
            Commit = true,
            NewValue = "new value"
        };

        UITypeEditor editor = CreateBuilderEditor(fixture);
        IntPtr ownerHandle = new(0x1234);
        TestUIServiceProvider provider = new(ownerHandle);

        object? result = editor.EditValue(context: null, provider, "original");

        Assert.Equal("new value", result);
        Assert.Equal(TestDispId, fixture.ExecuteBuilderDispId);
        Assert.Equal(BuilderGuid, fixture.ExecuteBuilderGuid);
        Assert.Equal(ownerHandle, fixture.ExecuteBuilderOwner);
        Assert.Equal(1, fixture.ExecuteBuilderCalls);
    }

    // Scenario: When the builder is cancelled (not committed), the original value is returned.
    [StaFact]
    public void EditValue_BuilderDoesNotCommit_ReturnsOriginalValue()
    {
        PropertyBuilderFixture fixture = new()
        {
            BuilderType = InternalBuilder,
            BuilderAvailable = true,
            Commit = false,
            NewValue = "ignored"
        };

        UITypeEditor editor = CreateBuilderEditor(fixture);

        Assert.Equal("original", editor.EditValue(context: null, new TestUIServiceProvider(new IntPtr(1)), "original"));
        Assert.Equal(1, fixture.ExecuteBuilderCalls);
    }

    // Scenario: A builder that edits the object directly never replaces the value, even when it commits.
    [StaFact]
    public void EditValue_BuilderEditsObjectDirectly_ReturnsOriginalValue()
    {
        PropertyBuilderFixture fixture = new()
        {
            BuilderType = InternalBuilder | CTLBLDTYPE.CTLBLDTYPE_FEDITSOBJIDRECTLY,
            BuilderAvailable = true,
            Commit = true,
            NewValue = "ignored"
        };

        UITypeEditor editor = CreateBuilderEditor(fixture);

        Assert.Equal("original", editor.EditValue(context: null, new TestUIServiceProvider(new IntPtr(1)), "original"));
        Assert.Equal(1, fixture.ExecuteBuilderCalls);
    }

    // Scenario: A native failure while executing the builder keeps the original value, even if the object reported a
    // commit.
    [StaFact]
    public void EditValue_ExecuteBuilderFails_ReturnsOriginalValue()
    {
        using NoAssertContext noAssert = new();

        PropertyBuilderFixture fixture = new()
        {
            BuilderType = InternalBuilder,
            BuilderAvailable = true,
            Commit = true,
            NewValue = "ignored",
            ExecuteBuilderResult = HRESULT.E_FAIL
        };

        UITypeEditor editor = CreateBuilderEditor(fixture);

        Assert.Equal("original", editor.EditValue(context: null, new TestUIServiceProvider(new IntPtr(1)), "original"));
        Assert.Equal(1, fixture.ExecuteBuilderCalls);
    }

    // Scenario: When the property owner is described by an ICustomTypeDescriptor, the builder is executed on the owner
    // it reports rather than the wrapper.
    [StaFact]
    public void EditValue_TargetIsCustomTypeDescriptor_ExecutesBuilderOnPropertyOwner()
    {
        PropertyBuilderFixture owner = new()
        {
            BuilderType = InternalBuilder,
            BuilderAvailable = true,
            Commit = true,
            NewValue = "from owner"
        };

        FixturePropertyDescriptor descriptor = new(target: null, TestDispId, propertyType: typeof(string));
        Mock<ICustomTypeDescriptor> wrapper = new();
        wrapper.Setup(w => w.GetPropertyOwner(descriptor)).Returns(owner);
        descriptor.Target = wrapper.Object;

        Com2PropertyBuilderUITypeEditor editor = new(descriptor, BuilderGuid, InternalBuilder, baseEditor: null);

        Assert.Equal("from owner", editor.EditValue(context: null, new TestUIServiceProvider(new IntPtr(1)), "original"));
        Assert.Equal(1, owner.ExecuteBuilderCalls);
    }

    // Scenario: Using the handler and editor against a COM object releases every native reference they acquired, so
    // the object can be collected afterwards.
    [StaFact]
    public void Handler_AfterBrowsingAndEditing_ReleasesNativeReferences()
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
        PropertyBuilderFixture fixture = new()
        {
            BuilderType = InternalBuilder,
            BuilderAvailable = true,
            Commit = true,
            NewValue = "new value"
        };

        FixturePropertyDescriptor descriptor = new(fixture, TestDispId, propertyType: typeof(string));
        Com2IProvidePropertyBuilderHandler handler = new();
        Assert.True(handler.ObjectSupportsInterface(fixture));
        handler.RegisterEvents([descriptor]);

        _ = descriptor.Attributes;
        UITypeEditor editor = (UITypeEditor)descriptor.GetEditor(typeof(UITypeEditor))!;
        editor.EditValue(context: null, new TestUIServiceProvider(new IntPtr(1)), "original");

        WeakReference weakFixture = new(fixture);
        descriptor.Target = null;
        return weakFixture;
    }

    private static UITypeEditor CreateBuilderEditor(PropertyBuilderFixture fixture)
    {
        FixturePropertyDescriptor descriptor = new(fixture, TestDispId, propertyType: typeof(string));
        new Com2IProvidePropertyBuilderHandler().RegisterEvents([descriptor]);
        UITypeEditor editor = Assert.IsAssignableFrom<UITypeEditor>(descriptor.GetEditor(typeof(UITypeEditor)));
        Assert.IsType<Com2PropertyBuilderUITypeEditor>(editor);
        return editor;
    }

    private sealed class TestUIServiceProvider : IServiceProvider
    {
        private readonly IUIService _uiService;

        public TestUIServiceProvider(IntPtr ownerHandle)
        {
            Mock<IWin32Window> owner = new();
            owner.Setup(o => o.Handle).Returns(ownerHandle);

            Mock<IUIService> uiService = new();
            uiService.Setup(u => u.GetDialogOwnerWindow()).Returns(owner.Object);
            _uiService = uiService.Object;
        }

        public object? GetService(Type serviceType) => serviceType == typeof(IUIService) ? _uiService : null;
    }

    // This type intentionally does not implement IManagedWrapper, so it is exposed to the handler through the
    // built-in COM interop support for the hand-written interface definition.
    private sealed class PropertyBuilderFixture : IProvidePropertyBuilder.Interface
    {
        public CTLBLDTYPE BuilderType { get; set; }
        public bool BuilderAvailable { get; set; }
        public HRESULT MapPropertyToBuilderResult { get; set; } = HRESULT.S_OK;
        public HRESULT ExecuteBuilderResult { get; set; } = HRESULT.S_OK;
        public bool Commit { get; set; }
        public object? NewValue { get; set; }

        public List<int> MapPropertyToBuilderDispIds { get; } = [];
        public int ExecuteBuilderCalls { get; private set; }
        public int ExecuteBuilderDispId { get; private set; }
        public string? ExecuteBuilderGuid { get; private set; }
        public IntPtr ExecuteBuilderOwner { get; private set; }

        public HRESULT MapPropertyToBuilder(int dispid, CTLBLDTYPE* pdwCtlBldType, BSTR* pbstrGuidBldr, VARIANT_BOOL* builderAvailable)
        {
            MapPropertyToBuilderDispIds.Add(dispid);
            if (MapPropertyToBuilderResult.Failed)
            {
                return MapPropertyToBuilderResult;
            }

            *pdwCtlBldType = BuilderType;

            // Ownership of the allocated string transfers to the caller.
            *pbstrGuidBldr = new BSTR(BuilderGuid);
            *builderAvailable = BuilderAvailable ? VARIANT_BOOL.VARIANT_TRUE : VARIANT_BOOL.VARIANT_FALSE;
            return HRESULT.S_OK;
        }

        public HRESULT ExecuteBuilder(
            int dispid,
            BSTR* bstrGuidBldr,
            IDispatch* pdispApp,
            HWND hwndBldrOwner,
            VARIANT* pvarValue,
            VARIANT_BOOL* pbActionCommitted)
        {
            ExecuteBuilderCalls++;
            ExecuteBuilderDispId = dispid;
            ExecuteBuilderGuid = bstrGuidBldr->ToString();
            ExecuteBuilderOwner = hwndBldrOwner;

            // Outputs are written even on failure so a failed HRESULT must override a reported commit.
            *pvarValue = VARIANT.FromObject(NewValue);
            *pbActionCommitted = Commit ? VARIANT_BOOL.VARIANT_TRUE : VARIANT_BOOL.VARIANT_FALSE;
            return ExecuteBuilderResult;
        }
    }
}
