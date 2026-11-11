"""Disk sweeps a harness's daily janitor runs, so a factory host's disk does not fill with finished work.

The harness passes each sweep its paths, time, limits and what only it knows, such as its running jobs' worktrees
and the branches of open pull requests. Each sweep honors ``dry_run`` and returns the actions it took or would take.
Branches, commits, job records and uncommitted work stay. ``sweep`` runs sweeps on their own, so one failure keeps
none of the others' space. The build-cache lock uses ``fcntl``, so this module needs a POSIX host.
"""

from __future__ import annotations

import contextlib
import fcntl
import fnmatch
import glob
import os
import re
import shutil
import subprocess
import time
from collections.abc import Callable, Collection, Mapping, Sequence
from pathlib import Path
from typing import TextIO

DAY = 86400
PROC = Path("/proc")
GIT_TIMEOUT = 120
DOCKER_TIMEOUT = 120
LOCK_WAIT = 60.0  # seconds to wait for builds to release the build-cache lock
PINNED_IMAGE = re.compile(r"[A-Za-z0-9][A-Za-z0-9._/:-]*@sha256:[0-9a-f]{64}")


class GitError(Exception):
    """A git command that failed or timed out."""


def sweep(sweeps: Mapping[str, Callable[[], list[str]]]) -> list[str]:
    """Each named sweep's actions in order. A sweep that raises adds ``<name> sweep failed: <error>`` and the others
    still run, so a harness wraps a read a sweep needs, such as its open pull requests, in that sweep."""
    actions = []
    for name, run in sweeps.items():
        try:
            actions += run()
        except Exception as error:  # reported as an action, so one failure keeps none of the others' space
            actions.append(f"{name} sweep failed: {error}")
    return actions


def _git(*args: str, cwd: Path) -> str:
    try:
        done = subprocess.run(["git", *args], cwd=cwd, capture_output=True, text=True, timeout=GIT_TIMEOUT,
                              check=False)
    except subprocess.TimeoutExpired:
        raise GitError(f"git {' '.join(args)} timed out") from None
    if done.returncode:
        raise GitError(done.stderr.strip() or f"git {' '.join(args)} exited {done.returncode}")
    return done.stdout.strip()


def _remove(path: Path) -> str | None:
    """Remove a file, link or directory tree without following links; return the error that stopped it, if any."""
    try:
        if path.is_dir() and not path.is_symlink():
            shutil.rmtree(path)
        else:
            path.unlink()
    except FileNotFoundError:
        return None
    except OSError as error:
        return str(error)
    return None


def _archives(directory: Path) -> list[Path]:
    if not directory.is_dir():
        return []
    return sorted(path for path in directory.iterdir() if path.is_dir() and not path.is_symlink())


def _matching(archive: Path, patterns: Collection[str]) -> list[Path]:
    return sorted(path for path in archive.iterdir() if any(fnmatch.fnmatchcase(path.name, p) for p in patterns))


def remove_archived_credentials(directory: Path, names: Collection[str], dry_run: bool) -> list[str]:
    """Delete, at any age, the entries of each archive under ``directory`` that match a pattern in ``names``, such
    as a job's private forge configuration holding its token, which a killed job leaves behind."""
    actions = []
    for archive in _archives(directory):
        if found := _matching(archive, names):
            errors = [] if dry_run else [error for error in map(_remove, found) if error]
            actions.append(f"cannot remove credentials from {archive.name}: {errors[0]}" if errors
                           else f"removed credentials from {archive.name}")
    return actions


def _age(archive: Path, now: float, dated_by: Sequence[str]) -> float:
    for path in (*(archive / name for name in dated_by), archive):
        with contextlib.suppress(OSError):
            return now - path.lstat().st_mtime
    return 0.0


def trim_archives(directory: Path, now: float, days: int, scratch: Collection[str], dry_run: bool,
                  dated_by: Sequence[str] = ()) -> list[str]:
    """Delete the entries matching a ``scratch`` pattern, such as logs and copied helpers, of each archive under
    ``directory`` that finished more than ``days`` ago, and keep its other entries.

    An archive finished when the first existing ``dated_by`` entry, such as its exit record, last changed, else
    when the archive directory did. Copied helpers keep their source's modification time, so they never date one.
    """
    actions = []
    for archive in _archives(directory):
        found = _matching(archive, scratch)
        if not found or _age(archive, now, dated_by) <= days * DAY:
            continue
        errors = [] if dry_run else [error for error in map(_remove, found) if error]
        actions.append(f"trimmed scratch files of {archive.name}" + (f"; kept some: {errors[0]}" if errors else ""))
    return actions


def _worktrees(root: Path, git: Callable[..., str]) -> list[dict[str, str]]:
    """`git worktree list --porcelain` records, each mapping a line's first word to the rest of the line."""
    records, record = [], {}
    for line in git("worktree", "list", "--porcelain", cwd=root).splitlines() + [""]:
        if line:
            key, _, value = line.partition(" ")
            record[key] = value
        elif record:
            records.append(record)
            record = {}
    return records


def _last_used(admin: Path) -> float:
    """Newest change to a worktree's Git state: every checkout, commit, stage or reset rewrites one of these."""
    times = []
    for path in (admin, admin / "HEAD", admin / "index", admin / "logs" / "HEAD"):
        with contextlib.suppress(OSError):
            times.append(path.stat().st_mtime)
    return max(times, default=float("inf"))


def in_use(path: Path, proc: Path = PROC) -> bool:
    """Whether a process this user can see works inside ``path``, such as an open shell, editor or build. Without
    ``proc`` (a host with no ``/proc``), every path counts as in use."""
    if not proc.is_dir():
        return True
    for cwd in proc.glob("[0-9]*/cwd"):
        with contextlib.suppress(OSError):
            if Path(os.readlink(cwd)).is_relative_to(path):
                return True
    return False


def idle_worktree_kept(path: Path, now: float, days: int, proc: Path = PROC,
                       git: Callable[..., str] | None = None) -> str | None:
    """Why a worktree must stay, or None: Git activity within ``days``, a process inside it, uncommitted or
    untracked files, or a detached commit that no ref contains. ``git`` runs Git as in ``remove_idle_worktrees``."""
    git = git or _git
    if now - _last_used(Path(git("rev-parse", "--absolute-git-dir", cwd=path))) <= days * DAY:
        return "recently used"
    if in_use(path, proc):
        return "a process works in it"
    # Explicit flags, because a status.showUntrackedFiles=no setting hides untracked files from git worktree remove.
    if git("--no-optional-locks", "status", "--porcelain", "--untracked-files=normal", "--ignore-submodules=none",
           cwd=path):
        return "it has uncommitted changes"
    if not git("for-each-ref", "--contains", "HEAD", "--count=1", "--format=%(refname)", cwd=path):
        return "no ref contains its HEAD"
    return None


def remove_idle_worktrees(root: Path, parent: Path, now: float, days: int, open_branches: Collection[str],
                          dry_run: bool, keep: Collection[str] = (), owned: Collection[str | Path] = (),
                          proc: Path = PROC, git: Callable[..., str] | None = None) -> list[str]:
    """Remove the linked worktrees of the repository at ``root`` that sit directly under ``parent``, that nobody
    used for ``days`` and that nothing else needs.

    A worktree stays when its name matches a ``keep`` pattern (such as the harness's own worktrees), ``owned``
    holds it (each live job's worktree; a relative entry, such as the ID of a job whose record is unreadable, names
    one under ``parent``), an open pull request heads its branch, Git marks it bare, locked or prunable, it holds
    ``root``, or ``idle_worktree_kept`` gives a reason. Removal runs ``git worktree remove`` without ``--force``,
    so Git itself refuses a worktree that changed since the check. Branches, commits and stashes stay; ignored
    files, such as build outputs, go with the worktree.

    ``git(*args, cwd=path)`` runs each Git command and returns its stripped output, raising ``GitError`` on failure,
    so a harness can pass its own runner with its timeouts, logging and test patches. Without it, the module's
    ``_git`` runs, read at each call, so a harness that rebinds ``_git`` keeps working.
    """
    git = git or _git
    parent, here = parent.resolve(), root.resolve()
    jobs = {(parent / entry).resolve() for entry in owned}
    actions = []
    # The first record is the main worktree, which is never removed.
    for record in sorted(_worktrees(root, git)[1:], key=lambda record: record.get("worktree", "")):
        if "worktree" not in record or {"bare", "locked", "prunable"} & record.keys():
            continue
        path = Path(record["worktree"]).resolve()
        branch = record.get("branch", "").removeprefix("refs/heads/")
        if (path.parent != parent or any(fnmatch.fnmatchcase(path.name, pattern) for pattern in keep)
                or path in jobs or branch in open_branches or here.is_relative_to(path) or not path.is_dir()):
            continue
        try:
            if idle_worktree_kept(path, now, days, proc, git):
                continue
            if not dry_run:
                git("worktree", "remove", str(path), cwd=root)
        except (GitError, OSError) as error:
            actions.append(f"kept worktree {path.name}: {error}")
            continue
        actions.append(f"removed idle worktree {path.name}" + (f" (branch {branch} kept)" if branch else ""))
    return actions


def directory_bytes(path: Path) -> int:
    """Bytes allocated under ``path``, like ``du``: links are not followed, hard links count once and entries that
    vanish during the walk count as zero."""
    total, seen, pending = 0, set(), [path]
    while pending:
        with contextlib.suppress(OSError), os.scandir(pending.pop()) as entries:
            for entry in entries:
                with contextlib.suppress(OSError):
                    if entry.is_dir(follow_symlinks=False):
                        pending.append(entry.path)
                        continue
                    info = entry.stat(follow_symlinks=False)
                    if info.st_nlink > 1:
                        if (info.st_dev, info.st_ino) in seen:
                            continue
                        seen.add((info.st_dev, info.st_ino))
                    total += info.st_blocks * 512
    return total


def _wait_for_lock(handle: TextIO, seconds: float) -> bool:
    deadline = time.monotonic() + seconds
    while True:
        try:
            fcntl.flock(handle, fcntl.LOCK_EX | fcntl.LOCK_NB)
            return True
        except BlockingIOError:
            if time.monotonic() >= deadline:
                return False
            time.sleep(1)


def cap_build_cache(cache: Path, cap: int, lock: Path, dry_run: bool, fence: Path | None = None,
                    lock_wait: float = LOCK_WAIT) -> list[str]:
    """Delete a shared build-output directory, such as a Cargo target, once it outgrows ``cap`` bytes; the next
    build rebuilds it cold. Leftover ``<cache>.trash-*`` siblings of an interrupted wipe go first.

    The cache is moved aside only while this process holds an exclusive ``flock`` on ``lock``, a file outside the
    cache that every build using the cache holds (shared or exclusive) while it runs, and while no ``fence`` file
    exists, which a build that links to the cache keeps until it unlinks, so no build uses a cache being deleted. A
    whole wipe, not a partial prune, keeps incremental-build fingerprints consistent.
    """
    actions = []
    for trash in sorted(cache.parent.glob(f"{glob.escape(cache.name)}.trash-*")):
        error = None if dry_run else _remove(trash)
        actions.append(f"cannot remove leftover {trash.name}: {error}" if error else f"removed leftover {trash.name}")
    if not cache.is_dir() or cache.is_symlink():
        return actions
    size = directory_bytes(cache)
    if size <= cap:
        return actions
    summary = f"{size / 2**30:.1f} GiB, over the {cap / 2**30:g} GiB cap"
    if dry_run:
        return actions + [f"deleted build cache {cache.name}: {summary}"]
    trash = cache.with_name(f"{cache.name}.trash-{time.time_ns()}")
    with open(lock, "a", encoding="utf-8") as handle:
        if not _wait_for_lock(handle, lock_wait):
            return actions + [f"kept build cache {cache.name} ({summary}): a build holds its lock"]
        if fence is not None and os.path.lexists(fence):
            return actions + [f"kept build cache {cache.name} ({summary}): a fence withholds it"]
        os.replace(cache, trash)
    remains = f"; {trash.name} remains: {error}" if (error := _remove(trash)) else ""
    return actions + [f"deleted build cache {cache.name}: {summary}{remains}"]


def image_repository(name: str) -> str:
    """``name`` without its tag; a registry port before the last slash is not a tag."""
    return name.rsplit(":", 1)[0] if ":" in name.rsplit("/", 1)[-1] else name


def stale_images(listing: str, pins: Collection[str]) -> list[str]:
    """``repository@digest`` of each untagged image of a pinned repository whose digest no pin references.

    ``listing`` holds ``docker image ls --digests`` rows of tab-separated repository, tag and digest. Images pulled
    by digest are untagged, so a superseded pin leaves one; tagged images and other repositories are someone's own.
    """
    pins = [pin for pin in pins if PINNED_IMAGE.fullmatch(pin)]
    digests = {pin.rsplit("@", 1)[1] for pin in pins}
    repositories = {image_repository(pin.rsplit("@", 1)[0]) for pin in pins}
    stale = set()
    for row in listing.splitlines():
        fields = row.split("\t")
        if (len(fields) == 3 and fields[0] in repositories and fields[1] == "<none>"
                and fields[2].startswith("sha256:") and fields[2] not in digests):
            stale.add(f"{fields[0]}@{fields[2]}")
    return sorted(stale)


def remove_stale_images(pins: Collection[str], dry_run: bool,
                        run: Callable[..., subprocess.CompletedProcess] = subprocess.run,
                        which: Callable[[str], str | None] = shutil.which) -> list[str]:
    """Remove the local Docker images ``stale_images`` finds; without docker or a valid pin, do nothing.

    ``pins`` holds ``repository[:tag]@sha256:<digest>`` references, such as every one in the base branch's tree;
    ``PINNED_IMAGE`` finds them in text. ``docker image rm`` runs without ``--force``, so an image a container still
    uses stays; a branch's newer pin is pulled again when it next runs.
    """
    if not any(PINNED_IMAGE.fullmatch(pin) for pin in pins) or which("docker") is None:
        return []

    def docker(*argv: str) -> subprocess.CompletedProcess:
        return run(["docker", *argv], capture_output=True, text=True, timeout=DOCKER_TIMEOUT, check=False)

    listed = docker("image", "ls", "--digests", "--format", "{{.Repository}}\t{{.Tag}}\t{{.Digest}}")
    if listed.returncode:
        return [f"kept Docker images: cannot list them: {(listed.stderr or '').strip()}"]
    actions = []
    for image in stale_images(listed.stdout, pins):
        removed = None if dry_run else docker("image", "rm", image)
        actions.append(f"kept Docker image {image}: {(removed.stderr or '').strip()}" if removed and removed.returncode
                       else f"removed Docker image {image}")
    return actions
