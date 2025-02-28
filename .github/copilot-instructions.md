Always use 2 spaces for each indentation level in xml-doc comments.

Suggest to break lines that are longer than 120 characters in C# files

In the new and existing C# code, prefer to break lines after lambda operator `=>` if needed, do not move `=>` to the new line.

If a line in the existing C# file starts with a `=>` then move the lambda operator to the previous line.

Always use the latest version C#, currently C# 13 features.

Always write code that runs on .NET 10.0.

Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.

Use file-scoped namespaces.

Do not edit files in `eng/common/`.