// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.VisualBasic.ApplicationServices.Tests
{
    public class ApplicationBaseTests
    {
        [Fact]
        public void Culture()
        {
            var app = new ApplicationBase();
            var culture = app.Culture;
            Assert.Equal(System.Threading.Thread.CurrentThread.CurrentCulture, culture);
            try
            {
                app.ChangeCulture("en-US");
                Assert.Equal(System.Threading.Thread.CurrentThread.CurrentCulture, app.Culture);
                Assert.Equal("en-US", app.Culture.Name);
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        [Fact]
        public void UICulture()
        {
            var app = new ApplicationBase();
            var culture = app.UICulture;
            Assert.Equal(System.Threading.Thread.CurrentThread.CurrentUICulture, culture);
            try
            {
                app.ChangeUICulture("en-US");
                Assert.Equal(System.Threading.Thread.CurrentThread.CurrentUICulture, app.UICulture);
                Assert.Equal("en-US", app.UICulture.Name);
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

        [Fact]
        public void GetEnvironmentVariable()
        {
            var app = new ApplicationBase();
            foreach (var (key, value) in GetEnvironmentVariables())
            {
                Assert.Equal(value, app.GetEnvironmentVariable(key));
            }
        }

        [Fact]
        public void GetEnvironmentVariable_ArgumentException()
        {
            var app = new ApplicationBase();
            var key = GetEnvironmentVariables().LastOrDefault().Item1 ?? "";
            var ex = Assert.Throws<ArgumentException>(() => app.GetEnvironmentVariable(key + "z"));
            _ = ex.ToString(); // ensure message can be formatted
        }

        private static (string, string)[] GetEnvironmentVariables()
        {
            var pairs = new List<(string, string)>();
            var vars = Environment.GetEnvironmentVariables();
            foreach (var key in vars.Keys)
            {
                pairs.Add(((string)key, (string)vars[key]));
            }
            return pairs.OrderBy(pair => pair.Item1).ToArray();
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Different entry assembly")]
        public void Info()
        {
            var app = new ApplicationBase();
            var assembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetCallingAssembly();
            var assemblyName = assembly.GetName();
            Assert.Equal(assemblyName.Name, app.Info.AssemblyName);
            Assert.Equal(assemblyName.Version, app.Info.Version);
        }

        [Fact]
        public void Log()
        {
            var app = new ApplicationBase();
            var log = app.Log;
            _ = log.TraceSource;
            _ = log.DefaultFileLogWriter;
        }
    }
}
