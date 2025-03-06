// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  This class is responsible for managing a set of properties for a native object. It determines
///  when the properties need to be refreshed, and owns the extended handlers for those properties.
/// </summary>
[RequiresUnreferencedCode(ComNativeDescriptor.ComTypeDescriptorsMessage + " Uses Com2TypeInfoProcessor which is not trim-compatible.")]
internal sealed class Com2Properties
{
    // This is the interval that we'll hold properties for. If someone doesn't touch an object for this amount of time,
    // we'll dump the properties from our cache. 5 minutes -- ticks are 1/10,000,000th of a second
    private const long AgeThreshold = 10000000L * 60L * 5L;

    // This is the object that gave us the properties. To avoid rooting the object we only hold the original object
    // here and always query for whatever interface we need.
    private WeakReference<object> _weakObjectReference;

    private Com2PropertyDescriptor[] _properties;

    private readonly int _defaultPropertyIndex = -1;

    // The timestamp of the last operation on this property manager, usually when the property list was fetched.
    private long _touchedTime;

    // For non-IProvideMultipleClassInfo ITypeInfos, this is the version number on the last ITypeInfo we looked at.
    // If this changes, we know we need to dump the cache.
    private (ushort FunctionCount, ushort VariableCount, ushort MajorVersion, ushort MinorVersion)[] _typeInfoVersions;

    private int _alwaysValid;

    public event EventHandler? Disposed;

    public Com2Properties(object comObject, Com2PropertyDescriptor[] properties, int defaultIndex)
    {
        ArgumentNullException.ThrowIfNull(comObject);
        ArgumentNullException.ThrowIfNull(properties);

        // Set up our variables.
        _properties = properties;
        for (int i = 0; i < _properties.Length; i++)
        {
            _properties[i].PropertyManager = this;
        }

        _weakObjectReference = new(comObject);
        _defaultPropertyIndex = defaultIndex;
        _typeInfoVersions = GetTypeInfoVersions(comObject);
        _touchedTime = DateTime.Now.Ticks;
    }

    internal bool AlwaysValid
    {
        get => _alwaysValid > 0;
        set
        {
            if (value)
            {
                if (_alwaysValid == 0 && CheckAndGetTarget(checkVersions: false, callDispose: true) is null)
                {
                    return;
                }

                _alwaysValid++;
            }
            else
            {
                if (_alwaysValid > 0)
                {
                    _alwaysValid--;
                }
            }
        }
    }

    public Com2PropertyDescriptor? DefaultProperty
    {
        get
        {
            if (CheckAndGetTarget(checkVersions: true, callDispose: true) is null)
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
    public object? TargetObject
    {
        get
        {
            if (CheckAndGetTarget(checkVersions: false, callDispose: true) is not { } target || _touchedTime == 0)
            {
                return null;
            }

            return target;
        }
    }

    /// <summary>
    ///  How long since these properties have been queried.
    /// </summary>
    public long TicksSinceTouched => _touchedTime == 0 ? 0 : DateTime.Now.Ticks - _touchedTime;

    public Com2PropertyDescriptor[]? Properties
    {
        get
        {
            CheckAndGetTarget(checkVersions: true, callDispose: true);
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
            CheckAndGetTarget(checkVersions: false, callDispose: false);
            return _touchedTime != 0 && TicksSinceTouched > AgeThreshold;
        }
    }

    /// <summary>
    ///  Checks the source object for each supported extended browsing inteface and adds the relevant handlers.
    /// </summary>
    public void RegisterPropertyEvents(IReadOnlyList<ICom2ExtendedBrowsingHandler> handlers)
    {
        if (TargetObject is not { } target)
        {
            return;
        }

        foreach (var handler in handlers)
        {
            if (handler.ObjectSupportsInterface(target))
            {
                Debug.WriteLine($"Adding browsing handler type {handler.GetType().Name} to object.");
                handler.RegisterEvents(_properties);
            }
        }
    }

    public void Dispose()
    {
        if (_properties is not null)
        {
            Disposed?.Invoke(this, EventArgs.Empty);

            _weakObjectReference = null!;
            _properties = null!;
            _touchedTime = 0;
        }
    }

    /// <summary>
    ///  Gets a list of version longs for each type info in the COM object representing the current version stamp,
    ///  function and variable count. If any of these things change, we'll re-fetch the properties.
    /// </summary>
    private unsafe (ushort FunctionCount, ushort VariableCount, ushort MajorVersion, ushort MinorVersion)[] GetTypeInfoVersions(object comObject)
    {
        ITypeInfo*[] pTypeInfos = Com2TypeInfoProcessor.FindTypeInfos(comObject);
        var versions = new (ushort, ushort, ushort, ushort)[pTypeInfos.Length];
        for (int i = 0; i < pTypeInfos.Length; i++)
        {
            TYPEATTR* pTypeAttr;
            HRESULT hr = pTypeInfos[i]->GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded || pTypeAttr is null)
            {
                versions[i] = (0, 0, 0, 0);
            }
            else
            {
                versions[i] = (pTypeAttr->cFuncs, pTypeAttr->cVars, pTypeAttr->wMajorVerNum, pTypeAttr->wMinorVerNum);
                pTypeInfos[i]->ReleaseTypeAttr(pTypeAttr);
            }

            pTypeInfos[i]->Release();
        }

        return versions;
    }

    /// <summary>
    ///  Make sure this property list is still valid. (The reference is still alive and we haven't passed the timeout.)
    /// </summary>
    /// <returns>
    ///  The object if it is still valid.
    /// </returns>
    internal object? CheckAndGetTarget(bool checkVersions, bool callDispose)
    {
        if (AlwaysValid)
        {
            return true;
        }

        bool valid = _weakObjectReference.TryGetTarget(out object? target);

        // Check the version information for each ITypeInfo the object exposes.
        if (target is not null && checkVersions)
        {
            (ushort, ushort, ushort, ushort)[] newTypeInfoVersions = GetTypeInfoVersions(target);
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
                target = null;
            }
        }

        if (!valid && callDispose)
        {
            // Weak reference has died, so remove this from the hash table
            Dispose();
        }

        return target;
    }
}
