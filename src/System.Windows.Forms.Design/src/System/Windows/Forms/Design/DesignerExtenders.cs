// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    /// <devdoc>
    ///     This class provides the Modifiers property to components.  It is shared between
    ///     the document designer and the component document designer.
    /// </devdoc>
    internal partial class DesignerExtenders
    {

        private IExtenderProvider[] providers;
        private IExtenderProviderService extenderService;

        /// <include file='doc\DesignerExtenders.uex' path='docs/doc[@for="DesignerExtenders.AddExtenderProviders"]/*' />
        /// <devdoc>
        ///     This is called by a root designer to add the correct extender providers.
        /// </devdoc>
        public DesignerExtenders(IExtenderProviderService ex)
        {
            this.extenderService = ex;
            if (providers == null)
            {
                providers = new IExtenderProvider[] {
                    new NameExtenderProvider(),
                    new NameInheritedExtenderProvider()
                };
            }

            for (int i = 0; i < providers.Length; i++)
            {
                ex.AddExtenderProvider(providers[i]);
            }
        }

        /// <include file='doc\DesignerExtenders.uex' path='docs/doc[@for="DesignerExtenders.RemoveExtenderProviders"]/*' />
        /// <devdoc>
        ///      This is called at the appropriate time to remove any extra extender
        ///      providers previously added to the designer host.
        /// </devdoc>
        public void Dispose()
        {
            if (extenderService != null && providers != null)
            {
                for (int i = 0; i < providers.Length; i++)
                {
                    extenderService.RemoveExtenderProvider(providers[i]);
                }

                providers = null;
                extenderService = null;
            }
        }
    }
}

