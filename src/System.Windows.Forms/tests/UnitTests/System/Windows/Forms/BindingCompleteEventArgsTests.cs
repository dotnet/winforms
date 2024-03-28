// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class BindingCompleteEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Binding_BindingCompleteState_BindingCompleteContext_TestData()
    {
        yield return new object[] { new Binding(null, new object(), "member"), BindingCompleteState.DataError, BindingCompleteContext.DataSourceUpdate };
        yield return new object[] { null, BindingCompleteState.Success - 1, BindingCompleteContext.ControlUpdate - 1 };
    }

    [Theory]
    [MemberData(nameof(Ctor_Binding_BindingCompleteState_BindingCompleteContext_TestData))]
    public void Ctor_Binding_BindingCompleteState_BindingCompleteContext(Binding binding, BindingCompleteState state, BindingCompleteContext context)
    {
        BindingCompleteEventArgs e = new(binding, state, context);
        Assert.Equal(binding, e.Binding);
        Assert.Equal(state, e.BindingCompleteState);
        Assert.Equal(context, e.BindingCompleteContext);
        Assert.Empty(e.ErrorText);
        Assert.Null(e.Exception);
        Assert.False(e.Cancel);
    }

    public static IEnumerable<object[]> Ctor_Binding_BindingCompleteState_BindingCompleteContext_String_TestData()
    {
        yield return new object[] { new Binding(null, new object(), "member"), BindingCompleteState.DataError, BindingCompleteContext.DataSourceUpdate, "errorText" };
        yield return new object[] { null, BindingCompleteState.Success - 1, BindingCompleteContext.ControlUpdate - 1, null };
    }

    [Theory]
    [MemberData(nameof(Ctor_Binding_BindingCompleteState_BindingCompleteContext_String_TestData))]
    public void Ctor_Binding_BindingCompleteState_BindingCompleteContext_String(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText)
    {
        BindingCompleteEventArgs e = new(binding, state, context, errorText);
        Assert.Equal(binding, e.Binding);
        Assert.Equal(state, e.BindingCompleteState);
        Assert.Equal(context, e.BindingCompleteContext);
        Assert.Equal(errorText ?? "", e.ErrorText);
        Assert.Null(e.Exception);
        Assert.True(e.Cancel);
    }

    public static IEnumerable<object[]> Ctor_Binding_BindingCompleteState_BindingCompleteContext_String_Exception_TestData()
    {
        yield return new object[] { new Binding(null, new object(), "member"), BindingCompleteState.DataError, BindingCompleteContext.DataSourceUpdate, "errorText", new InvalidOperationException() };
        yield return new object[] { null, BindingCompleteState.Success - 1, BindingCompleteContext.ControlUpdate - 1, null, null };
    }

    [Theory]
    [MemberData(nameof(Ctor_Binding_BindingCompleteState_BindingCompleteContext_String_Exception_TestData))]
    public void Ctor_Binding_BindingCompleteState_BindingCompleteContext_String_Exception(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception)
    {
        BindingCompleteEventArgs e = new(binding, state, context, errorText, exception);
        Assert.Equal(binding, e.Binding);
        Assert.Equal(state, e.BindingCompleteState);
        Assert.Equal(context, e.BindingCompleteContext);
        Assert.Equal(errorText ?? "", e.ErrorText);
        Assert.Equal(exception, e.Exception);
        Assert.True(e.Cancel);
    }

    public static IEnumerable<object[]> Ctor_Binding_BindingCompleteState_BindingCompleteContext_String_Exception_Bool_TestData()
    {
        yield return new object[] { new Binding(null, new object(), "member"), BindingCompleteState.DataError, BindingCompleteContext.DataSourceUpdate, "errorText", new InvalidOperationException(), true };
        yield return new object[] { null, BindingCompleteState.Success - 1, BindingCompleteContext.ControlUpdate - 1, null, null, false };
    }

    [Theory]
    [MemberData(nameof(Ctor_Binding_BindingCompleteState_BindingCompleteContext_String_Exception_Bool_TestData))]
    public void Ctor_Binding_BindingCompleteState_BindingCompleteContext_String_Exception_Bool(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception, bool cancel)
    {
        BindingCompleteEventArgs e = new(binding, state, context, errorText, exception, cancel);
        Assert.Equal(binding, e.Binding);
        Assert.Equal(state, e.BindingCompleteState);
        Assert.Equal(context, e.BindingCompleteContext);
        Assert.Equal(errorText ?? "", e.ErrorText);
        Assert.Equal(exception, e.Exception);
        Assert.Equal(cancel, e.Cancel);
    }
}
