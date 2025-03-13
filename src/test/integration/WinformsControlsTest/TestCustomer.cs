// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

/// <summary>
///  It is implementation of a simple customer class
///  to represent the test data items used by code example in <see cref="DataGridViewInVirtualModeTest"/>
/// </summary>
internal class TestCustomer
{
    public TestCustomer()
    {
        // Leave fields empty
    }

    public TestCustomer(string name, int age, bool hasAJob, string gender)
    {
        Name = name;
        Age = age;
        HasAJob = hasAJob;
        Gender = gender;
    }

    public string Name { get; set; }

    public int Age { get; set; }

    public bool HasAJob { get; set; }

    public string Gender { get; set; }
}
