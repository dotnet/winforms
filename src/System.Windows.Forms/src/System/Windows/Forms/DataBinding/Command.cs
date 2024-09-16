// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal class Command : WeakReference
{
    private static Command?[]? s_cmds;
    private static int s_icmdTry;
    private static readonly Lock s_internalSyncObject = new();
    private const int IdMin = 0x00100;
    private const int IdLim = 0x10000;

    internal int _id;

    public Command(ICommandExecutor target)
        : base(target, false)
    {
        AssignID(this);
    }

    public virtual int ID => _id;

    protected static void AssignID(Command cmd)
    {
        lock (s_internalSyncObject)
        {
            int icmd;

            if (s_cmds is null)
            {
                s_cmds = new Command[20];
                icmd = 0;
            }
            else
            {
                Debug.Assert(s_cmds.Length > 0, "why is cmds.Length zero?");
                Debug.Assert(s_icmdTry >= 0, "why is icmdTry negative?");

                int icmdLim = s_cmds.Length;

                if (s_icmdTry >= icmdLim)
                {
                    s_icmdTry = 0;
                }

                // First look for an empty slot (starting at icmdTry).
                for (icmd = s_icmdTry; icmd < icmdLim; icmd++)
                {
                    if (s_cmds[icmd] is null)
                    {
                        goto FindSlotComplete;
                    }
                }

                for (icmd = 0; icmd < s_icmdTry; icmd++)
                {
                    if (s_cmds[icmd] is null)
                    {
                        goto FindSlotComplete;
                    }
                }

                // All slots have Command objects in them. Look for a command
                // with a null referent.
                for (icmd = 0; icmd < icmdLim; icmd++)
                {
                    if (s_cmds[icmd]!.Target is null)
                    {
                        goto FindSlotComplete;
                    }
                }

                // Grow the array.
                icmd = s_cmds.Length;
                icmdLim = Math.Min(IdLim - IdMin, 2 * icmd);

                if (icmdLim <= icmd)
                {
                    // Already at maximal size. Do a garbage collect and look again.
                    GC.Collect();
                    for (icmd = 0; icmd < icmdLim; icmd++)
                    {
                        if (s_cmds[icmd] is null || s_cmds[icmd]!.Target is null)
                        {
                            goto FindSlotComplete;
                        }
                    }

                    throw new ArgumentException(SR.CommandIdNotAllocated);
                }
                else
                {
                    Command[] newCmds = new Command[icmdLim];
                    Array.Copy(s_cmds, 0, newCmds, 0, icmd);
                    s_cmds = newCmds;
                }
            }

        FindSlotComplete:

            cmd._id = icmd + IdMin;
            Debug.Assert(cmd._id is >= IdMin and < IdLim, "generated command id out of range");

            s_cmds[icmd] = cmd;
            s_icmdTry = icmd + 1;
        }
    }

    public static bool DispatchID(int id)
    {
        Command? cmd = GetCommandFromID(id);
        if (cmd is null)
        {
            return false;
        }

        return cmd.Invoke();
    }

    protected static void Dispose(Command cmd)
    {
        lock (s_internalSyncObject)
        {
            if (cmd._id >= IdMin)
            {
                cmd.Target = null;
                if (s_cmds![cmd._id - IdMin] == cmd)
                {
                    s_cmds[cmd._id - IdMin] = null;
                }

                cmd._id = 0;
            }
        }
    }

    public virtual void Dispose()
    {
        if (_id >= IdMin)
        {
            Dispose(this);
        }
    }

    public static Command? GetCommandFromID(int id)
    {
        lock (s_internalSyncObject)
        {
            if (s_cmds is null)
            {
                return null;
            }

            int i = id - IdMin;
            if (i < 0 || i >= s_cmds.Length)
            {
                return null;
            }

            return s_cmds[i];
        }
    }

    public virtual bool Invoke()
    {
        object? target = Target;
        if (target is not ICommandExecutor executor)
        {
            return false;
        }

        executor.Execute();
        return true;
    }
}
