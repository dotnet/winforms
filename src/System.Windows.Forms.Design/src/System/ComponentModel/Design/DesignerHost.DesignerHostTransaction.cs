// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerHost
    {
        /// <summary>
        ///  DesignerHostTransaction is our implementation of the  DesignerTransaction abstract class.
        /// </summary>
        private sealed class DesignerHostTransaction : DesignerTransaction
        {
            private DesignerHost _host;

            public DesignerHostTransaction(DesignerHost host, string description) : base(description)
            {
                _host = host;
                _host._transactions ??= new Stack();

                _host._transactions.Push(this);
                _host.OnTransactionOpening(EventArgs.Empty);
                _host.OnTransactionOpened(EventArgs.Empty);
            }

            /// <summary>
            ///  User code should implement this method to perform the actual work of committing a transaction.
            /// </summary>
            protected override void OnCancel()
            {
                if (_host is not null)
                {
                    if (_host._transactions.Peek() != this)
                    {
                        string nestedDescription = ((DesignerTransaction)_host._transactions.Peek()).Description;
                        throw new InvalidOperationException(string.Format(SR.DesignerHostNestedTransaction, Description, nestedDescription));
                    }

                    _host.IsClosingTransaction = true;
                    try
                    {
                        _host._transactions.Pop();
                        DesignerTransactionCloseEventArgs e = new DesignerTransactionCloseEventArgs(false, _host._transactions.Count == 0);
                        _host.OnTransactionClosing(e);
                        _host.OnTransactionClosed(e);
                    }
                    finally
                    {
                        _host.IsClosingTransaction = false;
                        _host = null;
                    }
                }
            }

            /// <summary>
            ///  User code should implement this method to perform the actual work of committing a transaction.
            /// </summary>
            protected override void OnCommit()
            {
                if (_host is not null)
                {
                    if (_host._transactions.Peek() != this)
                    {
                        string nestedDescription = ((DesignerTransaction)_host._transactions.Peek()).Description;
                        throw new InvalidOperationException(string.Format(SR.DesignerHostNestedTransaction, Description, nestedDescription));
                    }

                    _host.IsClosingTransaction = true;
                    try
                    {
                        _host._transactions.Pop();
                        DesignerTransactionCloseEventArgs e = new DesignerTransactionCloseEventArgs(true, _host._transactions.Count == 0);
                        _host.OnTransactionClosing(e);
                        _host.OnTransactionClosed(e);
                    }
                    finally
                    {
                        _host.IsClosingTransaction = false;
                        _host = null;
                    }
                }
            }
        }
    }
}
