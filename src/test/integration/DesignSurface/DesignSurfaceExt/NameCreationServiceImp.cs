// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DemoConsole;

/// <summary>
/// Implements <see cref="INameCreationService"/> to provide names for newly created controls.
/// </summary>
/// <remarks>
///  <para>
///   The <see cref="INameCreationService"/> interface is used to generate names for newly created controls.
///   The <c>CreateName</c> method follows the same naming algorithm used by Visual Studio:
///   it increments an integer counter until it finds a unique name that is not already in use.
///  </para>
/// </remarks>
internal sealed class NameCreationService : INameCreationService
{
    public string CreateName(IContainer container, Type type)
    {
        if (container is null)
            return string.Empty;

        ComponentCollection cc = container.Components;
        int min = int.MaxValue;
        int max = int.MinValue;
        int count = 0;

        int i = 0;
        while (i < cc.Count)
        {
            if (cc[i] is Component comp && comp.GetType() == type)
            {
                string name = comp.Site.Name;
                if (name.StartsWith(type.Name, StringComparison.Ordinal))
                {
                    count++;
                    try
                    {
                        int value;
                        value = int.Parse(name[type.Name.Length..]);
                        if (value < min)
                            min = value;
                        if (value > max)
                            max = value;
                    }
                    catch (Exception) { }
                }
            }

            i++;
        }

        if (count == 0)
        {
            return $"{type.Name}1";
        }
        else if (min > 1)
        {
            int j = min - 1;
            return $"{type.Name}{j}";
        }
        else
        {
            int j = max + 1;
            return $"{type.Name}{j}";
        }
    }

    public bool IsValidName(string name)
    {
        // - Check that name is "something" and that is a string with at least one char
        if (string.IsNullOrEmpty(name))
            return false;

        // - then the first character must be a letter
        if (!char.IsLetter(name, 0))
            return false;

        // - then don't allow a leading underscore
        if (name[0] == '_')
            return false;

        // - ok, it's a valid name
        return true;
    }

    public void ValidateName(string name)
    {
        // -  Use our existing method to check, if it's invalid throw an exception
        if (!IsValidName(name))
            throw new ArgumentException($"Invalid name: {name}");
    }
}
