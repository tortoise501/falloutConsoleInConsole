using System;
using System.Linq;
using System.Reflection.PortableExecutable;

class Program
{
  static Random rnd = new Random();


  static public readonly char[] symbols = {
    '{',
    '}',
    '[',
    ']',
    '(',
    ')',
    '#',
    '%',
    ',',
    '.',
    '/',
    '\\',
    '*',
    '@'
  };



  const int COLUMN_WIDTH = 12;
  const int COLUMN_HEIGHT = 16;
  const int WORD_LENGTH = 6;
  const int WORD_AMOUNT = 6;
  static string[] columnByElements = new string[(COLUMN_HEIGHT * COLUMN_HEIGHT) - (WORD_AMOUNT * WORD_LENGTH) + WORD_AMOUNT];




  static void Main(string[] args)
  {
    Console.CursorVisible = false;
    Console.ForegroundColor = ConsoleColor.Green;
    Console.BackgroundColor = ConsoleColor.Black;
    int selectorPos = 0;

    columnByElements = GenerateColumn(COLUMN_WIDTH, COLUMN_HEIGHT, WORD_LENGTH, WORD_AMOUNT);

    Dictionary<int, int> posToElement = MapPosToElements(columnByElements);
    Console.WriteLine("press Enter to play or any other key to exit");
    ConsoleKey key = Console.ReadKey().Key;
    if (key != ConsoleKey.Enter)
    {
      return;
    }
    while (true)
    {
      Console.Clear();
      int x = 0;
      int y = 0;
      for (int i = 0; i < columnByElements.Length; i++)
      {

        char[] charsToRender = columnByElements[i].ToCharArray();
        foreach (char c in charsToRender)
        {
          if (x + y * COLUMN_WIDTH == selectorPos)
          {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Blue;
          }
          else if (i == posToElement[selectorPos])
          {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
          }
          else
          {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
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

      key = Console.ReadKey().Key;


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
      selectorPos = Math.Clamp(selectorPos, 0, columnByElements.Length - 1);
    }
  }

  static private string[] GenerateColumn(int width = COLUMN_WIDTH, int height = COLUMN_HEIGHT, int wordLength = 4, int wordAmount = 6)
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
        row[i] = symbols[rnd.Next(symbols.Length)].ToString();
      }
    }
    return row;
  }



  static private string[] GenerateRandomWords(int amount = 6, int length = 6)
  {
    string[] res = new string[amount];
    HashSet<int> usedIndex = new HashSet<int>();
    for (int i = 0; i < amount; i++)
    {
      int index = rnd.Next(Words[length - 4].Length);
      if (usedIndex.Contains(index))
      {
        i--;
        continue;
      }
      res[i] = Words[length - 4][index];
    }
    return res;
  }



  static private Dictionary<int, int> MapPosToElements(string[] elements)
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



  static public string[][] Words = new string[5][]
  {
    new string[]
    {
        "book", "desk", "lamp", "moon", "star", "tree", "bird", "fish",
        "frog", "ball", "door", "rock", "bell", "fire", "food", "kite",
        "lake", "leaf", "milk", "nest"
    },
    new string[]
    {
        "apple", "chair", "horse", "mouse", "snake", "table", "house", "train",
        "beach", "clock", "glove", "knife", "cloud", "shoes", "socks", "pearl",
        "flute", "ghost", "leafy", "light"
    },
    new string[]
    {
        "pencil", "turtle", "rabbit", "rocket", "candle", "guitar", "sweater", "flower",
        "pillow", "butter", "laptop", "window", "orange", "banana", "muffins", "router",
        "spider", "apples", "school", "lemons"
    },
    new string[]
    {
        "friends", "helmets", "tunnels", "blocker", "cottage", "diamond", "fortune", "trailer",
        "kitchen", "library", "monster", "package", "rolling", "rainbow", "whistle", "vampire",
        "whisper", "mailbox", "through", "manager"
    },
    new string[]
    {
        "capture", "dinosaur", "football", "generator", "microscope", "photograph", "revolution", "television",
        "university", "volunteer", "watermelon", "xylophone", "celebrate", "fascinate", "happiness", "integrate",
        "juxtapose", "legitimate", "overwhelm", "provocative"
    }
  };
}