using System;
using System.Linq;
using System.Reflection.PortableExecutable;
//test
class Game
{
  public GameState gameState { get; private set; } = GameState.InProgress;

  Random rnd = new Random();
  const int COLUMN_WIDTH = 12;
  const int COLUMN_HEIGHT = 16;

  const int WORD_AMOUNT = 16;
  const int WORD_LENGTH = 6;

  const int ADDRESS_WIDTH = 6;
  const int ADDRESS_HEIGHT = COLUMN_HEIGHT;


  bool isStarted = false;
  const int columnAmount = 2;//!for tests



  int xCursorPosition = 0;
  int yCursorPosition = 0;
  int selectedColumn = 0;
  int selectedPos { get => xCursorPosition + yCursorPosition * COLUMN_WIDTH; }

  const int MAX_ATTEMPTS = 4;//!test
  // int attemptsLeft = 4;//!test
  Attempts attempts = new Attempts(MAX_ATTEMPTS);

  GameLogger gameLogger = new GameLogger(COLUMN_HEIGHT);

  ConsoleGameRenderer renderer = new ConsoleGameRenderer();
  Column[] columns = new Column[columnAmount];
  List<string> words = new List<string>();
  List<string>[] wordsByColumns = new List<string>[columnAmount];


  public Game(bool update = false, bool start = true)
  {
    if (start)
    {
      Start();
    }
    if (update)
    {
      Update();
    }
  }
  public void Start()
  {
    isStarted = true;
    Console.Clear();
    Console.CursorVisible = false;

    attempts = new Attempts(MAX_ATTEMPTS, 0, 0);
    renderer.AddObjectToRender(attempts);

    gameLogger = new GameLogger(COLUMN_HEIGHT, COLUMN_WIDTH * columnAmount + 2 * columnAmount, 1);
    renderer.AddObjectToRender(gameLogger);

    renderer.AddObjectToRender(new Addresses(ADDRESS_HEIGHT, ADDRESS_WIDTH, 0, 1));
    renderer.AddObjectToRender(new Addresses(ADDRESS_HEIGHT, ADDRESS_WIDTH, COLUMN_WIDTH + ADDRESS_WIDTH + 2, 1));

    words = GenerateRandomWords(WORD_AMOUNT, WORD_LENGTH).ToList();
    Column.SetRightWord(words[rnd.Next(0, words.Count())]);
    for (int i = 0; i < wordsByColumns.Length; i++)
    {
      wordsByColumns[i] = words.Skip(i * 8).Take(8).ToList();
    }
    columns = new Column[]{//!Test
      new Column(COLUMN_WIDTH, COLUMN_HEIGHT, WORD_LENGTH, WORD_AMOUNT/columnAmount,wordsByColumns[0].ToArray(),1,ADDRESS_WIDTH + 1),
      new Column(COLUMN_WIDTH, COLUMN_HEIGHT, WORD_LENGTH, WORD_AMOUNT/columnAmount,wordsByColumns[1].ToArray(),1,COLUMN_WIDTH + ADDRESS_WIDTH + 3 + ADDRESS_WIDTH)
    };
    foreach (Column column in columns)
    {
      renderer.AddObjectToRender(column);
    }

  }
  public bool Update()
  {
    if (!isStarted)
    {
      return false;
    }
    while (true)
    {
      renderer.UpdateColumnRenderData();
      renderer.FastRender();
      if (gameState != GameState.InProgress)
      {
        break;
      }
      HandleInput();
      columns[selectedColumn].SelectElement(selectedPos);
    }
    return true;
  }
  void HandleInput()
  {
    ConsoleKey key = Console.ReadKey().Key;
    switch (key)
    {
      case ConsoleKey.LeftArrow:
        {
          xCursorPosition--;
          if (xCursorPosition < 0)
          {
            xCursorPosition = COLUMN_WIDTH - 1;
            selectedColumn = Math.Max(0, selectedColumn - 1);
          }
          break;
        }
      case ConsoleKey.RightArrow:
        {
          xCursorPosition++;
          if (xCursorPosition > COLUMN_WIDTH - 1)
          {
            xCursorPosition = 0;
            selectedColumn = Math.Min(columnAmount - 1, selectedColumn + 1);
          }
          break;
        }
      case ConsoleKey.UpArrow:
        {
          yCursorPosition = Math.Max(0, yCursorPosition - 1);
          break;
        }
      case ConsoleKey.DownArrow:
        {
          yCursorPosition = Math.Min(COLUMN_HEIGHT - 1, yCursorPosition + 1);
          break;
        }
      case ConsoleKey.Enter:
        {
          ExecuteInput(columns[selectedColumn].CheckInput(), columns[selectedColumn]);
          break;
        }
    }
  }
  public void ExecuteInput(ExecutionCode code, Column column)
  {
    string log = column.GenerateLog(code, selectedPos);
    if (log.Length > 1)
    {
      gameLogger.AddGameLogs(log);
    }
    switch (code)
    {
      case ExecutionCode.Mistake:
        {
          LooseAttempt();
          break;
        }
      case ExecutionCode.CorrectWord:
        {
          gameState = GameState.Won;
          // throw new Exception("YOU WON!!! sorry for exception");//TODO: Do it properly
          break;
        }
      case ExecutionCode.HintDuds:
        {
          int rndCol = rnd.Next(0, columns.Length);
          while (columns[rndCol].words.Count(x => x[0] != '.') < 0)
          {
            rndCol = rnd.Next(0, columns.Length);
          }
          columns[rndCol].RemoveDud(selectedPos);
          break;
        }
      case ExecutionCode.HintLife:
        {
          ResetAttempt();
          break;
        }
      case ExecutionCode.WrongInput:
        {
          return;
        }
    }
  }

  private void LooseAttempt()
  {
    bool isLost = attempts.LooseAttemptAndCheckForLoose();
    if (isLost)
    {
      LooseGame();
    }
  }
  private void LooseGame()
  {
    gameState = GameState.Lost;
    // throw new Exception("You lost :(");
  }
  private void ResetAttempt()
  {
    // attemptsLeft = MAX_ATTEMPTS;
    attempts.ResetAttempts();
  }

  private string[] GenerateRandomWords(int amount = 6, int length = 6)
  {
    HashSet<string> res = new HashSet<string>();
    int SafetyCounter = 0;
    while (res.Count() != amount)
    {
      res.Add(Constants.WordsPull[length - 4][rnd.Next(0, Constants.WordsPull[length - 4].Length - 1)]);
      if (SafetyCounter > 100)
      {
        break;
      }
      SafetyCounter++;
    }
    return res.ToArray();
  }
}