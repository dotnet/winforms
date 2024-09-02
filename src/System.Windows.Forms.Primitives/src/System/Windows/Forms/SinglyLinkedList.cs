// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

/// <devdoc>
///  This class is used in <see cref="RefCountedCache{TObject, TCacheEntryData, TKey}"/> which is performance
///  sensitive. Do not make changes without validating performance impacts.
/// </devdoc>
internal class SinglyLinkedList<T>
{
    public SinglyLinkedList() { }

    public int Count { get; private set; }
    public Node? First { get; private set; }
    public Node? Last { get; private set; }

    public Node AddFirst(T value)
    {
        Node node = new(value);

        if (Count == 0)
        {
            // Nothing in the list yet
            First = Last = node;
        }
        else
        {
            // Last doesn't change, insert in the front
            node.Next = First;
            First = node;
        }

        Count++;
        return node;
    }

    public Node AddLast(T value)
    {
        Node node = new(value);

        if (Count == 0)
        {
            // Nothing in the list yet
            First = Last = node;
        }
        else
        {
            // Add at the end
            Debug.Assert(First is not null && Last is not null);
            Last!.Next = node;
            Last = node;
        }

        Count++;
        return node;
    }

    public Enumerator GetEnumerator() => new(this);

    public class Node(T value)
    {
        public Node? Next { get; set; }
        public T Value { get; set; } = value;

        public static implicit operator T(Node? node) => node is null ? default! : node.Value;
    }

    public struct Enumerator(SinglyLinkedList<T> list)
    {
        private readonly SinglyLinkedList<T> _list = list;
        private bool _removed = false;
        private bool _finished = false;
        private Node? _current = null;
        private Node? _previous = null;

        public readonly Node Current => _current!;

        /// <summary>
        ///  Resets the enumerator.
        /// </summary>
        public void Reset()
        {
            _finished = false;
            _removed = false;
            _current = null;
            _previous = null;
        }

        /// <summary>
        ///  Attempts to move to the next node. Sets <see cref="Current"/> when successful. If there are no more
        ///  nodes returns false and <see cref="Current"/> will be null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            // This code has been carefully laid out to optimize for successful MoveNext(). Do not change the
            // code without careful performance measurement.

            _removed = false;
            bool result = true;
            if (_finished || _list.Count == 0)
            {
                result = false;
            }
            else if (_current is null)
            {
                // At the start
                _current = _list.First;
            }
            else if (_current.Next is not null)
            {
                // Not to the end yet
                _previous = _current;
                _current = _current.Next;
            }
            else
            {
                // At the end
                _finished = true;
                _current = null;
                result = false;
            }

            return result;
        }

        /// <summary>
        ///  Removes the <see cref="Current"/> node. Note that this will make <see cref="Current"/> the prior node
        ///  so that <see cref="MoveNext"/> will place on the next node (if any).
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///  Thrown if attempting to remove without successfully calling <see cref="MoveNext"/>. Will also
        ///  throw if <see cref="RemoveCurrent"/> or <see cref="MoveCurrentToFront"/> is called more than once
        ///  without calling <see cref="MoveNext"/>.
        /// </exception>
        public void RemoveCurrent()
        {
            if (_current is null || _removed)
                throw new InvalidOperationException();

            // Do not change the code without careful performance measurement.

            // MoveNext after removing should give the next node, if any

            if (_current == _list.First)
            {
                // Front of the list, set next to first
                _list.First = _current.Next;
                _current = null;
            }
            else if (_current == _list.Last)
            {
                // End of list, set last to previous
                Debug.Assert(_previous is not null);
                _list.Last = _previous;
                _previous!.Next = null;
                _current = _previous;
            }
            else
            {
                // In the middle
                Debug.Assert(_previous is not null);
                Node? next = _current.Next;
                _current = _previous;
                _previous!.Next = next;
            }

            _removed = true;
            _list.Count--;
        }

        /// <summary>
        ///  Moves the <see cref="Current"/> node to the front of the list. Note that this will make
        ///  <see cref="Current"/> the prior node so that <see cref="MoveNext"/> will place on the next node
        ///  (if any).
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///  Thrown if attempting to move without successfully calling <see cref="MoveNext"/>. Will also
        ///  throw if <see cref="RemoveCurrent"/> or <see cref="MoveCurrentToFront"/> is called more than once
        ///  without calling <see cref="MoveNext"/>.
        /// </exception>
        public void MoveCurrentToFront()
        {
            if (_current is null || _removed)
                throw new InvalidOperationException();

            if (_current == _list.First)
            {
                // Already at the front, reposition to the start
                _current = null;
                return;
            }

            Debug.Assert(_previous is not null);

            if (_current.Next is null)
            {
                // End of the list, set last to the prior node and clear next
                Debug.Assert(_list.Last == _current);
                _list.Last = _previous;
                _previous.Next = null;
            }
            else
            {
                // In the middle, attach the prior node to the next node
                _previous.Next = _current.Next;
            }

            // Set the current node's next node to the current first node and set to first.
            _current.Next = _list.First;
            _list.First = _current;

            // Set current to the prior node so MoveNext moves to the next node
            _current = _previous;
            _removed = true;
        }
    }
}
