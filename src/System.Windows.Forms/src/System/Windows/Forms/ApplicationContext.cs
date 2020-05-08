// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    ///  ApplicationContext provides contextual information about an application
    ///  thread. Specifically this allows an application author to redifine what
    ///  circurmstances cause a message loop to exit. By default the application
    ///  context listens to the close event on the mainForm, then exits the
    ///  thread's message loop.
    /// </summary>
    public class ApplicationContext : IDisposable
    {
        private Form? _mainForm;

        /// <summary>
        ///  Creates a new ApplicationContext with no mainForm.
        /// </summary>
        public ApplicationContext() : this(null)
        {
        }

        /// <summary>
        ///  Creates a new ApplicationContext with the specified mainForm.
        ///  If OnMainFormClosed is not overriden, the thread's message
        ///  loop will be terminated when mainForm is closed.
        /// </summary>
        public ApplicationContext(Form? mainForm)
        {
            MainForm = mainForm;
        }

        ~ApplicationContext() => Dispose(false);

        /// <summary>
        ///  Determines the mainForm for this context. This may be changed
        ///  at anytime.
        ///  If OnMainFormClosed is not overriden, the thread's message
        ///  loop will be terminated when mainForm is closed.
        /// </summary>
        public Form? MainForm
        {
            get => _mainForm;
            set
            {
                EventHandler onClose = OnMainFormDestroy;
                if (_mainForm != null)
                {
                    _mainForm.HandleDestroyed -= onClose;
                }

                _mainForm = value;

                if (_mainForm != null)
                {
                    _mainForm.HandleDestroyed += onClose;
                }
            }
        }

        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object? Tag { get; set; }

        /// <summary>
        ///  Is raised when the thread's message loop should be terminated.
        ///  This is raised by calling ExitThread.
        /// </summary>
        public event EventHandler? ThreadExit;

        /// <summary>
        ///  Disposes the context. This should dispose the mainForm. This is
        ///  called immediately after the thread's message loop is terminated.
        ///  Application will dispose all forms on this thread by default.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_mainForm != null)
                {
                    if (!_mainForm.IsDisposed)
                    {
                        _mainForm.Dispose();
                    }

                    _mainForm = null;
                }
            }
        }

        /// <summary>
        ///  Causes the thread's message loop to be terminated. This will call ExitThreadCore.
        /// </summary>
        public void ExitThread() => ExitThreadCore();

        /// <summary>
        ///  Causes the thread's message loop to be terminated.
        /// </summary>
        protected virtual void ExitThreadCore() => ThreadExit?.Invoke(this, EventArgs.Empty);

        /// <summary>
        ///  Called when the mainForm is closed. The default implementation
        ///  of this will call ExitThreadCore.
        /// </summary>
        protected virtual void OnMainFormClosed(object? sender, EventArgs e) => ExitThreadCore();

        /// <summary>
        ///  Called when the mainForm is closed. The default implementation
        ///  of this will call ExitThreadCore.
        /// </summary>
        private void OnMainFormDestroy(object? sender, EventArgs e)
        {
            Debug.Assert(sender is Form);
            Form form = (Form)sender;
            if (!form.RecreatingHandle)
            {
                form.HandleDestroyed -= new EventHandler(OnMainFormDestroy);
                OnMainFormClosed(sender, e);
            }
        }
    }
}
