﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Xunit;

namespace Microsoft.VisualBasic.ApplicationServices.Tests
{
    public class WindowsFormsApplicationBaseTests
    {
        private static string GetAppID(Assembly assembly)
        {
            var type = typeof(WindowsFormsApplicationBase);
            var method = type.GetMethod("GetApplicationInstanceID", BindingFlags.NonPublic | BindingFlags.Static);
            return (string)method.Invoke(null, new object[] { assembly });
        }

        [Fact]
        public void GetApplicationInstanceID()
        {
            var assembly = typeof(WindowsFormsApplicationBaseTests).Assembly;
            var expectedId = assembly.ManifestModule.ModuleVersionId.ToString();
            Assert.Equal(expectedId, GetAppID(assembly));
        }

        [Fact]
        public void GetApplicationInstanceID_GuidAttribute()
        {
            var guid = Guid.NewGuid().ToString();
            var attributeBuilder = new CustomAttributeBuilder(
                typeof(GuidAttribute).GetConstructor(new[] { typeof(string) }), new[] { guid });
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(Guid.NewGuid().ToString()) { Version = new Version(1, 2, 3, 4) },
                AssemblyBuilderAccess.RunAndCollect,
                new[] { attributeBuilder });
            assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
            var type = typeof(WindowsFormsApplicationBase);
            var method = type.GetMethod("GetApplicationInstanceID",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.Equal(
                $"{guid}1.2",
                (string)method.Invoke(null, new object[] { assemblyBuilder }));
        }

        [Fact]
        public void GetApplicationInstanceID_GuidAttributeNewVersion()
        {
            var guid = Guid.NewGuid().ToString();
            var attributeBuilder = new CustomAttributeBuilder(
                typeof(GuidAttribute).GetConstructor(new[] { typeof(string) }), new[] { guid });
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(Guid.NewGuid().ToString()) { Version = new Version() },
                AssemblyBuilderAccess.RunAndCollect,
                new[] { attributeBuilder });
            assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
            var type = typeof(WindowsFormsApplicationBase);
            var method = type.GetMethod("GetApplicationInstanceID",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.Equal(
                $"{guid}0.0",
                (string)method.Invoke(null, new object[] { assemblyBuilder }));
        }

        [Fact]
        public void GetApplicationInstanceID_GuidAttributeNullVersion()
        {
            var guid = Guid.NewGuid().ToString();
            var attributeBuilder = new CustomAttributeBuilder(
                typeof(GuidAttribute).GetConstructor(new[] { typeof(string) }), new[] { guid });
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(Guid.NewGuid().ToString()) { Version = null },
                AssemblyBuilderAccess.RunAndCollect,
                new[] { attributeBuilder });
            assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
            var type = typeof(WindowsFormsApplicationBase);
            var method = type.GetMethod("GetApplicationInstanceID",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.Equal(
                $"{guid}0.0",
                (string)method.Invoke(null, new object[] { assemblyBuilder }));
        }
    }
}
