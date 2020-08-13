# Issue Guide

This page outlines how WinForms team thinks about and handles issues.
For us, issues on GitHub represent actionable work that should be done at some future point.
It may be as simple as a small product or test bug or as large as the work tracking the design of a new feature.

We will keep issues open even if the WinForms team internally has no plans to address them in an upcoming release, as long as we believe they are in line with our direction.

# How to file issues

You can help us streamline our response time to your feedback and ideas by filing high-quality reports.

### Known Issues

If you encounter an issue using the latest .NET Core 3.1 SDK, please:

1. Check out the [Centralized 3.1 Preview Known Issues document][.net-core-3.1-known-issues],
1. Explore [this repositories issues][winforms-issues], and then
1. [Open a new issue][new-issue] if there is none.

### Bug reports

In general, try to be specific. Get straight to the main point. Leave additional details, options, and alternatives to the end (hint: separate them visually). Don't write long bug reports, unless you have to.

* Include a minimal reproduction (repro) in your bug if at all possible (chop off dependencies, remove as much code as possible). If it is not possible, say why.<br />
  :warning: Note: Yes, it may take some time to minimize a repro from your larger app - but that is exactly what we would do in most cases anyway. Issues with clear, concise reproduction guidelines are easier for us to investigate; such issues will have higher chance of being addressed quickly.
* Include call-stacks, symptom descriptions, and differences between actual and expected behaviors.

### Feature and API suggestions

Assume that the reader has minimal knowledge and experience with writing apps/libraries that would benefit from the feature, so try and write to that audience. Provide clear description of your suggestion, and explain scenarios in which it would be helpful and why (motivation).

Prepare code examples that showcase the need and general usefulness of the proposal. Specifically:
* Code that shows the surface area of the API.
* Code that shows real world scenarios, and how they would otherwise be handled.
* Details showing the usage/consumption of the proposed new package, and alternatives (e.g. not having this be a separate package).

:warning: All new API or changes to public API must undergo an API review process. Learn more what the [API review process][api-review-process] entails.

Here are few example of a good API proposal: 
* https://github.com/dotnet/runtime/issues/15725
* https://github.com/dotnet/runtime/issues/15725


## Assignee

We assign each issue to assignee, when the assignee is ready to pick up the work and start working on it.
If the issue is not assigned to anyone and you want to pick it up, please say so - we will assign the issue to you.
If the issue is already assigned to someone, please coordinate with the assignee before you start working on it.

## Up-votes on issues

Up-votes on first post of each issue are useful hint for our prioritization.
We can [sort issues by number of up-votes][up-votes], and we will review the top list on a regular basis.

## Triage rules

Guidance for triage of issues for Windows Forms team members:

1. Issue has no **Assignee**, unless someone is working on the issue at the moment.
1. Use **up-for-grabs** as much as possible, ideally with a quick note about next steps / complexity of the issue.
1. Set milestone to **Future**, unless you can 95%-commit you can fund the issue in specific milestone.
1. Each issue has exactly one "*issue type*" label (**bug**, **enhancement**, **api-suggestion**, **test-bug**, **test-enhancement**, **question**, **documentation**, etc.).
1. Don't be afraid to say no, or close issues — just explain why and be polite.
1. Don't be afraid to be wrong - just be flexible when new information appears.

Feel free to use other labels if it helps your triage efforts (for example, **needs-more-info**, **design-discussion**, **tenet-compatibility**, etc.).

### Motivation for triage rules

1. Issue has no **Assignee**, unless someone is working on the issue at the moment.
    * Motivation: Observation is that contributors are less likely to grab assigned issues, no matter what the repository rules say.

1. Use **up-for-grabs** as much as possible, ideally with a quick note about next steps / complexity of the issue.<br />
    NB: Per [http://up-for-grabs.net][up-for-grabs-net], such issues should be no longer than few nights' worth of work. They should be actionable (that is, no mysterious CI failures that can't be tested in the open).

1. Set milestone to **Future**, unless you can 95%-commit you can fund the issue in specific milestone.
    * Motivation: Helps communicate desire/timeline to community. Can spark further priority/impact discussion.

1. Each issue has exactly one "*issue type*" label (**bug**, **enhancement**, **api-suggestion**, **test-bug**, **test-enhancement**, **question**, **documentation**, etc.).
    * Don't be afraid to be wrong when deciding 'bug' vs. 'test-bug' (flip a coin if you must). The most useful values for tracking are 'api-&#42;' vs. 'enhancement', 'question', and 'documentation'.


# Pull-Request (PR) Guide

Each PR has to have reviewer approval from at least one Windows Forms team member who is not author of the change, before it can be merged.


1. Please don't set any labels on PRs. 
    * exceptions:
      * **NO-MERGE** may be supplied by the WinForms team in order to indicate that the PR should be halted; a reason should be given in the comments
      * **waiting-on-testing** may be supplied by the WinForms team to inform the PR author(s) that their PR is being delayed due to internal, manual testing; this action does not require action by the author(s).
    * Motivation: All the important info (*issue type* label, API approval label, etc.) is already captured on the associated issue.

1. Push PRs forward, don't let them go stale (response every 5+ days, ideally no PRs older than 2 weeks).

1. Close stuck or long-term blocked PRs (for example, due to missing API approval, etc.) and reopen them once they are unstuck.
    * **Motivation: Keep only active PRs. WIP (work-in-progress) PRs should be rare and should not become stale (2+ weeks old). If a PR is stale and there is not immediate path forward, consider closing the PR until it is unblocked/unstuck.

1. Link PR to related issue via [auto-closing][auto-closing] (add "Fixes #12345" into your PR description).

[comment]: <> (URI Links)


[winforms-issues]: https://github.com/dotnet/winforms/issues
[new-issue]: https://github.com/dotnet/winforms/issues/new/choose
[api-review-process]: https://github.com/dotnet/runtime/blob/master/docs/project/api-review-process.md
[labels]: https://github.com/dotnet/winforms/labels
[api-suggestion]: https://github.com/dotnet/winforms/labels/api-suggestion
[API Review process]: https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/api-review-process.md
[bug]: https://github.com/dotnet/winforms/labels/bug
[enhancement]: https://github.com/dotnet/winforms/labels/enhancement
[test-bug]: https://github.com/dotnet/winforms/labels/test-bug
[test-enhancement]: https://github.com/dotnet/winforms/labels/test-enhancement
[question]: https://github.com/dotnet/winforms/labels/question
[documentation]: https://github.com/dotnet/winforms/labels/documentation
[label-description]: https://github.com/dotnet/winforms/labels
[up-for-grabs]: https://github.com/dotnet/winforms/labels/up-for-grabs
[needs-more-info]: https://github.com/dotnet/winforms/labels/needs-more-info
[tenet-compatibility]: https://github.com/dotnet/winforms/labels/tenet-compatibility
[milestones]: https://github.com/dotnet/winforms/milestones
[up-votes]: #upvotes-on-issues
[sort issues by number of up-votes]: https://github.com/dotnet/winforms/issues?q=is%3Aissue+is%3Aopen+sort%3Areactions-%2B1-desc
[up-for-grabs-net]: http://up-for-grabs.net
[auto-closing]: https://help.github.com/articles/closing-issues-via-commit-messages/