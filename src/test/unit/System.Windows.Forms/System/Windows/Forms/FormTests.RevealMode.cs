// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public partial class FormTests
{
#if NET11_0_OR_GREATER
    [WinFormsFact]
    public void Form_FormRevealMode_Default_ResolvesFromApplication()
    {
        using SubForm form = new();

        Application.SetDefaultFormRevealMode(FormRevealMode.Classic);
        Assert.Equal(FormRevealMode.Classic, form.FormRevealMode);

        Application.SetDefaultFormRevealMode(FormRevealMode.Deferred);
        Assert.Equal(FormRevealMode.Deferred, form.FormRevealMode);
    }

    [WinFormsTheory]
    [InlineData(FormRevealMode.Classic)]
    [InlineData(FormRevealMode.Deferred)]
    public void Form_FormRevealMode_SetExplicit_OverridesApplicationDefault(FormRevealMode mode)
    {
        using SubForm form = new();
        FormRevealMode otherMode = mode == FormRevealMode.Classic ? FormRevealMode.Deferred : FormRevealMode.Classic;

        Application.SetDefaultFormRevealMode(otherMode);
        form.FormRevealMode = mode;

        Assert.Equal(mode, form.FormRevealMode);
    }

    [WinFormsFact]
    public void Form_FormRevealMode_SetInherit_ClearsLocalOverride()
    {
        using SubForm form = new();

        Application.SetDefaultFormRevealMode(FormRevealMode.Deferred);
        form.FormRevealMode = FormRevealMode.Classic;
        Assert.Equal(FormRevealMode.Classic, form.FormRevealMode);

        form.FormRevealMode = FormRevealMode.Inherit;

        Assert.Equal(FormRevealMode.Deferred, form.FormRevealMode);
    }

    [WinFormsFact]
    public void Form_FormRevealMode_InvalidValue_ThrowsInvalidEnumArgumentException()
    {
        using SubForm form = new();

        Assert.Throws<InvalidEnumArgumentException>(
            "value",
            () => form.FormRevealMode = (FormRevealMode)int.MaxValue);
    }

    [WinFormsFact]
    public void Form_FormRevealMode_ShouldSerialize_ResetRoundTrips()
    {
        using SubForm form = new();
        PropertyDescriptor property = TypeDescriptor.GetProperties(form)[nameof(Form.FormRevealMode)];

        Assert.False(property.ShouldSerializeValue(form));

        form.FormRevealMode = FormRevealMode.Classic;
        Assert.True(property.ShouldSerializeValue(form));

        property.ResetValue(form);
        Assert.False(property.ShouldSerializeValue(form));
    }
#endif
}
