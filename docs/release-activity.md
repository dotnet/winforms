# Runtime release procedures

## Milestone releases (dotnet/winforms + dotnet/windowsdesktop)

1. Merge any outstanding PRs, if necessary
2. Ensure the HEAD of the main branch builds successfully and all tests pass
3. Publish any unpublished API - run [.\eng\ApiCompatibility\mark-shipped.cmd](https://github.com/dotnet/winforms/blob/main/eng/ApiCompatibility/mark-shipped.cmd)

4. Commit and wait for a successful build

5. (Milestone releases) Create "release/X.Y-qwe" branch (e.g. release/6.0-preview5) and push it into the main public trunk and the internal trunk.
Wait for both build successfully.
* Main public trunk: https://github.com/dotnet/winforms/
* Internal trunk: <need url>
6. Disable loc on "release/X.Y-qwe" branch (e.g.: https://github.com/dotnet/winforms/pull/6753)
7. Switch back to the main branch
8. Update the Service Fabric bot with the new release label in .\github\fabricbot.json. Refer to .\docs\infra\automation.md, if have questions.
9. Update the branding (e.g.: https://github.com/dotnet/winforms/commit/719b2112778ffd1e7ded559ea2736b4011428117)
10. Commit and wait for a successful build.

11. Create new [milestone](https://github.com/dotnet/winforms/milestones) for the new release before updating following bot service.  

12. Move work items to this new milestone if they were on previous milestone and close the previous milestone.

13. Update necessary reports with the progress

14. If you are responsible for [dotnet/windowsdesktop](https://github.com/dotnet/windowsdesktop) repo, make sure you change the branding there too.

## Servicing releases (dotnet/winforms + dotnet/windowsdesktop)

[NET Servicing - Overview (azure.com)](https://dev.azure.com/devdiv/DevDiv/_wiki/wikis/DevDiv.wiki/545/NET-Servicing)

1. Merge internal servicing branch (i.e. internal/internal/release/*) into the public branch, if necessary.  

2. Update the branding (e.g.: https://github.com/dotnet/winforms/commit/480bd703c5af93b9e23dc7435c6029d685be7097)
3. Commit and wait for a successful build.

4. Ensure the HEAD of the servicing branch builds successfully and all tests pass

5. Update [dotnet/installer](https://github.com/dotnet/installer) with the released version (e.g.: https://github.com/dotnet/installer/commit/34ed90f436809631e254b9e15628cbcad7307629)

6. Create a new [milestone](https://github.com/dotnet/winforms/milestones).

7. Close the current milestone on the GitHub, ensuring all open issues/PRs are either closed or bumped to the following milestone

8. Merge any outstanding PRs, if necessary
