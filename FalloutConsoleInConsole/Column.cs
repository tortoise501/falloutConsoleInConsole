using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Dynamic;
//!Problems:
//!A lot of possible infinite loops during column generation
//!Reserve hint generation doesn't work sometimes
public class Column : IRenderable
{
  static Random rnd = new Random();

  /// <summary>
  /// Array that store all elements   
  /// </summary>
  string[] columnByElements = new string[0];
  readonly int columnWidth;
  readonly int columnHeight;



  public string[] words { get; private set; } = new string[0];//!is public temporarily for dud validation in Game class 
  private static string? rightWord;
  static public void SetRightWord(string rightWord)
  {
    if (rightWord != null)
    {
      Column.rightWord = rightWord;
    }
  }

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
  Dictionary<int, int> indexToHintPos;
  /// <summary>
  /// Key = Index,
  /// Value = Type of hint
  /// </summary>
  Dictionary<int, HintType> indexToHintType = new Dictionary<int, HintType>();

  /// <summary>
  /// Key = index
  /// Value = Word on that place
  /// </summary>
  Dictionary<int, string> indexToWord = new Dictionary<int, string>();

  //!probably deprecated
  int selectorPos = 0;  //selected position(as character index)
  bool isColumnSelected = false;

  //*----------------------------------------------------------------------LOG
  // int logLength = 7;
  // Queue<string> gameLogs = new Queue<string>();



  public Column(int width, int height, int wordLength, int wordAmount, string[] words, int y = 0, int x = 0, int hintAmount = 4, int resetAttemptsHintAmount = 0)
  {
    ((IRenderable)this).x = x;
    ((IRenderable)this).y = y;

    columnWidth = width;
    columnHeight = height;
    this.words = words; //GenerateRandomWords(this.wordAmount, this.wordLength);
    // rightWord = words[rnd.Next(0, wordAmount - 1)];//!test
    columnByElements = GenerateColumn(words, columnWidth, columnHeight, wordLength, wordAmount);
    posToElement = MapPosToElements(columnByElements);
    indexToHintPos = GenerateHintData(columnWidth, hintAmount, resetAttemptsHintAmount);
    PlaceRandomSymbols();
    //TODO: there is no sense in all those functions to return value, they can just modify global variables 
    // string[] testToDelete = NewGenerateColumn(words, COLUMN_WIDTH, COLUMN_HEIGHT, WORD_LENGTH, WORD_AMOUNT);
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


  public ExecutionCode CheckInput()
  {
    int selectedIndex = posToElement[selectorPos];
    string selectedItem = columnByElements[selectedIndex];
    if (selectedItem == rightWord)//is this right word
    {
      return ExecutionCode.CorrectWord;
    }
    if (indexToHintPos.ContainsKey(selectedIndex))//is this a hint
    {
      if (indexToHintType[selectedIndex] == HintType.Dud)
      {
        return ExecutionCode.HintDuds;
      }
      else
      {
        return ExecutionCode.HintLife;
      }
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


  public string GenerateLog(ExecutionCode executionCode, int selectedPos)
  {
    string el = columnByElements[posToElement[selectedPos]];
    string res = "";
    if (executionCode == ExecutionCode.Mistake)
    {
      if (rightWord == null)
        throw new NullReferenceException();
      res = $">{el}\n>Entry Denied\n>Likeness={CheckForLikeness(el, rightWord)}";
    }
    if (executionCode == ExecutionCode.CorrectWord)
    {
      res = $">{el}\n>Exact match\n>Please Wait\n>while system\n>is accepted";
    }
    if (executionCode == ExecutionCode.HintDuds)
    {
      char[] hint = GetCharsOf(posToElement[selectedPos], indexToHintPos[posToElement[selectedPos]]).ToArray();
      res = $">{string.Join("", hint)}\n>Dud removed";
    }
    if (executionCode == ExecutionCode.HintLife)
    {
      res = $"ATTEMPTS RESTORED";
    }
    if (executionCode == ExecutionCode.HintLife || executionCode == ExecutionCode.HintDuds)
    {
      indexToHintPos.Remove(posToElement[selectedPos]);
      indexToHintType.Remove(posToElement[selectedPos]);
    }
    return res;
  }
  public void RemoveDud(int selectedPos)//!public temporarily for Game class dud removal
  {
    if (indexToWord.Count == 0)
    {
      return;
    }
    int randomWordIndex = rnd.Next(0, indexToWord.Count);
    List<int> poses = Enumerable.ToList(indexToWord.Keys);
    int posToReplace = poses[randomWordIndex];
    string replacement = "";
    for (int i = 0; i < columnByElements[posToReplace].Length; i++)
    {
      replacement += ".";
    }
    columnByElements[posToReplace] = replacement;
    indexToWord.Remove(poses[randomWordIndex]);
  }

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
          indexToWord.Add(i, words[wordI]);
        }
        wordI++;
      }
      else
      {
        column[i] = "U";
      }
    }
    return column;
  }



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
  private Dictionary<int, int> GenerateHintData(int rowWidth, int hintAmount, int resetAttemptsHintAmount)
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
    HashSet<int> resetAttemptHintsOrder = new HashSet<int>();//witch hints reset attempt 
    for (int i = 0; i < resetAttemptsHintAmount; i++)//!Possible infinite loops
    {
      int index = rnd.Next(0, hintAmount);
      if (!resetAttemptHintsOrder.Contains(index))
      {
        resetAttemptHintsOrder.Add(index);
        continue;
      }
      i--;
    }
    int hintNumber = 0;

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
          indexToHintType.Add(i, resetAttemptHintsOrder.Contains(hintNumber) ? HintType.Attempt : HintType.Dud);
          hintNumber++;
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

  public string GetElement(int pos)
  {
    return columnByElements[posToElement[pos]];
  }

  public void SelectElement(int posOfCursor)
  {
    isColumnSelected = true;
    selectorPos = posOfCursor;
  }





  //IRenderable interface implementation
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
      if (indexToHintPos.ContainsKey(i) && posToElement[selectorPos] == i)
      {
        jump = 0;
        for (int j = 0; j <= indexToHintPos[i]; j++)
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
        if (x + y * columnWidth == selectorPos && isColumnSelected)
        {
          res.Last().Add(new RenderData(c, CharacterState.selectedAsChar));
        }
        else if (i == posToElement[selectorPos] && isColumnSelected && c != '.')
        {
          res.Last().Add(new RenderData(c, CharacterState.selectedAsElement));
        }
        else
        {
          res.Last().Add(new RenderData(c, CharacterState.notSelected));
        }
        x++;
        if (x % columnWidth == 0)
        {
          y++;
          x = 0;
          res.Add(new List<RenderData>());
        }
      }
    }
    isColumnSelected = false;//expire "selection"
    return res;
  }

  int IRenderable.x { get; set; }
  int IRenderable.y { get; set; }
}