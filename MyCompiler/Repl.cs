using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MyCompiler;

internal abstract class Repl
{
    private List<string> _submissionHistory = new();
    private int _submissionHistoryIndex;
    private bool _done;

    public void Run()
    {
        while (true)
        {
            var text = EditSubmission();

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!text.Contains(Environment.NewLine) && text.StartsWith('#'))
            {
                EvaluateMetaInput(text);
                continue;
            }

            EvaluateSubmission(text);
            _submissionHistory.Add(text);
            _submissionHistoryIndex = 0;
        }
    }

    protected abstract bool IsCompleteSubmission(string text);

    protected abstract void EvaluateSubmission(string text);

    protected virtual void EvaluateMetaInput(string input)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"Invalid command {input}");
        Console.ResetColor();
    }
    
    protected virtual void RenderLine(string line) => Console.Write(line);

    protected void ClearHistory() => _submissionHistory.Clear();

    private string EditSubmission()
    {
        _done = false;
        var document = new ObservableCollection<string> { "" };
        var view = new SubmissionView(document, RenderLine);

        while (!_done)
        {
            var key = Console.ReadKey(true);
            HandleKey(key, document, view);
        }

        view.CurrentLineIndex = document.Count - 1;
        view.CurrentCharacterIndex = document[view.CurrentLineIndex].Length;
        Console.WriteLine();

        return string.Join(Environment.NewLine, document);
    }

    private void HandleKey(ConsoleKeyInfo key, ObservableCollection<string> document, SubmissionView view)
    {
        switch (key.Modifiers)
        {
            case ConsoleModifiers.Shift:
            case ConsoleModifiers.None:
            {
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        HandleEscape(document, view);
                        break;
                    case ConsoleKey.Enter:
                        HandleEnter(document, view);
                        break;
                    case ConsoleKey.LeftArrow:
                        HandleLeftArrow(document, view);
                        break;
                    case ConsoleKey.RightArrow:
                        HandleRightArrow(document, view);
                        break;
                    case ConsoleKey.UpArrow:
                        HandleUpArrow(view);
                        break;
                    case ConsoleKey.DownArrow:
                        HandleDownArrow(document, view);
                        break;
                    case ConsoleKey.Backspace:
                        HandleBackspace(document, view);
                        break;
                    case ConsoleKey.Delete:
                        HandleDelete(document, view);
                        break;
                    case ConsoleKey.Home:
                        HandleHome(view);
                        break;
                    case ConsoleKey.End:
                        HandleEnd(document, view);
                        break;
                    case ConsoleKey.Tab:
                        HandleTab(document, view);
                        break;
                    case ConsoleKey.PageUp:
                        HandlePageUp(document, view);
                        break;
                    case ConsoleKey.PageDown:
                        HandlePageDown(document, view);
                        break;
                }

                if (key.KeyChar >= ' ')
                {
                    HandleTyping(document, view, key.KeyChar.ToString());
                }

                break;
            }
            case ConsoleModifiers.Control:
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleControlEnter(document, view);
                        break;
                }

                break;
            case ConsoleModifiers.Alt:
            default:
                break;
        }
    }

    private static void HandleEscape(ObservableCollection<string> document, SubmissionView view)
    {
        document[view.CurrentLineIndex] = string.Empty;
        view.CurrentCharacterIndex = 0;
    }

    private void HandleEnter(ObservableCollection<string> document, SubmissionView view)
    {
        var submissionText = string.Join(Environment.NewLine, document);
        if (submissionText.StartsWith('#') || IsCompleteSubmission(submissionText))
        {
            _done = true;
            return;
        }

        InsertLine(document, view);
    }

    private static void HandleControlEnter(ObservableCollection<string> document, SubmissionView view) =>
        InsertLine(document, view);

    private static void InsertLine(ObservableCollection<string> document, SubmissionView view)
    {
        var lineIndex = view.CurrentLineIndex;
        var start = view.CurrentCharacterIndex;
        var line = document[lineIndex];
        var newLine = line[start..];
        document[lineIndex] = line[..start];
        document.Insert(lineIndex + 1, newLine);
        view.CurrentLineIndex++;
        view.CurrentCharacterIndex = 0;
    }

    private static void HandleLeftArrow(ObservableCollection<string> document, SubmissionView view)
    {
        if (view.CurrentCharacterIndex > 0)
        {
            view.CurrentCharacterIndex--;
        }
        else
        {
            if (view.CurrentLineIndex > 0)
            {
                view.CurrentLineIndex--;
                view.CurrentCharacterIndex = document[view.CurrentLineIndex].Length - 1;
            }
        }
    }

    private static void HandleRightArrow(ObservableCollection<string> document, SubmissionView view)
    {
        var line = document[view.CurrentLineIndex];
        if (view.CurrentCharacterIndex < line.Length)
        {
            view.CurrentCharacterIndex++;
        }
        else if (view.CurrentLineIndex < document.Count - 1)
        {
            view.CurrentLineIndex++;
            view.CurrentCharacterIndex = 0;
        }
    }

    private static void HandleUpArrow(SubmissionView view)
    {
        if (view.CurrentLineIndex > 0)
        {
            view.CurrentLineIndex--;
        }
    }

    private static void HandleDownArrow(ObservableCollection<string> document, SubmissionView view)
    {
        if (view.CurrentLineIndex < document.Count - 1)
        {
            view.CurrentLineIndex++;
        }
    }

    private static void HandleBackspace(ObservableCollection<string> document, SubmissionView view)
    {
        var lineIndex = view.CurrentLineIndex;
        var start = view.CurrentCharacterIndex;
        if (start == 0)
        {
            if (lineIndex == 0)
            {
                return;
            }

            var currentLine = document[lineIndex];
            var previousLine = document[lineIndex - 1];
            document.RemoveAt(lineIndex);
            view.CurrentLineIndex--;
            document[view.CurrentLineIndex] = previousLine + currentLine;
            view.CurrentCharacterIndex = previousLine.Length;
        }
        else
        {
            document[lineIndex] = document[lineIndex].Remove(start - 1, 1);
            view.CurrentCharacterIndex--;
        }
    }

    private static void HandleDelete(ObservableCollection<string> document, SubmissionView view)
    {
        var lineIndex = view.CurrentLineIndex;
        var start = view.CurrentCharacterIndex;
        var line = document[lineIndex];
        if (start >= line.Length)
        {
            if (lineIndex == document.Count - 1)
            {
                return;
            }

            var nextLine = document[lineIndex + 1];
            document.RemoveAt(lineIndex + 1);
            document[lineIndex] += nextLine;
        }
        else
        {
            document[lineIndex] = line.Remove(start, 1);
        }
    }

    private static void HandleHome(SubmissionView view)
    {
        view.CurrentCharacterIndex = 0;
    }

    private static void HandleEnd(ObservableCollection<string> document, SubmissionView view)
    {
        view.CurrentCharacterIndex = document[view.CurrentLineIndex].Length;
    }

    private static void HandleTab(ObservableCollection<string> document, SubmissionView view)
    {
        var lineIndex = view.CurrentLineIndex;
        var start = view.CurrentCharacterIndex;
        var line = document[lineIndex];
        var remainder = line[start..];
        var spaces = 4 - start % 4;
        document[lineIndex] = line[..start] + new string(' ', spaces) + remainder;
        view.CurrentCharacterIndex += spaces;
    }

    private void HandlePageUp(ObservableCollection<string> document, SubmissionView view)
    {
        if (_submissionHistory.Count == 0)
        {
            return;
        }
        _submissionHistoryIndex--;
        if (_submissionHistoryIndex < 0)
        {
            _submissionHistoryIndex = _submissionHistory.Count - 1;
        }

        UpdateDocumentFromHistory(document, view);
    }

    private void HandlePageDown(ObservableCollection<string> document, SubmissionView view)
    {
        if (_submissionHistory.Count == 0)
        {
            return;
        }
        _submissionHistoryIndex++;
        if (_submissionHistoryIndex > _submissionHistory.Count - 1)
        {
            _submissionHistoryIndex = 0;
        }

        UpdateDocumentFromHistory(document, view);
    }

    private void UpdateDocumentFromHistory(ObservableCollection<string> document, SubmissionView view)
    {
        document.Clear();
        var historyItem = _submissionHistory[_submissionHistoryIndex];
        var lines = historyItem.Split(Environment.NewLine);
        foreach (var line in lines)
        {
            document.Add(line);
        }

        view.CurrentLineIndex = document.Count - 1;
        view.CurrentCharacterIndex = document[view.CurrentLineIndex].Length;
    }

    private static void HandleTyping(ObservableCollection<string> document, SubmissionView view, string text)
    {
        var lineIndex = view.CurrentLineIndex;
        var start = view.CurrentCharacterIndex;
        document[lineIndex] = document[lineIndex].Insert(start, text);
        view.CurrentCharacterIndex += text.Length;
    }

    private sealed class SubmissionView
    {
        private readonly ObservableCollection<string> _submissionDocument;
        private readonly Action<string> _lineRederer;
        private readonly int _cursorTop;
        private int _renderedLineCount;
        private int _currentLineIndex;
        private int _currentCharacterIndex;

        public SubmissionView(ObservableCollection<string> submissionDocument, Action<string> lineRederer)
        {
            _submissionDocument = submissionDocument;
            _lineRederer = lineRederer;
            _submissionDocument.CollectionChanged += SubmissionDocumentChanged;
            _cursorTop = Console.CursorTop;
            Render();
        }

        private void SubmissionDocumentChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            Console.CursorVisible = false;

            var lineCount = 0;

            foreach (var line in _submissionDocument)
            {
                Console.SetCursorPosition(0, _cursorTop + lineCount);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(lineCount == 0 ? "» " : "· ");
                Console.ResetColor();
                _lineRederer(line);
                Console.WriteLine(new string(' ', Console.WindowWidth - line.Length));
                lineCount++;
            }

            var numberOfBlankLines = _renderedLineCount - lineCount;

            if (numberOfBlankLines > 0)
            {
                var blankLine = new string(' ', Console.WindowWidth);
                for (var i = 0; i < numberOfBlankLines; i++)
                {
                    Console.SetCursorPosition(0, _cursorTop + lineCount + i);
                    Console.Write(blankLine);
                }
            }

            _renderedLineCount = lineCount;
            Console.CursorVisible = true;
            UpdateCursorPosition();
        }

        private void UpdateCursorPosition()
        {
            Console.CursorTop = _cursorTop + _currentLineIndex;
            Console.CursorLeft = _currentCharacterIndex + 2;
        }

        public int CurrentLineIndex
        {
            get => _currentLineIndex;
            set
            {
                if (_currentLineIndex == value)
                {
                    return;
                }

                _currentLineIndex = value;
                UpdateCursorPosition();
            }
        }

        public int CurrentCharacterIndex
        {
            get => _currentCharacterIndex;
            set
            {
                if (_currentCharacterIndex == value)
                {
                    return;
                }

                _currentCharacterIndex = value;
                UpdateCursorPosition();
            }
        }
    }
}