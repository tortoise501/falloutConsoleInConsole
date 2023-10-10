using System;
using System.Linq;
using System.Reflection.PortableExecutable;
//test
public class Game
{

  public GameState gameState { get; private set; } = GameState.InProgress;

  Random rnd = new Random();
  public readonly int COLUMN_WIDTH;
  public readonly int COLUMN_HEIGHT;
  public readonly int COLUMN_AMOUNT;

  readonly int WORD_AMOUNT;
  readonly int WORD_LENGTH;

  readonly int ADDRESS_WIDTH;
  readonly int ADDRESS_HEIGHT;

  readonly int LOGGER_HEIGHT;

  readonly int HINT_AMOUNT;
  const int RESET_ATTEMPTS_HINT_AMOUNT = 1;//!DO not put in settings


  bool isStarted = false;



  int xCursorPosition = 0;
  int yCursorPosition = 0;

  readonly int MAX_ATTEMPTS;
  public Attempts attempts { get; private set; }// = new Attempts(MAX_ATTEMPTS);

  GameLogger gameLogger;// = new GameLogger(COLUMN_HEIGHT);

  ConsoleGameRenderer renderer = new ConsoleGameRenderer();
  public Column[] columns { get; private set; }// = new Column[COLUMN_AMOUNT];
  List<string> words = new List<string>();
  List<string>[] wordsByColumns;// = new List<string>[COLUMN_AMOUNT];


  InputHandler inputHandler;


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
    HINT_AMOUNT = 8;//!add to settings

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


    words = GenerateRandomWords(WORD_AMOUNT, WORD_LENGTH).ToList();
    inputHandler = new InputHandler(words[rnd.Next(0, words.Count())], this);
    wordsByColumns = new List<string>[COLUMN_AMOUNT];
    for (int i = 0; i < wordsByColumns.Length; i++)
    {
      wordsByColumns[i] = words.Skip(i * 8).Take(8).ToList();
    }

    int columnWithResetHint = rnd.Next(0, COLUMN_AMOUNT);

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
      //!change hint amount
      columns[i] = new Column(COLUMN_WIDTH, COLUMN_HEIGHT, WORD_LENGTH, WORD_AMOUNT / COLUMN_AMOUNT, wordsByColumns[i].ToArray(), y, x, HINT_AMOUNT / COLUMN_AMOUNT, i == columnWithResetHint ? RESET_ATTEMPTS_HINT_AMOUNT : 0);
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
      columns[InputHandler.selectedColumn].PlaceCursorForFrame(InputHandler.selectedPos);
      renderer.UpdateColumnRenderData();
      renderer.FastRender();
      if (gameState != GameState.InProgress)
      {
        break;
      }
      HandleInput();
    }
    return true;
  }
  void HandleInput()
  {
    inputHandler.HandleInput();
  }

  public void LooseAttempt()
  {
    bool isLost = attempts.LooseAttemptAndCheckForLoose();
    if (isLost)
    {
      LooseGame();
    }
  }
  public void WinGame()
  {
    gameState = GameState.Won;
  }
  private void LooseGame()
  {
    gameState = GameState.Lost;
  }
  public void ResetAttempt()
  {
    attempts.ResetAttempts();
  }
  public void AddLog(string log)//needed as interface for inputHandler
  {
    if (log == "")
    {
      return;
    }
    gameLogger.AddGameLogs(log);
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