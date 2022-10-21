// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class is responsible for managing a set or properties for a native object.  It determines
    ///  when the properties need to be refreshed, and owns the extended handlers for those properties.
    /// </summary>
    internal class Com2Properties
    {
#if DEBUG
        private static readonly TraceSwitch DbgCom2PropertiesSwitch = new TraceSwitch("DbgCom2Properties", "Com2Properties: debug Com2 properties manager");
#endif

        // This is the interval that we'll hold properties for. If someone doesn't touch an object for this amount of time,
        // we'll dump the properties from our cache. 5 minutes -- ticks are 1/10,000,000th of a second
        private const long AgeThreshold = 10000000L * 60L * 5L;

        // This is the object that gave us the properties.  We hold a WeakRef so we don't addref the object.
        private WeakReference _weakObjectReference;

        private Com2PropertyDescriptor[] _properties;

        private readonly int _defaultPropertyIndex = -1;

        // The timestamp of the last operation on this property manager, usually when the property list was fetched.
        private long _touchedTime;

        // For non-IProvideMultipleClassInfo ITypeInfos, this is the version number on the last ITypeInfo we looked at.
        // If this changes, we know we need to dump the cache.
        private (ushort FunctionCount, ushort VariableCount, ushort MajorVersion, ushort MinorVersion)[] _typeInfoVersions;

        private int alwaysValid;

        // The interfaces we recognize for extended browsing.
        private static readonly Type[] s_extendedInterfaces = new Type[]
        {
            typeof(VSSDK.ICategorizeProperties),
            typeof(VSSDK.IProvidePropertyBuilder),
            typeof(IPerPropertyBrowsing.Interface),
            typeof(VSSDK.IVsPerPropertyBrowsing),
            typeof(VSSDK.IVSMDPerPropertyBrowsing)
        };

        // The handler classes corresponding to the extended interfaces above.
        private static readonly Type[] s_extendedInterfaceHandlerTypes = new Type[]
        {
            typeof(Com2ICategorizePropertiesHandler),
            typeof(Com2IProvidePropertyBuilderHandler),
            typeof(Com2IPerPropertyBrowsingHandler),
            typeof(Com2IVsPerPropertyBrowsingHandler),
            typeof(Com2IManagedPerPropertyBrowsingHandler)
        };

        public event EventHandler Disposed;

        public Com2Properties(object obj, Com2PropertyDescriptor[] props, int defaultIndex)
        {
#if DEBUG
            if (DbgCom2PropertiesSwitch.TraceVerbose)
            {
                Debug.WriteLine($"Creating Com2Properties for object {ComNativeDescriptor.GetName(obj) ?? "(null)"}, class={ComNativeDescriptor.GetClassName(obj) ?? "(null)"}");
            }
#endif

            // set up our variables
            SetProperties(props);
            _weakObjectReference = new WeakReference(obj);

            _defaultPropertyIndex = defaultIndex;

            _typeInfoVersions = GetTypeInfoVersions(obj);

            _touchedTime = DateTime.Now.Ticks;
        }

        internal bool AlwaysValid
        {
            get => alwaysValid > 0;
            set
            {
                if (value)
                {
                    if (alwaysValid == 0 && !CheckValidity())
                    {
                        return;
                    }

                    alwaysValid++;
                }
                else
                {
                    if (alwaysValid > 0)
                    {
                        alwaysValid--;
                    }
                }
            }
        }

        public Com2PropertyDescriptor DefaultProperty
        {
            get
            {
                if (!CheckValidity(true))
                {
                    return null;
                }

                if (_defaultPropertyIndex == -1)
                {
                    return _properties.Length > 0 ? _properties[0] : null;
                }

                Debug.Assert(_defaultPropertyIndex < _properties.Length, "Whoops! default index is > props.Length");
                return _properties[_defaultPropertyIndex];
            }
        }

        /// <summary>
        ///  The object that created the list of properties. This will return null if the timeout has passed or the
        ///  reference has died.
        /// </summary>
        public object TargetObject
        {
            get
            {
                if (!CheckValidity(false) || _touchedTime == 0)
                {
#if DEBUG
                    if (DbgCom2PropertiesSwitch.TraceVerbose)
                    {
                        Debug.WriteLine("CheckValid called on dead object!");
                    }
#endif
                    return null;
                }

                return _weakObjectReference.Target;
            }
        }

        /// <summary>
        ///  How long since these properties have been queried.
        /// </summary>
        public long TicksSinceTouched => _touchedTime == 0 ? 0 : DateTime.Now.Ticks - _touchedTime;

        public Com2PropertyDescriptor[] Properties
        {
            get
            {
                CheckValidity(true);
                if (_touchedTime == 0 || _properties is null)
                {
                    return null;
                }

                _touchedTime = DateTime.Now.Ticks;

                // Refresh everything.
                for (int i = 0; i < _properties.Length; i++)
                {
                    _properties[i].SetNeedsRefresh(Com2PropertyDescriptorRefresh.All, true);
                }

#if DEBUG
                if (DbgCom2PropertiesSwitch.TraceVerbose)
                {
                    Debug.WriteLine("Returning property array for object.");
                }
#endif
                return _properties;
            }
        }

        /// <summary>
        ///  Should this be refreshed because of old age?
        /// </summary>
        public bool NeedsRefreshed
        {
            get
            {
                // Check if the property is valid but don't dispose it if it's not.
                CheckValidity(checkVersions: false, callDispose: false);
                return _touchedTime == 0 ? false : TicksSinceTouched > AgeThreshold;
            }
        }

        /// <summary>
        ///  Checks the source object for each supported extended browsing inteface and adds the relevant handlers.
        /// </summary>
        public void AddExtendedBrowsingHandlers(Hashtable handlers)
        {
            object target = TargetObject;
            if (target is null)
            {
                return;
            }

            // Process all our registered types.
            Type t;
            for (int i = 0; i < s_extendedInterfaces.Length; i++)
            {
                t = s_extendedInterfaces[i];

                // Is this object an implementor of the interface?
                if (!t.IsInstanceOfType(target))
                {
                    continue;
                }

                // Since handlers must be stateless, check to see if we've already created one of this type
                Com2ExtendedBrowsingHandler handler = (Com2ExtendedBrowsingHandler)handlers[t];
                if (handler is null)
                {
                    handler = (Com2ExtendedBrowsingHandler)Activator.CreateInstance(s_extendedInterfaceHandlerTypes[i]);
                    handlers[t] = handler;
                }

                // Make sure we got the right one.
                if (t.IsAssignableFrom(handler.Interface))
                {
#if DEBUG
                    if (DbgCom2PropertiesSwitch.TraceVerbose)
                    {
                        Debug.WriteLine($"Adding browsing handler type {handler.Interface.Name} to object.");
                    }
#endif
                    // Allow the handler to attach itself to the appropriate properties.
                    handler.SetupPropertyHandlers(_properties);
                }
                else
                {
                    throw new ArgumentException(string.Format(SR.COM2BadHandlerType, t.Name, handler.Interface.Name));
                }
            }
        }

        public void Dispose()
        {
#if DEBUG
            if (DbgCom2PropertiesSwitch.TraceVerbose)
            {
                Debug.WriteLine("Disposing property manager.");
            }
#endif

            if (_properties is not null)
            {
                Disposed?.Invoke(this, EventArgs.Empty);

                _weakObjectReference = null;
                _properties = null;
                _touchedTime = 0;
            }
        }

        /// <summary>
        ///  Gets a list of version longs for each type info in the COM object representing the current version stamp,
        ///  function and variable count. If any of these things change, we'll re-fetch the properties.
        /// </summary>
        private unsafe (ushort FunctionCount, ushort VariableCount, ushort MajorVersion, ushort MinorVersion)[] GetTypeInfoVersions(object comObject)
        {
            Oleaut32.ITypeInfo[] pTypeInfos = Com2TypeInfoProcessor.FindTypeInfos(comObject, false);
            var versions = new (ushort, ushort, ushort, ushort)[pTypeInfos.Length];
            for (int i = 0; i < pTypeInfos.Length; i++)
            {
                TYPEATTR* pTypeAttr = null;
                HRESULT hr = pTypeInfos[i].GetTypeAttr(&pTypeAttr);
                if (!hr.Succeeded || pTypeAttr is null)
                {
                    versions[i] = (0, 0, 0, 0);
                }
                else
                {
                    versions[i] = (pTypeAttr->cFuncs, pTypeAttr->cVars, pTypeAttr->wMajorVerNum, pTypeAttr->wMinorVerNum);
                    pTypeInfos[i].ReleaseTypeAttr(pTypeAttr);
                }
            }

            return versions;
        }

        /// <summary>
        ///  Make sure this property list is still valid. (The reference is still alive and we haven't passed the
        ///  timeout.)
        /// </summary>
        internal bool CheckValidity(bool checkVersions = false, bool callDispose = true)
        {
            if (AlwaysValid)
            {
                return true;
            }

            bool valid = _weakObjectReference is not null && _weakObjectReference.IsAlive;

            // Check the version information for each ITypeInfo the object exposes.
            if (valid && checkVersions)
            {
                (ushort, ushort, ushort, ushort)[] newTypeInfoVersions = GetTypeInfoVersions(_weakObjectReference.Target);
                if (newTypeInfoVersions.Length != _typeInfoVersions.Length)
                {
                    valid = false;
                }
                else
                {
                    // Compare each version number to the old one.
                    for (int i = 0; i < newTypeInfoVersions.Length; i++)
                    {
                        if (newTypeInfoVersions[i] != _typeInfoVersions[i])
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                if (!valid)
                {
                    // Update to the new version list we have.
                    _typeInfoVersions = newTypeInfoVersions;
                }
            }

            if (!valid && callDispose)
            {
                // Weak reference has died, so remove this from the hash table
#if DEBUG
                if (DbgCom2PropertiesSwitch.TraceVerbose)
                {
                    Debug.WriteLine($"Disposing reference to object (weakRef {(_weakObjectReference is null ? "null" : "dead")})");
                }
#endif

                Dispose();
            }

            return valid;
        }

        /// <summary>
        ///  Set the proerties for this object, and notify each property that we are now it's manager.
        /// </summary>
        internal void SetProperties(Com2PropertyDescriptor[] properties)
        {
            _properties = properties;
            if (properties is not null)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    properties[i].PropertyManager = this;
                }
            }
        }
    }
}
