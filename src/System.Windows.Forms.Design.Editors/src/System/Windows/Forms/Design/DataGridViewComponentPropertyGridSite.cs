// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms.Design
{
    internal class DataGridViewComponentPropertyGridSite : ISite
    {

        private IServiceProvider sp;
        private IComponent comp;
        private bool inGetService = false;

        public DataGridViewComponentPropertyGridSite(IServiceProvider sp, IComponent comp)
        {
            this.sp = sp;
            this.comp = comp;
        }

        /** The component sited by this component site. */
        /// <include file='doc\ISite.uex' path='docs/doc[@for="ISite.Component"]/*' />
        /// <devdoc>
        ///    <para>When implemented by a class, gets the component associated with the <see cref='System.ComponentModel.ISite'/>.</para>
        /// </devdoc>
        public IComponent Component { get { return comp; } }

        /** The container in which the component is sited. */
        /// <include file='doc\ISite.uex' path='docs/doc[@for="ISite.Container"]/*' />
        /// <devdoc>
        /// <para>When implemented by a class, gets the container associated with the <see cref='System.ComponentModel.ISite'/>.</para>
        /// </devdoc>
        public IContainer Container { get { return null; } }

        /** Indicates whether the component is in design mode. */
        /// <include file='doc\ISite.uex' path='docs/doc[@for="ISite.DesignMode"]/*' />
        /// <devdoc>
        ///    <para>When implemented by a class, determines whether the component is in design mode.</para>
        /// </devdoc>
        public bool DesignMode { get { return false; } }

        /**
         * The name of the component.
         */
        /// <include file='doc\ISite.uex' path='docs/doc[@for="ISite.Name"]/*' />
        /// <devdoc>
        ///    <para>When implemented by a class, gets or sets the name of
        ///       the component associated with the <see cref='System.ComponentModel.ISite'/>.</para>
        /// </devdoc>
        public string Name
        {
            get { return null; }
            set { }
        }

        public object GetService(Type t)
        {
            if (!inGetService && sp != null)
            {
                try
                {
                    inGetService = true;
                    return sp.GetService(t);
                }
                finally
                {
                    inGetService = false;
                }
            }
            return null;
        }
    }
}
