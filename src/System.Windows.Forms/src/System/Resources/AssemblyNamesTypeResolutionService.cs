// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;

namespace System.Resources
{
    internal class AssemblyNamesTypeResolutionService : ITypeResolutionService
    {
        private AssemblyName[] _names;
        private Hashtable _cachedAssemblies;
        private Hashtable _cachedTypes;

        private static readonly string s_dotNetPath = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), "dotnet\\shared");
        private static readonly string s_dotNetPathX86 = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "dotnet\\shared");

        internal AssemblyNamesTypeResolutionService(AssemblyName[] names)
        {
            _names = names;
        }

        public Assembly GetAssembly(AssemblyName name)
        {
            return GetAssembly(name, true);
        }

        public Assembly GetAssembly(AssemblyName name, bool throwOnError)
        {
            Assembly result = null;

            if (_cachedAssemblies is null)
            {
                _cachedAssemblies = Hashtable.Synchronized(new Hashtable());
            }

            if (_cachedAssemblies.Contains(name))
            {
                result = _cachedAssemblies[name] as Assembly;
            }

            if (result is null)
            {
                result = Assembly.Load(name.FullName);
                if (result != null)
                {
                    _cachedAssemblies[name] = result;
                }
                else if (_names != null)
                {
                    foreach (AssemblyName asmName in _names.Where(an => an.Equals(name)))
                    {
                        try
                        {
                            result = Assembly.LoadFrom(GetPathOfAssembly(asmName));
                            if (result != null)
                            {
                                _cachedAssemblies[asmName] = result;
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
            }

            return result;
        }

        public string GetPathOfAssembly(AssemblyName name)
        {
            return name.CodeBase;
        }

        public Type GetType(string name)
        {
            return GetType(name, true);
        }

        public Type GetType(string name, bool throwOnError)
        {
            return GetType(name, throwOnError, false);
        }

        public Type GetType(string name, bool throwOnError, bool ignoreCase)
        {
            Type result = null;

            // Check type cache first
            if (_cachedTypes is null)
            {
                _cachedTypes = Hashtable.Synchronized(new Hashtable(StringComparer.Ordinal));
            }

            if (_cachedTypes.Contains(name))
            {
                result = _cachedTypes[name] as Type;
                return result;
            }

            // Missed in cache, try to resolve the type from the reference assemblies.
            if (name.IndexOf(',') != -1)
            {
                result = Type.GetType(name, false, ignoreCase);
            }

            if (result is null && _names != null)
            {
                // If the type is assembly qualified name, we sort the assembly names
                // to put assemblies with same name in the front so that they can
                // be searched first.
                int pos = name.IndexOf(',');
                if (pos > 0 && pos < name.Length - 1)
                {
                    string fullName = name.Substring(pos + 1).Trim();
                    AssemblyName assemblyName = null;
                    try
                    {
                        assemblyName = new AssemblyName(fullName);
                    }
                    catch
                    {
                    }

                    if (assemblyName != null)
                    {
                        List<AssemblyName> assemblyList = new List<AssemblyName>(_names.Length);
                        foreach (AssemblyName asmName in _names)
                        {
                            if (string.Compare(assemblyName.Name, asmName.Name, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                assemblyList.Insert(0, asmName);
                            }
                            else
                            {
                                assemblyList.Add(asmName);
                            }
                        }
                        _names = assemblyList.ToArray();
                    }
                }

                // Search each reference assembly
                foreach (AssemblyName asmName in _names)
                {
                    Assembly asm = GetAssembly(asmName, false);
                    if (asm != null)
                    {
                        result = asm.GetType(name, false, ignoreCase);
                        if (result is null)
                        {
                            int indexOfComma = name.IndexOf(',');
                            if (indexOfComma != -1)
                            {
                                string shortName = name.Substring(0, indexOfComma);
                                result = asm.GetType(shortName, false, ignoreCase);
                            }
                        }
                    }

                    if (result != null)
                    {
                        break;
                    }
                }
            }

            if (result is null && throwOnError)
            {
                throw new ArgumentException(string.Format(SR.InvalidResXNoType, name));
            }

            if (result != null)
            {
                // Only cache types from the shared framework  because they don't need to update.
                // For simplicity, don't cache custom types
                if (IsDotNetAssembly(result.Assembly.Location))
                {
                    _cachedTypes[name] = result;
                }
            }

            return result;
        }

        /// <summary>
        ///  This is matching %windir%\Microsoft.NET\Framework*, so both 32bit and 64bit framework will be covered.
        /// </summary>
        private bool IsDotNetAssembly(string assemblyPath)
        {
            return assemblyPath != null && (assemblyPath.StartsWith(s_dotNetPath, StringComparison.OrdinalIgnoreCase) || assemblyPath.StartsWith(s_dotNetPathX86, StringComparison.OrdinalIgnoreCase));
        }

        public void ReferenceAssembly(AssemblyName name)
        {
            throw new NotSupportedException();
        }
    }
}
