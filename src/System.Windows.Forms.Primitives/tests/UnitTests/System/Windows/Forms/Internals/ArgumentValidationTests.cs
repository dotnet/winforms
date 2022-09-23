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
            object? param = null;
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfNull());
            Assert.Equal(nameof(param), exception.ParamName);
        }

        [Fact]
        public void OrThrowIfNull_ParamIsNull_DifferentParamName()
        {
            string paramName = "param2";
            object? param = null;
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfNull(paramName));
            Assert.Equal(paramName, exception.ParamName);
        }

        [Fact]
        public void OrThrowIfNull_ParamIsNotNull()
        {
            var param = new object();
            var variable = param.OrThrowIfNull();
            Assert.Equal(param, variable);
        }

        [Fact]
        public void OrThrowIfNullWithMessage_ParamIsNull()
        {
            object? param = null;
            var message = "message";
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfNullWithMessage(message));
            Assert.StartsWith(message, exception.Message);
            Assert.Equal(nameof(param), exception.ParamName);
        }

        [Fact]
        public void OrThrowIfNullWithMessage_ParamIsNull_DifferentParamName()
        {
            var paramName = "param2";
            object? param = null;
            var message = "message";
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfNullWithMessage(message, paramName));
            Assert.StartsWith(message, exception.Message);
            Assert.Equal(paramName, exception.ParamName);
        }

        [Fact]
        public void OrThrowIfNullWithMessage_ParamIsNotNull()
        {
            var param = new object();
            var message = "message";
            var variable = param.OrThrowIfNullWithMessage(message);
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

        [Fact]
        public void ThrowIfNull_HDC_ParamIsNull()
        {
            var param = new HDC(IntPtr.Zero);
            var exception = Assert.Throws<ArgumentNullException>(() => ArgumentValidation.ThrowIfNull(param));
            Assert.Equal(nameof(param), exception.ParamName);
        }

        [Fact]
        public void ThrowIfNull_HDC_ParamIsNotNull()
        {
            var param = new HDC(new IntPtr(24));
            ArgumentValidation.ThrowIfNull(param);
        }

        [Fact]
        public void ThrowIfNull_HDC_DifferentParamName()
        {
            var param = new HDC(IntPtr.Zero);
            var exception = Assert.Throws<ArgumentNullException>(() => ArgumentValidation.ThrowIfNull(param, "paramName"));
            Assert.Equal("paramName", exception.ParamName);
        }

        [Fact]
        public void ThrowIfNullOrEmpty_ParamIsValid()
        {
            var param = "valid";
            ArgumentValidation.ThrowIfNullOrEmpty(param);
        }

        [Fact]
        public void ThrowIfNullOrEmpty_ParamIsNull()
        {
            string? param = null;
            var exception = Assert.Throws<ArgumentNullException>(() => param.ThrowIfNullOrEmpty());
            Assert.Equal(nameof(param), exception.ParamName);
        }

        [Fact]
        public void ThrowIfNullOrEmpty_ParamIsEmpty()
        {
            string param = string.Empty;
            var exception = Assert.Throws<ArgumentNullException>(() => param.ThrowIfNullOrEmpty());
            Assert.Equal(nameof(param), exception.ParamName);
        }

        [Fact]
        public void ThrowIfNullOrEmpty_ParamIsValid_DifferentParamName()
        {
            string param = "valid";
            ArgumentValidation.ThrowIfNullOrEmpty(param, "paramName");
        }

        [Fact]
        public void ThrowIfNullOrEmpty_ParamIsNull_DifferentParamName()
        {
            string? param = null;
            var exception = Assert.Throws<ArgumentNullException>(() => param.ThrowIfNullOrEmpty("paramName"));
            Assert.Equal("paramName", exception.ParamName);
        }

        [Fact]
        public void ThrowIfNullOrEmpty_ParamIsEmpty_DifferentParamName()
        {
            var param = string.Empty;
            var exception = Assert.Throws<ArgumentNullException>(() => param.ThrowIfNullOrEmpty("paramName"));
            Assert.Equal("paramName", exception.ParamName);
        }

        [Fact]
        public void ThrowIfNullOrEmptyWithMessage_ParamIsValid()
        {
            string param = "valid";
            param.ThrowIfNullOrEmptyWithMessage("message");
        }

        [Fact]
        public void ThrowIfNullOrEmptyWithMessage_ParamIsNull()
        {
            string? param = null;
            var message = "message";
            var exception = Assert.Throws<ArgumentNullException>(() => param.ThrowIfNullOrEmptyWithMessage(message));
            Assert.StartsWith(message, exception.Message);
            Assert.Equal(nameof(param), exception.ParamName);
        }

        [Fact]
        public void ThrowIfNullOrEmptyWithMessage_ParamIsEmpty()
        {
            var param = string.Empty;
            var message = "message";
            var exception = Assert.Throws<ArgumentNullException>(() => param.ThrowIfNullOrEmptyWithMessage(message));
            Assert.StartsWith(message, exception.Message);
            Assert.Equal(nameof(param), exception.ParamName);
        }

        [Fact]
        public void ThrowIfNullOrEmptyWithMessage_ParamIsValid_DifferentParamName()
        {
            string param = "valid";
            param.ThrowIfNullOrEmptyWithMessage("message", "paramName");
        }

        [Fact]
        public void ThrowIfNullOrEmptyWithMessage_ParamIsNull_DifferentParamName()
        {
            string? param = null;
            var message = "message";
            var exception = Assert.Throws<ArgumentNullException>(() => param.ThrowIfNullOrEmptyWithMessage(message, "paramName"));
            Assert.StartsWith(message, exception.Message);
            Assert.Equal("paramName", exception.ParamName);
        }

        [Fact]
        public void ThrowIfNullOrEmptyWithMessage_ParamIsEmpty_DifferentParamName()
        {
            var param = string.Empty;
            var message = "message";
            var exception = Assert.Throws<ArgumentNullException>(() => param.ThrowIfNullOrEmptyWithMessage(message, "paramName"));
            Assert.StartsWith(message, exception.Message);
            Assert.Equal("paramName", exception.ParamName);
        }

        [Fact]
        public void OrThrowIfNullOrEmpty_ParamIsValid()
        {
            var param = "valid";
            var value = param.OrThrowIfNullOrEmpty();
            Assert.Equal(param, value);
        }

        [Fact]
        public void OrThrowIfNullOrEmpty_ParamIsNull()
        {
            string? param = null;
#pragma warning disable CS8604 // Possible null reference argument.
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfNullOrEmpty());
#pragma warning restore CS8604 // Possible null reference argument.
            Assert.Equal(nameof(param), exception.ParamName);
        }

        [Fact]
        public void OrThrowIfNullOrEmpty_ParamIsEmpty()
        {
            var param = string.Empty;
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfNullOrEmpty());
            Assert.Equal(nameof(param), exception.ParamName);
        }

        [Fact]
        public void OrThrowIfNullOrEmpty_ParamIsValid_DifferentParamName()
        {
            var param = "valid";
            var value = param.OrThrowIfNullOrEmpty("paramName");
            Assert.Equal(param, value);
        }

        [Fact]
        public void OrThrowIfNullOrEmpty_ParamIsNull_DifferentParamName()
        {
            string? param = null;
#pragma warning disable CS8604 // Possible null reference argument.
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfNullOrEmpty("paramName"));
#pragma warning restore CS8604 // Possible null reference argument.
            Assert.Equal("paramName", exception.ParamName);
        }

        [Fact]
        public void OrThrowIfNullOrEmpty_ParamIsEmpty_DifferentParamName()
        {
            var param = string.Empty;
            var exception = Assert.Throws<ArgumentNullException>(() => _ = param.OrThrowIfNullOrEmpty("paramName"));
            Assert.Equal("paramName", exception.ParamName);
        }
    }
}
