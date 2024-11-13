namespace CSharpCourse.DesignPatterns.Assignments
{
    internal record TextEditorMemento
    {
        public required string Content { get; init; }
    }

    internal interface IVersionedTextEditor
    {
        string Content { get; }
        int RedoCount { get; }
        int UndoCount { get; }

        void ChangeContent(string content);

        void Redo();

        void Undo();
    }

    internal class VersionedTextEditor : IVersionedTextEditor
    {
        private readonly Stack<TextEditorMemento> _redoStack = new();
        private readonly Stack<TextEditorMemento> _undoStack = new();
        private string _content = string.Empty;
        public string Content => _content;
        public int RedoCount => _redoStack.Count;
        public int UndoCount => _undoStack.Count;

        public void ChangeContent(string content)
        {
            if (content != _content)
            {
                _undoStack.Push(new TextEditorMemento { Content = _content });
                _redoStack.Clear();
                _content = content;
            }
        }

        public void Redo()
        {
            if (_redoStack.Count == 0)
                throw new InvalidOperationException("No actions to redo.");

            _undoStack.Push(new TextEditorMemento { Content = _content });
            _content = _redoStack.Pop().Content;
        }

        public void Undo()
        {
            if (_undoStack.Count == 0)
                throw new InvalidOperationException("No actions to undo.");

            _redoStack.Push(new TextEditorMemento { Content = _content });
            _content = _undoStack.Pop().Content;
        }
    }
}