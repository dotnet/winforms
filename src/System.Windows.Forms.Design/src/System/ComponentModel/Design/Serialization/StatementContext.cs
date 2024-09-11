// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Collections;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  This object can be placed on the context stack to provide a place for
///  statements to be serialized into. Normally, statements are serialized
///  into whatever statement collection that is on the context stack. You
///  can modify this behavior by creating a statement context and calling
///  Populate with a collection of objects whose statements you would like
///  stored in the statement table. As each object is serialized in
///  SerializeToExpression it will have its contents placed in the statement
///  table. saved in a table within the context. If you push this object on
///  the stack it is your responsibility to integrate the statements added
///  to it into your own collection of statements.
/// </summary>
public sealed class StatementContext
{
    private ObjectStatementCollection? _statements;

    /// <summary>
    ///  This is a table of statements that is offered by the statement context.
    /// </summary>
    public ObjectStatementCollection StatementCollection => _statements ??= new ObjectStatementCollection();
}

/// <summary>
///  This is a table of statements that is offered by the statement context.
/// </summary>
public sealed class ObjectStatementCollection : IEnumerable
{
    private List<TableEntry>? _table;
    private int _version;

    /// <summary>
    ///  Only creatable by the StatementContext.
    /// </summary>
    internal ObjectStatementCollection()
    {
    }

    /// <summary>
    ///  Adds an owner to the table. Statements can be null, in which
    ///  case it will be demand created when fished out of the table.
    ///  This will throw if there is already a valid collection for the owner.
    /// </summary>
    private void AddOwner(object statementOwner, CodeStatementCollection? statements)
    {
        if (_table is null)
        {
            _table = [];
        }
        else
        {
            for (int idx = 0; idx < _table.Count; idx++)
            {
                if (ReferenceEquals(_table[idx].Owner, statementOwner))
                {
                    if (_table[idx].Statements is not null)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        if (statements is not null)
                        {
                            _table[idx] = new TableEntry(statementOwner, statements);
                        }

                        return;
                    }
                }
            }
        }

        _table.Add(new TableEntry(statementOwner, statements));
        _version++;
    }

    /// <summary>
    ///  Indexer. This will return the statement collection for the given owner.
    ///  It will return null only if the owner is not in the table.
    /// </summary>
    public CodeStatementCollection? this[object statementOwner]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(statementOwner);

            if (_table is not null)
            {
                for (int idx = 0; idx < _table.Count; idx++)
                {
                    if (ReferenceEquals(_table[idx].Owner, statementOwner))
                    {
                        if (_table[idx].Statements is null)
                        {
                            _table[idx] = new TableEntry(statementOwner, []);
                        }

                        return _table[idx].Statements;
                    }
                }

                foreach (TableEntry e in _table)
                {
                    if (ReferenceEquals(e.Owner, statementOwner))
                    {
                        return e.Statements;
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    ///  Returns true if the given statement owner is in the table.
    /// </summary>
    public bool ContainsKey(object statementOwner)
    {
        ArgumentNullException.ThrowIfNull(statementOwner);

        if (_table is not null)
        {
            return (this[statementOwner] is not null);
        }

        return false;
    }

    /// <summary>
    ///  Returns an enumerator for this table.
    ///  The keys of the enumerator are statement owner objects and the values are instances of CodeStatementCollection.
    /// </summary>
    public IDictionaryEnumerator GetEnumerator()
    {
        return new TableEnumerator(this);
    }

    /// <summary>
    ///  This method populates the statement table with a collection of statement owners.
    ///  The creator of the statement context should do this if it wishes statement tables
    ///  to be used to store values for certain objects.
    /// </summary>
    public void Populate(ICollection statementOwners)
    {
        ArgumentNullException.ThrowIfNull(statementOwners);

        foreach (object o in statementOwners)
        {
            Populate(o);
        }
    }

    /// <summary>
    ///  This method populates the statement table with a collection of statement owners.
    ///  The creator of the statement context should do this if it wishes statement tables to be used to
    ///  store values for certain objects.
    /// </summary>
    public void Populate(object owner)
    {
        ArgumentNullException.ThrowIfNull(owner);

        AddOwner(owner, null);
    }

    /// <summary>
    ///  Returns an enumerator for this table.
    ///  The value is a DictionaryEntry containing the statement owner and the statement collection.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private struct TableEntry
    {
        public object Owner;
        public CodeStatementCollection? Statements;
        public TableEntry(object owner, CodeStatementCollection? statements)
        {
            Owner = owner;
            Statements = statements;
        }
    }

    private struct TableEnumerator : IDictionaryEnumerator
    {
        private readonly ObjectStatementCollection _table;
        private readonly int _version;
        private int _position;

        public TableEnumerator(ObjectStatementCollection table)
        {
            _table = table;
            _version = _table._version;
            _position = -1;
        }

        public readonly object Current => Entry;

        public readonly DictionaryEntry Entry
        {
            get
            {
                if (_version != _table._version)
                {
                    throw new InvalidOperationException();
                }

                if (_position < 0 || _table._table is null || _position >= _table._table.Count)
                {
                    throw new InvalidOperationException();
                }

                if (_table._table[_position].Statements is null)
                {
                    _table._table[_position] = new TableEntry(_table._table[_position].Owner, []);
                }

                TableEntry entry = _table._table[_position];
                return new DictionaryEntry(entry.Owner, entry.Statements);
            }
        }

        public readonly object Key => Entry.Key;

        public readonly object? Value => Entry.Value;

        public bool MoveNext()
        {
            if (_table._table is not null && (_position + 1) < _table._table.Count)
            {
                _position++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _position = -1;
        }
    }
}
