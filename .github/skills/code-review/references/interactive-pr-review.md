# Interactive PR Review Workflow

Read this reference only for pull request reviews initiated in an interactive
CLI or cloud-agent conversation. Hosted GitHub Copilot code review must use the
platform flow in `SKILL.md` instead.

## Prepare the PR source

Resolve the PR head locally before inspecting its patch. Use an isolated
detached worktree when the current checkout is not exactly the PR head or
contains local changes. This preserves the user's branch and provides full
source context.

1. Read PR metadata:

   ```powershell
   $pr = <number>
   $repo = "dotnet/winforms"
   gh pr view $pr --repo $repo --json number,title,body,author,baseRefName,baseRefOid,headRefName,headRefOid,url,isDraft,mergeable,reviewDecision
   ```

2. Fetch the GitHub pull ref and base branch:

   ```powershell
   $baseBranch = gh pr view $pr --repo $repo --json baseRefName --jq ".baseRefName"
   $headOid = gh pr view $pr --repo $repo --json headRefOid --jq ".headRefOid"
   $remote = "upstream" # Remote that points to $repo; use `git remote -v` to confirm
   git fetch $remote "pull/$pr/head:refs/remotes/pull/$pr/head" --quiet
   git fetch $remote $baseBranch --quiet
   ```

3. Create a uniquely named temporary worktree outside the repository:

   ```powershell
   $reviewRoot = Join-Path $env:TEMP "winforms-pr-$pr-$($headOid.Substring(0, 8))-$PID"
   git worktree add --detach $reviewRoot $headOid
   ```

4. Compute the actual PR range:

   ```powershell
   $baseOid = git merge-base "$remote/$baseBranch" $headOid
   git -C $reviewRoot diff --name-status --find-renames --find-copies "$baseOid...$headOid"
   git -C $reviewRoot diff --find-renames --find-copies "$baseOid...$headOid"
   ```

If fetching or creating a worktree is impossible, use `gh pr diff` and GitHub
file reads as a fallback, and state that surrounding local context was limited.
Do not use `gh pr checkout`, stash, or switch branches in the user's working
tree.

After the review, remove only the temporary worktree created by this workflow:

```powershell
git worktree remove $reviewRoot
```

Do not remove a worktree if it contains unexpected changes.

## Publish selected findings

1. Present the numbered findings locally first.
2. Ask the user which findings to post. Accept selections such as "all",
   "1 and 3", modified wording, or "none".
3. Re-read the target diff lines and existing review comments immediately
   before posting.
4. Put line-specific findings on changed lines. Put issues that cannot be
   attached accurately to a changed line in the review summary.
5. Post one problem per comment. Identify the primary site for a repeated root
   cause and list other occurrences there.
6. Default the review event to `COMMENT`. Use `APPROVE` or
   `REQUEST_CHANGES` only when the user explicitly requests it.
7. Before an approval, check `autoMergeRequest`. If auto-merge is enabled and
   comments remain, warn that approval may merge the PR before the comments are
   addressed and wait for the user's decision.
8. When posting under the user's identity, include a concise visible note in
   the review summary that the content was AI-generated, unless the user asks
   to omit it.

Use an available GitHub review tool or `gh api` to submit one review containing
the selected comments. Never post draft, duplicate, or unselected findings.
