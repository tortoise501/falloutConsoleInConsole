using System;
using System.Linq;
using System.Reflection.PortableExecutable;
//test
class Game
{

  public GameState gameState { get; private set; } = GameState.InProgress;

  Random rnd = new Random();
  readonly int COLUMN_WIDTH;
  readonly int COLUMN_HEIGHT;

  readonly int WORD_AMOUNT;
  readonly int WORD_LENGTH;

  readonly int ADDRESS_WIDTH;
  readonly int ADDRESS_HEIGHT;

  readonly int LOGGER_HEIGHT;


  bool isStarted = false;
  readonly int COLUMN_AMOUNT;//!for tests



  int xCursorPosition = 0;
  int yCursorPosition = 0;
  int selectedColumn = 0;
  int selectedPos { get => xCursorPosition + yCursorPosition * COLUMN_WIDTH; }

  readonly int MAX_ATTEMPTS;
  Attempts attempts;// = new Attempts(MAX_ATTEMPTS);

  GameLogger gameLogger;// = new GameLogger(COLUMN_HEIGHT);

  ConsoleGameRenderer renderer = new ConsoleGameRenderer();
  Column[] columns;// = new Column[COLUMN_AMOUNT];
  List<string> words = new List<string>();
  List<string>[] wordsByColumns;// = new List<string>[COLUMN_AMOUNT];


  public Game(Settings settings, bool update = false, bool start = true)
  {
    COLUMN_WIDTH = settings.ColumnWidth;
    COLUMN_HEIGHT = settings.ColumnHeight;
    WORD_AMOUNT = settings.WordAmount;
    WORD_LENGTH = settings.WordLength;
    ADDRESS_WIDTH = settings.AddressWidth;
    ADDRESS_HEIGHT = settings.AddressHeight;
    MAX_ATTEMPTS = settings.MaxAttempts;
    COLUMN_AMOUNT = settings.ColumnAmount;
    LOGGER_HEIGHT = settings.LoggerHeight;

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
    // FileSystem testFileSystem = new FileSystem();
    // testFileSystem.WriteTextToTestFile(new string[] { "first line", "second line", "third line" });


    isStarted = true;
    Console.Clear();
    Console.CursorVisible = false;


    words = GenerateRandomWords(WORD_AMOUNT, WORD_LENGTH).ToList();
    Column.SetRightWord(words[rnd.Next(0, words.Count())]);
    wordsByColumns = new List<string>[COLUMN_AMOUNT];
    for (int i = 0; i < wordsByColumns.Length; i++)
    {
      wordsByColumns[i] = words.Skip(i * 8).Take(8).ToList();
    }

    columns = new Column[COLUMN_AMOUNT];
    List<IRenderable> addToRender = new List<IRenderable>();
    int y = 3;
    int spaceBetweenElements = 1;//TODO make it a setting
    int x = 0;
    attempts = new Attempts(MAX_ATTEMPTS, 0, 1);
    addToRender.Add(attempts);
    for (int i = 0; i < COLUMN_AMOUNT; i++)
    {
      addToRender.Add(new Addresses(ADDRESS_HEIGHT, ADDRESS_WIDTH, x, y));
      x += ADDRESS_WIDTH + spaceBetweenElements;
      columns[i] = new Column(COLUMN_WIDTH, COLUMN_HEIGHT, WORD_LENGTH, WORD_AMOUNT / COLUMN_AMOUNT, wordsByColumns[0].ToArray(), y, x);
      addToRender.Add(columns[i]);
      x += COLUMN_WIDTH + spaceBetweenElements;
    }
    gameLogger = new GameLogger(LOGGER_HEIGHT, x, y);
    addToRender.Add(gameLogger);


    foreach (IRenderable elementToRender in addToRender)
    {
      renderer.AddObjectToRender(elementToRender);
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
            selectedColumn = Math.Min(COLUMN_AMOUNT - 1, selectedColumn + 1);
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
    gameLogger.SetSelectedElement(columns[selectedColumn].GetElement(selectedPos));
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
      res.Add(Constants.wordsPull.words[length - 4][rnd.Next(0, Constants.wordsPull.words[length - 4].Length - 1)]);
      if (SafetyCounter > 100)
      {
        break;
      }
      SafetyCounter++;
    }
    return res.Select(x => x.ToUpper()).ToArray();
  }
}