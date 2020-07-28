// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class SinglyLinkedListTests
    {
        [Fact]
        public void AddFirst()
        {
            var list = new SinglyLinkedList<int>();

            Assert.Equal(0, list.Count);
            Assert.Null(list.First);
            Assert.Null(list.Last);

            list.AddFirst(1);
            Assert.Equal(1, list.Count);
            Assert.NotNull(list.First);
            Assert.NotNull(list.Last);
            Assert.Same(list.First, list.Last);
            Assert.Null(list.First!.Next);
            Assert.Equal(1, list.First);

            list.AddFirst(2);
            Assert.Equal(2, list.Count);
            Assert.NotNull(list.First);
            Assert.NotNull(list.Last);
            Assert.NotSame(list.First, list.Last);
            Assert.Same(list.First.Next, list.Last);
            Assert.Null(list.Last!.Next);
            Assert.Equal(2, list.First);
            Assert.Equal(1, list.Last);
        }

        [Fact]
        public void AddLast()
        {
            var list = new SinglyLinkedList<int>();

            Assert.Equal(0, list.Count);
            Assert.Null(list.First);
            Assert.Null(list.Last);

            list.AddLast(1);
            Assert.Equal(1, list.Count);
            Assert.NotNull(list.First);
            Assert.NotNull(list.Last);
            Assert.Same(list.First, list.Last);
            Assert.Null(list.First!.Next);
            Assert.Equal(1, list.First);

            list.AddLast(2);
            Assert.Equal(2, list.Count);
            Assert.NotNull(list.First);
            Assert.NotNull(list.Last);
            Assert.NotSame(list.First, list.Last);
            Assert.Same(list.First.Next, list.Last);
            Assert.Null(list.Last!.Next);
            Assert.Equal(1, list.First);
            Assert.Equal(2, list.Last);
        }

        [Fact]
        public void MoveToFront()
        {
            var list = new SinglyLinkedList<int>();
            list.AddAll(1, 2, 3, 4, 5);

            var enumerator = list.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            enumerator.MoveCurrentToFront();

            // Should be moved back in front
            Assert.Null(enumerator.Current);
            Assert.Equal(5, list.Count);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(1, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);
            enumerator.MoveCurrentToFront();
            Assert.Equal(1, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(3, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(4, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(5, enumerator.Current);

            enumerator.MoveCurrentToFront();
            Assert.Equal(4, enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Null(enumerator.Current);
            Assert.Equal(5, list!.First);

            Assert.Equal(new int[] { 5, 2, 1, 3, 4 }, list.WalkToList());
            Assert.Equal(new int[] { 5, 2, 1, 3, 4 }, list.EnumerateToList());
        }

        [Fact]
        public void MoveToFront_InvalidOperations()
        {
            var list = new SinglyLinkedList<int>();
            list.AddFirst(1);

            var enumerator = list.GetEnumerator();
            Assert.Throws<InvalidOperationException>(() => enumerator.MoveCurrentToFront());
            Assert.True(enumerator.MoveNext());
            enumerator.MoveCurrentToFront();
            Assert.Throws<InvalidOperationException>(() => enumerator.MoveCurrentToFront());
        }

        [Fact]
        public void RemoveCurrent()
        {
            var list = new SinglyLinkedList<int>();
            list.AddAll(1, 2, 3, 4, 5);

            var enumerator = list.GetEnumerator();
            Assert.True(enumerator.MoveNext());

            enumerator.RemoveCurrent();
            Assert.Equal(4, list.Count);
            Assert.Null(enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(3, enumerator.Current);
            enumerator.RemoveCurrent();
            Assert.Equal(3, list.Count);
            Assert.Equal(2, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.True(enumerator.MoveNext());
            Assert.Equal(5, enumerator.Current);
            enumerator.RemoveCurrent();
            Assert.Equal(2, list.Count);
            Assert.Equal(4, enumerator.Current);
            Assert.False(enumerator.MoveNext());
            Assert.Null(enumerator.Current);

            Assert.Equal(new int[] { 2, 4 }, list.WalkToList());
            Assert.Equal(new int[] { 2, 4 }, list.EnumerateToList());
        }

        [Fact]
        public void RemoveCurrent_InvalidOperations()
        {
            var list = new SinglyLinkedList<int>();
            list.AddFirst(1);
            list.AddLast(2);

            var enumerator = list.GetEnumerator();
            Assert.Throws<InvalidOperationException>(() => enumerator.RemoveCurrent());
            Assert.True(enumerator.MoveNext());
            enumerator.RemoveCurrent();
            Assert.Throws<InvalidOperationException>(() => enumerator.RemoveCurrent());
        }
    }

    internal static class ListExtensions
    {
        public static void AddAll<T>(this SinglyLinkedList<T> linkedList, params T[] values)
        {
            foreach (T value in values)
            {
                linkedList.AddLast(value);
            }
        }

        public static List<T> WalkToList<T>(this SinglyLinkedList<T> linkedList)
        {
            List<T> list = new List<T>(linkedList.Count);
            var node = linkedList.First;
            while (node != null)
            {
                list.Add(node);
                node = node.Next;
            }
            return list;
        }

        public static List<T> EnumerateToList<T>(this SinglyLinkedList<T> linkedList)
        {
            List<T> list = new List<T>(linkedList.Count);
            var enumerator = linkedList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            return list;
        }
    }
}
