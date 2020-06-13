// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Linq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListBoxIntegerCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ListBoxIntegerCollection_Ctor_ListBox()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            Assert.Equal(0, collection.Count);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Ctor_NullOwner_ThrowsArgumentNullException()
        {
            using var owner = new ListBox();
            Assert.Throws<ArgumentNullException>("owner", () => new ListBox.IntegerCollection(null));
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_ICollection_Properties_GetReturnsExpected()
        {
            using var owner = new ListBox();
            ICollection collection = new ListBox.IntegerCollection(owner);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.IsSynchronized);
            Assert.Same(collection, collection.SyncRoot);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IList_Properties_GetReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.Equal(0, collection.Count);
            Assert.False(collection.IsFixedSize);
            Assert.False(collection.IsReadOnly);
            Assert.True(collection.IsSynchronized);
            Assert.Same(collection, collection.SyncRoot);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Item_Get_ReturnsExpected()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            collection.AddRange(new int[] { 2, 1, 1, 3 });

            Assert.Equal(1, collection[0]);
            Assert.Equal(2, collection[1]);
            Assert.Equal(3, collection[2]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ListBoxIntegerCollection_Item_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        public void ListBoxIntegerCollection_Item_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            collection.Add(1);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Item_Set_ReturnsExpected()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            collection.AddRange(new int[] { 2, 1, 1, 3 });

            // Set first.
            collection[0] = 4;
            Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Set middle.
            collection[1] = 1;
            Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Set last.
            collection[2] = 4;
            Assert.Equal(new int[] { 4, 1, 4 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Item_SetCustomTabOffsets_ReturnsExpected()
        {
            using var owner = new ListBox();
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            collection.AddRange(new int[] { 2, 1, 1, 3 });

            // Set first.
            collection[0] = 4;
            Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Set middle.
            collection[1] = 1;
            Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Set last.
            collection[2] = 4;
            Assert.Equal(new int[] { 4, 1, 4 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 1, 4 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Item_SetWithHandle_ReturnsExpected()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            collection.AddRange(new int[] { 2, 1, 1, 3 });
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Set first.
            collection[0] = 4;
            Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set middle.
            collection[1] = 1;
            Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set last.
            collection[2] = 4;
            Assert.Equal(new int[] { 4, 1, 4 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Item_SetCustomTabOffsetsWithHandle_ReturnsExpected()
        {
            using var owner = new ListBox();
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            collection.AddRange(new int[] { 2, 1, 1, 3 });
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Set first.
            collection[0] = 4;
            Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set middle.
            collection[1] = 1;
            Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set last.
            collection[2] = 4;
            Assert.Equal(new int[] { 4, 1, 4 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 1, 4 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ListBoxIntegerCollection_Item_SetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = 2);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        public void ListBoxIntegerCollection_Item_SetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            collection.Add(1);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = 2);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_Add_Invoke_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            var collection = new ListBox.IntegerCollection(owner);

            // Add one.
            Assert.Equal(0, collection.Add(2));
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Add another.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Add same.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Add another.
            Assert.Equal(2, collection.Add(3));
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_Add_InvokeCustomTabOffsets_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;

            // Add one.
            Assert.Equal(0, collection.Add(2));
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Add another.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Add same.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Add another.
            Assert.Equal(2, collection.Add(3));
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 1)]
        [InlineData(true, false, 0)]
        [InlineData(false, true, 1)]
        [InlineData(false, false, 0)]
        public void ListBoxIntegerCollection_Add_InvokeWithHandle_Success(bool sorted, bool useCustomTabOffset, int expectedInvalidatedCallCount)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            var collection = new ListBox.IntegerCollection(owner);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Add one.
            Assert.Equal(0, collection.Add(2));
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add another.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add same.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add another.
            Assert.Equal(2, collection.Add(3));
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true, 1)]
        [InlineData(true, false, 0)]
        [InlineData(false, true, 1)]
        [InlineData(false, false, 0)]
        public void ListBoxIntegerCollection_Add_InvokeCustomTabOffsetsWithHandle_Success(bool sorted, bool useCustomTabOffset, int expectedInvalidatedCallCount)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Add one.
            Assert.Equal(0, collection.Add(2));
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add another.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add same.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add another.
            Assert.Equal(2, collection.Add(3));
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_AddRange_InvokeIntAray_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            var collection = new ListBox.IntegerCollection(owner);

            collection.AddRange(new int[] { 2, 1, 1, 3 });
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Add empty.
            collection.AddRange(Array.Empty<int>());
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_AddRange_InvokeIntArayCustomTabOffsets_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;

            collection.AddRange(new int[] { 2, 1, 1, 3 });
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Add empty.
            collection.AddRange(Array.Empty<int>());
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 2)]
        [InlineData(true, false, 1)]
        [InlineData(false, true, 2)]
        [InlineData(false, false, 1)]
        public void ListBoxIntegerCollection_AddRange_InvokeIntArayWithHandle_Success(bool sorted, bool useCustomTabOffset, int expectedInvalidatedCallCount)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            var collection = new ListBox.IntegerCollection(owner);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            collection.AddRange(new int[] { 2, 1, 1, 3 });
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add empty.
            collection.AddRange(Array.Empty<int>());
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true, 2)]
        [InlineData(true, false, 1)]
        [InlineData(false, true, 2)]
        [InlineData(false, false, 1)]
        public void ListBoxIntegerCollection_AddRange_InvokeIntArayCustomTabOffsetsWithHandle_Success(bool sorted, bool useCustomTabOffset, int expectedInvalidatedCallCount)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            collection.AddRange(new int[] { 2, 1, 1, 3 });
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add empty.
            collection.AddRange(Array.Empty<int>());
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_AddRange_InvokeIntegerCollection_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            var collection = new ListBox.IntegerCollection(owner);

            using var otherOwner = new ListBox();
            var otherCollection = new ListBox.IntegerCollection(otherOwner);
            otherCollection.AddRange(new int[] { 2, 1, 1, 3 });
            collection.AddRange(otherCollection);
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Add empty.
            collection.AddRange(Array.Empty<int>());
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_AddRange_InvokeIntegerCollectionCustomTabOffsets_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;

            using var otherOwner = new ListBox();
            var otherCollection = new ListBox.IntegerCollection(otherOwner);
            otherCollection.AddRange(new int[] { 2, 1, 1, 3 });
            collection.AddRange(otherCollection);
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Add empty.
            collection.AddRange(Array.Empty<int>());
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 2)]
        [InlineData(true, false, 1)]
        [InlineData(false, true, 2)]
        [InlineData(false, false, 1)]
        public void ListBoxIntegerCollection_AddRange_InvokeIntegerCollectionWithHandle_Success(bool sorted, bool useCustomTabOffset, int expectedInvalidatedCallCount)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            var collection = new ListBox.IntegerCollection(owner);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            using var otherOwner = new ListBox();
            var otherCollection = new ListBox.IntegerCollection(otherOwner);
            otherCollection.AddRange(new int[] { 2, 1, 1, 3 });
            collection.AddRange(otherCollection);
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add empty.
            collection.AddRange(Array.Empty<int>());
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true, 2)]
        [InlineData(true, false, 1)]
        [InlineData(false, true, 2)]
        [InlineData(false, false, 1)]
        public void ListBoxIntegerCollection_AddRange_InvokeIntegerCollectionCustomTabOffsetsWithHandle_Success(bool sorted, bool useCustomTabOffset, int expectedInvalidatedCallCount)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            using var otherOwner = new ListBox();
            var otherCollection = new ListBox.IntegerCollection(otherOwner);
            otherCollection.AddRange(new int[] { 2, 1, 1, 3 });
            collection.AddRange(otherCollection);
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add empty.
            collection.AddRange(Array.Empty<int>());
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_AddRange_NullItems_ThrowsArgumentNullException()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            Assert.Throws<ArgumentNullException>("items", () => collection.AddRange((int[])null));
            Assert.Throws<ArgumentNullException>("items", () => collection.AddRange((ListBox.IntegerCollection)null));
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Contains_InvokeEmpty_ReturnsExpected()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            Assert.False(collection.Contains(0));
            Assert.False(collection.Contains(1));
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Contains_InvokeWithValues_ReturnsExpected()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);

            // Add one.
            collection.Add(1);
            Assert.False(collection.Contains(0));
            Assert.True(collection.Contains(1));
            Assert.False(collection.Contains(2));
            Assert.False(owner.IsHandleCreated);

            // Add another.
            collection.Add(0);
            Assert.True(collection.Contains(0));
            Assert.True(collection.Contains(1));
            Assert.False(collection.Contains(2));
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Clear_InvokeEmpty_Success()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Clear_InvokeCustomTabOffsetsEmpty_Success()
        {
            using var owner = new ListBox();
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Clear_InvokeNotEmpty_Success()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            collection.Add(1);

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Clear_InvokeCustomTabOffsetNotEmpty_Success()
        {
            using var owner = new ListBox();
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            collection.Add(1);

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Clear_InvokeEmptyWithHande_Success()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Clear_InvokeCustomTabOffsetsEmptyWithHande_Success()
        {
            using var owner = new ListBox();
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Clear_InvokeNotEmptyWithHande_Success()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;
            collection.Add(1);

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_Clear_InvokeCustomTabOffsetNotEmptyWithHande_Success()
        {
            using var owner = new ListBox();
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;
            collection.Add(1);

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_CopyTo_InvokeEmpty_Success()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            var array = new object[] { "1", "2", "3" };
            collection.CopyTo(array, 1);
            Assert.Equal(new object[] { "1", "2", "3" }, array);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_CopyTo_InvokeNotEmpty_ReturnsExpected()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            collection.Add(1);
            collection.Add(2);
            var array = new object[] { "1", "2", "3" };
            collection.CopyTo(array, 1);
            Assert.Equal(new object[] { "1", 1, 2 }, array);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_CopyTo_NullArrayEmpty_ThrowsArgumentNullException()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            Assert.Throws<ArgumentNullException>("destination", () => collection.CopyTo(null, 0));
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_CopyTo_NullArrayNotEmpty_ThrowsArgumentNullException()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            collection.Add(1);
            Assert.Throws<ArgumentNullException>("destination", () => collection.CopyTo(null, 0));
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IndexOf_InvokeEmpty_ReturnsExpected()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            Assert.Equal(-1, collection.IndexOf(0));
            Assert.Equal(-1, collection.IndexOf(1));
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IndexOf_InvokeWithValues_ReturnsExpected()
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);

            // Add one.
            collection.Add(1);
            Assert.Equal(-1, collection.IndexOf(0));
            Assert.Equal(0, collection.IndexOf(1));
            Assert.Equal(-1, collection.IndexOf(2));
            Assert.False(owner.IsHandleCreated);

            // Add another.
            collection.Add(0);
            Assert.Equal(0, collection.IndexOf(0));
            Assert.Equal(1, collection.IndexOf(1));
            Assert.Equal(-1, collection.IndexOf(2));
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_Remove_Invoke_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            var collection = new ListBox.IntegerCollection(owner);
            collection.AddRange(new int[] { 2, 1, 1, 3 });

            // Remove middle.
            collection.Remove(2);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove last.
            collection.Remove(3);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            collection.Remove(1);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove no such.
            collection.Remove(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_Remove_InvokeCustomTabOffsets_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            collection.AddRange(new int[] { 2, 1, 1, 3 });

            // Remove middle.
            collection.Remove(2);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Remove last.
            collection.Remove(3);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            collection.Remove(1);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove no such.
            collection.Remove(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_Remove_InvokeWithHandle_Success(bool sorted, bool useCustomTabOffset)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            var collection = new ListBox.IntegerCollection(owner);
            collection.AddRange(new int[] { 2, 1, 1, 3 });
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Remove middle.
            collection.Remove(2);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove last.
            collection.Remove(3);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove first.
            collection.Remove(1);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove no such.
            collection.Remove(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_Remove_InvokeCustomTabOffsetsWithHandle_Success(bool sorted, bool useCustomTabOffset)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            collection.AddRange(new int[] { 2, 1, 1, 3 });
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Remove middle.
            collection.Remove(2);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove last.
            collection.Remove(3);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove first.
            collection.Remove(1);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove no such.
            collection.Remove(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_RemoveAt_Invoke_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            var collection = new ListBox.IntegerCollection(owner);
            collection.AddRange(new int[] { 2, 1, 1, 3 });

            // Remove middle.
            collection.RemoveAt(1);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove last.
            collection.RemoveAt(1);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            collection.RemoveAt(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_RemoveAt_InvokeCustomTabOffsets_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            collection.AddRange(new int[] { 2, 1, 1, 3 });

            // Remove middle.
            collection.RemoveAt(1);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Remove last.
            collection.RemoveAt(1);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            collection.RemoveAt(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_RemoveAt_InvokeWithHandle_Success(bool sorted, bool useCustomTabOffset)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            var collection = new ListBox.IntegerCollection(owner);
            collection.AddRange(new int[] { 2, 1, 1, 3 });
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Remove middle.
            collection.RemoveAt(1);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove last.
            collection.RemoveAt(1);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove first.
            collection.RemoveAt(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_RemoveAt_InvokeCustomTabOffsetsWithHandle_Success(bool sorted, bool useCustomTabOffset)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            ListBox.IntegerCollection collection = owner.CustomTabOffsets;
            collection.AddRange(new int[] { 2, 1, 1, 3 });
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Remove middle.
            collection.RemoveAt(1);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove last.
            collection.RemoveAt(1);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove first.
            collection.RemoveAt(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ListBoxIntegerCollection_RemoveAt_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        public void ListBoxIntegerCollection_RemoveAt_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            var collection = new ListBox.IntegerCollection(owner);
            collection.Add(1);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListItem_Get_ReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);

            Assert.Equal(1, collection[0]);
            Assert.Equal(2, collection[1]);
            Assert.Equal(3, collection[2]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ListBoxIntegerCollection_IListItem_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        public void ListBoxIntegerCollection_IListItem_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(1);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListItem_Set_ReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);

            // Set first.
            collection[0] = 4;
            Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Set middle.
            collection[1] = 1;
            Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Set last.
            collection[2] = 4;
            Assert.Equal(new int[] { 4, 1, 4 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListItem_SetCustomTabOffsets_ReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = owner.CustomTabOffsets;
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);

            // Set first.
            collection[0] = 4;
            Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Set middle.
            collection[1] = 1;
            Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Set last.
            collection[2] = 4;
            Assert.Equal(new int[] { 4, 1, 4 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 1, 4 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListItem_SetWithHandle_ReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Set first.
            collection[0] = 4;
            Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set middle.
            collection[1] = 1;
            Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set last.
            collection[2] = 4;
            Assert.Equal(new int[] { 4, 1, 4 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListItem_SetCustomTabOffsetsWithHandle_ReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = owner.CustomTabOffsets;
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Set first.
            collection[0] = 4;
            Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set middle.
            collection[1] = 1;
            Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set last.
            collection[2] = 4;
            Assert.Equal(new int[] { 4, 1, 4 }, collection.Cast<int>());
            Assert.Equal(new int[] { 4, 1, 4 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ListBoxIntegerCollection_IListItem_SetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = 2);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        public void ListBoxIntegerCollection_IListItem_SetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(1);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = 2);
        }

        [WinFormsTheory]
        [InlineData(-1, null)]
        [InlineData(0, null)]
        [InlineData(1, null)]
        [InlineData(-1, "1")]
        [InlineData(0, "1")]
        [InlineData(1, "1")]
        public void ListBoxIntegerCollection_IListItem_SetValueNotInt_ThrowsArgumentException(int index, object item)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.Throws<ArgumentException>(null, () => collection[index] = item);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_IListAdd_Invoke_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            IList collection = new ListBox.IntegerCollection(owner);

            // Add one.
            Assert.Equal(0, collection.Add(2));
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Add another.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Add same.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Add another.
            Assert.Equal(2, collection.Add(3));
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_IListAdd_InvokeCustomTabOffsets_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            IList collection = owner.CustomTabOffsets;

            // Add one.
            Assert.Equal(0, collection.Add(2));
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Add another.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Add same.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Add another.
            Assert.Equal(2, collection.Add(3));
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 1)]
        [InlineData(true, false, 0)]
        [InlineData(false, true, 1)]
        [InlineData(false, false, 0)]
        public void ListBoxIntegerCollection_IListAdd_InvokeWithHandle_Success(bool sorted, bool useCustomTabOffset, int expectedInvalidatedCallCount)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Add one.
            Assert.Equal(0, collection.Add(2));
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add another.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add same.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add another.
            Assert.Equal(2, collection.Add(3));
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true, 1)]
        [InlineData(true, false, 0)]
        [InlineData(false, true, 1)]
        [InlineData(false, false, 0)]
        public void ListBoxIntegerCollection_IListAdd_InvokeCustomTabOffsetsWithHandle_Success(bool sorted, bool useCustomTabOffset, int expectedInvalidatedCallCount)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            IList collection = owner.CustomTabOffsets;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Add one.
            Assert.Equal(0, collection.Add(2));
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add another.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add same.
            Assert.Equal(0, collection.Add(1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 2 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Add another.
            Assert.Equal(2, collection.Add(3));
            Assert.Equal(3, collection.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 2, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("1")]
        public void ListBoxIntegerCollection_IListAdd_ValueNotInt_ThrowsArgumentException(object item)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.Throws<ArgumentException>(null, () => collection.Add(item));
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListContains_InvokeEmpty_ReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.False(collection.Contains(0));
            Assert.False(collection.Contains(1));
            Assert.False(collection.Contains(new object()));
            Assert.False(collection.Contains(null));
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListContains_InvokeWithValues_ReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);

            // Add one.
            collection.Add(1);
            Assert.False(collection.Contains(0));
            Assert.True(collection.Contains(1));
            Assert.False(collection.Contains(2));
            Assert.False(collection.Contains(new object()));
            Assert.False(collection.Contains(null));
            Assert.False(owner.IsHandleCreated);

            // Add another.
            collection.Add(0);
            Assert.True(collection.Contains(0));
            Assert.True(collection.Contains(1));
            Assert.False(collection.Contains(2));
            Assert.False(collection.Contains(new object()));
            Assert.False(collection.Contains(null));
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListClear_InvokeEmpty_Success()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListClear_InvokeCustomTabOffsetsEmpty_Success()
        {
            using var owner = new ListBox();
            IList collection = owner.CustomTabOffsets;
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListClear_InvokeNotEmpty_Success()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(1);

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListClear_InvokeCustomTabOffsetNotEmpty_Success()
        {
            using var owner = new ListBox();
            IList collection = owner.CustomTabOffsets;
            collection.Add(1);

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListClear_InvokeEmptyWithHande_Success()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListClear_InvokeCustomTabOffsetsEmptyWithHande_Success()
        {
            using var owner = new ListBox();
            IList collection = owner.CustomTabOffsets;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListClear_InvokeNotEmptyWithHande_Success()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;
            collection.Add(1);

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListClear_InvokeCustomTabOffsetNotEmptyWithHande_Success()
        {
            using var owner = new ListBox();
            IList collection = owner.CustomTabOffsets;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;
            collection.Add(1);

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, owner.CustomTabOffsets.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListGetEnumerator_InvokeEmpty_ReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);

            IEnumerator enumerator = collection.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                // Move again.
                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                // Reset.
                enumerator.Reset();
            }
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListGetEnumerator_InvokeNotEmpty_ReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(2);

            IEnumerator enumerator = collection.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Equal(2, enumerator.Current);

                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                // Move again.
                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                // Reset.
                enumerator.Reset();
            }
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListIndexOf_InvokeEmpty_ReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.Equal(-1, collection.IndexOf(0));
            Assert.Equal(-1, collection.IndexOf(1));
            Assert.Equal(-1, collection.IndexOf(new object()));
            Assert.Equal(-1, collection.IndexOf(null));
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBoxIntegerCollection_IListIndexOf_InvokeWithValues_ReturnsExpected()
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);

            // Add one.
            collection.Add(1);
            Assert.Equal(-1, collection.IndexOf(0));
            Assert.Equal(0, collection.IndexOf(1));
            Assert.Equal(-1, collection.IndexOf(2));
            Assert.Equal(-1, collection.IndexOf(new object()));
            Assert.Equal(-1, collection.IndexOf(null));
            Assert.False(owner.IsHandleCreated);

            // Add another.
            collection.Add(0);
            Assert.Equal(0, collection.IndexOf(0));
            Assert.Equal(1, collection.IndexOf(1));
            Assert.Equal(-1, collection.IndexOf(2));
            Assert.Equal(-1, collection.IndexOf(new object()));
            Assert.Equal(-1, collection.IndexOf(null));
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_IListRemove_Invoke_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);

            // Remove middle.
            collection.Remove(2);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove last.
            collection.Remove(3);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            collection.Remove(1);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove no such.
            collection.Remove(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_IListRemove_InvokeCustomTabOffsets_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            IList collection = owner.CustomTabOffsets;
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);

            // Remove middle.
            collection.Remove(2);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Remove last.
            collection.Remove(3);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            collection.Remove(1);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove no such.
            collection.Remove(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_IListRemove_InvokeWithHandle_Success(bool sorted, bool useCustomTabOffset)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Remove middle.
            collection.Remove(2);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove last.
            collection.Remove(3);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove first.
            collection.Remove(1);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove no such.
            collection.Remove(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_IListRemove_InvokeCustomTabOffsetsWithHandle_Success(bool sorted, bool useCustomTabOffset)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            IList collection = owner.CustomTabOffsets;
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Remove middle.
            collection.Remove(2);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove last.
            collection.Remove(3);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove first.
            collection.Remove(1);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove no such.
            collection.Remove(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("1")]
        public void ListBoxIntegerCollection_IListRemove_ValueNotInt_ThrowsArgumentException(object item)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.Throws<ArgumentException>(null, () => collection.Remove(item));
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_IListRemoveAt_Invoke_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);

            // Remove middle.
            collection.RemoveAt(1);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove last.
            collection.RemoveAt(1);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            collection.RemoveAt(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_IListRemoveAt_InvokeCustomTabOffsets_Success(bool sorted, bool useCustomTabOffsets)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffsets
            };
            IList collection = owner.CustomTabOffsets;
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);

            // Remove middle.
            collection.RemoveAt(1);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Remove last.
            collection.RemoveAt(1);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1 }, owner.CustomTabOffsets.Cast<int>());
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            collection.RemoveAt(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_IListRemoveAt_InvokeWithHandle_Success(bool sorted, bool useCustomTabOffset)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Remove middle.
            collection.RemoveAt(1);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove last.
            collection.RemoveAt(1);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove first.
            collection.RemoveAt(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ListBoxIntegerCollection_IListRemoveAt_InvokeCustomTabOffsetsWithHandle_Success(bool sorted, bool useCustomTabOffset)
        {
            using var owner = new ListBox
            {
                Sorted = sorted,
                UseCustomTabOffsets = useCustomTabOffset
            };
            IList collection = owner.CustomTabOffsets;
            collection.Add(2);
            collection.Add(1);
            collection.Add(1);
            collection.Add(3);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int invalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            owner.HandleCreated += (sender, e) => createdCallCount++;

            // Remove middle.
            collection.RemoveAt(1);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1, 3 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove last.
            collection.RemoveAt(1);
            Assert.Equal(1, collection.Count);
            Assert.Equal(new int[] { 1 }, collection.Cast<int>());
            Assert.Equal(new int[] { 1 }, owner.CustomTabOffsets.Cast<int>());
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove first.
            collection.RemoveAt(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Empty(owner.CustomTabOffsets);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ListBoxIntegerCollection_IListRemoveAt_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        public void ListBoxIntegerCollection_IListRemoveAt_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var owner = new ListBox();
            IList collection = new ListBox.IntegerCollection(owner);
            collection.Add(1);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
        }
    }
}
