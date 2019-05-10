// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Security;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class BindingTests
    {
        public static IEnumerable<object[]> Ctor_String_Object_String_TestData()
        {
            yield return new object[] { null, null, null };
            yield return new object[] { string.Empty, new object(), string.Empty };
            yield return new object[] { "propertyName", new object(), "dataMember" };
            yield return new object[] { "propertyName", new object(), "dataMember.subDataMember" };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Object_String_TestData))]
        public void Binding_Ctor_String_Object_String(string propertyName, object dataSource, string dataMember)
        {
            var binding = new Binding(propertyName, dataSource, dataMember);
            Assert.Null(binding.BindableComponent);
            Assert.Null(binding.BindingManagerBase);
            Assert.Equal(new BindingMemberInfo(dataMember), binding.BindingMemberInfo);
            Assert.Null(binding.Control);
            Assert.Equal(ControlUpdateMode.OnPropertyChanged, binding.ControlUpdateMode);
            Assert.Same(dataSource, binding.DataSource);
            Assert.Same(DBNull.Value, binding.DataSourceNullValue);
            Assert.Equal(DataSourceUpdateMode.OnValidation, binding.DataSourceUpdateMode);
            Assert.Null(binding.FormatInfo);
            Assert.Empty(binding.FormatString);
            Assert.False(binding.FormattingEnabled);
            Assert.False(binding.IsBinding);
            Assert.Null(binding.NullValue);
            Assert.Same(propertyName, binding.PropertyName);
        }

        public static IEnumerable<object[]> Ctor_String_Object_String_Bool_TestData()
        {
            yield return new object[] { null, null, null, true };
            yield return new object[] { string.Empty, new object(), string.Empty, true };
            yield return new object[] { "propertyName", new object(), "dataMember", false };
            yield return new object[] { "propertyName", new object(), "dataMember.subDataMember", false };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Object_String_Bool_TestData))]
        public void Binding_Ctor_String_Object_String_Bool(string propertyName, object dataSource, string dataMember, bool formattingEnabled)
        {
            var binding = new Binding(propertyName, dataSource, dataMember, formattingEnabled);
            Assert.Null(binding.BindableComponent);
            Assert.Null(binding.BindingManagerBase);
            Assert.Equal(new BindingMemberInfo(dataMember), binding.BindingMemberInfo);
            Assert.Null(binding.Control);
            Assert.Equal(ControlUpdateMode.OnPropertyChanged, binding.ControlUpdateMode);
            Assert.Same(dataSource, binding.DataSource);
            Assert.Same(DBNull.Value, binding.DataSourceNullValue);
            Assert.Equal(DataSourceUpdateMode.OnValidation, binding.DataSourceUpdateMode);
            Assert.Null(binding.FormatInfo);
            Assert.Empty(binding.FormatString);
            Assert.Equal(formattingEnabled, binding.FormattingEnabled);
            Assert.False(binding.IsBinding);
            Assert.Null(binding.NullValue);
            Assert.Same(propertyName, binding.PropertyName);
        }

        public static IEnumerable<object[]> Ctor_String_Object_String_Bool_DataSourceUpdateMode_TestData()
        {
            yield return new object[] { null, null, null, true, DataSourceUpdateMode.OnValidation };
            yield return new object[] { string.Empty, new object(), string.Empty, true, DataSourceUpdateMode.OnValidation -1 };
            yield return new object[] { "propertyName", new object(), "dataMember", false, DataSourceUpdateMode.Never };
            yield return new object[] { "propertyName", new object(), "dataMember.subDataMember", false, DataSourceUpdateMode.Never + 1 };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Object_String_Bool_DataSourceUpdateMode_TestData))]
        public void Binding_Ctor_String_Object_String_Bool_DataSourceUpdateMode(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode)
        {
            var binding = new Binding(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode);
            Assert.Null(binding.BindableComponent);
            Assert.Null(binding.BindingManagerBase);
            Assert.Equal(new BindingMemberInfo(dataMember), binding.BindingMemberInfo);
            Assert.Null(binding.Control);
            Assert.Equal(ControlUpdateMode.OnPropertyChanged, binding.ControlUpdateMode);
            Assert.Same(dataSource, binding.DataSource);
            Assert.Same(DBNull.Value, binding.DataSourceNullValue);
            Assert.Equal(dataSourceUpdateMode, binding.DataSourceUpdateMode);
            Assert.Null(binding.FormatInfo);
            Assert.Empty(binding.FormatString);
            Assert.Equal(formattingEnabled, binding.FormattingEnabled);
            Assert.False(binding.IsBinding);
            Assert.Null(binding.NullValue);
            Assert.Same(propertyName, binding.PropertyName);
        }

        public static IEnumerable<object[]> Ctor_String_Object_String_Bool_DataSourceUpdateMode_Object_TestData()
        {
            yield return new object[] { null, null, null, true, DataSourceUpdateMode.OnValidation, null };
            yield return new object[] { string.Empty, new object(), string.Empty, true, DataSourceUpdateMode.OnValidation -1, DBNull.Value };
            yield return new object[] { "propertyName", new object(), "dataMember", false, DataSourceUpdateMode.Never, new object() };
            yield return new object[] { "propertyName", new object(), "dataMember.subDataMember", false, DataSourceUpdateMode.Never + 1, new object() };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Object_String_Bool_DataSourceUpdateMode_Object_TestData))]
        public void Binding_Ctor_String_Object_String_Bool_DataSourceUpdateMode_Object(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue)
        {
            var binding = new Binding(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue);
            Assert.Null(binding.BindableComponent);
            Assert.Null(binding.BindingManagerBase);
            Assert.Equal(new BindingMemberInfo(dataMember), binding.BindingMemberInfo);
            Assert.Null(binding.Control);
            Assert.Equal(ControlUpdateMode.OnPropertyChanged, binding.ControlUpdateMode);
            Assert.Same(dataSource, binding.DataSource);
            Assert.Same(DBNull.Value, binding.DataSourceNullValue);
            Assert.Equal(dataSourceUpdateMode, binding.DataSourceUpdateMode);
            Assert.Null(binding.FormatInfo);
            Assert.Empty(binding.FormatString);
            Assert.Equal(formattingEnabled, binding.FormattingEnabled);
            Assert.False(binding.IsBinding);
            Assert.Same(nullValue, binding.NullValue);
            Assert.Same(propertyName, binding.PropertyName);
        }

        public static IEnumerable<object[]> Ctor_String_Object_String_Bool_DataSourceUpdateMode_Object_String_TestData()
        {
            yield return new object[] { null, null, null, true, DataSourceUpdateMode.OnValidation, null, null };
            yield return new object[] { string.Empty, new object(), string.Empty, true, DataSourceUpdateMode.OnValidation -1, DBNull.Value, string.Empty };
            yield return new object[] { "propertyName", new object(), "dataMember", false, DataSourceUpdateMode.Never, new object(), "formatString" };
            yield return new object[] { "propertyName", new object(), "dataMember.subDataMember", false, DataSourceUpdateMode.Never + 1, new object(), "formatString" };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Object_String_Bool_DataSourceUpdateMode_Object_String_TestData))]
        public void Binding_Ctor_String_Object_String_Bool_DataSourceUpdateMode_Object_String(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString)
        {
            var binding = new Binding(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue, formatString);
            Assert.Null(binding.BindableComponent);
            Assert.Null(binding.BindingManagerBase);
            Assert.Equal(new BindingMemberInfo(dataMember), binding.BindingMemberInfo);
            Assert.Null(binding.Control);
            Assert.Equal(ControlUpdateMode.OnPropertyChanged, binding.ControlUpdateMode);
            Assert.Same(dataSource, binding.DataSource);
            Assert.Same(DBNull.Value, binding.DataSourceNullValue);
            Assert.Equal(dataSourceUpdateMode, binding.DataSourceUpdateMode);
            Assert.Null(binding.FormatInfo);
            Assert.Same(formatString, binding.FormatString);
            Assert.Equal(formattingEnabled, binding.FormattingEnabled);
            Assert.False(binding.IsBinding);
            Assert.Same(nullValue, binding.NullValue);
            Assert.Same(propertyName, binding.PropertyName);
        }

        public static IEnumerable<object[]> Ctor_String_Object_String_Bool_DataSourceUpdateMode_Object_String_IFormatProvider_TestData()
        {
            yield return new object[] { null, null, null, true, DataSourceUpdateMode.OnValidation, null, null, null };
            yield return new object[] { string.Empty, new object(), string.Empty, true, DataSourceUpdateMode.OnValidation -1, DBNull.Value, string.Empty, CultureInfo.CurrentCulture };
            yield return new object[] { "propertyName", new object(), "dataMember", false, DataSourceUpdateMode.Never, new object(), "formatString", CultureInfo.InvariantCulture };
            yield return new object[] { "propertyName", new object(), "dataMember.subDataMember", false, DataSourceUpdateMode.Never + 1, new object(), "formatString", CultureInfo.CurrentCulture };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Object_String_Bool_DataSourceUpdateMode_Object_String_IFormatProvider_TestData))]
        public void Binding_Ctor_String_Object_String_Bool_DataSourceUpdateMode_Object_String_IFormatProvider(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString, IFormatProvider formatInfo)
        {
            var binding = new Binding(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue, formatString, formatInfo);
            Assert.Null(binding.BindableComponent);
            Assert.Null(binding.BindingManagerBase);
            Assert.Equal(new BindingMemberInfo(dataMember), binding.BindingMemberInfo);
            Assert.Null(binding.Control);
            Assert.Equal(ControlUpdateMode.OnPropertyChanged, binding.ControlUpdateMode);
            Assert.Same(dataSource, binding.DataSource);
            Assert.Same(DBNull.Value, binding.DataSourceNullValue);
            Assert.Equal(dataSourceUpdateMode, binding.DataSourceUpdateMode);
            Assert.Equal(formatInfo, binding.FormatInfo);
            Assert.Same(formatString, binding.FormatString);
            Assert.Equal(formattingEnabled, binding.FormattingEnabled);
            Assert.False(binding.IsBinding);
            Assert.Same(nullValue, binding.NullValue);
            Assert.Same(propertyName, binding.PropertyName);
        }

        public static IEnumerable<object[]> DataSourceNullValue_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { DBNull.Value };
        }

        [Theory]
        [MemberData(nameof(DataSourceNullValue_Set_TestData))]
        public void Binding_DataSourceNullValue_Set_GetReturnsExpected(object value)
        {
            var binding = new Binding("propertyName", new object(), "dataMember")
            {
                DataSourceNullValue = value
            };
            Assert.Equal(value, binding.DataSourceNullValue);

            // Set same.
            binding.DataSourceNullValue = value;
            Assert.Equal(value, binding.DataSourceNullValue);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ControlUpdateMode))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ControlUpdateMode))]
        public void Binding_ControlUpdateMode_Set_GetReturnsExpected(ControlUpdateMode value)
        {
            var binding = new Binding("propertyName", new object(), "dataMember")
            {
                ControlUpdateMode = value
            };
            Assert.Equal(value, binding.ControlUpdateMode);

            // Set same.
            binding.ControlUpdateMode = value;
            Assert.Equal(value, binding.ControlUpdateMode);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataSourceUpdateMode))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DataSourceUpdateMode))]
        public void Binding_DataSourceUpdateMode_Set_GetReturnsExpected(DataSourceUpdateMode value)
        {
            var binding = new Binding("propertyName", new object(), "dataMember")
            {
                DataSourceUpdateMode = value
            };
            Assert.Equal(value, binding.DataSourceUpdateMode);

            // Set same.
            binding.DataSourceUpdateMode = value;
            Assert.Equal(value, binding.DataSourceUpdateMode);
        }

        public static IEnumerable<object[]> FormatInfo_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { CultureInfo.CurrentCulture };
        }

        [Theory]
        [MemberData(nameof(FormatInfo_Set_TestData))]
        public void Binding_FormatInfo_Set_GetReturnsExpected(IFormatProvider value)
        {
            var binding = new Binding("propertyName", new object(), "dataMember")
            {
                FormatInfo = value
            };
            Assert.Same(value, binding.FormatInfo);

            // Set same.
            binding.FormatInfo = value;
            Assert.Same(value, binding.FormatInfo);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Binding_FormatString_Set_GetReturnsExpected(string value, string expected)
        {
            var binding = new Binding("propertyName", new object(), "dataMember")
            {
                FormatString = value
            };
            Assert.Same(expected, binding.FormatString);

            // Set same.
            binding.FormatString = value;
            Assert.Same(expected, binding.FormatString);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Binding_FormattingEnabled_Set_GetReturnsExpected(bool value)
        {
            var binding = new Binding("propertyName", new object(), "dataMember")
            {
                FormattingEnabled = value
            };
            Assert.Equal(value, binding.FormattingEnabled);

            // Set same.
            binding.FormattingEnabled = value;
            Assert.Equal(value, binding.FormattingEnabled);

            // Set different.
            binding.FormattingEnabled = !value;
            Assert.Equal(!value, binding.FormattingEnabled);
        }

        public static IEnumerable<object[]> NullValue_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { DBNull.Value };
        }

        [Theory]
        [MemberData(nameof(NullValue_Set_TestData))]
        public void Binding_NullValue_Set_GetReturnsExpected(object value)
        {
            var binding = new Binding("propertyName", new object(), "dataMember")
            {
                NullValue = value
            };
            Assert.Same(value, binding.NullValue);

            // Set same.
            binding.NullValue = value;
            Assert.Same(value, binding.NullValue);
        }

        public static IEnumerable<object[]> BindingCompleteEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new BindingCompleteEventArgs(null, BindingCompleteState.Success, BindingCompleteContext.ControlUpdate) };
        }

        [Theory]
        [MemberData(nameof(BindingCompleteEventArgs_TestData))]
        public void Binding_OnBindingComplete_Invoke_CallsBindingComplete(BindingCompleteEventArgs eventArgs)
        {
            var binding = new SubBinding("propertyName", new object(), "dataMember");

            // No handler.
            binding.OnBindingComplete(eventArgs);

            // Handler.
            int callCount = 0;
            BindingCompleteEventHandler handler = (sender, e) =>
            {
                Assert.Equal(binding, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            binding.BindingComplete += handler;
            binding.OnBindingComplete(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            binding.BindingComplete -= handler;
            binding.OnBindingComplete(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [MemberData(nameof(BindingCompleteEventArgs_TestData))]
        public void Binding_OnBindingComplete_InvokeInsideBindingComplete_DoesNotCallBindingComplete(BindingCompleteEventArgs eventArgs)
        {
            var binding = new SubBinding("propertyName", new object(), "dataMember");

            int callCount = 0;
            BindingCompleteEventHandler handler = (sender, e) =>
            {
                Assert.Equal(binding, sender);
                Assert.Same(eventArgs, e);
                callCount++;

                binding.OnBindingComplete(e);
            };

            binding.BindingComplete += handler;
            binding.OnBindingComplete(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnBindingComplete_CriticalException_TestData()
        {
            yield return new object[] { null, new NullReferenceException() };
            yield return new object[] { new BindingCompleteEventArgs(null, BindingCompleteState.Success, BindingCompleteContext.ControlUpdate), new SecurityException() };
        }

        [Theory]
        [MemberData(nameof(OnBindingComplete_CriticalException_TestData))]
        public void Binding_OnBindingComplete_ThrowsCriticalException_Rethrows(BindingCompleteEventArgs eventArgs, Exception exception)
        {
            var binding = new SubBinding("propertyName", new object(), "dataMember");

            int callCount = 0;
            BindingCompleteEventHandler handler = (sender, e) =>
            {
                Assert.Equal(binding, sender);
                Assert.Same(eventArgs, e);
                callCount++;

                throw exception;
            };

            binding.BindingComplete += handler;
            Assert.Throws(exception.GetType(), () => binding.OnBindingComplete(eventArgs));
            if (eventArgs != null)
            {
                Assert.False(eventArgs.Cancel);
            }
            Assert.Equal(1, callCount);
        }

        [Theory]
        [MemberData(nameof(BindingCompleteEventArgs_TestData))]
        public void Binding_OnBindingComplete_ThrowsNonCriticalException_SetsCancelToTrue(BindingCompleteEventArgs eventArgs)
        {
            var binding = new SubBinding("propertyName", new object(), "dataMember");

            int callCount = 0;
            BindingCompleteEventHandler handler = (sender, e) =>
            {
                Assert.Equal(binding, sender);
                Assert.Same(eventArgs, e);
                callCount++;

                throw new DivideByZeroException();
            };

            binding.BindingComplete += handler;
            binding.OnBindingComplete(eventArgs);
            if (eventArgs != null)
            {
                Assert.True(eventArgs.Cancel);
            }
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> ConvertEventArgs_TestData()
        {
            yield return new object[] { false, null, null };
            yield return new object[] { false, new ConvertEventArgs(null, typeof(int)), null };
            yield return new object[] { false, new ConvertEventArgs("1", null), "1" };
            yield return new object[] { false, new ConvertEventArgs("1", typeof(string)), "1" };
            yield return new object[] { false, new ConvertEventArgs("1", typeof(int)), 1 };
            yield return new object[] { false, new ConvertEventArgs(1.1.ToString(CultureInfo.CurrentCulture), typeof(double)), 1.1 };
            yield return new object[] { false, new ConvertEventArgs(DBNull.Value, typeof(int)), DBNull.Value };

            var o = new object();
            yield return new object[] { false, new ConvertEventArgs(o, typeof(object)), o };
            yield return new object[] { false, new ConvertEventArgs(o, typeof(int)), o };

            yield return new object[] { true, null, null };
            yield return new object[] { true, new ConvertEventArgs("1", typeof(int)), "1" };
        }

        [Theory]
        [MemberData(nameof(ConvertEventArgs_TestData))]
        public void Binding_OnFormat_Invoke_CallsFormat(bool formattingEnabled, ConvertEventArgs eventArgs, object expectedValue)
        {
            var binding = new SubBinding("propertyName", new object(), "dataMember")
            {
                FormattingEnabled = formattingEnabled
            };

            // No handler.
            object oldValue = eventArgs?.Value;
            binding.OnFormat(eventArgs);
            Assert.Equal(expectedValue, eventArgs?.Value);

            // Handler.
            int callCount = 0;
            ConvertEventHandler handler = (sender, e) =>
            {
                Assert.Equal(binding, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            binding.Format += handler;
            if (eventArgs != null)
            {
                eventArgs.Value = oldValue;
            }
            binding.OnFormat(eventArgs);
            Assert.Equal(expectedValue, eventArgs?.Value);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            binding.Format -= handler;
            if (eventArgs != null)
            {
                eventArgs.Value = oldValue;
            }
            binding.OnFormat(eventArgs);
            Assert.Equal(expectedValue, eventArgs?.Value);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [MemberData(nameof(ConvertEventArgs_TestData))]
        public void Binding_OnParse_Invoke_CallsParse(bool formattingEnabled, ConvertEventArgs eventArgs, object expectedValue)
        {
            var binding = new SubBinding("propertyName", new object(), "dataMember")
            {
                FormattingEnabled = formattingEnabled
            };

            // No handler.
            object oldValue = eventArgs?.Value;
            binding.OnParse(eventArgs);
            Assert.Equal(expectedValue, eventArgs?.Value);

            // Handler.
            int callCount = 0;
            ConvertEventHandler handler = (sender, e) =>
            {
                Assert.Equal(binding, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            binding.Parse += handler;
            if (eventArgs != null)
            {
                eventArgs.Value = oldValue;
            }
            binding.OnParse(eventArgs);
            Assert.Equal(expectedValue, eventArgs?.Value);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            binding.Parse -= handler;
            if (eventArgs != null)
            {
                eventArgs.Value = oldValue;
            }
            binding.OnParse(eventArgs);
            Assert.Equal(expectedValue, eventArgs?.Value);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> ReadValue_TestData()
        {
            foreach (ControlUpdateMode controlUpdateMode in Enum.GetValues(typeof(ControlUpdateMode)))
            {
                yield return new object[] { controlUpdateMode, true, 1 };
                yield return new object[] { controlUpdateMode, false, 0 };
            }
        }

        [Theory]
        [MemberData(nameof(ReadValue_TestData))]
        public void Binding_ReadValue_Invoke_CallsBindingCompleteIfFormattingEnabled(ControlUpdateMode controlUpdateMode, bool formattingEnabled, int expectedCallCount)
        {
            var binding = new Binding("propertyName", new object(), "dataMember")
            {
                ControlUpdateMode = controlUpdateMode,
                FormattingEnabled = formattingEnabled
            };

            // No handler.
            binding.ReadValue();

            // Handler.
            int callCount = 0;
            BindingCompleteEventHandler handler = (sender, e) =>
            {
                Assert.Same(binding, sender);
                Assert.Same(binding, e.Binding);
                Assert.Equal(BindingCompleteContext.ControlUpdate, e.BindingCompleteContext);
                Assert.Equal(BindingCompleteState.Success, e.BindingCompleteState);
                Assert.False(e.Cancel);
                Assert.Empty(e.ErrorText);
                Assert.Null(e.Exception);
                callCount++;
            };

            binding.BindingComplete += handler;
            binding.ReadValue();
            Assert.Equal(expectedCallCount, callCount);

            // Should not call if the handler is removed.
            binding.BindingComplete -= handler;
            binding.ReadValue();
            Assert.Equal(expectedCallCount, callCount);
        }

        public static IEnumerable<object[]> WriteValue_TestData()
        {
            foreach (ControlUpdateMode controlUpdateMode in Enum.GetValues(typeof(ControlUpdateMode)))
            {
                yield return new object[] { controlUpdateMode, true };
                yield return new object[] { controlUpdateMode, false };
            }
        }

        [Theory]
        [MemberData(nameof(WriteValue_TestData))]
        public void Binding_WriteValue_Invoke_DoesNotCallBindingComplete(ControlUpdateMode controlUpdateMode, bool formattingEnabled)
        {
            var binding = new Binding("propertyName", new object(), "dataMember")
            {
                ControlUpdateMode = controlUpdateMode,
                FormattingEnabled = formattingEnabled
            };

            // No handler.
            binding.WriteValue();

            // Handler.
            int callCount = 0;
            BindingCompleteEventHandler handler = (sender, e) => callCount++;

            binding.BindingComplete += handler;
            binding.WriteValue();
            Assert.Equal(0, callCount);

            // Should not call if the handler is removed.
            binding.BindingComplete -= handler;
            binding.WriteValue();
            Assert.Equal(0, callCount);
        }

        private class SubBinding : Binding
        {
            public SubBinding(string propertyName, object dataSource, string dataMember) : base(propertyName, dataSource, dataMember)
            {
            }

            public new void OnBindingComplete(BindingCompleteEventArgs e) => base.OnBindingComplete(e);

            public new void OnFormat(ConvertEventArgs cevent) => base.OnFormat(cevent);

            public new void OnParse(ConvertEventArgs cevent) => base.OnParse(cevent);
        }
    }
}
