// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace DesignSurfaceExt;

public class UndoEngineExt : UndoEngine
{
    private readonly Stack<UndoUnit> _undoStack = new();
    private readonly Stack<UndoUnit> _redoStack = new();

    public UndoEngineExt(IServiceProvider provider) : base(provider) { }

    public bool EnableUndo
    {
        get { return _undoStack.Count > 0; }
    }

    public bool EnableRedo
    {
        get { return _redoStack.Count > 0; }
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            try
            {
                UndoUnit unit = _undoStack.Pop();
                unit.Undo();
                _redoStack.Push(unit);
                // Log("::Undo - undo action performed: " + unit.Name);
            }
            catch
            {
                // Log("::Undo() - Exception " + ex.Message + " (line:" + new StackFrame(true).GetFileLineNumber() + ")");
            }
        }
        else
        {
            // Log("::Undo - NO undo action to perform!");
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            try
            {
                UndoUnit unit = _redoStack.Pop();
                unit.Undo();
                _undoStack.Push(unit);
                // Log("::Redo - redo action performed: " + unit.Name);
            }
            catch
            {
                // Log("::Redo() - Exception " + ex.Message + " (line:" + new StackFrame(true).GetFileLineNumber() + ")");
            }
        }
        else
        {
            // Log("::Redo - NO redo action to perform!");
        }
    }

    protected override void AddUndoUnit(UndoUnit unit)
    {
        _undoStack.Push(unit);
    }
}
