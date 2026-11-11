# Thin wrapper: `copilot-fix-issue.yml`

[Architecture](architecture.md) | [Tier 1 adoption](adoption.md)

A single GitHub Actions workflow
(`.github/workflows/copilot-fix-issue.yml`) that runs the vendored
`fix-issue` skill against one issue and opens a **draft** pull request. This
is the step beyond Tier 1 described in [architecture.md](architecture.md#5-what-this-repository-has-adopted-tier-1-only):
no claiming, no scheduling, no auto-merge, no multi-host coordination — one
issue, one run, one human-reviewed draft PR.

## What it does, step by step

1. Resolves the issue number (manual input, or the issue a maintainer just labeled `copilot-fix`).
2. Checks out the repo on `windows-latest`, installs Node.js and `@github/copilot` (`npm install -g @github/copilot`).
3. Reads the issue's title/body with `gh issue view`.
4. Creates a branch `copilot/<issue>-<slug>`.
5. Builds a prompt that tells the agent to follow the vendored `fix-issue` + `factory-coordination` skills (loaded via `--plugin-dir scripts/agent-factory/agent_factory/plugin`), this repository's `issue_tracking`/`pr_template` policy files, and to stop (writing `docs/factory/BLOCKED.md`, never committed) instead of guessing on ambiguous scope or sensitive paths.
6. Runs `copilot --prompt ... --mode autopilot --allow-all-tools` with explicit `--deny-tool` entries blocking `git push`, `gh pr merge`, `gh pr review` and `gh api`, and budget flags (`--max-ai-credits`, `--max-autopilot-continues`) mirroring `.agent-factory/profile.toml`'s `[budgets]`.
7. If the agent wrote `docs/factory/BLOCKED.md`: posts its reason as an issue comment and stops — no branch is pushed.
8. Otherwise: pushes the branch and opens a **draft** PR using the agent's `docs/factory/PR_BODY.md` as the body (falling back to a generic body if it didn't write one).
9. Uploads the CLI's `usage.json` as a workflow artifact either way.

## Prerequisites

- **Org policy:** "Allow use of Copilot CLI billed to the organization" enabled for Copilot CLI (Copilot org settings). No extra secret is needed — the workflow uses the default `GITHUB_TOKEN` with the `copilot-requests: write` permission.
- **Repository permissions:** the default `GITHUB_TOKEN` must be allowed `contents: write`, `issues: write`, `pull-requests: write` (repository Settings → Actions → General → Workflow permissions, or this workflow's own `permissions:` block, which already requests them).
- Nothing from `scripts/agent-factory/` needs installing on the runner; the workflow points `--plugin-dir` straight at the vendored copy already committed in this repository.

## Triggering it

- **Manual (recommended for first runs):** Actions tab → "Copilot fix issue (agent-factory, thin wrapper)" → Run workflow → enter an issue number.
- **Automatic:** apply the `copilot-fix` label to an open issue. The job only runs for labels applied by an `OWNER`/`MEMBER`/`COLLABORATOR` (checked via `github.event.sender.author_association`), so an external triager or the issue's own author cannot trigger it by labeling.

## Safety notes / what is deliberately NOT automatic

- **No merge, ever.** The workflow never calls `gh pr merge`; the PR is opened as a draft for a human to review and merge like any other PR. `--deny-tool` also blocks the agent itself from merging, approving or pushing to any ref.
- **Prompt-injection awareness.** The issue body is fed to the agent as data; `--allow-all-tools` is required for non-interactive mode, so the `--deny-tool` list is the real backstop against an issue body instructing the agent to do something destructive. Treat this list as a minimum, not a complete sandbox — see the harness design doc's stated non-goal: "Hostile-code isolation... guardrails, not a sandbox."
- **Budgets are duplicated, not read from the profile.** `MAX_AI_CREDITS`/`MAX_AUTOPILOT_CONTINUES` at the top of the workflow mirror `.agent-factory/profile.toml`'s `[budgets]` by hand. Keep them in sync yourself; no shared reader wires the two together yet.
- **Not yet executed end-to-end.** The workflow's YAML was validated and its `copilot` CLI flags checked against `copilot --help` output, but it has not been run live in this repository. Test it first with `workflow_dispatch` on a small, low-risk issue before relying on the `copilot-fix` label trigger.
