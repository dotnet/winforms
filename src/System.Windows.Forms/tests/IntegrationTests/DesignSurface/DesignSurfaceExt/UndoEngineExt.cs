using System.ComponentModel.Design;

namespace DesignSurfaceExt;

public class UndoEngineExt : UndoEngine
{
    private Stack<UndoEngine.UndoUnit> undoStack = new();
    private Stack<UndoEngine.UndoUnit> redoStack = new();

    public UndoEngineExt(IServiceProvider provider) : base(provider) { }

    public bool EnableUndo
    {
        get { return undoStack.Count > 0; }
    }

    public bool EnableRedo
    {
        get { return redoStack.Count > 0; }
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            try
            {
                UndoEngine.UndoUnit unit = undoStack.Pop();
                unit.Undo();
                redoStack.Push(unit);
                //Log("::Undo - undo action performed: " + unit.Name);
            }
            catch
            {
                //Log("::Undo() - Exception " + ex.Message + " (line:" + new StackFrame(true).GetFileLineNumber() + ")");
            }
        }
        else
        {
            //Log("::Undo - NO undo action to perform!");
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            try
            {
                UndoEngine.UndoUnit unit = redoStack.Pop();
                unit.Undo();
                undoStack.Push(unit);
                //Log("::Redo - redo action performed: " + unit.Name);
            }
            catch
            {
                //Log("::Redo() - Exception " + ex.Message + " (line:" + new StackFrame(true).GetFileLineNumber() + ")");
            }
        }
        else
        {
            //Log("::Redo - NO redo action to perform!");
        }
    }

    protected override void AddUndoUnit(UndoEngine.UndoUnit unit)
    {
        undoStack.Push(unit);
    }
}

