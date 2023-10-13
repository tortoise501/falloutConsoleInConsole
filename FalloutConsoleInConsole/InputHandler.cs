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
  public int selectedColumn { get; private set; } = 0;
  static int width;
  public Coordinates cursorPos = new Coordinates(0, 0);
  Game game;
  public void HandleInput()
  {
    ConsoleKey key = Console.ReadKey().Key;

    Element element = game.columns[selectedColumn].GetSymbolElement(cursorPos);
    switch (key)
    {
      case ConsoleKey.LeftArrow:
        {
          Element bringCursorTo = null;
          if (element is Symbol && element.elementType == ElementType.Word)
          {
            if ((element as Symbol).belongsToWord.coordinates.y == cursorPos.y)
            {
              bringCursorTo = (element as Symbol).belongsToWord;
            }
            else
            {
              bringCursorTo = (element as Symbol).belongsToWord.slaveElements.Where(slave => slave.coordinates.y == cursorPos.y).MinBy(slave => slave.coordinates.x);
            }
          }
          if (bringCursorTo != null)
          {
            cursorPos.x = bringCursorTo.coordinates.x - 1;
          }
          else
          {
            cursorPos.x--;
          }
          if (cursorPos.x < 0)
          {
            if (selectedColumn != 0)
            {
              cursorPos.x = game.COLUMN_WIDTH - 1;
            }
            else
            {
              cursorPos.x = 0;
            }
            selectedColumn = Math.Max(0, selectedColumn - 1);
          }
          break;
        }
      case ConsoleKey.RightArrow:
        {
          if (element.elementType == ElementType.Word)
          {
            Element bringCursorTo = null;
            if (element is Word)
            {
              bringCursorTo = (element as Word).slaveElements.Where(slave => slave.coordinates.y == cursorPos.y).MaxBy(slave => slave.coordinates.x);
            }
            else
            {
              bringCursorTo = (element as Symbol).belongsToWord.slaveElements.Where(slave => slave.coordinates.y == cursorPos.y).MaxBy(slave => slave.coordinates.x);
            }

            if (bringCursorTo != null)
            {
              cursorPos.x = bringCursorTo.coordinates.x + 1;
            }
          }
          else
          {
            cursorPos.x++;
          }
          if (cursorPos.x > game.COLUMN_WIDTH - 1)
          {
            if (selectedColumn != game.COLUMN_AMOUNT - 1)
            {
              cursorPos.x = 0;
            }
            else
            {
              cursorPos.x = game.COLUMN_WIDTH - 1;
            }
            selectedColumn = Math.Min(game.COLUMN_AMOUNT - 1, selectedColumn + 1);
          }
          break;
        }
      case ConsoleKey.UpArrow:
        {
          cursorPos.y = Math.Max(0, cursorPos.y - 1);
          break;
        }
      case ConsoleKey.DownArrow:
        {
          cursorPos.y = Math.Min(game.COLUMN_HEIGHT - 1, cursorPos.y + 1);
          break;
        }
      case ConsoleKey.Enter:
        {
          element = game.columns[selectedColumn].GetElement(cursorPos);
          string log = ExecuteCodeAndReturnLogs(GetExecutionCodeForElement(element), element, game.columns[selectedColumn]);
          game.AddLog(log);
          break;
        }
    }
    game.SetSelectedElement(game.columns[selectedColumn].GetElement(cursorPos).ToString());
  }
  public string ExecuteCodeAndReturnLogs(ExecutionCode code, Element element, Column column)
  {
    string log = "";
    switch (code)
    {
      case ExecutionCode.HintLife:
        {
          column.RemoveHint(element.coordinates);
          game.ResetAttempt();
          log = $"Tries Reset.";
          break;
        }
      case ExecutionCode.HintDuds:
        {
          column.RemoveHint(element.coordinates);
          column.RemoveDud(rightWord);
          log = $">{element}\n>Dud Removed.";
          break;
        }
      case ExecutionCode.Mistake:
        {
          game.LooseAttempt();
          try
          {
            log = $">{element}\n>Entry denied.\n>Likeness={CheckForLikeness(element.ToString())}";
          }
          catch (Exception exception)
          {
            string exceptionStr = exception.Message;
            exceptionStr = exception.Message;
          }
          if (game.attempts.attemptsLeft == 0)
          {
            log += $">\n>Lockout in\n>progress";
          }
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
      throw new Exception($"Stupid chat gpt exception, right word = {rightWord}, word = {word}");
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