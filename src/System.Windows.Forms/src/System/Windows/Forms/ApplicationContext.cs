// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Diagnostics;
    using System.ComponentModel;

    /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for="ApplicationContext"]/*' />
    /// <devdoc>
    ///    ApplicationContext provides contextual information about an application
    ///    thread. Specifically this allows an application author to redifine what
    ///    circurmstances cause a message loop to exit. By default the application
    ///    context listens to the close event on the mainForm, then exits the
    ///    thread's message loop.
    /// </devdoc>
    public class ApplicationContext : IDisposable {
        Form mainForm;
        object userData;

        /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for="ApplicationContext.ApplicationContext"]/*' />
        /// <devdoc>
        ///     Creates a new ApplicationContext with no mainForm.
        /// </devdoc>
        public ApplicationContext() : this(null) {
        }

        /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for="ApplicationContext.ApplicationContext1"]/*' />
        /// <devdoc>
        ///     Creates a new ApplicationContext with the specified mainForm.
        ///     If OnMainFormClosed is not overriden, the thread's message
        ///     loop will be terminated when mainForm is closed.
        /// </devdoc>
        public ApplicationContext(Form mainForm) {
            this.MainForm = mainForm;
        }

        /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for=".Finalize"]/*' />
        ~ApplicationContext() {
            Dispose(false);
        }

        /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for="ApplicationContext.MainForm"]/*' />
        /// <devdoc>
        ///     Determines the mainForm for this context. This may be changed
        ///     at anytime.
        ///     If OnMainFormClosed is not overriden, the thread's message
        ///     loop will be terminated when mainForm is closed.
        /// </devdoc>
        public Form MainForm {
            get {
                return mainForm;
            }
            set {
                EventHandler onClose = new EventHandler(OnMainFormDestroy);
                if (mainForm != null) {
                    mainForm.HandleDestroyed -= onClose;
                }

                mainForm = value;

                if (mainForm != null) {
                    mainForm.HandleDestroyed += onClose;
                }
            }
        }

        /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for="ApplicationContext.Tag"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag {
            get {
                return userData;
            }
            set {
                userData = value;
            }
        }

        /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for="ApplicationContext.ThreadExit"]/*' />
        /// <devdoc>
        ///     Is raised when the thread's message loop should be terminated.
        ///     This is raised by calling ExitThread.
        /// </devdoc>
        public event EventHandler ThreadExit;

        /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for="ApplicationContext.Dispose"]/*' />
        /// <devdoc>
        ///     Disposes the context. This should dispose the mainForm. This is
        ///     called immediately after the thread's message loop is terminated.
        ///     Application will dispose all forms on this thread by default.
        /// </devdoc>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for="ApplicationContext.Dispose2"]/*' />
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (mainForm != null) {
                    if (!mainForm.IsDisposed) {
                        mainForm.Dispose();
                    }
                    mainForm = null;
                }
            }
        }

        /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for="ApplicationContext.ExitThread"]/*' />
        /// <devdoc>
        ///     Causes the thread's message loop to be terminated. This
        ///     will call ExitThreadCore.
        /// </devdoc>
        public void ExitThread() {
            ExitThreadCore();
        }

        /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for="ApplicationContext.ExitThreadCore"]/*' />
        /// <devdoc>
        ///     Causes the thread's message loop to be terminated.
        /// </devdoc>
        protected virtual void ExitThreadCore() {
            if (ThreadExit != null) {
                ThreadExit(this, EventArgs.Empty);
            }
        }

        /// <include file='doc\ApplicationContext.uex' path='docs/doc[@for="ApplicationContext.OnMainFormClosed"]/*' />
        /// <devdoc>
        ///     Called when the mainForm is closed. The default implementation
        ///     of this will call ExitThreadCore.
        /// </devdoc>
        protected virtual void OnMainFormClosed(object sender, EventArgs e) {
            ExitThreadCore();
        }
    
        /// <devdoc>
        ///     Called when the mainForm is closed. The default implementation
        ///     of this will call ExitThreadCore.
        /// </devdoc>
        private void OnMainFormDestroy(object sender, EventArgs e) {
            Form form = (Form)sender;
            if (!form.RecreatingHandle) {
                form.HandleDestroyed -= new EventHandler(OnMainFormDestroy);
                OnMainFormClosed(sender, e);
            }
        }
    }
}

