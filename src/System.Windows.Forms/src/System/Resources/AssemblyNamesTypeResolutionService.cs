// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Reflection;

namespace System.Resources;

internal class AssemblyNamesTypeResolutionService : ITypeResolutionService
{
    private AssemblyName[]? _names;
    private ConcurrentDictionary<AssemblyName, Assembly>? _cachedAssemblies;
    private ConcurrentDictionary<string, Type>? _cachedTypes;

    private static readonly string s_dotNetPath = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles") ?? string.Empty, "dotnet\\shared");
    private static readonly string s_dotNetPathX86 = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? string.Empty, "dotnet\\shared");

    internal AssemblyNamesTypeResolutionService(AssemblyName[]? names) => _names = names;

    public Assembly? GetAssembly(AssemblyName name) => GetAssembly(name, true);

    [UnconditionalSuppressMessage("SingleFile", "IL3002", Justification = "Handles single file case")]
    public Assembly? GetAssembly(AssemblyName name, bool throwOnError)
    {
        _cachedAssemblies ??= new();
        if (_cachedAssemblies.TryGetValue(name, out Assembly? result) && result is not null)
        {
            return result;
        }

        result = Assembly.Load(name.FullName);
        if (result is not null)
        {
            _cachedAssemblies[name] = result;
        }
        else if (_names is not null)
        {
            foreach (AssemblyName assemblyName in _names.Where(an => an.Equals(name)))
            {
                try
                {
                    result = Assembly.LoadFrom(GetPathOfAssembly(assemblyName));
                    if (result is not null)
                    {
                        _cachedAssemblies[assemblyName] = result;
                    }
                }
                catch
                {
                    if (throwOnError)
                    {
                        throw;
                    }
                }
            }
        }

        return result;
    }

    [UnconditionalSuppressMessage("SingleFile", "IL3002", Justification = "Returns null if in a single file")]
    public string GetPathOfAssembly(AssemblyName name)
    {
#pragma warning disable SYSLIB0044 // Type or member is obsolete. Ref https://github.com/dotnet/winforms/issues/7308
#pragma warning disable IL3000 // Avoid accessing Assembly file path when publishing as a single file
        return name.CodeBase ?? string.Empty;
#pragma warning restore IL3000
#pragma warning restore SYSLIB0044
    }

    public Type? GetType(string name) => GetType(name, true);

    public Type? GetType(string name, bool throwOnError) => GetType(name, throwOnError, false);

    public Type? GetType(string name, bool throwOnError, bool ignoreCase)
    {
        // Check type cache first
        _cachedTypes ??= new(StringComparer.Ordinal);
        if (_cachedTypes.TryGetValue(name, out Type? result) && result is not null)
        {
            return result;
        }

        // Missed in cache, try to resolve the type from the reference assemblies.
        if (name.Contains(','))
        {
            result = Type.GetType(name, false, ignoreCase);
        }

        if (result is null && _names is not null)
        {
            // If the type is assembly qualified name, we sort the assembly names
            // to put assemblies with same name in the front so that they can
            // be searched first.
            int pos = name.IndexOf(',');
            if (pos > 0 && pos < name.Length - 1)
            {
                string fullName = name[(pos + 1)..].Trim();
                AssemblyName? assemblyName = null;
                try
                {
                    assemblyName = new AssemblyName(fullName);
                }
                catch
                {
                }

                if (assemblyName is not null)
                {
                    List<AssemblyName> assemblyList = new(_names.Length);
                    foreach (AssemblyName asmName in _names)
                    {
                        if (string.Equals(assemblyName.Name, asmName.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            assemblyList.Insert(0, asmName);
                        }
                        else
                        {
                            assemblyList.Add(asmName);
                        }
                    }

                    _names = [.. assemblyList];
                }
            }

            // Search each reference assembly
            foreach (AssemblyName assemblyName in _names)
            {
                Assembly? assembly = GetAssembly(assemblyName, false);
                if (assembly is not null)
                {
                    result = assembly.GetType(name, false, ignoreCase);
                    if (result is null)
                    {
                        int indexOfComma = name.IndexOf(',');
                        if (indexOfComma != -1)
                        {
                            string shortName = name[..indexOfComma];
                            result = assembly.GetType(shortName, false, ignoreCase);
                        }
                    }
                }

                if (result is not null)
                {
                    break;
                }
            }
        }

        if (result is null && throwOnError)
        {
            throw new ArgumentException(string.Format(SR.InvalidResXNoType, name));
        }

        if (result is not null)
        {
            // Only cache types from the shared framework because they don't need to update.
            // For simplicity, don't cache custom types
#pragma warning disable IL3000 // Avoid accessing Assembly file path when publishing as a single file
            if (IsDotNetAssembly(result.Assembly.Location))
            {
                _cachedTypes[name] = result;
            }
#pragma warning restore IL3000
        }

        return result;
    }

    /// <summary>
    ///  This is matching %windir%\Microsoft.NET\Framework*, so both 32bit and 64bit framework will be covered.
    /// </summary>
    private static bool IsDotNetAssembly(string assemblyPath)
        => assemblyPath is not null
        && (assemblyPath.StartsWith(s_dotNetPath, StringComparison.OrdinalIgnoreCase)
        || assemblyPath.StartsWith(s_dotNetPathX86, StringComparison.OrdinalIgnoreCase));

    public void ReferenceAssembly(AssemblyName name) => throw new NotSupportedException();
}
