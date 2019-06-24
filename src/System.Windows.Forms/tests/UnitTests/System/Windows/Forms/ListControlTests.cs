// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListControlTests
    {
        // Commenting out this flaky test. Issue is tracked at https://github.com/dotnet/winforms/issues/1222
        // [Fact]
        // public void Ctor_Default()
        // {
        //     var control = new SubListControl();
        //     Assert.True(control.AllowSelectionEntry);
        //     Assert.Equal(Control.DefaultBackColor, control.BackColor);
        //     Assert.Null(control.BackgroundImage);
        //     Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        //     Assert.Equal(Rectangle.Empty, control.Bounds);
        //     Assert.Equal(Size.Empty, control.ClientSize);
        //     Assert.Equal(Rectangle.Empty, control.ClientRectangle);
        //     Assert.Null(control.DataManager);
        //     Assert.Null(control.DataSource);
        //     Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        //     Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        //     Assert.Equal(Padding.Empty, control.DefaultPadding);
        //     Assert.Equal(Size.Empty, control.DefaultSize);
        //     Assert.Empty(control.DisplayMember);
        //     Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
        //     Assert.Null(control.FormatInfo);
        //     Assert.Empty(control.FormatString);
        //     Assert.False(control.FormattingEnabled);
        //     Assert.Same(Control.DefaultFont, control.Font);
        //     Assert.Equal(SystemColors.ControlText, control.ForeColor);
        //     Assert.Equal(0, control.Height);
        //     Assert.Equal(Point.Empty, control.Location);
        //     Assert.Equal(Size.Empty, control.MaximumSize);
        //     Assert.Equal(Size.Empty, control.MinimumSize);
        //     Assert.Equal(Padding.Empty, control.Padding);
        //     Assert.Equal(Size.Empty, control.PreferredSize);
        //     Assert.Equal(RightToLeft.No, control.RightToLeft);
        //     Assert.Null(control.SelectedValue);
        //     Assert.Equal(Size.Empty, control.Size);
        //     Assert.Empty(control.Text);
        //     Assert.Empty(control.ValueMember);
        //     Assert.Equal(0, control.Width);
        // }

        public static IEnumerable<object[]> BindingContext_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new BindingContext() };
        }

        [Theory]
        [MemberData(nameof(BindingContext_Set_TestData))]
        public void BindingContext_Set_GetReturnsExpected(BindingContext value)
        {
            var control = new SubListControl
            {
                BindingContext = value
            };
            Assert.Same(value, control.BindingContext);
            Assert.Null(control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);

            // Set same.
            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
            Assert.Null(control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [MemberData(nameof(BindingContext_Set_TestData))]
        public void BindingContext_SetWithNonNullBindingContext_GetReturnsExpected(BindingContext value)
        {
            var control = new SubListControl
            {
                BindingContext = new BindingContext()
            };

            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
            Assert.Null(control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);

            // Set same.
            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
            Assert.Null(control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Fact]
        public void BindingContext_SetWithDataSource_GetReturnsExpected()
        {
            var value = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                DataSource = dataSource
            };

            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
            Assert.Same(dataSource, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Same(value[dataSource], control.DataManager);

            // Set same.
            control.BindingContext = value;
            Assert.Same(dataSource, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Same(value[dataSource], control.DataManager);
        }

        [Fact]
        public void BindingContext_SetWithDataSourceAndDisplayMember_GetReturnsExpected()
        {
            var value = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                DataSource = dataSource,
                DisplayMember = "Value"
            };

            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
            Assert.Same(dataSource, control.DataSource);
            Assert.Equal("Value", control.DisplayMember);
            Assert.Same(value[dataSource], control.DataManager);

            // Set same.
            control.BindingContext = value;
            Assert.Same(dataSource, control.DataSource);
            Assert.Equal("Value", control.DisplayMember);
            Assert.Same(value[dataSource], control.DataManager);
        }

        [Fact]
        public void BindingContext_SetWithDataSourceWithBindingContext_GetReturnsExpected()
        {
            var originalValue = new BindingContext();
            var value = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                BindingContext = originalValue,
                DataSource = dataSource
            };

            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
            Assert.Same(dataSource, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Same(value[dataSource], control.DataManager);

            // Set same.
            control.BindingContext = value;
            Assert.Same(dataSource, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Same(value[dataSource], control.DataManager);
        }

        [Fact]
        public void BindingContext_SetWithDataSourceAndDisplayMemberWithBindingContext_GetReturnsExpected()
        {
            var originalValue = new BindingContext();
            var value = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                BindingContext = originalValue,
                DataSource = dataSource,
                DisplayMember = "Value"
            };

            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
            Assert.Same(dataSource, control.DataSource);
            Assert.Equal("Value", control.DisplayMember);
            Assert.Same(value[dataSource], control.DataManager);

            // Set same.
            control.BindingContext = value;
            Assert.Same(dataSource, control.DataSource);
            Assert.Equal("Value", control.DisplayMember);
            Assert.Same(value[dataSource], control.DataManager);
        }

        [Fact]
        public void BindingContext_SetWithHandler_CallsBindingContextChanged()
        {
            var control = new SubListControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BindingContextChanged += handler;

            // Set different.
            var context1 = new BindingContext();
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set same.
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set different.
            var context2 = new BindingContext();
            control.BindingContext = context2;
            Assert.Same(context2, control.BindingContext);
            Assert.Equal(2, callCount);

            // Set null.
            control.BindingContext = null;
            Assert.Null(control.BindingContext);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.BindingContextChanged -= handler;
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void DataManager_ChangePosition_UpdatesSelectionIndex()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass(), new DataClass() };
            var control = new SubListControl
            {
                BindingContext = context,
                DataSource = dataSource
            };

            control.DataManager.Position = 1;
            Assert.Equal(1, control.SelectedIndex);
        }

        [Fact]
        public void DataManager_ChangePositionDoesNotAllowSelection_DoesNotUpdateSelectionIndex()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass(), new DataClass() };
            var control = new AllowSelectionFalseListControl
            {
                SelectedIndex = -2,
                BindingContext = context,
                DataSource = dataSource,
            };

            control.DataManager.Position = 1;
            Assert.Equal(-2, control.SelectedIndex);
        }

        [Fact]
        public void DataManager_SuspendResumeBinding_CallsSetItemsCore()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass(), new DataClass() };
            var control = new SubListControl
            {
                SelectedIndex = -1,
                BindingContext = context,
                DataSource = dataSource
            };
            int callCount = 0;
            control.SetItemsCoreHandler += (items) =>
            {
                Assert.Same(dataSource, items);
                callCount++;
            };

            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(0, control.DataManager.Position);

            // Supsending should call.
            control.DataManager.SuspendBinding();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, callCount);

            // Suspending again should be a nop.
            control.DataManager.SuspendBinding();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, callCount);

            // Resuming should call.
            control.DataManager.ResumeBinding();
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(2, callCount);

            // Resuming again should be a nop.
            control.DataManager.ResumeBinding();
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal(2, callCount);

            // Should not call if the DataSource is changed.
            control.DataSource = new List<DataClass>();
        }

        [Fact]
        public void DataManager_SuspendResumeBindingAfterDataManagerChanged_DoesNotCallSetItemsCore()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass(), new DataClass() };
            var control = new SubListControl
            {
                SelectedIndex = -1,
                BindingContext = context,
                DataSource = dataSource
            };
            int callCount = 0;
            control.SetItemsCoreHandler += (items) =>
            {
                Assert.Same(dataSource, items);
                callCount++;
            };

            CurrencyManager dataManger = control.DataManager;
            dataManger.SuspendBinding();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, callCount);

            // Should not call if the DataSource is changed.
            control.DataSource = new List<DataClass>();
            Assert.NotSame(dataManger, control.DataManager);
            dataManger.ResumeBinding();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void DataManager_SuspendResumeBindingDoesNotAllowSelection_CallsSetItemsCore()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass(), new DataClass() };
            var control = new AllowSelectionFalseListControl
            {
                SelectedIndex = -1,
                BindingContext = context,
                DataSource = dataSource
            };
            int callCount = 0;
            control.SetItemsCoreHandler += (items) =>
            {
                Assert.Same(dataSource, items);
                callCount++;
            };

            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(0, control.DataManager.Position);

            // Supsending should call.
            control.DataManager.SuspendBinding();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, callCount);

            // Suspending again should be a nop.
            control.DataManager.SuspendBinding();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, callCount);

            // Resuming should call.
            control.DataManager.ResumeBinding();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(2, callCount);

            // Resuming again should be a nop.
            control.DataManager.ResumeBinding();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataManager_CancelCurrentEdit_CallsSetItemCore()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass(), new DataClass() };
            var control = new SubListControl
            {
                SelectedIndex = -1,
                BindingContext = context,
                DataSource = dataSource
            };
            int callCount = 0;
            control.SetItemCoreHandler += (index, value) =>
            {
                Assert.Equal(0, index);
                Assert.Same(dataSource[0], value);
                callCount++;
            };

            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(0, control.DataManager.Position);

            control.DataManager.CancelCurrentEdit();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void DataManager_CancelCurrentEditAfterDataManagerChanged_DoesNotCallSetItemCore()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass(), new DataClass() };
            var control = new SubListControl
            {
                SelectedIndex = -1,
                BindingContext = context,
                DataSource = dataSource
            };
            int callCount = 0;
            control.SetItemCoreHandler += (index, value) =>
            {
                Assert.Equal(0, index);
                Assert.Same(dataSource[0], value);
                callCount++;
            };

            CurrencyManager dataManger = control.DataManager;
            dataManger.CancelCurrentEdit();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, callCount);

            // Should not call if the DataSource is changed.
            control.DataSource = new List<DataClass>();
            Assert.NotSame(dataManger, control.DataManager);
            dataManger.CancelCurrentEdit();
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> DataSource_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new List<int>() };
            yield return new object[] { Array.Empty<int>() };

            var mockSource = new Mock<IListSource>(MockBehavior.Strict);
            mockSource
                .Setup(s => s.GetList())
                .Returns(new int[] { 1 });
            yield return new object[] { mockSource.Object };
        }

        [Theory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void DataSource_Set_GetReturnsExpected(object value)
        {
            var control = new SubListControl
            {
                DataSource = value
            };
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);

            // Set same.
            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void DataSource_SetWithDataSourceNoDisplayMember_GetReturnsExpected(object value)
        {
            var control = new SubListControl
            {
                DataSource = new List<int>()
            };

            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);

            // Set same.
            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void DataSource_SetWithDisplayMember_GetReturnsExpected(object value)
        {
            var control = new SubListControl
            {
                DisplayMember = "Capacity"
            };

            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Equal("Capacity", control.DisplayMember);
            Assert.Null(control.DataManager);

            // Set same.
            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Equal("Capacity", control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void DataSource_SetWithDataSourceNoSuchDisplayMemberAnymore_GetReturnsExpected(object value)
        {
            var originalValue = new List<int>();
            var control = new SubListControl
            {
                DataSource = originalValue,
                DisplayMember = "Capacity"
            };

            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Equal(value == null ? string.Empty : "Capacity", control.DisplayMember);
            Assert.Null(control.DataManager);

            // Set same.
            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Equal(value == null ? string.Empty : "Capacity", control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Fact]
        public void DataSource_SetComponent_DisposeValue_Removes()
        {
            var value = new ComponentList();
            var control = new SubListControl
            {
                DataSource = value,
                DisplayMember = "Count"
            };
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);

            value.Dispose();
            Assert.Null(control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Fact]
        public void DataSource_SetComponent_DisposeValueDifferentSender_Removes()
        {
            var value = new ComponentList();
            var control = new SubListControl
            {
                DataSource = value,
                DisplayMember = "Count"
            };
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);

            value.OnDisposed(new object(), EventArgs.Empty);
            Assert.Null(control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Fact]
        public void DataSource_SetComponent_DisposeValueNullSender_Removes()
        {
            var value = new ComponentList();
            var control = new SubListControl
            {
                DataSource = value,
                DisplayMember = "Count"
            };
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);

            value.OnDisposed(null, null);
            Assert.Null(control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Fact]
        public void DataSource_SetOverridenComponent_DisposeValue_DoesNotRemove()
        {
            var originalValue = new ComponentList();
            var value = new List<int>();
            var control = new SubListControl
            {
                DataSource = originalValue,
                DisplayMember = "Count"
            };
            Assert.Same(originalValue, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);

            control.DataSource = value;
            originalValue.Dispose();
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Fact]
        public void DataSource_SetSupportInitializeNotification_InitializeValueNotInitializedSetsInitialized_Success()
        {
            var value = new SupportInitializeNotificationList();
            value.Initialized += (sender, e) =>
            {
                value.IsInitialized = true;
            };

            var control = new SubListControl
            {
                DataSource = value,
                DisplayMember = "Count"
            };
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);

            value.OnInitialized(value, EventArgs.Empty);
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Fact]
        public void DataSource_SetSupportInitializeNotification_InitializeValueNotInitialized_Success()
        {
            var value = new SupportInitializeNotificationList();
            var control = new SubListControl
            {
                DataSource = value,
                DisplayMember = "Count"
            };
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);

            value.OnInitialized(value, EventArgs.Empty);
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Fact]
        public void DataSource_SetSupportInitializeNotification_InitializeValueInitialized_Success()
        {
            var value = new SupportInitializeNotificationList();
            var control = new SubListControl
            {
                DataSource = value,
                DisplayMember = "Count"
            };
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);

            value.OnInitialized(value, EventArgs.Empty);
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Fact]
        public void DataSource_SetSupportInitializeNotification_InitializeValueDifferentSender_Success()
        {
            var value = new SupportInitializeNotificationList();
            var control = new SubListControl
            {
                DataSource = value,
                DisplayMember = "Count"
            };
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);

            value.OnInitialized(new object(), EventArgs.Empty);
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Fact]
        public void DataSource_SetSupportInitializeNotification_InitializeValueNullSender_Succes()
        {
            var value = new SupportInitializeNotificationList();
            var control = new SubListControl
            {
                DataSource = value,
                DisplayMember = "Count"
            };
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);

            value.OnInitialized(null, null);
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataSource_SetOverridenSupportInitializeNotification_InitializeValue_Success(bool isInitialized)
        {
            var originalValue = new SupportInitializeNotificationList();
            var value = new List<int>();
            var control = new SubListControl
            {
                DataSource = originalValue,
                DisplayMember = "Count"
            };
            Assert.Same(originalValue, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);

            originalValue.IsInitialized = isInitialized;
            control.DataSource = value;
            originalValue.OnInitialized(originalValue, EventArgs.Empty);
            Assert.Same(value, control.DataSource);
            Assert.Equal("Count", control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void DataSource_SetWithBindingContext_GetReturnsExpected(object value)
        {
            var context = new BindingContext();
            var control = new SubListControl
            {
                BindingContext = context
            };

            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            if (value == null)
            {
                Assert.Null(control.DataManager);
            }
            else
            {
                Assert.Same(context[value], control.DataManager);
            }

            // Set same.
            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            if (value == null)
            {
                Assert.Null(control.DataManager);
            }
            else
            {
                Assert.Same(context[value], control.DataManager);
            }
        }

        [Theory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void DataSource_SetWithBindingContextWithDataSource_GetReturnsExpected(object value)
        {
            var context = new BindingContext();
            var control = new SubListControl
            {
                BindingContext = context,
                DataSource = new List<int>()
            };

            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            if (value == null)
            {
                Assert.Null(control.DataManager);
            }
            else
            {
                Assert.Same(context[value], control.DataManager);
            }

            // Set same.
            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            if (value == null)
            {
                Assert.Null(control.DataManager);
            }
            else
            {
                Assert.Same(context[value], control.DataManager);
            }
        }

        [Fact]
        public void DataSource_SetWithBindingContextWithDisplayMemberCanCreate_GetReturnsExpected()
        {
            var context = new BindingContext();
            var value = new List<DataClass>();
            var control = new SubListControl
            {
                BindingContext = context,
                DisplayMember = "Value"
            };

            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Equal("Value", control.DisplayMember);
            Assert.Same(context[value], control.DataManager);

            // Set same.
            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Equal("Value", control.DisplayMember);
            Assert.Same(context[value], control.DataManager);
        }

        [Theory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void DataSource_SetWithBindingContextWithDisplayMemberCantCreate_GetReturnsExpected(object value)
        {
            var context = new BindingContext();
            var control = new SubListControl
            {
                BindingContext = context,
                DisplayMember = "NoSuchDisplayMember"
            };

            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Equal(value == null ? "NoSuchDisplayMember" : string.Empty, control.DisplayMember);
            if (value == null)
            {
                Assert.Null(control.DataManager);
            }
            else
            {
                Assert.Same(context[value], control.DataManager);
            }

            // Set same.
            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Equal(value == null ? "NoSuchDisplayMember" : string.Empty, control.DisplayMember);
            if (value == null)
            {
                Assert.Null(control.DataManager);
            }
            else
            {
                Assert.Same(context[value], control.DataManager);
            }
        }

        [Fact]
        public void DataSource_SetWithHandler_CallsDataSourceChanged()
        {
            var control = new SubListControl();
            int dataSourceCallCount = 0;
            int displayMemberCallCount = 0;
            EventHandler dataSourceHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                dataSourceCallCount++;
            };
            EventHandler displayMemberHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                displayMemberCallCount++;
            };
            control.DataSourceChanged += dataSourceHandler;
            control.DisplayMemberChanged += displayMemberHandler;

            // Set different.
            var dataSource1 = new List<int>();
            control.DataSource = dataSource1;
            Assert.Same(dataSource1, control.DataSource);
            Assert.Equal(1, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set same.
            control.DataSource = dataSource1;
            Assert.Same(dataSource1, control.DataSource);
            Assert.Equal(1, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set different.
            var dataSource2 = new List<int>();
            control.DataSource = dataSource2;
            Assert.Same(dataSource2, control.DataSource);
            Assert.Equal(2, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set null.
            control.DataSource = null;
            Assert.Null(control.DataSource);
            Assert.Equal(3, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Remove handler.
            control.DataSourceChanged -= dataSourceHandler;
            control.DisplayMemberChanged -= displayMemberHandler;
            control.DataSource = dataSource1;
            Assert.Same(dataSource1, control.DataSource);
            Assert.Equal(3, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);
        }

        [Fact]
        public void DataSource_SetInsideDataSourceChanged_Nop()
        {
            var control = new SubListControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                control.DataSource = Array.Empty<int>();
                callCount++;
            };
            control.DataSourceChanged += handler;

            var value = new List<int>();
            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void DataSource_SetInvalid_ThrowsArgumentException()
        {
            var control = new SubListControl();
            Assert.Throws<ArgumentException>("value", () => control.DataSource = new object());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DisplayMember_Set_GetReturnsExpected(string value, string expected)
        {
            var control = new SubListControl
            {
                DisplayMember = value
            };
            Assert.Null(control.DataSource);
            Assert.Equal(expected, control.DisplayMember);
            Assert.Null(control.DataManager);

            // Set same.
            control.DisplayMember = value;
            Assert.Null(control.DataSource);
            Assert.Equal(expected, control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataMember_SetWithDisplayMember_GetReturnsExpected(string value, string expected)
        {
            var control = new SubListControl
            {
                DisplayMember = "DataMember"
            };

            control.DisplayMember = value;
            Assert.Equal(expected, control.DisplayMember);
            Assert.Null(control.DataSource);
            Assert.Null(control.DataManager);

            // Set same.
            control.DisplayMember = value;
            Assert.Equal(expected, control.DisplayMember);
            Assert.Null(control.DataSource);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void DisplayMember_SetWithDataSource_GetReturnsExpected(string value)
        {
            var dataSource = new List<int>();
            var control = new SubListControl
            {
                DataSource = dataSource
            };

            control.DisplayMember = value;
            Assert.Equal(value, control.DisplayMember);
            Assert.Same(dataSource, control.DataSource);
            Assert.Null(control.DataManager);

            // Set same.
            control.DisplayMember = value;
            Assert.Equal(value, control.DisplayMember);
            Assert.Same(dataSource, control.DataSource);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DisplayMember_SetWithBindingContext_GetReturnsExpected(string value, string expected)
        {
            var context = new BindingContext();
            var control = new SubListControl
            {
                BindingContext = context
            };

            control.DisplayMember = value;
            Assert.Equal(expected, control.DisplayMember);
            Assert.Null(control.DataSource);
            Assert.Null(control.DataManager);

            // Set same.
            control.DisplayMember = value;
            Assert.Equal(expected, control.DisplayMember);
            Assert.Null(control.DataSource);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void DisplayMember_SetWithBindingContextWithDataSource_GetReturnsExpected(string value)
        {
            var context = new BindingContext();
            var dataSource = new List<int>();
            var control = new SubListControl
            {
                BindingContext = context,
                DataSource = dataSource
            };

            control.DisplayMember = value;
            Assert.Empty(control.DisplayMember);
            Assert.Same(dataSource, control.DataSource);
            Assert.Same(context[dataSource], control.DataManager);

            // Set same.
            control.DisplayMember = value;
            Assert.Empty(control.DisplayMember);
            Assert.Same(dataSource, control.DataSource);
            Assert.Same(context[dataSource], control.DataManager);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("Value", "Value")]
        [InlineData("value", "value")]
        public void DisplayMember_SetWithBindingContextWithDataSourceCanCreate_GetReturnsExpected(string value, string expected)
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                BindingContext = context,
                DataSource = dataSource
            };

            control.DisplayMember = value;
            Assert.Equal(expected, control.DisplayMember);
            Assert.Equal(dataSource, control.DataSource);
            Assert.Same(context[dataSource], control.DataManager);

            // Set same.
            control.DisplayMember = value;
            Assert.Equal(expected, control.DisplayMember);
            Assert.Equal(dataSource, control.DataSource);
            Assert.Same(context[dataSource], control.DataManager);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        [InlineData("ListValue")]
        public void DisplayMember_SetWithBindingContextWithDataSourceCantCreate_GetReturnsExpected(string value)
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                BindingContext = context,
                DataSource = dataSource
            };

            control.DisplayMember = value;
            Assert.Same(dataSource, control.DataSource);
            Assert.Empty(control.DisplayMember);
            if (value == null)
            {
                Assert.Null(control.DataManager);
            }
            else
            {
                Assert.Same(context[dataSource], control.DataManager);
            }

            // Set same.
            control.DisplayMember = value;
            Assert.Same(dataSource, control.DataSource);
            Assert.Empty(control.DisplayMember);
            if (value == null)
            {
                Assert.Null(control.DataManager);
            }
            else
            {
                Assert.Same(context[dataSource], control.DataManager);
            }
        }

        [Fact]
        public void DisplayMember_SetWithHandler_CallsDisplayMemberChanged()
        {
            var control = new SubListControl();
            int displayMemberCallCount = 0;
            int dataSourceCallCount = 0;
            EventHandler displayMemberHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                displayMemberCallCount++;
            };
            EventHandler dataSourceHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                dataSourceCallCount++;
            };
            control.DisplayMemberChanged += displayMemberHandler;
            control.DataSourceChanged += dataSourceHandler;

            // Set different.
            control.DisplayMember = "Value1";
            Assert.Same("Value1", control.DisplayMember);
            Assert.Equal(1, displayMemberCallCount);
            Assert.Equal(0, dataSourceCallCount);

            // Set same.
            control.DisplayMember = "Value1";
            Assert.Same("Value1", control.DisplayMember);
            Assert.Equal(1, displayMemberCallCount);
            Assert.Equal(0, dataSourceCallCount);

            // Set different.
            control.DisplayMember = "Value2";
            Assert.Same("Value2", control.DisplayMember);
            Assert.Equal(2, displayMemberCallCount);
            Assert.Equal(0, dataSourceCallCount);

            // Set null.
            control.DisplayMember = null;
            Assert.Empty(control.DisplayMember);
            Assert.Equal(3, displayMemberCallCount);
            Assert.Equal(0, dataSourceCallCount);

            // Set empty.
            control.DisplayMember = string.Empty;
            Assert.Empty(control.DisplayMember);
            Assert.Equal(3, displayMemberCallCount);
            Assert.Equal(0, dataSourceCallCount);

            // Remove handler.
            control.DisplayMemberChanged -= displayMemberHandler;
            control.DataSourceChanged -= dataSourceHandler;
            control.DisplayMember = "Value1";
            Assert.Equal("Value1", control.DisplayMember);
            Assert.Equal(3, displayMemberCallCount);
            Assert.Equal(0, dataSourceCallCount);
        }

        [Fact]
        public void DisplayMember_SetInsideDisplayMemberChanged_Nop()
        {
            var control = new SubListControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                control.DisplayMember = "Value2";
                callCount++;
            };
            control.DisplayMemberChanged += handler;

            control.DisplayMember = "Value1";
            Assert.Equal("Value1", control.DisplayMember);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void Format_AddRemove_CallsRefreshItems()
        {
            var control = new SubListControl();
            int callCount = 0;
            control.RefreshItemsHandler = () => callCount++;
            ListControlConvertEventHandler handler = (sender, e) => { };

            // Add.
            control.Format += handler;
            Assert.Equal(1, callCount);

            // Remove.
            control.Format -= handler;
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> FormatInfo_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { CultureInfo.CurrentCulture };
        }

        [Theory]
        [MemberData(nameof(FormatInfo_Set_TestData))]
        public void FormatInfo_Set_GetReturnsExpected(IFormatProvider value)
        {
            var control = new SubListControl
            {
                FormatInfo = value
            };
            Assert.Same(value, control.FormatInfo);

            // Set same.
            control.FormatInfo = value;
            Assert.Same(value, control.FormatInfo);
        }

        [Theory]
        [MemberData(nameof(FormatInfo_Set_TestData))]
        public void FormatInfo_SetWithFormatInfo_GetReturnsExpected(IFormatProvider value)
        {
            var control = new SubListControl
            {
                FormatInfo = CultureInfo.InvariantCulture
            };

            control.FormatInfo = value;
            Assert.Same(value, control.FormatInfo);

            // Set same.
            control.FormatInfo = value;
            Assert.Same(value, control.FormatInfo);
        }

        [Fact]
        public void FormatInfo_Set_CallsRefreshItems()
        {
            var control = new SubListControl();
            int callCount = 0;
            control.RefreshItemsHandler = () => callCount++;

            // Set different.
            control.FormatInfo = CultureInfo.CurrentCulture;
            Assert.Same(CultureInfo.CurrentCulture, control.FormatInfo);
            Assert.Equal(1, callCount);

            // Set same.
            control.FormatInfo = CultureInfo.CurrentCulture;
            Assert.Same(CultureInfo.CurrentCulture, control.FormatInfo);
            Assert.Equal(1, callCount);

            // Set different.
            control.FormatInfo = CultureInfo.InvariantCulture;
            Assert.Same(CultureInfo.InvariantCulture, control.FormatInfo);
            Assert.Equal(2, callCount);

            // Set null.
            control.FormatInfo = null;
            Assert.Null(control.FormatInfo);
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void FormatInfo_SetWithHandler_CallsOnFormatInfoChanged()
        {
            var control = new SubListControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.FormatInfoChanged += handler;

            // Set different.
            control.FormatInfo = CultureInfo.CurrentCulture;
            Assert.Same(CultureInfo.CurrentCulture, control.FormatInfo);
            Assert.Equal(1, callCount);

            // Set same.
            control.FormatInfo = CultureInfo.CurrentCulture;
            Assert.Same(CultureInfo.CurrentCulture, control.FormatInfo);
            Assert.Equal(1, callCount);

            // Set different.
            control.FormatInfo = CultureInfo.InvariantCulture;
            Assert.Same(CultureInfo.InvariantCulture, control.FormatInfo);
            Assert.Equal(2, callCount);

            // Set null.
            control.FormatInfo = null;
            Assert.Null(control.FormatInfo);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.FormatInfoChanged -= handler;
            control.FormatInfo = CultureInfo.CurrentCulture;
            Assert.Same(CultureInfo.CurrentCulture, control.FormatInfo);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void FormatString_Set_GetReturnsExpected(string value, string expected)
        {
            var control = new SubListControl
            {
                FormatString = value
            };
            Assert.Equal(expected, control.FormatString);

            // Set same.
            control.FormatString = value;
            Assert.Equal(expected, control.FormatString);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void FormatString_SetWithFormatString_GetReturnsExpected(string value, string expected)
        {
            var control = new SubListControl
            {
                FormatString = "FormatString"
            };

            control.FormatString = value;
            Assert.Equal(expected, control.FormatString);

            // Set same.
            control.FormatString = value;
            Assert.Equal(expected, control.FormatString);
        }

        [Fact]
        public void FormatString_Set_CallsRefreshItems()
        {
            var control = new SubListControl();
            int callCount = 0;
            control.RefreshItemsHandler = () => callCount++;

            // Set different.
            control.FormatString = "Value1";
            Assert.Equal("Value1", control.FormatString);
            Assert.Equal(1, callCount);

            // Set same.
            control.FormatString = "Value1";
            Assert.Equal("Value1", control.FormatString);
            Assert.Equal(1, callCount);

            // Set different.
            control.FormatString = "Value2";
            Assert.Equal("Value2", control.FormatString);
            Assert.Equal(2, callCount);

            // Set null.
            control.FormatString = null;
            Assert.Empty(control.FormatString);
            Assert.Equal(3, callCount);

            // Set empty.
            control.FormatString = string.Empty;
            Assert.Empty(control.FormatString);
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void FormatStringSetWithHandler_CallsOnFormatStringChanged()
        {
            var control = new SubListControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.FormatStringChanged += handler;

            // Set different.
            control.FormatString = "Value1";
            Assert.Same("Value1", control.FormatString);
            Assert.Equal(1, callCount);

            // Set same.
            control.FormatString = "Value1";
            Assert.Same("Value1", control.FormatString);
            Assert.Equal(1, callCount);

            // Set different.
            control.FormatString = "Value2";
            Assert.Same("Value2", control.FormatString);
            Assert.Equal(2, callCount);

            // Set null.
            control.FormatString = null;
            Assert.Empty(control.FormatString);
            Assert.Equal(3, callCount);

            // Set empty.
            control.FormatString = string.Empty;
            Assert.Empty(control.FormatString);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.FormatStringChanged -= handler;
            control.FormatString = "Value1";
            Assert.Equal("Value1", control.FormatString);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FormattingEnabled_Set_GetReturnsExpected(bool value)
        {
            var control = new SubListControl
            {
                FormattingEnabled = value
            };
            Assert.Equal(value, control.FormattingEnabled);

            // Set same.
            control.FormattingEnabled = value;
            Assert.Equal(value, control.FormattingEnabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FormattingEnabled_SetWithFormattingEnabled_GetReturnsExpected(bool value)
        {
            var control = new SubListControl
            {
                FormattingEnabled = true
            };

            control.FormattingEnabled = value;
            Assert.Equal(value, control.FormattingEnabled);

            // Set same.
            control.FormattingEnabled = value;
            Assert.Equal(value, control.FormattingEnabled);
        }

        [Fact]
        public void FormattingEnabled_Set_CallsRefreshItems()
        {
            var control = new SubListControl();
            int callCount = 0;
            control.RefreshItemsHandler = () => callCount++;

            // Set different.
            control.FormattingEnabled = true;
            Assert.True(control.FormattingEnabled);
            Assert.Equal(1, callCount);

            // Set same.
            control.FormattingEnabled = true;
            Assert.True(control.FormattingEnabled);
            Assert.Equal(1, callCount);

            // Set different.
            control.FormattingEnabled = false;
            Assert.False(control.FormattingEnabled);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void FormattingEnabledSetWithHandler_CallsOnFormattingEnabledChanged()
        {
            var control = new SubListControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.FormattingEnabledChanged += handler;

            // Set different.
            control.FormattingEnabled = true;
            Assert.True(control.FormattingEnabled);
            Assert.Equal(1, callCount);

            // Set same.
            control.FormattingEnabled = true;
            Assert.True(control.FormattingEnabled);
            Assert.Equal(1, callCount);

            // Set different.
            control.FormattingEnabled = false;
            Assert.False(control.FormattingEnabled);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.FormattingEnabledChanged -= handler;
            control.FormattingEnabled = true;
            Assert.True(control.FormattingEnabled);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ValueMember_Set_GetReturnsExpected(string value, string expected)
        {
            var control = new SubListControl
            {
                ValueMember = value
            };
            Assert.Null(control.DataSource);
            Assert.Equal(expected, control.DisplayMember);
            Assert.Equal(expected, control.ValueMember);
            Assert.Null(control.DataManager);

            // Set same.
            control.ValueMember = value;
            Assert.Null(control.DataSource);
            Assert.Equal(expected, control.DisplayMember);
            Assert.Equal(expected, control.ValueMember);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ValueMember_SetWithDisplayMember_GetReturnsExpected(string value, string expected)
        {
            var control = new SubListControl
            {
                DisplayMember = "DisplayMember"
            };

            control.ValueMember = value;
            Assert.Null(control.DataSource);
            Assert.Equal("DisplayMember", control.DisplayMember);
            Assert.Equal(expected, control.ValueMember);
            Assert.Null(control.DataManager);

            // Set same.
            control.ValueMember = value;
            Assert.Null(control.DataSource);
            Assert.Equal("DisplayMember", control.DisplayMember);
            Assert.Equal(expected, control.ValueMember);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ValueMember_SetWithBindingContext_GetReturnsExpected(string value, string expected)
        {
            var context = new BindingContext();
            var control = new SubListControl
            {
                BindingContext = context
            };

            control.ValueMember = value;
            Assert.Null(control.DataSource);
            Assert.Equal(expected, control.DisplayMember);
            Assert.Equal(expected, control.ValueMember);
            Assert.Null(control.DataManager);

            // Set same.
            control.ValueMember = value;
            Assert.Null(control.DataSource);
            Assert.Equal(expected, control.DisplayMember);
            Assert.Equal(expected, control.ValueMember);
            Assert.Null(control.DataManager);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("Value", "Value")]
        [InlineData("value", "value")]
        public void ValueMember_SetWithBindingContextWithDataSourceCanCreate_GetReturnsExpected(string value, string expected)
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                BindingContext = context,
                DataSource = dataSource
            };

            control.ValueMember = value;
            Assert.Same(dataSource, control.DataSource);
            Assert.Equal(expected, control.DisplayMember);
            Assert.Equal(expected, control.ValueMember);
            Assert.Same(context[dataSource], control.DataManager);

            // Set same.
            control.ValueMember = value;
            Assert.Same(dataSource, control.DataSource);
            Assert.Equal(expected, control.DisplayMember);
            Assert.Equal(expected, control.ValueMember);
            Assert.Same(context[dataSource], control.DataManager);
        }

        [Fact]
        public void ValueMember_SetWithBindingContextWithDataSourceCantCreate_ThrowsArgumentException()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                BindingContext = context,
                DataSource = dataSource
            };

            Assert.Throws<ArgumentException>("newDisplayMember", (() => control.ValueMember = "NoSuchValue"));
            Assert.Equal("NoSuchValue", control.DisplayMember);
            Assert.Empty(control.ValueMember);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("Value", "Value")]
        [InlineData("value", "value")]
        public void ValueMember_SetWithBindingContextWithDataSourceWithDisplayMemberCanCreate_GetReturnsExpected(string value, string expected)
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                BindingContext = context,
                DataSource = dataSource,
                DisplayMember = "OtherValue"
            };

            control.ValueMember = value;
            Assert.Same(dataSource, control.DataSource);
            Assert.Equal("OtherValue", control.DisplayMember);
            Assert.Equal(expected, control.ValueMember);
            Assert.Same(context[dataSource], control.DataManager);

            // Set same.
            control.ValueMember = value;
            Assert.Same(dataSource, control.DataSource);
            Assert.Equal("OtherValue", control.DisplayMember);
            Assert.Equal(expected, control.ValueMember);
            Assert.Same(context[dataSource], control.DataManager);
        }

        [Fact]
        public void ValueMember_SetWithBindingContextWithDataSourceWithDisplayMemberCantCreate_ThrowsArgumentException()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                BindingContext = context,
                DataSource = dataSource,
                DisplayMember = "Value"
            };

            Assert.Throws<ArgumentException>("value", (() => control.ValueMember = "NoSuchValue"));
            Assert.Equal("Value", control.DisplayMember);
            Assert.Empty(control.ValueMember);
        }

        [Fact]
        public void ValueMember_SetWithHandler_CallsValueMemberChanged()
        {
            var control = new SubListControl();
            int valueMemberCallCount = 0;
            int selectedValueCallCount = 0;
            EventHandler valueMemberHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                valueMemberCallCount++;
            };
            EventHandler selectedValueHanlder = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.True(valueMemberCallCount > selectedValueCallCount);
                selectedValueCallCount++;
            };
            control.ValueMemberChanged += valueMemberHandler;
            control.SelectedValueChanged += selectedValueHanlder;

            // Set different.
            control.ValueMember = "Value1";
            Assert.Same("Value1", control.ValueMember);
            Assert.Equal(1, valueMemberCallCount);
            Assert.Equal(1, selectedValueCallCount);

            // Set same.
            control.ValueMember = "Value1";
            Assert.Same("Value1", control.ValueMember);
            Assert.Equal(1, valueMemberCallCount);
            Assert.Equal(1, selectedValueCallCount);

            // Set different.
            control.ValueMember = "Value2";
            Assert.Same("Value2", control.ValueMember);
            Assert.Equal(2, valueMemberCallCount);
            Assert.Equal(2, selectedValueCallCount);

            // Set null.
            control.ValueMember = null;
            Assert.Empty(control.ValueMember);
            Assert.Equal(3, valueMemberCallCount);
            Assert.Equal(3, selectedValueCallCount);

            // Set empty.
            control.ValueMember = string.Empty;
            Assert.Empty(control.ValueMember);
            Assert.Equal(3, valueMemberCallCount);
            Assert.Equal(3, selectedValueCallCount);

            // Remove handler.
            control.ValueMemberChanged -= valueMemberHandler;
            control.SelectedValueChanged -= selectedValueHanlder;
            control.ValueMember = "Value1";
            Assert.Equal("Value1", control.ValueMember);
            Assert.Equal(3, valueMemberCallCount);
            Assert.Equal(3, selectedValueCallCount);
        }

        [Theory]
        [InlineData("Value")]
        [InlineData("value")]
        public void SelectedValue_SetWithMatchingValue_Success(string valueMember)
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass { Value = "StringValue" } };
            var control = new SubListControl
            {
                SelectedIndex = 0,
                ValueMember = valueMember,
                BindingContext = context,
                DataSource = dataSource
            };

            control.SelectedValue = "StringValue";
            Assert.Equal(0, control.SelectedIndex);
            Assert.Equal("StringValue", control.SelectedValue);
        }

        public static IEnumerable<object[]> SelectedValue_NoMatchingValue_TestData()
        {
            yield return new object[] { new List<DataClass>(), "selected" };
            yield return new object[] { new List<DataClass> { new DataClass { Value = "NoSuchValue" } }, string.Empty };
            yield return new object[] { new List<DataClass> { new DataClass { Value = "NoSuchValue" } }, "selected" };
            yield return new object[] { new List<DataClass> { new DataClass { Value = "NoSuchValue" } }, "nosuchvalue" };
        }

        [Theory]
        [MemberData(nameof(SelectedValue_NoMatchingValue_TestData))]
        public void SelectedValue_SetWithNoMatchingValue_Success(object dataSource, string value)
        {
            var context = new BindingContext();
            var control = new SubListControl
            {
                SelectedIndex = 0,
                ValueMember = "Value",
                BindingContext = context,
                DataSource = dataSource
            };

            control.SelectedValue = value;
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedValue);
        }

        [Fact]
        public void SelectedValue_SetWithChangedDataManager_Success()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                SelectedIndex = 0,
                ValueMember = "ValueMember",
                BindingContext = context,
                DataSource = dataSource
            };

            control.SelectedValue = "selected";
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedValue);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public void SelectedValue_SetWithoutDataManager_GetReturnsNull(object value)
        {
            var control = new SubListControl
            {
                SelectedIndex = 0,
                SelectedValue = value
            };
            Assert.Equal(0, control.SelectedIndex);
            Assert.Null(control.SelectedValue);

            // Not set even when given a DataManager.
            control.BindingContext = new BindingContext();
            control.DataSource = new List<int>();
            Assert.Equal(0, control.SelectedIndex);
            Assert.Throws<IndexOutOfRangeException>(() => control.SelectedValue);
        }

        [Fact]
        public void SelectedValue_SetWithHandler_DoesNotCallSelectedValueChanged()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass> { new DataClass { Value = "StringValue" } };
            var control = new SubListControl
            {
                SelectedIndex = 0,
                ValueMember = "Value",
                BindingContext = context,
                DataSource = dataSource
            };

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.SelectedValueChanged += handler;

            // Set different.
            control.SelectedValue = "StringValue";
            Assert.Equal("StringValue", control.SelectedValue);
            Assert.Equal(0, callCount);

            // Set same.
            control.SelectedValue = "StringValue";
            Assert.Equal("StringValue", control.SelectedValue);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.SelectedValueChanged -= handler;
            control.SelectedValue = "StringValue";
            Assert.Equal("StringValue", control.SelectedValue);
            Assert.Equal(0, callCount);
        }

        [Fact]
        public void SelectedValue_SetNull_ThrowsArgumentNullException()
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                SelectedIndex = 0,
                BindingContext = context,
                DataSource = dataSource,
                ValueMember = "Value"
            };
            Assert.Throws<ArgumentNullException>("key", () => control.SelectedValue = null);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedValue);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public void SelectedValue_SetWithoutValueMember_ThrowsInvalidOperationException(object value)
        {
            var context = new BindingContext();
            var dataSource = new List<DataClass>();
            var control = new SubListControl
            {
                SelectedIndex = 0,
                BindingContext = context,
                DataSource = dataSource
            };
            Assert.Throws<InvalidOperationException>(() => control.SelectedValue = value);
            Assert.Equal(0, control.SelectedIndex);
            Assert.Throws<IndexOutOfRangeException>(() => control.SelectedValue);
        }

        [Theory]
        [InlineData(Keys.Alt, false)]
        [InlineData(Keys.Alt | Keys.PageUp, false)]
        [InlineData(Keys.PageUp, true)]
        [InlineData(Keys.PageDown, true)]
        [InlineData(Keys.Home, true)]
        [InlineData(Keys.End, true)]
        [InlineData(Keys.A, false)]
        public void IsInputKey_Invoke_ReturnsExpected(Keys keyData, bool expected)
        {
            var control = new SubListControl();
            Assert.Equal(expected, control.IsInputKey(keyData));
        }

        public static IEnumerable<object[]> FilterItemOnProperty_TestData()
        {
            yield return new object[]
            {
                new SubListControl(),
                null,
                null
            };

            var item = new object();
            yield return new object[]
            {
                new SubListControl(),
                item,
                item
            };

            yield return new object[]
            {
                new SubListControl { DisplayMember = "Length" },
                "abc",
                3
            };
            yield return new object[]
            {
                new SubListControl { DisplayMember = "length" },
                "abc",
                3
            };
            yield return new object[]
            {
                new SubListControl { DisplayMember = "Path.Length" },
                "abc",
                3
            };
            yield return new object[]
            {
                new SubListControl { DisplayMember = "NoSuchProperty" },
                "abc",
                "abc"
            };

            var dataClass = new DataClass { Value = 10 };
            var list = new List<DataClass> { dataClass };
            yield return new object[]
            {
                new SubListControl
                {
                    DataSource = list
                },
                list,
                list
            };

            yield return new object[]
            {
                new SubListControl
                {
                    BindingContext = new BindingContext(),
                    DataSource = list
                },
                list,
                list
            };
            yield return new object[]
            {
                new SubListControl
                {
                    BindingContext = new BindingContext(),
                    DataSource = list,
                    DisplayMember = "Value"
                },
                list,
                list
            };
            yield return new object[]
            {
                new SubListControl
                {
                    BindingContext = new BindingContext(),
                    DataSource = list,
                    DisplayMember = "Value"
                },
                dataClass,
                10
            };
            yield return new object[]
            {
                new SubListControl
                {
                    BindingContext = new BindingContext(),
                    DataSource = list,
                    DisplayMember = "value"
                },
                dataClass,
                10
            };
            yield return new object[]
            {
                new SubListControl
                {
                    BindingContext = new BindingContext(),
                    DataSource = list,
                    DisplayMember = "NoSuchProperty"
                },
                dataClass,
                dataClass
            };
        }

        [Theory]
        [MemberData(nameof(FilterItemOnProperty_TestData))]
        public void FilterItemOnProperty_Invoke_ReturnsExpected(SubListControl control, object item, object expected)
        {
            Assert.Equal(expected, control.FilterItemOnProperty(item));
        }

        public static IEnumerable<object[]> FilterItemOnProperty_String_TestData()
        {
            yield return new object[]
            {
                new SubListControl(),
                null,
                "Field",
                null
            };

            var item = new object();
            yield return new object[]
            {
                new SubListControl(),
                item,
                null,
                item
            };
            yield return new object[]
            {
                new SubListControl(),
                item,
                string.Empty,
                item
            };

            yield return new object[]
            {
                new SubListControl(),
                "abc",
                "Length",
                3
            };
            yield return new object[]
            {
                new SubListControl(),
                "abc",
                "length",
                3
            };
            yield return new object[]
            {
                new SubListControl(),
                "abc",
                "NoSuchProperty",
                "abc"
            };

            var dataClass = new DataClass { Value = 10 };
            var list = new List<DataClass> { dataClass };
            yield return new object[]
            {
                new SubListControl
                {
                    BindingContext = new BindingContext(),
                    DataSource = list
                },
                list,
                "NoSuchField",
                list
            };
            yield return new object[]
            {
                new SubListControl
                {
                    BindingContext = new BindingContext(),
                    DataSource = list
                },
                list,
                "Value",
                list
            };
            yield return new object[]
            {
                new SubListControl
                {
                    BindingContext = new BindingContext(),
                    DataSource = list
                },
                dataClass,
                "Value",
                10
            };
            yield return new object[]
            {
                new SubListControl
                {
                    BindingContext = new BindingContext(),
                    DataSource = list
                },
                dataClass,
                "value",
                10
            };
            yield return new object[]
            {
                new SubListControl
                {
                    BindingContext = new BindingContext(),
                    DataSource = list
                },
                dataClass,
                "NoSuchProperty",
                dataClass
            };
        }

        [Theory]
        [MemberData(nameof(FilterItemOnProperty_String_TestData))]
        public void FilterItemOnProperty_InvokeString_ReturnsExpected(SubListControl control, object item, string field, object expected)
        {
            Assert.Equal(expected, control.FilterItemOnProperty(item, field));
        }

        public static IEnumerable<object[]> GetItemText_TestData()
        {
            yield return new object[]
            {
                new SubListControl(), null, string.Empty
            };
            yield return new object[]
            {
                new SubListControl(),
                "abc",
                "abc"
            };
            yield return new object[]
            {
                new SubListControl
                {
                    DisplayMember = "Length"
                },
                "abc",
                3
            };
            yield return new object[]
            {
                new SubListControl
                {
                    DisplayMember = "Value"
                },
                new DataClass { Value = 1 },
                "1"
            };
            yield return new object[]
            {
                new SubListControl
                {
                    DisplayMember = "Value"
                },
                new DataClass { Value = null },
                string.Empty
            };
        }

        [Theory]
        [MemberData(nameof(GetItemText_TestData))]
        public void GetItemText_Invoke_ReturnsExpected(ListControl control, object item, string expected)
        {
            Assert.Equal(expected, control.GetItemText(item));

            // Test caching behaviour.
            Assert.Equal(expected, control.GetItemText(item));
        }

        public static IEnumerable<object[]> GetItemText_HasHandler_TestData()
        {
            var item = new DataClass { Value = 3 };
            yield return new object[] { item, null, "3" };
            yield return new object[] { item, new object(), "3" };
            yield return new object[] { item, item, "3" };
            yield return new object[] { item, "custom", "custom" };
        }

        [Theory]
        [MemberData(nameof(GetItemText_HasHandler_TestData))]
        public void GetItemText_HasHandler_CallsFormat(object item, object value, object expected)
        {
            var control = new SubListControl
            {
                FormattingEnabled = true,
                DisplayMember = "Value"
            };

            // Handler.
            int callCount = 0;
            ListControlConvertEventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Equal(3, e.Value);
                Assert.Equal(typeof(string), e.DesiredType);
                Assert.Same(item, e.ListItem);

                e.Value = value;
                callCount++;
            };

            control.Format += handler;
            Assert.Equal(expected, control.GetItemText(item));
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.Format -= handler;
            Assert.Equal("3", control.GetItemText(item));
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> GetItemText_HasHandlerFormattingDisabled_TestData()
        {
            var item = new DataClass { Value = 3 };
            yield return new object[] { item, null };
            yield return new object[] { item, new object() };
            yield return new object[] { item, item };
            yield return new object[] { item, "custom" };
        }

        [Theory]
        [MemberData(nameof(GetItemText_HasHandlerFormattingDisabled_TestData))]
        public void GetItemText_HasHandlerFormattingDisabled_DoesNotCallFormat(object item, object value)
        {
            var control = new SubListControl
            {
                FormattingEnabled = false,
                DisplayMember = "Value"
            };

            // Handler.
            int callCount = 0;
            ListControlConvertEventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Equal(3, e.Value);
                Assert.Equal(typeof(string), e.DesiredType);
                Assert.Same(item, e.ListItem);

                e.Value = value;
                callCount++;
            };

            control.Format += handler;
            Assert.Equal("3", control.GetItemText(item));
            Assert.Equal(0, callCount);

            // Should not call if the handler is removed.
            control.Format -= handler;
            Assert.Equal("3", control.GetItemText(item));
            Assert.Equal(0, callCount);
        }

        [Fact]
        public void GetItemText_CustomConverter_ReturnsExpected()
        {
            var item = new CustomTypeConverterDataClass { Value = 10 };
            var control = new SubListControl
            {
                FormattingEnabled = true,
                BindingContext = new BindingContext(),
                DataSource = new List<CustomTypeConverterDataClass> { item },
                DisplayMember = "Value"
            };
            Assert.Equal("custom", control.GetItemText(item));

            // Test caching behaviour.
            Assert.Equal("custom", control.GetItemText(item));
        }

        [Fact]
        public void GetItemText_CustomConverterFormattingDisabled_ReturnsExpected()
        {
            var item = new CustomTypeConverterDataClass { Value = 10 };
            var control = new SubListControl
            {
                FormattingEnabled = false,
                BindingContext = new BindingContext(),
                DataSource = new List<CustomTypeConverterDataClass> { item },
                DisplayMember = "Value"
            };
            Assert.Equal("10", control.GetItemText(item));

            // Test caching behaviour.
            Assert.Equal("10", control.GetItemText(item));
        }

        [Fact]
        public void GetItemText_CustomConverterNoContext_ReturnsExpected()
        {
            var item = new CustomTypeConverterDataClass { Value = 10 };
            var control = new SubListControl
            {
                FormattingEnabled = true,
                DataSource = new List<CustomTypeConverterDataClass> { item },
                DisplayMember = "Value"
            };
            Assert.Equal("10", control.GetItemText(item));

            // Test caching behaviour.
            Assert.Equal("10", control.GetItemText(item));
        }

        [Fact]
        public void GetItemText_NonCriticalThrowingExceptionType_ReturnsExpected()
        {
            var item = new NonCriticalThrowingTypeConverterDataClass();
            var control = new SubListControl
            {
                FormattingEnabled = true,
                BindingContext = new BindingContext(),
                DataSource = new List<NonCriticalThrowingTypeConverterDataClass> { item },
                DisplayMember = "Value"
            };
            Assert.Equal("NonCriticalThrowingTypeConverterDataClassToString", control.GetItemText(item));

            // Test caching behaviour.
            Assert.Equal("NonCriticalThrowingTypeConverterDataClassToString", control.GetItemText(item));
        }

        [Fact]
        public void GetItemText_CriticalThrowingExceptionType_RethrowsException()
        {
            var item = new CriticalThrowingTypeConverterDataClass();
            var control = new SubListControl
            {
                FormattingEnabled = true,
                BindingContext = new BindingContext(),
                DataSource = new List<CriticalThrowingTypeConverterDataClass> { item },
                DisplayMember = "Value"
            };
            Assert.Throws<StackOverflowException>(() => control.GetItemText(item));

            // Test caching behaviour.
            Assert.Throws<StackOverflowException>(() => control.GetItemText(item));
        }

        [Fact]
        public void OnBindingContextChanged_Invoke_CallsBindingContextChanged()
        {
            var control = new SubListControl();
            var eventArgs = new EventArgs();

            // No handler.
            control.OnBindingContextChanged(eventArgs);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            control.BindingContextChanged += handler;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.BindingContextChanged -= handler;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void OnDataSourceChanged_Invoke_CallsDataSourceChanged()
        {
            var control = new SubListControl();
            var eventArgs = new EventArgs();

            // No handler.
            control.OnDataSourceChanged(eventArgs);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            control.DataSourceChanged += handler;
            control.OnDataSourceChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.DataSourceChanged -= handler;
            control.OnDataSourceChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void OnDisplayMemberChanged_Invoke_CallsDisplayMemberChanged()
        {
            var control = new SubListControl();
            var eventArgs = new EventArgs();

            // No handler.
            control.OnDisplayMemberChanged(eventArgs);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            control.DisplayMemberChanged += handler;
            control.OnDisplayMemberChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.DisplayMemberChanged -= handler;
            control.OnDisplayMemberChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void OnFormat_Invoke_CallsFormat()
        {
            var control = new SubListControl();
            var eventArgs = new ListControlConvertEventArgs(new object(), typeof(int), new object());

            // No handler.
            control.OnFormat(eventArgs);

            // Handler.
            int callCount = 0;
            ListControlConvertEventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            control.Format += handler;
            control.OnFormat(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.Format -= handler;
            control.OnFormat(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void OnFormatInfoChanged_Invoke_CallsFormatInfoChanged()
        {
            var control = new SubListControl();
            var eventArgs = new EventArgs();

            // No handler.
            control.OnFormatInfoChanged(eventArgs);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            control.FormatInfoChanged += handler;
            control.OnFormatInfoChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.FormatInfoChanged -= handler;
            control.OnFormatInfoChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void OnFormatStringChanged_Invoke_CallsFormatStringChanged()
        {
            var control = new SubListControl();
            var eventArgs = new EventArgs();

            // No handler.
            control.OnFormatStringChanged(eventArgs);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            control.FormatStringChanged += handler;
            control.OnFormatStringChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.FormatStringChanged -= handler;
            control.OnFormatStringChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void OnFormattingEnabledChanged_Invoke_CallsFormattingEnabledChanged()
        {
            var control = new SubListControl();
            var eventArgs = new EventArgs();

            // No handler.
            control.OnFormattingEnabledChanged(eventArgs);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            control.FormattingEnabledChanged += handler;
            control.OnFormattingEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.FormattingEnabledChanged -= handler;
            control.OnFormattingEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void OnSelectedIndexChanged_Invoke_CallsSelectedValueChanged()
        {
            var control = new SubListControl();
            var eventArgs = new EventArgs();

            // No handler.
            control.OnSelectedIndexChanged(eventArgs);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            control.SelectedValueChanged += handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.SelectedValueChanged -= handler;
            control.OnSelectedIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void OnSelectedValueChanged_Invoke_CallsSelectedValueChanged()
        {
            var control = new SubListControl();
            var eventArgs = new EventArgs();

            // No handler.
            control.OnSelectedValueChanged(eventArgs);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            control.SelectedValueChanged += handler;
            control.OnSelectedValueChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.SelectedValueChanged -= handler;
            control.OnSelectedValueChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void OnValueMemberChanged_Invoke_CallsValueMemberChanged()
        {
            var control = new SubListControl();
            var eventArgs = new EventArgs();

            // No handler.
            control.OnValueMemberChanged(eventArgs);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            control.ValueMemberChanged += handler;
            control.OnValueMemberChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.ValueMemberChanged -= handler;
            control.OnValueMemberChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void RefreshItems_Invoke_Nop()
        {
            var control = new SubListControl();
            control.RefreshItemsEntry();
        }

        [Theory]
        [InlineData(0, null)]
        [InlineData(-1, 1)]
        public void SetItemCore_Invoke_Nop(int index, object value)
        {
            var control = new SubListControl();
            control.SetItemCoreEntry(index, value);
        }

        private class DataClass
        {
            public object Value { get; set; }
            public object OtherValue { get; set; }
            public IList ListValue { get; set; }

            public override string ToString() => "DataClassToString";
        }

        private class ComponentList : List<int>, IComponent
        {
            public ISite Site { get; set; }

            public event EventHandler Disposed;

            public void Dispose() => OnDisposed(this, EventArgs.Empty);

            public void OnDisposed(object sender, EventArgs e)
            {
                Disposed?.Invoke(sender, e);
            }
        }

        private class SupportInitializeNotificationList : List<int>, ISupportInitializeNotification
        {
            public bool IsInitialized { get; set; }

            public event EventHandler Initialized;

            public void BeginInit()
            {
            }

            public void EndInit()
            {
            }

            public void OnInitialized(object sender, EventArgs e)
            {
                Initialized?.Invoke(sender, e);
            }
        }

        public class SubListControl : ListControl
        {
            public override int SelectedIndex { get; set; }

            public bool AllowSelectionEntry => base.AllowSelection;

            public new CurrencyManager DataManager => base.DataManager;

            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

            public new bool IsInputKey(Keys keyData) => base.IsInputKey(keyData);

            public new object FilterItemOnProperty(object item) => base.FilterItemOnProperty(item);

            public new object FilterItemOnProperty(object item, string field) => base.FilterItemOnProperty(item, field);

            public new void OnBindingContextChanged(EventArgs e) => base.OnBindingContextChanged(e);

            public new void OnDataSourceChanged(EventArgs e) => base.OnDataSourceChanged(e);

            public new void OnDisplayMemberChanged(EventArgs e) => base.OnDisplayMemberChanged(e);

            public new void OnFormat(ListControlConvertEventArgs e) => base.OnFormat(e);

            public new void OnFormatInfoChanged(EventArgs e) => base.OnFormatInfoChanged(e);

            public new void OnFormatStringChanged(EventArgs e) => base.OnFormatStringChanged(e);

            public new void OnFormattingEnabledChanged(EventArgs e) => base.OnFormattingEnabledChanged(e);

            public new void OnSelectedIndexChanged(EventArgs e) => base.OnSelectedIndexChanged(e);

            public new void OnSelectedValueChanged(EventArgs e) => base.OnSelectedValueChanged(e);

            public new void OnValueMemberChanged(EventArgs e) => base.OnValueMemberChanged(e);

            public Action RefreshItemsHandler { get; set; }

            public void RefreshItemsEntry() => RefreshItems();

            protected override void RefreshItem(int index)
            {
            }

            protected override void RefreshItems()
            {
                RefreshItemsHandler?.Invoke();
                base.RefreshItems();
            }

            public Action<int, object> SetItemCoreHandler { get; set; }

            public void SetItemCoreEntry(int index, object value) => base.SetItemCore(index, value);

            protected override void SetItemCore(int index, object value)
            {
                SetItemCoreHandler?.Invoke(index, value);
            }

            public Action<IList> SetItemsCoreHandler { get; set; }

            protected override void SetItemsCore(IList items)
            {
                SetItemsCoreHandler?.Invoke(items);
            }
        }

        public class AllowSelectionFalseListControl : SubListControl
        {
            protected override bool AllowSelection => false;
        }

        public class CustomTypeConverterDataClass
        {
            [TypeConverter(typeof(CustomTypeConverter))]
            public int Value { get; set; }

            public override string ToString() => "CustomTypeConverterDataClassToString";
        }

        private class CustomTypeConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                Assert.Equal(10, value);
                Assert.Equal(typeof(string), destinationType);
                return "custom";
            }
        }

        public class NonCriticalThrowingTypeConverterDataClass
        {
            [TypeConverter(typeof(NonCriticalThrowingTypeConverter))]
            public int Value { get; set; }

            public override string ToString() => "NonCriticalThrowingTypeConverterDataClassToString";
        }

        private class NonCriticalThrowingTypeConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                throw new InvalidOperationException();
            }
        }

        public class CriticalThrowingTypeConverterDataClass
        {
            [TypeConverter(typeof(CriticalThrowingTypeConverter))]
            public int Value { get; set; }
        }

        private class CriticalThrowingTypeConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                throw new StackOverflowException();
            }
        }
    }
}
