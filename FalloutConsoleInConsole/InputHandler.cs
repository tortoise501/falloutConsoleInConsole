public class InputHandler
{
  private string rightWord = "";
  public InputHandler(string rightWord, Game game)
  {
    this.rightWord = rightWord;
    this.game = game;
    width = game.COLUMN_WIDTH;
  }
  public ExecutionCode GetExecutionCodeForElement(Element element)
  {
    if (element is Word)
    {
      if (rightWord == ((Word)element).word)
      {
        return ExecutionCode.CorrectWord;
      }
      else
      {
        return ExecutionCode.Mistake;
      }
    }
    if (element is Hint)
    {
      if (((Hint)element).hintType == HintType.Dud)
      {
        return ExecutionCode.HintDuds;
      }
      else
      {
        return ExecutionCode.HintLife;
      }
    }
    return ExecutionCode.WrongInput;
  }

  static int xCursorPos = 0;
  static int yCursorPos = 0;
  public static int selectedColumn { get; private set; } = 0;
  static int width;
  public static int selectedPos { get => xCursorPos + yCursorPos * width; private set { selectedPos = value; } }
  Game game;
  public void HandleInput()
  {
    ConsoleKey key = Console.ReadKey().Key;

    Element element = game.columns[selectedColumn].GetElement(selectedPos);
    switch (key)
    {
      case ConsoleKey.LeftArrow:
        {
          xCursorPos--;
          if (xCursorPos < 0)
          {
            xCursorPos = game.COLUMN_WIDTH - 1;
            selectedColumn = Math.Max(0, selectedColumn - 1);
          }
          break;
        }
      case ConsoleKey.RightArrow:
        {
          xCursorPos++;
          if (xCursorPos > game.COLUMN_WIDTH - 1)
          {
            xCursorPos = 0;
            selectedColumn = Math.Min(game.COLUMN_AMOUNT - 1, selectedColumn + 1);
          }
          break;
        }
      case ConsoleKey.UpArrow:
        {
          yCursorPos = Math.Max(0, yCursorPos - 1);
          break;
        }
      case ConsoleKey.DownArrow:
        {
          yCursorPos = Math.Min(game.COLUMN_HEIGHT - 1, yCursorPos + 1);
          break;
        }
      case ConsoleKey.Enter:
        {
          string log = ExecuteCodeAndReturnLogs(GetExecutionCodeForElement(element), element, game.columns[selectedColumn]);
          game.AddLog(log);
          break;
        }
    }
  }
  public string ExecuteCodeAndReturnLogs(ExecutionCode code, Element element, Column column)
  {
    string log = "";
    switch (code)
    {
      case ExecutionCode.HintLife:
        {
          column.RemoveHint(element.index);
          game.ResetAttempt();
          log = $"Tries Reset.";
          break;
        }
      case ExecutionCode.HintDuds:
        {
          column.RemoveHint(element.index);
          column.RemoveDud(selectedPos, rightWord);
          log = $">{element}\n>Dud Removed.";
          break;
        }
      case ExecutionCode.Mistake:
        {
          game.LooseAttempt();
          log = $">{element}\n>Entry denied.\n>Likeness={CheckForLikeness(element.ToString())}";
          break;
        }
      case ExecutionCode.CorrectWord:
        {
          game.WinGame();
          log = $">{element}\n>Password Accepted.";
          break;
        }
      case ExecutionCode.WrongInput:
        {
          return "";
        }
    }
    return log;
  }
  private int CheckForLikeness(string word)//TODO: make it better adn shorter
  {
    if (word.Length != rightWord.Length)
    {
      throw new Exception("Stupid chat gpt exception");
    }
    int res = 0;
    for (int i = 0; i < word.Length; i++)
    {
      if (word[i] == rightWord[i])
      {
        res++;
      }
    }
    return res;
  }
}