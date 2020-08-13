// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;
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

        /// <summary>
        ///  This is the interval that we'll hold props for.  If someone doesn't touch an object
        ///  for this amount of time, we'll dump the properties from our cache.
        ///
        ///  5 minutes -- ticks are 1/10,000,000th of a second
        /// </summary>
        private const long AGE_THRESHHOLD = (long)(10000000L * 60L * 5L);

        /// <summary>
        ///  This is the object that gave us the properties.  We hold a WeakRef so we don't addref the object.
        /// </summary>
        internal WeakReference weakObjRef;

        /// <summary>
        ///  This is our list of properties.
        /// </summary>
        private Com2PropertyDescriptor[] props;

        /// <summary>
        ///  The index of the default property
        /// </summary>
        private readonly int defaultIndex = -1;

        /// <summary>
        ///  The timestamp of the last operation on this property manager, usually
        ///  when the property list was fetched.
        /// </summary>
        private long touchedTime;

        /// <summary>
        ///  For non-IProvideMultipleClassInfo ITypeInfos, this is the version number on the last
        ///  ITypeInfo we looked at.  If this changes, we know we need to dump the cache.
        /// </summary>
        private (ushort, ushort, ushort, ushort)[] _typeInfoVersions;

#if DEBUG
        private readonly string dbgObjName;
        private readonly string dbgObjClass;
#endif

        private int alwaysValid;

        /// <summary>
        ///  These are the interfaces we recognize for extended browsing.
        /// </summary>
        private static readonly Type[] extendedInterfaces = new Type[]
        {
            typeof(VSSDK.ICategorizeProperties),
            typeof(VSSDK.IProvidePropertyBuilder),
            typeof(Oleaut32.IPerPropertyBrowsing),
            typeof(VSSDK.IVsPerPropertyBrowsing),
            typeof(VSSDK.IVSMDPerPropertyBrowsing)
        };

        /// <summary>
        ///  These are the classes of handlers corresponding to the extended
        ///  interfaces above.
        /// </summary>
        private static readonly Type[] extendedInterfaceHandlerTypes = new Type[]{
                                                        typeof(Com2ICategorizePropertiesHandler),
                                                        typeof(Com2IProvidePropertyBuilderHandler),
                                                        typeof(Com2IPerPropertyBrowsingHandler),
                                                        typeof(Com2IVsPerPropertyBrowsingHandler),
                                                        typeof(Com2IManagedPerPropertyBrowsingHandler)};

        public event EventHandler Disposed;

        /// <summary>
        ///  Default ctor.
        /// </summary>
        public Com2Properties(object obj, Com2PropertyDescriptor[] props, int defaultIndex)
        {
#if DEBUG
            ComNativeDescriptor cnd = new ComNativeDescriptor();
            dbgObjName = cnd.GetName(obj);
            if (dbgObjName is null)
            {
                dbgObjName = "(null)";
            }
            dbgObjClass = cnd.GetClassName(obj);
            if (dbgObjClass is null)
            {
                dbgObjClass = "(null)";
            }
            if (DbgCom2PropertiesSwitch.TraceVerbose)
            {
                Debug.WriteLine("Creating Com2Properties for object " + dbgObjName + ", class=" + dbgObjClass);
            }
#endif

            // set up our variables
            SetProps(props);
            weakObjRef = new WeakReference(obj);

            this.defaultIndex = defaultIndex;

            _typeInfoVersions = GetTypeInfoVersions(obj);

            touchedTime = DateTime.Now.Ticks;
        }

        internal bool AlwaysValid
        {
            get
            {
                return alwaysValid > 0;
            }
            set
            {
                if (value)
                {
                    if (alwaysValid == 0 && !CheckValid())
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

        /// <summary>
        ///  Retrieve the default property.
        /// </summary>
        public Com2PropertyDescriptor DefaultProperty
        {
            get
            {
                if (!CheckValid(true))
                {
                    return null;
                }
                if (defaultIndex == -1)
                {
                    if (props.Length > 0)
                    {
                        return props[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                Debug.Assert(defaultIndex < props.Length, "Whoops! default index is > props.Length");
                return props[defaultIndex];
            }
        }

        /// <summary>
        ///  The object that created the list of properties.  This will
        ///  return null if the timeout has passed or the ref has died.
        /// </summary>
        public object TargetObject
        {
            get
            {
                if (!CheckValid(false) || touchedTime == 0)
                {
#if DEBUG
                    if (DbgCom2PropertiesSwitch.TraceVerbose)
                    {
                        Debug.WriteLine("CheckValid called on dead object!");
                    }
#endif
                    return null;
                }
                return weakObjRef.Target;
            }
        }

        /// <summary>
        ///  How long since these props have been queried.
        /// </summary>
        public long TicksSinceTouched
        {
            get
            {
                if (touchedTime == 0)
                {
                    return 0;
                }
                return DateTime.Now.Ticks - touchedTime;
            }
        }

        /// <summary>
        ///  Returns the list of properties
        /// </summary>
        public Com2PropertyDescriptor[] Properties
        {
            get
            {
                CheckValid(true);
                if (touchedTime == 0 || props is null)
                {
                    return null;
                }
                touchedTime = DateTime.Now.Ticks;

                // refresh everyone!
                for (int i = 0; i < props.Length; i++)
                {
                    props[i].SetNeedsRefresh(Com2PropertyDescriptorRefresh.All, true);
                }

#if DEBUG
                if (DbgCom2PropertiesSwitch.TraceVerbose)
                {
                    Debug.WriteLine("Returning prop array for object " + dbgObjName + ", class=" + dbgObjClass);
                }
#endif
                return props;
            }
        }

        /// <summary>
        ///  Should this guy be refreshed because of old age?
        /// </summary>
        public bool TooOld
        {
            get
            {
                // check if the property is valid but don't dispose it if it's not
                CheckValid(false, false);
                if (touchedTime == 0)
                {
                    return false;
                }
                return TicksSinceTouched > AGE_THRESHHOLD;
            }
        }

        /// <summary>
        ///  Checks the source object for eache extended browsing inteface
        ///  listed in extendedInterfaces and creates a handler from extendedInterfaceHandlerTypes
        ///  to handle it.
        /// </summary>
        public void AddExtendedBrowsingHandlers(Hashtable handlers)
        {
            object target = TargetObject;
            if (target is null)
            {
                return;
            }

            // process all our registered types
            Type t;
            for (int i = 0; i < extendedInterfaces.Length; i++)
            {
                t = extendedInterfaces[i];

                // is this object an implementor of the interface?
                //
                if (t.IsInstanceOfType(target))
                {
                    // since handlers must be stateless, check to see if we've already
                    // created one of this type
                    //
                    Com2ExtendedBrowsingHandler handler = (Com2ExtendedBrowsingHandler)handlers[t];
                    if (handler is null)
                    {
                        handler = (Com2ExtendedBrowsingHandler)Activator.CreateInstance(extendedInterfaceHandlerTypes[i]);
                        handlers[t] = handler;
                    }

                    // make sure we got the right one
                    //
                    if (t.IsAssignableFrom(handler.Interface))
                    {
#if DEBUG
                        if (DbgCom2PropertiesSwitch.TraceVerbose)
                        {
                            Debug.WriteLine("Adding browsing handler type " + handler.Interface.Name + " to object " + dbgObjName + ", class=" + dbgObjClass);
                        }
#endif
                        // allow the handler to attach itself to the appropriate properties
                        //
                        handler.SetupPropertyHandlers(props);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format(SR.COM2BadHandlerType, t.Name, handler.Interface.Name));
                    }
                }
            }
        }

        public void Dispose()
        {
#if DEBUG
            if (DbgCom2PropertiesSwitch.TraceVerbose)
            {
                Debug.WriteLine("Disposing property manager for " + dbgObjName + ", class=" + dbgObjClass);
            }
#endif

            if (props != null)
            {
                Disposed?.Invoke(this, EventArgs.Empty);

                weakObjRef = null;
                props = null;
                touchedTime = 0;
            }
        }

        public bool CheckValid()
        {
            return CheckValid(false);
        }

        /// <summary>
        ///  Make sure this property list is still valid.
        ///
        ///  1) WeakRef is still alive
        ///  2) Our timeout hasn't passed
        /// </summary>
        public bool CheckValid(bool checkVersions)
        {
            return CheckValid(checkVersions, true);
        }

        /// <summary>
        ///  Gets a list of version longs for each type info in the COM object representing the
        ///  current version stamp, function and variable count.
        ///  If any of these things change, we'll re-fetch the properties.
        /// </summary>
        private (ushort, ushort, ushort, ushort)[] GetTypeInfoVersions(object comObject)
        {
            Oleaut32.ITypeInfo[] pTypeInfos = Com2TypeInfoProcessor.FindTypeInfos(comObject, false);
            var versions = new (ushort, ushort, ushort, ushort)[pTypeInfos.Length];
            for (int i = 0; i < pTypeInfos.Length; i++)
            {
                versions[i] = GetTypeInfoVersion(pTypeInfos[i]);
            }
            return versions;
        }

        private unsafe (ushort, ushort, ushort, ushort) GetTypeInfoVersion(Oleaut32.ITypeInfo pTypeInfo)
        {
            Ole32.TYPEATTR* pTypeAttr = null;
            HRESULT hr = pTypeInfo.GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded() || pTypeAttr is null)
            {
                return (0, 0, 0, 0);
            }

            try
            {
                return (pTypeAttr->cFuncs, pTypeAttr->cVars, pTypeAttr->wMajorVerNum, pTypeAttr->wMinorVerNum);
            }
            finally
            {
                pTypeInfo.ReleaseTypeAttr(pTypeAttr);
            }
        }

        internal bool CheckValid(bool checkVersions, bool callDispose)
        {
            if (AlwaysValid)
            {
                return true;
            }

            bool valid = weakObjRef != null && weakObjRef.IsAlive;

            // check the version information for each ITypeInfo the object exposes.
            if (valid && checkVersions)
            {
                (ushort, ushort, ushort, ushort)[] newTypeInfoVersions = GetTypeInfoVersions(weakObjRef.Target);
                if (newTypeInfoVersions.Length != _typeInfoVersions.Length)
                {
                    valid = false;
                }
                else
                {
                    // compare each version number to the old one.
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
                    // update to the new version list we have.
                    //
                    _typeInfoVersions = newTypeInfoVersions;
                }
            }

            if (!valid && callDispose)
            {
                // weak ref has died, so remove this from the hash table
                //
#if DEBUG
                if (DbgCom2PropertiesSwitch.TraceVerbose)
                {
                    Debug.WriteLine("Disposing reference to object " + dbgObjName + ", class=" + dbgObjClass + " (weakRef " + (weakObjRef is null ? "null" : "dead") + ")");
                }
#endif

                Dispose();
            }

            return valid;
        }

        /// <summary>
        ///  Set the props for this object, and notify each property
        ///  that we are now it's manager
        /// </summary>
        internal void SetProps(Com2PropertyDescriptor[] props)
        {
            this.props = props;
            if (props != null)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    props[i].PropertyManager = this;
                }
            }
        }
    }
}
