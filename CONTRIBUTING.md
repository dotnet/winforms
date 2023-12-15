# Contributing Guide

> :warning: Please note, this document is a subset of [Contribution to .NET Runtime][net-contributing], make sure to read it first.

You can contribute to Windows Forms with issues, pull-requests, and general reviews of both issues and pull-requests. Simply filing issues for problems you encounter is a great way to contribute. Contributing implementations is greatly appreciated.

## Contribution "Bar"

Project maintainers will merge changes that improve the product significantly and broadly and that align with the our [Roadmap](docs/roadmap.md).

Maintainers will not merge changes that have narrowly-defined benefits, due to compatibility risk. The Windows Forms .NET codebase is used by a significant number of internal and external customers world-wide. We may revert changes if they are found to be breaking.

Whilst most .NET Core/.NET components are cross-platform Windows Forms implementations remain tightly coupled with Win32 API. With this we will typically not accept contributions that provide cross-platform implementations.

## DOs and DON'Ts

Please do:

* **DO** follow our [coding style][coding-style] (C# code-specific)<br/>
  We strive to wrap the lines around 120 mark, and it's acceptable to stretch to no more than 150 chars (with some exceptions being URLs). [EditorGuidelines VS extension](https://marketplace.visualstudio.com/items?itemName=PaulHarrington.EditorGuidelines) makes it easier to visualise (see https://github.com/dotnet/winforms/pull/4836).
* **DO** give priority to the current style of the project or file you're changing even if it diverges from the general guidelines.
* **DO** include tests when adding new features. When fixing bugs, start with
  adding a test that highlights how the current behavior is broken.
* **DO** keep the discussions focused. When a new or related topic comes up
  it's often better to create new issue than to side track the discussion.
* **DO** blog and tweet (or whatever) about your contributions, frequently!

Please do not:

* **DON'T** make PRs for style changes.
* **DON'T** surprise us with big pull requests. Instead, file an issue and start
  a discussion so we can agree on a direction before you invest a large amount
  of time.
* **DON'T** commit code that you didn't write. If you find code that you think is a good fit to add to .NET Core, file an issue and start a discussion before proceeding.
* **DON'T** submit PRs that alter licensing related files or headers. If you believe there's a problem with them, file an issue and we'll be happy to discuss it.
* **DON'T** add API additions without filing an issue and discussing with us first. See [API Review Process][api-review-process].

## Breaking Changes

Contributions must maintain [API signature][breaking-changes-public-contract] and behavioral compatibility. Contributions that include [breaking changes][breaking-changes] will be rejected. Please file an issue to discuss your idea or change if you believe that it may affect managed code compatibility.

## Up for Grabs

The team marks the most straightforward issues as [good first issue](https://github.com/dotnet/winforms/labels/good%20first%20issue) and  [help wanted](https://github.com/dotnet/winforms/labels/help%20wanted). This set of issues is the place to start if you are interested in contributing but new to the codebase.

## PR Feedback

Project maintainers and community members will provide feedback on your change. Community feedback is highly valued. You will often see the absence of team feedback if the community has already provided good review feedback.

One or more project maintainers members will review every PR prior to merge. They will often reply with "LGTM, modulo comments". That means that the PR will be merged once the feedback is resolved. "LGTM" == "looks good to me".

There are lots of thoughts and [approaches](https://github.com/antlr/antlr4-cpp/blob/master/CONTRIBUTING.md#emoji) for how to efficiently discuss changes. It is best to be clear and explicit with your feedback. Please be patient with people who might not understand the finer details about your approach to feedback.


[comment]: <> (URI Links)

[api-review-process]: https://github.com/dotnet/runtime/blob/master/docs/project/api-review-process.md
[breaking-changes]: https://github.com/dotnet/runtime/blob/master/docs/coding-guidelines/breaking-changes.md#bucket-1-public-contract
[breaking-changes-public-contract]: https://github.com/dotnet/runtime/blob/master/docs/coding-guidelines/breaking-changes.md#bucket-1-public-contract
[coding-style]: docs/coding-style.md
[net-contributing]: https://github.com/dotnet/runtime/blob/master/CONTRIBUTING.md
