C# only cares about type names for supporting types for C# features. As such, to
enable features that depend on types, but not other runtime support, we copy types
here as internal types.

This allows using null annotation, ranges, etc. on .NET Framework.