// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DragEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_IDataObject_Int_Int_Int_DragDropEffects_DragDropEffects_TestData()
        {
            yield return new object[] { null, -1, -2, -3, (DragDropEffects)(DragDropEffects.None - 1), (DragDropEffects)(DragDropEffects.None - 1) };
            yield return new object[] { new CustomDataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Move };
        }

        [Theory]
        [MemberData(nameof(Ctor_IDataObject_Int_Int_Int_DragDropEffects_DragDropEffects_TestData))]
        public void Ctor_IDataObject_Int_Int_Int_DragDropEffects_DragDropEffects(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect)
        {
            var e = new DragEventArgs(data, keyState, x, y, allowedEffect, effect);
            Assert.Equal(data, e.Data);
            Assert.Equal(keyState, e.KeyState);
            Assert.Equal(x, e.X);
            Assert.Equal(y, e.Y);
            Assert.Equal(allowedEffect, e.AllowedEffect);
            Assert.Equal(effect, e.Effect);
        }

        [Theory]
        [InlineData(DragDropEffects.Copy)]
        [InlineData((DragDropEffects)(DragDropEffects.None - 1))]
        public void Effect_Set_GetReturnsExpected(DragDropEffects value)
        {
            var e = new DragEventArgs(new CustomDataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Move)
            {
                Effect = value
            };
            Assert.Equal(value, e.Effect);
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
}
