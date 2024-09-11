// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

internal sealed partial class DesignerHost
{
    /// <summary>
    ///  DesignerHostTransaction is our implementation of the DesignerTransaction abstract class.
    /// </summary>
    private sealed class DesignerHostTransaction : DesignerTransaction
    {
        private DesignerHost? _host;

        public DesignerHostTransaction(DesignerHost host, string description) : base(description)
        {
            _host = host;
            _host._transactions ??= new Stack<DesignerTransaction>();

            _host._transactions.Push(this);
            _host.OnTransactionOpening(EventArgs.Empty);
            _host.OnTransactionOpened(EventArgs.Empty);
        }

        /// <summary>
        ///  User code should implement this method to perform the actual work of committing a transaction.
        /// </summary>
        protected override void OnCancel()
        {
            if (_host is null)
            {
                return;
            }

            Stack<DesignerTransaction> transactions = _host._transactions!;

            if (transactions.Peek() != this)
            {
                string nestedDescription = transactions.Peek().Description;
                throw new InvalidOperationException(string.Format(SR.DesignerHostNestedTransaction, Description, nestedDescription));
            }

            _host.IsClosingTransaction = true;
            try
            {
                transactions.Pop();
                DesignerTransactionCloseEventArgs e = new(false, transactions.Count == 0);
                _host.OnTransactionClosing(e);
                _host.OnTransactionClosed(e);
            }
            finally
            {
                _host.IsClosingTransaction = false;
                _host = null;
            }
        }

        /// <summary>
        ///  User code should implement this method to perform the actual work of committing a transaction.
        /// </summary>
        protected override void OnCommit()
        {
            if (_host is null)
            {
                return;
            }

            Stack<DesignerTransaction> transactions = _host._transactions!;

            if (transactions.Peek() != this)
            {
                string nestedDescription = transactions.Peek().Description;
                throw new InvalidOperationException(string.Format(SR.DesignerHostNestedTransaction, Description, nestedDescription));
            }

            _host.IsClosingTransaction = true;
            try
            {
                transactions.Pop();
                DesignerTransactionCloseEventArgs e = new(true, transactions.Count == 0);
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
