// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms
{
    public partial class ToolStripControlHost
    {
        /// <remarks>
        ///  Our implementation of ISite:
        ///  Since the Control which is wrapped by ToolStripControlHost is a runtime instance, there is no way of knowing
        ///  whether the control is in runtime or designtime.
        ///  This implementation of ISite would be set to Control.Site when ToolStripControlHost.Site is set at DesignTime. (Refer to Site property on ToolStripControlHost)
        ///  This implementation just returns the DesigMode property to be ToolStripControlHost's DesignMode property.
        ///  Everything else is pretty much default implementation.
        /// </remarks>
        private class StubSite : ISite, IDictionaryService
        {
            private Hashtable _dictionary;
            readonly IComponent _comp;
            readonly IComponent _owner;

            public StubSite(IComponent control, IComponent host)
            {
                _comp = control;
                _owner = host;
            }

            /// <summary>
            ///  When implemented by a class, gets the component associated with the <see cref='ISite'/>.
            /// </summary>
            IComponent ISite.Component => _comp;

            /// <summary>
            ///  When implemented by a class, gets the container associated with the <see cref='ISite'/>.
            /// </summary>
            IContainer ISite.Container => _owner.Site.Container;

            /// <summary>
            ///  When implemented by a class, determines whether the component is in design mode.
            /// </summary>
            bool ISite.DesignMode => _owner.Site.DesignMode;

            /// <summary>
            ///  When implemented by a class, gets or sets the name of
            ///  the component associated with the <see cref='ISite'/>.
            /// </summary>
            string ISite.Name
            {
                get => _owner.Site.Name;
                set => _owner.Site.Name = value;
            }

            /// <summary>
            ///  Returns the requested service.
            /// </summary>
            object IServiceProvider.GetService(Type service)
            {
                if (service is null)
                {
                    throw new ArgumentNullException(nameof(service));
                }

                // We have to implement our own dictionary service. If we don't,
                // the properties of the underlying component will end up being
                // overwritten by our own properties when GetProperties is called
                if (service == typeof(IDictionaryService))
                {
                    return this;
                }

                return _owner.Site?.GetService(service);
            }

            /// <summary>
            ///  Retrieves the key corresponding to the given value.
            /// </summary>
            object IDictionaryService.GetKey(object value)
            {
                if (_dictionary != null)
                {
                    foreach (DictionaryEntry de in _dictionary)
                    {
                        object o = de.Value;
                        if (value != null && value.Equals(o))
                        {
                            return de.Key;
                        }
                    }
                }

                return null;
            }

            /// <summary>
            ///  Retrieves the value corresponding to the given key.
            /// </summary>
            object IDictionaryService.GetValue(object key)
            {
                if (_dictionary != null)
                {
                    return _dictionary[key];
                }

                return null;
            }

            /// <summary>
            ///  Stores the given key-value pair in an object's site.  This key-value
            ///  pair is stored on a per-object basis, and is a handy place to save
            ///  additional information about a component.
            /// </summary>
            void IDictionaryService.SetValue(object key, object value)
            {
                if (_dictionary is null)
                {
                    _dictionary = new Hashtable();
                }

                if (value is null)
                {
                    _dictionary.Remove(key);
                }
                else
                {
                    _dictionary[key] = value;
                }
            }
        }
    }
}
