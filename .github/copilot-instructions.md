# Instructions for GitHub and VisualStudio Copilot
### https://github.blog/changelog/2025-01-21-custom-repository-instructions-are-now-available-for-copilot-on-github-com-public-preview/

## General

* In the main branch, write code that runs on .NET 10.0, but keep in mind that we have a couple of.NET Framework and multi-targeted code files.
* Make only high-confidence suggestions when reviewing code changes.
* Do not edit files in `eng/common/`.
* Apply guidelines defined in `.editorconfig`.


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