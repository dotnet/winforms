﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ArgumentValidationTests
    {
        [Fact]
        public void OrThrowIfNull_ParamIsNull()
        {
            string? param = null;
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfNull());
            Assert.Equal(nameof(param), exception.ParamName);
        }

        [Fact]
        public void OrThrowIfNull_ParamIsNull_DifferentParamName()
        {
            string paramName = "param2";
            string? param = null;
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfNull(paramName));
            Assert.Equal(paramName, exception.ParamName);
        }

        [Fact]
        public void OrThrowIfNull_ParamIsNotNull()
        {
            var param = "metamorphosator";
            var variable = param.OrThrowIfNull();
            Assert.Equal(param, variable);
        }

        [Fact]
        public void OrThrowIfZero_ParamIsZero()
        {
            var param = IntPtr.Zero;
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfZero());
            Assert.Equal(nameof(param), exception.ParamName);
        }

        [Fact]
        public void OrThrowIfZero_ParamIsNull_DifferentParamName()
        {
            var paramName = "param2";
            var param = IntPtr.Zero;
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfZero(paramName));
            Assert.Equal(paramName, exception.ParamName);
        }

        [Fact]
        public void OrThrowIfZero_ParamIsNotZero()
        {
            var param = new IntPtr(24);
            var variable = param.OrThrowIfZero();
            Assert.Equal(param, variable);
        }
    }
}
