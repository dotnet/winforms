# Instructions for GitHub and VisualStudio Copilot
### https://github.blog/changelog/2025-01-21-custom-repository-instructions-are-now-available-for-copilot-on-github-com-public-preview/


## General

* In the main branch, write code that runs on .NET 10.0, but keep in mind that we have only a few .NET Framework and multi-targeted code files.

* Make only high confidence suggestions when reviewing code changes.

* Do not edit files in `eng/common/`.


## Formatting

* Apply code-formatting style defined in `.editorconfig`.

* Follow instructions in `docs\coding-style.md`.


## Nullable Reference Types

* Declare variables non-nullable, and check for `null` at entry points.

* Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.


## Language features.

* Always use the latest version C# or VisualBasic, currently C# 13 features.

* Use the new collection initializer syntax when possible. For example:
  - Prefer
    ```csharp
    List<int> list = [1, 2, 3];
    ```
    over
    ```csharp
    List<int> list = new List<int> { 1, 2, 3 };
    ```