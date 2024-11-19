// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DragEventArgsTests
{
    public static IEnumerable<object[]> Ctor_IDataObject_Int_Int_Int_DragDropEffects_DragDropEffects_TestData()
    {
        yield return new object[] { null, -1, -2, -3, DragDropEffects.None - 1, DragDropEffects.None - 1 };
        yield return new object[] { new CustomDataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Move };
    }

    public static IEnumerable<object[]> Ctor_IDataObject_Int_Int_Int_DragDropEffects_DragDropEffects_DropImageType_string_string_TestData()
    {
        yield return new object[] { null, -1, -2, -3, DragDropEffects.None - 1, DragDropEffects.None - 1, (DropImageType.Invalid - 1), null, null };
        yield return new object[] { new CustomDataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Move, DropImageType.Copy, "Move to %1", "Documents" };
    }

    [Theory]
    [MemberData(nameof(Ctor_IDataObject_Int_Int_Int_DragDropEffects_DragDropEffects_TestData))]
    public void Ctor_IDataObject_Int_Int_Int_DragDropEffects_DragDropEffects(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect)
    {
        DragEventArgs e = new(data, keyState, x, y, allowedEffect, effect);
        Assert.Equal(data, e.Data);
        Assert.Equal(keyState, e.KeyState);
        Assert.Equal(x, e.X);
        Assert.Equal(y, e.Y);
        Assert.Equal(allowedEffect, e.AllowedEffect);
        Assert.Equal(effect, e.Effect);
    }

    [Theory]
    [MemberData(nameof(Ctor_IDataObject_Int_Int_Int_DragDropEffects_DragDropEffects_DropImageType_string_string_TestData))]
    public void Ctor_IDataObject_Int_Int_Int_DragDropEffects_DragDropEffects_DropImageType_string_string(IDataObject data, int keyState, int x,
        int y, DragDropEffects allowedEffect, DragDropEffects effect, DropImageType dropImageType, string message, string messageReplacementToken)
    {
        DragEventArgs e = new(data, keyState, x, y, allowedEffect, effect, dropImageType, message, messageReplacementToken);
        Assert.Equal(data, e.Data);
        Assert.Equal(keyState, e.KeyState);
        Assert.Equal(x, e.X);
        Assert.Equal(y, e.Y);
        Assert.Equal(allowedEffect, e.AllowedEffect);
        Assert.Equal(effect, e.Effect);
        Assert.Equal(dropImageType, e.DropImageType);
        Assert.Equal(message, e.Message);
        Assert.Equal(messageReplacementToken, e.MessageReplacementToken);
    }

    [Theory]
    [InlineData(DragDropEffects.Copy)]
    [InlineData((DragDropEffects.None - 1))]
    public void Effect_Set_GetReturnsExpected(DragDropEffects value)
    {
        DragEventArgs e = new(new CustomDataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Move)
        {
            Effect = value
        };
        Assert.Equal(value, e.Effect);
    }

    [Theory]
    [InlineData(DropImageType.Copy)]
    [InlineData(DropImageType.Invalid - 1)]
    public void DropImageType_Set_GetReturnsExpected(DropImageType value)
    {
        DragEventArgs e = new(new CustomDataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Move, DropImageType.Copy, "Copy to %1", "Documents")
        {
            DropImageType = value
        };
        Assert.Equal(value, e.DropImageType);
    }

    [Theory]
    [InlineData("Copy to %1")]
    [InlineData(null)]
    public void Message_Set_GetReturnsExpected(string value)
    {
        DragEventArgs e = new(new CustomDataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Move, DropImageType.Copy, "Move to %1", "Documents")
        {
            Message = value
        };
        Assert.Equal(value, e.Message);
    }

    [Theory]
    [InlineData("Documents")]
    [InlineData(null)]
    public void MessageReplacementToken_Set_GetReturnsExpected(string value)
    {
        DragEventArgs e = new(new CustomDataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Move, DropImageType.Copy, "Move to %1", "Desktop")
        {
            MessageReplacementToken = value
        };
        Assert.Equal(value, e.MessageReplacementToken);
    }

    private class CustomDataObject : IDataObject
    {
        public object GetData(string format, bool autoConvert) => throw new NotImplementedException();
        public object GetData(string format) => throw new NotImplementedException();
        public object GetData(Type format) => throw new NotImplementedException();
        public void SetData(string format, bool autoConvert, object data) => throw new NotImplementedException();
        public void SetData(string format, object data) => throw new NotImplementedException();
        public void SetData(Type format, object data) => throw new NotImplementedException();
        public void SetData(object data) => throw new NotImplementedException();
        public bool GetDataPresent(string format, bool autoConvert) => throw new NotImplementedException();
        public bool GetDataPresent(string format) => throw new NotImplementedException();
        public bool GetDataPresent(Type format) => throw new NotImplementedException();
        public string[] GetFormats(bool autoConvert) => throw new NotImplementedException();
        public string[] GetFormats() => throw new NotImplementedException();
    }
}
