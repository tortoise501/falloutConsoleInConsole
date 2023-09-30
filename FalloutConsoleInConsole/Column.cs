using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
//TODO ---------------------------------------------------------------------------------------------------------------------------------------------------------------------
// TODOs:
// -Life Hint
//?-Better hint creation(Probably finished)
// -Make Logging as a class, for better rendering
// -New rendering to render two columns side to side
//   -save render data as an array of rows to render with special symbols for color changing
//   -implement render in other class or in Program.cs 
//
//
//!Problems:
//!A lot of possible infinite loops during column generation
//!Reserve hint generation doesn't work ideally
public class Column : IRenderable
{
  static Random rnd = new Random();

  /// <summary>
  /// Array that store all elements
  /// Elements are: symbols, words
  /// </summary>
  string[] columnByElements = new string[0];
  readonly int COLUMN_WIDTH;
  readonly int COLUMN_HEIGHT;



  string[] words = new string[0];
  readonly string rightWord;
  readonly int WORD_LENGTH;
  readonly int WORD_AMOUNT;


  public GameState gameState { get; private set; } = GameState.InProgress;

  //*----------------------------------------------------------------------Attempts
  //TODO: integrate this parameters to constructor
  int maxAttempts = 4;
  int attemptsLeft = 4;

  //*----------------------------------------------------------------------Data dictionaries
  /// <summary>
  /// Key = position(as in char array)
  /// Value = index(as in element array)
  /// </summary>
  Dictionary<int, int> posToElement;

  /// <summary>
  /// Key = Index,
  /// Value = length of hint
  /// </summary>
  Dictionary<int, int> hintPosData;
  /// <summary>
  /// Key = Index,
  /// Value = Type of hint
  /// </summary>
  Dictionary<int, HintType> hintTypeData = new Dictionary<int, HintType>();

  /// <summary>
  /// Key = index
  /// Value = Word on that place
  /// </summary>
  Dictionary<int, string> posToWord = new Dictionary<int, string>();


  int selectorPos = 0;  //selected position(as character index)

  //*----------------------------------------------------------------------LOG
  int logLength = 7;
  Queue<string> gameLogs = new Queue<string>();



  public Column(int width, int height, int wordLength, int wordAmount)
  {
    COLUMN_WIDTH = width;
    COLUMN_HEIGHT = height;
    WORD_LENGTH = wordLength;
    WORD_AMOUNT = wordAmount;
    words = GenerateRandomWords(WORD_AMOUNT, WORD_LENGTH);
    rightWord = words[rnd.Next(0, wordAmount)];
    columnByElements = GenerateColumn(words, COLUMN_WIDTH, COLUMN_HEIGHT, WORD_LENGTH, WORD_AMOUNT);
    posToElement = MapPosToElements(columnByElements);
    hintPosData = GenerateHintData(COLUMN_WIDTH);
    PlaceRandomSymbols();
    //TODO: there is no sense in all those functions to return value, they can just modify global variables 
    // string[] testToDelete = NewGenerateColumn(words, COLUMN_WIDTH, COLUMN_HEIGHT, WORD_LENGTH, WORD_AMOUNT);
  }



  public void LoseHitPoint()
  {
    attemptsLeft--;
    if (attemptsLeft == 0)
    {
      LooseTheGame();
    }
  }

  private void LooseTheGame()
  {
    gameState = GameState.Lost;
  }

  public void RenderGameLogs()
  {
    foreach (string LogOutputLine in gameLogs)
    {
      Console.Write(LogOutputLine);
    }
    Console.WriteLine();
  }
  public void AddToGameLogs(string LogOutputLine)
  {
    gameLogs.Enqueue(LogOutputLine);
    if (gameLogs.Count() > logLength)
    {
      gameLogs.Dequeue();
    }
  }

  public void RenderHitPoints()
  {
    Console.Write("[ ");
    for (int i = 0; i < attemptsLeft; i++)
    {
      Console.Write("\u2580 ");
    }
    for (int i = 0; i < maxAttempts - attemptsLeft; i++)
    {
      Console.Write("  ");
    }
    Console.Write("]\n");
  }

  public void Render(bool clear = true)
  {
    if (clear)
    {
      Console.Clear();
    }
    int x = 0;
    int y = 0;
    int jump;
    for (int i = 0; i < columnByElements.Length; i += jump)
    {
      jump = 1;//TODO:use private function GetCharsOf();
      List<char> charsToRender = new List<char>();
      if (hintPosData.ContainsKey(i) && posToElement[selectorPos] == i)
      {
        jump = 0;
        for (int j = 0; j <= hintPosData[i]; j++)
        {
          List<char> charr = columnByElements[i + j].ToCharArray().ToList();
          charsToRender = charsToRender.Concat(charr).ToList();
          jump++;
        }
      }
      else
      {
        charsToRender = columnByElements[i].ToCharArray().ToList();
      }
      foreach (char c in charsToRender)
      {
        if (x + y * COLUMN_WIDTH == selectorPos)
        {
          Console.ForegroundColor = ConsoleColor.Black;
          Console.BackgroundColor = ConsoleColor.DarkGreen;
        }
        else if (i == posToElement[selectorPos])
        {
          Console.ForegroundColor = ConsoleColor.Black;
          Console.BackgroundColor = ConsoleColor.Green;
        }
        else
        {
          Console.ForegroundColor = ConsoleColor.Green;
          Console.BackgroundColor = ConsoleColor.Black;
        }
        Console.Write(c);
        x++;
        if (x % COLUMN_WIDTH == 0)
        {
          y++;
          x = 0;
          Console.WriteLine("");
        }
      }
    }
  }

  /// <summary>
  /// Use for hints
  /// </summary>
  /// <param name="startingIndex">index of an element in columnByElements</param>
  /// <returns></returns>
  public List<char> GetCharsOf(int startingIndex, int length)
  {
    List<char> charsToRender = new List<char>();
    for (int j = 0; j <= length; j++)
    {
      List<char> charr = columnByElements[startingIndex + j].ToCharArray().ToList();
      charsToRender = charsToRender.Concat(charr).ToList();
    }
    return charsToRender;
  }

  public void HandleInput()
  {
    ConsoleKey key = Console.ReadKey().Key;


    switch (key)
    {
      case ConsoleKey.RightArrow:
        selectorPos += 1;
        break;
      case ConsoleKey.LeftArrow:
        selectorPos -= 1;
        break;
      case ConsoleKey.UpArrow:
        selectorPos -= COLUMN_WIDTH;
        break;
      case ConsoleKey.DownArrow:
        selectorPos += COLUMN_WIDTH;
        break;
      case ConsoleKey.Enter:
        int selectedIndex = posToElement[selectorPos];
        string selectedItem = columnByElements[selectedIndex];
        ExecuteInput(CheckInput(selectedIndex, selectedItem), selectedIndex, selectedItem);
        break;
    }
    selectorPos = Math.Clamp(selectorPos, 0, posToElement.Count - 1);
  }

  private ExecutionCode CheckInput(int selectedIndex, string selectedItem)
  {
    if (selectedItem == rightWord)//is this right word
    {
      return ExecutionCode.CorrectWord;
    }
    if (hintPosData.ContainsKey(selectedIndex))//is this a hint
    {
      return ExecutionCode.HintDuds;
      //TODO: Add HintLife execution with certain chance
    }
    if (selectedItem.Length > 1)//is it a wrong word
    {
      return ExecutionCode.Mistake;
    }
    else//then it is a symbol
    {
      return ExecutionCode.WrongInput;
    }
  }

  private void ExecuteInput(ExecutionCode executionCode, int selectedIndex, string selectedItem)
  {
    if (executionCode == ExecutionCode.WrongInput)
    {
      return;
    }
    if (executionCode == ExecutionCode.Mistake)
    {
      AddToGameLogs($">{columnByElements[selectedIndex]}\n>Entry Denied\n>Likeness={CheckForLikeness(columnByElements[selectedIndex], rightWord)}\n");
      LoseHitPoint();
    }
    if (executionCode == ExecutionCode.CorrectWord)
    {
      AddToGameLogs($">{columnByElements[selectedIndex]}\n>Exact match\n>Please Wait\n>while system\n>is accepted");
      gameState = GameState.Won;
    }
    if (executionCode == ExecutionCode.HintDuds)
    {
      char[] hint = GetCharsOf(selectedIndex, hintPosData[selectedIndex]).ToArray();
      AddToGameLogs($">{string.Join("", hint)}\n>Dud removed\n");
      RemoveDud();
    }
    if (executionCode == ExecutionCode.HintLife)
    {
      AddToGameLogs($"ATTEMPTS RESTORED\n");
      attemptsLeft = maxAttempts;
    }
    if (executionCode == ExecutionCode.HintLife || executionCode == ExecutionCode.HintDuds)
    {
      hintPosData.Remove(selectedIndex);
    }

  }
  private void RemoveDud()
  {
    if (posToWord.Count == 0)
    {
      return;
    }
    int randomWordIndex = rnd.Next(0, posToWord.Count);
    List<int> poses = Enumerable.ToList(posToWord.Keys);
    int posToReplace = poses[randomWordIndex];
    string replacement = "";
    for (int i = 0; i < columnByElements[posToReplace].Length; i++)
    {
      replacement += ".";
    }
    columnByElements[posToReplace] = replacement;
    posToWord.Remove(poses[randomWordIndex]);
  }








  //TODO: create system to limit amount of hints for ex. create special symbols 'S' that will be replaced to right parentheses to create hint 
  // private string[] NewGenerateColumn(string[] words, int width = 12, int height = 16, int wordLength = 4, int wordAmount = 6, int HintAmount = 7)
  // {
  //   int AmountOfElements = (height * width) - (wordAmount * wordLength) + wordAmount;
  //   string[] column = new string[AmountOfElements];
  //   int wordI = 0;
  //   ElementClass[] columnByClasses = new ElementClass[(wordAmount + HintAmount) * 2 + 1];//words + elements + spaces after them + 1 before them
  //   columnByClasses = columnByClasses.Select(x => ElementClass.Random).ToArray();//Convert everything to random
  //   //Generate columnByClasses list
  //   int generatedWords = 0;
  //   int generatedHints = 0;
  //   for (int i = 1; i < columnByClasses.Length - 1; i += 2)
  //   {
  //     int rand = rnd.Next(0, wordAmount + HintAmount);
  //     if (rand < wordAmount && generatedWords != wordAmount)
  //     {
  //       columnByClasses[i] = ElementClass.Word;
  //     }
  //     else if (generatedHints != HintAmount)
  //     {
  //       columnByClasses[i] = ElementClass.Hint;
  //     }
  //     else if (generatedWords != wordAmount)
  //     {
  //       columnByClasses[i] = ElementClass.Word;
  //     }
  //   }
  //   //Process columnByClasses list
  //   int recommendedHintSize = 4;
  //   int recommendedRandomSize = (AmountOfElements - wordAmount - HintAmount) / ((columnByClasses.Length / 2) - 1);//super formula
  //   int maxDeviation = recommendedRandomSize / 3;
  //   int randomLeft = AmountOfElements - wordAmount - HintAmount;
  //   int ElementIndex = 0;
  //   foreach (ElementClass ElClass in columnByClasses)
  //   {
  //     switch (ElClass)
  //     {
  //       case ElementClass.Word:
  //         {
  //           ElementIndex += rnd.Next(recommendedRandomSize - maxDeviation, recommendedRandomSize + maxDeviation);
  //           column[ElementIndex] = words[wordI];
  //           wordI++;
  //           break;
  //         }
  //       case ElementClass.Hint:
  //         {
  //           //TODO                                  custom chance
  //           //             || 4 means 1 in 4 chance of being double hint 
  //           //             \/
  //           if (rnd.Next(0, 4) == 0)
  //           {

  //           }
  //           else
  //           {
  //             int length = rnd.Next(2, recommendedHintSize);
  //             char parenthesis = '}';//TODO random parenthesis type
  //           }
  //           break;
  //         }
  //     }
  //     //Generate Randoms
  //   }
  //   throw new NotImplementedException();
  // }
  private string[] GenerateColumn(string[] words, int width = 12, int height = 16, int wordLength = 4, int wordAmount = 6)
  {
    int length = (height * width) - (wordAmount * wordLength) + wordAmount;
    string[] column = new string[length];
    int wordI = 0;
    HashSet<int> randomWordPos = new HashSet<int>();
    for (int i = 0; i < words.Length; i++)
    {
      int pos = rnd.Next(length / wordAmount + length / wordAmount * i);
      if (randomWordPos.Contains(pos))
      {
        i--;
        continue;
      }
      randomWordPos.Add(pos);
    }


    for (int i = 0; i < length; i++)
    {
      if (randomWordPos.Contains(i))
      {
        column[i] = words[wordI];
        if (words[wordI] != rightWord)
        {
          posToWord.Add(i, words[wordI]);
        }
        wordI++;
      }
      else
      {
        column[i] = "U";// U for undefined// = Constants.symbols[rnd.Next(Constants.symbols.Length)].ToString();
      }
    }
    return column;
  }

  // private string[] GenerateHint(int length, char parenthesis = 'R')//R means random
  // {
  //   string[] hint = new string[length];
  //   if (parenthesis == 'R')
  //   {
  //     switch (rnd.Next(0, 4))
  //     {
  //       case 0:
  //         parenthesis = '}';
  //         break;
  //       case 1:
  //         parenthesis = ']';
  //         break;
  //       case 2:
  //         parenthesis = ')';
  //         break;
  //       case 3:
  //         parenthesis = '>';
  //         break;
  //     }
  //     for (int i = 0; i < length - 1; i++)
  //     {

  //     }
  //   }
  //   throw new NotImplementedException();
  // }





  private Dictionary<int, int> MapPosToElements(string[] elements)
  {
    Dictionary<int, int> res = new Dictionary<int, int>();
    int i = 0;
    for (int a = 0; a < elements.Length; a++)
    {
      string el = elements[a];
      foreach (char c in el)
      {
        res.Add(i, a);
        i++;
      }
    }
    return res;
  }







  /// <summary>
  ///   returns dictionary where key is start of a hint represented as an index in an element array and value is a length of that hint 
  /// </summary>
  /// <param name="elements">array of elements</param>
  /// <param name="rowWidth">width of each column row</param>
  /// <returns></returns>
  private Dictionary<int, int> GenerateHintData(int rowWidth, int hintAmount = 4)
  {
    Dictionary<int, int> res = new Dictionary<int, int>();
    int count = columnByElements.Count(x => x.Length > 1);

    Stack<char> searchedChar = new Stack<char>();
    Stack<int> searchedCharIndex = new Stack<int>();
    HashSet<int> randomHintPos = new HashSet<int>();
    for (int i = 0; i < hintAmount; i++)//!Possible infinite loops
    {
      int pos = rnd.Next(columnByElements.Length / hintAmount + columnByElements.Length / hintAmount * i);
      if (columnByElements[pos].Length <= 1)
      {
        randomHintPos.Add(pos);//!Can generate hint on row borders
        continue;
      }
      i--;
    }


    foreach (int pos in randomHintPos)
    {
      columnByElements[pos] = "S";//Start of hint
    }
    int spawnedHints = 0;
    int widthSearched = 0;//how many chars were searched
    for (int i = 0; i < columnByElements.Length && spawnedHints < hintAmount; i++)
    {
      string el = columnByElements[i];
      widthSearched += el.Length;
      if (el == "S" && 0 != (widthSearched % rowWidth))//is a start of a hint
      {
        List<int> PossibleClosingPos = new List<int>();//? probably fixedTODO: Fix row restrictions
        int checkedWidth = 0;//width checked in chars
        for (int j = 1; rowWidth > (widthSearched % rowWidth) + checkedWidth; j++)
        {
          if (columnByElements[i + j] != "S" && columnByElements[i + j].Length <= 1)
          {
            PossibleClosingPos.Add(i + j);
            if (Constants.Parentheses.Count(x => x.ToString() == columnByElements[i + j]) > 0)
            {
              break;
            }
          }
          checkedWidth += columnByElements[i + j].Length;
        }
        if (PossibleClosingPos.Count == 0)//If no possible hints, create new hint start later in list
        {
          int NewPos = rnd.Next(i + 1, columnByElements.Length);
          columnByElements[i] = "U";
          columnByElements[NewPos] = "S";
        }
        else
        {
          int endingPos = PossibleClosingPos[rnd.Next(0, PossibleClosingPos.Count)];
          if (columnByElements[endingPos] == "U")
          {
            char randomPar = Constants.Parentheses[rnd.Next(0, Constants.Parentheses.Length)];
            columnByElements[endingPos] = randomPar.ToString();
            columnByElements[i] = Constants.GetOppositeParentheses[randomPar].ToString();
          }
          else
          {
            columnByElements[i] = Constants.GetOppositeParentheses[columnByElements[endingPos][0]].ToString();
          }
          spawnedHints += 1;
          res.Add(i, endingPos - i);
        }
      }
    }
    return res;
  }
  private void PlaceRandomSymbols()
  {
    for (int i = 0; i < columnByElements.Length; i++)
    {
      if (columnByElements[i] == "U" || columnByElements[i] == "S")
      {
        columnByElements[i] = Constants.symbols[rnd.Next(0, Constants.symbols.Length)].ToString();
      }
    }
  }
  private string[] GenerateRandomWords(int amount = 6, int length = 6)
  {
    string[] res = new string[amount];
    HashSet<int> usedIndex = new HashSet<int>();
    for (int i = 0; i < amount; i++)
    {
      int index = rnd.Next(Constants.WordsPull[length - 4].Length);
      if (usedIndex.Contains(index))
      {
        i--;
        continue;
      }
      res[i] = Constants.WordsPull[length - 4][index];
    }
    return res;
  }

  private int CheckForLikeness(string input, string compareTo)
  {
    if (input.Length != compareTo.Length)
    {
      throw new Exception("Stupid chat gpt exception");
    }
    int res = 0;
    for (int i = 0; i < input.Length; i++)
    {
      if (input[i] == compareTo[i])
      {
        res++;
      }
    }
    return res;
  }

  List<List<RenderData>> IRenderable.GetRenderData()
  {
    List<List<RenderData>> res = new List<List<RenderData>>();
    int x = 0;
    int y = 0;
    int jump;
    res.Add(new List<RenderData>());
    for (int i = 0; i < columnByElements.Length; i += jump)
    {
      jump = 1;//TODO:use private function GetCharsOf();
      List<char> charsToRender = new List<char>();
      if (hintPosData.ContainsKey(i) && posToElement[selectorPos] == i)
      {
        jump = 0;
        for (int j = 0; j <= hintPosData[i]; j++)
        {
          List<char> charr = columnByElements[i + j].ToCharArray().ToList();
          charsToRender = charsToRender.Concat(charr).ToList();
          jump++;
        }
      }
      else
      {
        charsToRender = columnByElements[i].ToCharArray().ToList();
      }
      foreach (char c in charsToRender)
      {
        if (x + y * COLUMN_WIDTH == selectorPos)
        {
          res.Last().Add(new RenderData(c, ConsoleColor.DarkGreen, ConsoleColor.Black));
        }
        else if (i == posToElement[selectorPos])
        {
          res.Last().Add(new RenderData(c, ConsoleColor.Green, ConsoleColor.Black));
        }
        else
        {
          res.Last().Add(new RenderData(c, ConsoleColor.Black, ConsoleColor.Green));
        }
        x++;
        if (x % COLUMN_WIDTH == 0)
        {
          y++;
          x = 0;
          res.Add(new List<RenderData>());
        }
      }
    }
    return res;
  }

  public int GetPositionX()
  {
    throw new NotImplementedException();
  }

  public int GetPositionY()
  {
    throw new NotImplementedException();
  }
}