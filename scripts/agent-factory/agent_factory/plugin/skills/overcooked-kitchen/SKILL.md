---
name: overcooked-kitchen
description: 'Starts or stops Overcooked Agents, a local read-only 3D kitchen that shows each Copilot session on this machine, including factory jobs, as a chef, filtered to the current repository. Use when asked to run, show, open or stop the agent kitchen or overcooked-agents.'
---

# Run the Agent Kitchen Locally

**Inputs:** start or stop, and optionally a port other than `7425`.

[Overcooked Agents](https://github.com/patniko/overcooked-agents) is an
external, unlicensed hobby tool. It needs no install and reads
`events.jsonl` and `workspace.yaml` from the Copilot session-state folder, so
it can see every prompt on the machine. These steps run it at the reviewed
commit below, which reads that folder, runs `ps -p` (or PowerShell on
Windows) for liveness, and makes no writes or outbound requests.

## Steps

1. Check `node --version` reports 20 or later.
2. Clone the tool outside any repository you work in, or reuse an existing
   clone, and check out the reviewed commit:

   ```bash
   (set -e
   dir=~/repos/overcooked-agents pin=8e025a5a23f1293624de5c08e2febd24f12a77c7
   [ -d "$dir" ] || git clone -q https://github.com/patniko/overcooked-agents "$dir"
   [ -z "$(git -C "$dir" status --porcelain --ignored=no)" ] || { echo "uncommitted changes in $dir" >&2; exit 1; }
   git -C "$dir" fetch -q origin
   git -C "$dir" checkout -q --detach "$pin"
   [ "$(git -C "$dir" rev-parse HEAD)" = "$pin" ])
   ```

   If it fails, stop: never reset, stash or discard the clone's changes;
   ask their owner, or clone afresh into another directory.

   To move the pin, review `server/` for file writes, child processes and
   outbound requests, then update this skill in a reviewed pull request.
3. Never reuse a running kitchen: an answering health URL does not show
   which commit it runs or which address it listens on. Stop one you
   started (step 7), or pick a free `PORT` (default `7425`):
   `ss -ltn "sport = :$PORT"` lists no listener (macOS:
   `lsof -nP -iTCP:"$PORT" -sTCP:LISTEN` prints nothing). Use that `PORT`
   below.
4. From the clone, start the server as a long-lived background process (an
   agent uses a detached shell), and note its PID:

   ```bash
   node server/index.js --port "$PORT" --state-dir "${COPILOT_HOME:-$HOME/.copilot}/session-state"
   ```

   Factory jobs on this machine write to the same folder, so they appear
   beside human-steered sessions.
5. Confirm `ss -ltnp "sport = :$PORT"` (macOS:
   `lsof -nP -iTCP:"$PORT" -sTCP:LISTEN`) shows only a `127.0.0.1` listener
   whose PID is the one you started; match the PID, not the name, which
   recent Node releases report as `MainThread`. Otherwise stop it (step 7).
   Confirm `curl -s "http://127.0.0.1:$PORT/api/health"` returns `"ok":true`
   with `sessions` and `visible` counts. The port opens only after the first
   scan of the folder, which can take several seconds; retry until then.
6. Open `http://127.0.0.1:$PORT/?repo=$REPO`. The filter keeps only
   sessions whose `workspace.yaml` `repository` equals `REPO`, ignoring
   case. Copilot records the `owner/name` of the session's `origin`, so
   derive it in the repository whose agents you want to watch:
   `REPO=$(git remote get-url origin | sed -E 's#\.git$##; s#^.*[:/]([^/]+/[^/]+)$#\1#')`.
   Omit `?repo=` to show every repository. Add `&privacy=1` before screen
   sharing; it hides prompts in the page only.
   - WSL forwards localhost to Windows:
     `powershell.exe -NoProfile -Command "Start-Process '<url>'"`.
   - macOS: `open '<url>'`. Linux desktop: `xdg-open '<url>'`.
   - Headless or remote host: forward the port over SSH
     bound to loopback, so `GatewayPorts` cannot expose it
     (`ssh -L 127.0.0.1:$PORT:127.0.0.1:$PORT <host>`), or with VS Code port
     forwarding.
   - With no sessions to show, `?demo` plays simulated agents.
7. To stop, run `kill <PID>` on only the PID noted in step 5 (or, for a
   kitchen you started earlier, the one step 5's `ss` or `lsof` command
   shows), never another kitchen's, and confirm the health URL no longer
   answers.

**Output:** the kitchen URL with the health counts, or the stop confirmation.

**Done when:** the browser shows the kitchen and health answers, or after a
stop that PID has exited and the port is free.

**Stop and escalate:** you need a change beyond the pinned commit; the
upstream repository is deleted or force-pushed, so step 2's clone, fetch or
checkout fails (never substitute a fork, mirror or other commit without a
reviewed pull request that moves the pin); or someone asks to share the
kitchen with other machines.

**Never:** pass a `--host` other than `127.0.0.1`, because `/api/state` serves
every session's prompts to whoever can reach it; copy, vendor or commit the
tool, or screenshots that show prompts, into a repository; run `npm install`
or an unreviewed commit; stop it with `pkill` or `killall`.
