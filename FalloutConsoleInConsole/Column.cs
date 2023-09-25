public class Column
{
  static Random rnd = new Random();


  readonly int COLUMN_WIDTH;
  readonly int COLUMN_HEIGHT;
  readonly int WORD_LENGTH;
  readonly int WORD_AMOUNT;

  string[] columnByElements = new string[0];  //array of elements in column
  Dictionary<int, int> posToElement;  //relation between position in column(character index) and element this character belongs to
  Dictionary<int, int> hintWidth;  //hintPositionAndWidth
  // Dictionary<int, ExecutionCode> elementExecutionCode = new Dictionary<int, ExecutionCode>();
  int selectorPos = 0;  //selected position(as character index)



  public Column(int width, int height, int wordLength, int wordAmount)
  {
    COLUMN_WIDTH = width;
    COLUMN_HEIGHT = height;
    WORD_LENGTH = wordLength;
    WORD_AMOUNT = wordAmount;
    columnByElements = GenerateColumn(COLUMN_WIDTH, COLUMN_HEIGHT, WORD_LENGTH, WORD_AMOUNT);
    posToElement = MapPosToElements(columnByElements);
    hintWidth = GenerateHintData(columnByElements, COLUMN_WIDTH);
  }

  public bool init()
  {
    Console.CursorVisible = false;
    Console.ForegroundColor = ConsoleColor.Green;
    Console.BackgroundColor = ConsoleColor.Black;



    Console.WriteLine("press Enter to play or any other key to exit");
    ConsoleKey key = Console.ReadKey().Key;
    if (key != ConsoleKey.Enter)
    {
      return false;
    }
    return true;
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
      jump = 1;
      List<char> charsToRender = new List<char>();
      if (hintWidth.ContainsKey(i) && posToElement[selectorPos] == i)
      {
        jump = 0;
        for (int j = 0; j <= hintWidth[i]; j++)
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
    }
    selectorPos = Math.Clamp(selectorPos, 0, posToElement.Count - 1);
  }



  private string[] GenerateColumn(int width = 12, int height = 16, int wordLength = 4, int wordAmount = 6)
  {
    int length = (height * width) - (wordAmount * wordLength) + wordAmount;
    string[] row = new string[length];

    string[] words = GenerateRandomWords(wordAmount, wordLength);
    int wordi = 0;
    HashSet<int> randomWordPos = new HashSet<int>();
    for (int i = 0; i < words.Length; i++)
    {
      randomWordPos.Add(rnd.Next(length / wordAmount + length / wordAmount * i));
    }


    for (int i = 0; i < length; i++)
    {
      if (randomWordPos.Contains(i))
      {
        row[i] = words[wordi];
        wordi++;
      }
      else
      {
        row[i] = Constants.symbols[rnd.Next(Constants.symbols.Length)].ToString();
      }
    }
    return row;
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
  private Dictionary<int, int> GenerateHintData(string[] elements, int rowWidth)
  {
    Dictionary<int, int> res = new Dictionary<int, int>();

    Stack<char> searchedChar = new Stack<char>();
    Stack<int> searchedCharIndex = new Stack<int>();

    int widthSearched = 0;//how many chars were searched

    for (int a = 0; a < elements.Length; a++)
    {
      string el = elements[a];
      char ch = el[0];
      bool isOpen = Constants.GetOppositeParentheses.ContainsValue(ch);
      bool isClosed = Constants.GetOppositeParentheses.ContainsKey(ch);
      foreach (char c in el)
      {
        if (widthSearched % rowWidth == 0)
        {
          searchedChar = new Stack<char>();
          searchedCharIndex = new Stack<int>();
        }
        widthSearched++;
      }
      if (el.Length == 1 && Constants.IsParentheses(ch))
      {
        if (isOpen)//add open parentheses
        {
          searchedChar.Push(ch);
          searchedCharIndex.Push(a);
        }
        if (isClosed && searchedChar.Contains(Constants.GetOppositeParentheses[ch]))//if closed par.. is in search list
        {
          while (searchedChar.Count > 0)
          {

            char c = searchedChar.Pop();
            int i = searchedCharIndex.Pop();
            if (c == Constants.GetOppositeParentheses[ch])
            {
              res.Add(i, a - i);
            }
          }
        }
      }
    }
    return res;
  }
}