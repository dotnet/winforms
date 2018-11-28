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

## Installation

TODO: Add link

## Contributing

Since we're currently still porting parts of the code to GitHub, we're not
equipped to handle larger contributions yet. Our goal is to accept contributions
on day one, but we'll only be able to accept minor modifications (typos, build
fixes, test breaks etc). We'll be happy to hear your feedback!

* For WinForms on .NET Core, you can simply file bugs and feature requests in
  [this repo](https://github.com/dotnet/winforms/issues/new).
* For WinForms on .NET Framework, please us the existing [Visual Studio
  developer community](https://developercommunity.visualstudio.com/spaces/61/index.html).
* For .NET Core 3 in general, use the [.NET Core](https://github.com/dotnet/core/issues/) repo.

For more details, take a look at the [.NET Core Contribution
Guidelines](https://github.com/dotnet/coreclr/blob/master/Documentation/project-docs/contributing.md).

## Community

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org/) 
to clarify expected behavior in our community.
For more information, see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## Reporting security issues and security bugs

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
