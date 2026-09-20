// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public partial class FormTests
{
#if NET11_0_OR_GREATER
    [WinFormsFact]
    public void Form_FormRevealMode_Default_ReturnsInherit()
    {
        using SubForm form = new();

        Assert.Equal(FormRevealMode.Inherit, form.FormRevealMode);
        Assert.False(form.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(FormRevealMode.Classic)]
    [InlineData(FormRevealMode.Deferred)]
    public void Form_FormRevealMode_SetExplicit_OverridesApplicationDefault(FormRevealMode mode)
    {
        FormRevealMode originalDefault = Application.DefaultFormRevealMode;

        try
        {
            FormRevealMode otherMode = mode == FormRevealMode.Classic
                ? FormRevealMode.Deferred
                : FormRevealMode.Classic;
            Application.SetDefaultFormRevealMode(otherMode);

            using SubForm form = new() { FormRevealMode = mode };

            Assert.Equal(mode, form.FormRevealMode);
            Assert.Equal(mode, (FormRevealMode)form.TestAccessor.Dynamic.EffectiveFormRevealMode);
            Assert.False(form.IsHandleCreated);
        }
        finally
        {
            Application.SetDefaultFormRevealMode(originalDefault);
        }
    }

    [WinFormsTheory]
    [InlineData(FormRevealMode.Classic)]
    [InlineData(FormRevealMode.Deferred)]
    public void Form_FormRevealMode_SetInherit_ReturnsInheritAndUsesApplicationDefault(
        FormRevealMode applicationDefault)
    {
        FormRevealMode originalDefault = Application.DefaultFormRevealMode;

        try
        {
            Application.SetDefaultFormRevealMode(applicationDefault);

            using SubForm form = new()
            {
                FormRevealMode = applicationDefault == FormRevealMode.Classic
                    ? FormRevealMode.Deferred
                    : FormRevealMode.Classic
            };

            form.FormRevealMode = FormRevealMode.Inherit;

            Assert.Equal(FormRevealMode.Inherit, form.FormRevealMode);
            Assert.Equal(
                applicationDefault,
                (FormRevealMode)form.TestAccessor.Dynamic.EffectiveFormRevealMode);
            Assert.False(form.IsHandleCreated);
        }
        finally
        {
            Application.SetDefaultFormRevealMode(originalDefault);
        }
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
        Assert.Equal(FormRevealMode.Inherit, form.FormRevealMode);
        Assert.False(property.ShouldSerializeValue(form));
    }

    [WinFormsFact]
    public void Form_FormRevealModeChanged_RaisedOnlyWhenEffectiveValueChanges()
    {
        using SubForm form = new();
        form.FormRevealMode = FormRevealMode.Classic;

        int callCount = 0;
        object eventSender = null;
        EventArgs eventArgs = null;
        EventHandler handler = (sender, e) =>
        {
            callCount++;
            eventSender = sender;
            eventArgs = e;
        };
        form.FormRevealModeChanged += handler;

        // Changing the effective value raises the event.
        form.FormRevealMode = FormRevealMode.Deferred;
        Assert.Equal(1, callCount);
        Assert.Same(form, eventSender);
        Assert.Same(EventArgs.Empty, eventArgs);

        // Setting the same effective value again does not raise the event.
        form.FormRevealMode = FormRevealMode.Deferred;
        Assert.Equal(1, callCount);

        // After removing the handler no further notifications are raised.
        form.FormRevealModeChanged -= handler;
        form.FormRevealMode = FormRevealMode.Classic;
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void Form_FormRevealMode_Deferred_ShowDoesNotRemainCloaked()
    {
        FormRevealMode originalDefault = Application.DefaultFormRevealMode;

        try
        {
            Application.SetDefaultFormRevealMode(FormRevealMode.Deferred);

            using SubForm form = new();
            Assert.Equal(FormRevealMode.Inherit, form.FormRevealMode);
            Assert.Equal(
                FormRevealMode.Deferred,
                (FormRevealMode)form.TestAccessor.Dynamic.EffectiveFormRevealMode);

            form.Show();

            // A deferred form is cloaked while it paints its first frame and is revealed from the
            // posted OnShown callback; pump the message queue so that reveal runs. Regardless of
            // whether the DWM cloak actually engaged in this environment, the form must never be
            // left cloaked once shown.
            Application.DoEvents();

            Assert.True(form.Visible);
            Assert.False(form.DeferredAppearanceCloaked);
        }
        finally
        {
            Application.SetDefaultFormRevealMode(originalDefault);
        }
    }
#endif
}
