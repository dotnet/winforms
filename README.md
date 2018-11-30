# Windows Forms
 [![Build status](https://dnceng.visualstudio.com/7ea9116e-9fac-403d-b258-b31fcf1bb293/_apis/build/status/199?branchName=master)](https://dnceng.visualstudio.com/internal/_build/latest?definitionId=199&branch=master)

Windows Forms (WinForms) is a framework for building rich Windows desktop
applications using .NET. Since it leverages a what-you-see-is-what-you-get
(WYSIWYG) graphical editor with drag & drop, it's also one of the easiest and
most productive ways to create desktop applications.

This repo contains the open-source components of WinForms that run on top of
.NET Core. It is based on, but separate from, the version of WinForms that is
part of the .NET Framework.

We haven't finished porting WinForms to .NET Core yet, which means not all
source code is on GitHub. You can expect us to complete this over the next
months in 2019. The reason it takes some time is that we need to support & build all the
pieces in an open source way, which requires decoupling the code base from our
internal engineering system. At the same time, we don't want to block open
sourcing until the port is complete. This is similar to how other .NET
Core repos with existing code have been brought up, such as
[CoreFx](https://github.com/dotnet/corefx) in 2014.

Even though .NET Core is a cross-platform technology, WinForms only runs on
Windows.



## What are the benefits of building WinForms apps with .NET Core?

There are three primary reasons for considering .NET Core over the .NET
Framework for building desktop apps with WinForms:

1. **More deployment options**. You can deploy .NET Core side-by-side or even
   produce self-contained apps that you can just XCOPY deploy.

2. **Contains features that we can't provide in .NET Framework**. Due to the
   fact that .NET Framework is deployed as an in-place update, the
   compatibility requirements are extremely high, which prevents us from making fundamental
   changes, such as performance improvements or making the UI more high-DPI
   aware by default.

3. **.NET Core goodness**. As an open source platform with side-by-side deployment, 
   .NET Core is receiving a lot of innovations and contributions from Microsoft teams and 
   from the open source community. As an application author, you also benefit from those 
   when building WinForms apps with .NET Core.

To learn more, read the blog post [Update on .NET Core 3.0 and .NET Framework 4.8][update-post].



## Quick Links

* [.NET Core 3.0 SDK Preview 1](https://www.microsoft.com/net/download)
* [Overall .NET Core roadmap & shipdates](https://github.com/dotnet/core/blob/master/roadmap.md)



## Getting started with WinForms on .NET Core

Follow [getting started instructions](Documentation/getting-started.md).



## How to Engage, Contribute and Provide Feedback

Some of the best ways to contribute are to try things out, file bugs, join in design conversations, and fix issues.

* Use [daily builds](Documentation/getting-started.md#installation).
* If you have a question or found a bug, [file a new issue](https://github.com/dotnet/winforms/issues/new).
    * Issues with WinForms on .NET Framework should be filed on [VS developer community](https://developercommunity.visualstudio.com/spaces/61/index.html), or [Product Support](https://support.microsoft.com/en-us/contactus?ws=support) if you have a contract.

**IMPORTANT:** WinForms for .NET Core 3.0 release focuses on parity with WinForms for .NET Framework.
We do not plan to take contributions or address bugs that are not unique to WinForms on .NET Core in 3.0 release.
Bugs which are present on both WinForms platforms (for .NET Core and .NET Framework) will be prioritized for future releases of .NET Core (post-3.0).

### Issue Guide

Read our detailed [issue guide](Documentation/issue-guide.md) which covers:

* How to file high-quality bug reports
* How to use and understand Labels, Milestones, Assignees and Upvotes on issues
* How to escalate (accidentally) neglected issue or PR
* How we triage issues

For general .NET Core 3 issues (not specific to WinForms), use the [.NET Core repo](https://github.com/dotnet/core/issues) or other repos if appropriate (e.g. [CoreFX](https://github.com/dotnet/corefx/issues), [WPF](https://github.com/dotnet/wpf/issues)).

### Contributing Guide

Read our detailed [contributing guide](Documentation/contributing-guide.md) which covers:

* Which kind of PRs we accept/reject for .NET Core 3.0 release
* Coding style and PR gotchas
* Developer guide for building and testing WinForms code

### Community

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org/) 
to clarify expected behavior in our community.
For more information, see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

### Reporting security issues and security bugs

Security issues and bugs should be reported privately, via email, to the Microsoft Security Response Center (MSRC) <secure@microsoft.com>. You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the MSRC PGP key, can be found in the [Security TechCenter](https://www.microsoft.com/msrc/faqs-report-an-issue).

Also see info about related [Microsoft .NET Core and ASP.NET Core Bug Bounty Program](https://www.microsoft.com/msrc/bounty-dot-net-core).



## License

.NET Core (including WinForms repo) is licensed under the [MIT license](LICENSE.TXT).



## .NET Foundation

.NET Core WinForms is a [.NET Foundation](https://www.dotnetfoundation.org/projects) project.

There are many .NET related projects on GitHub.

- [.NET home repo](https://github.com/Microsoft/dotnet) - links to 100s of .NET
  projects, from Microsoft and the community.

[update-post]: https://blogs.msdn.microsoft.com/dotnet/2018/10/04/update-on-net-core-3-0-and-net-framework-4-8/
