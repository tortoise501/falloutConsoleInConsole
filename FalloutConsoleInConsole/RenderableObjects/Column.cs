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

  Element[,] columnByElements = new Element[0, 0];
  readonly int columnWidth;
  readonly int columnHeight;



  // public string[] words { get; private set; } = new string[0];//!is public temporarily for dud validation in Game class 

  // int selectorPos = 0;
  Coordinates cursorPos = new Coordinates(0, 0);
  bool isColumnSelected = false;
  public Column(int width, int height, int wordLength, int wordAmount, string[] words, int y = 0, int x = 0, int hintAmount = 4, int resetAttemptsHintAmount = 0)
  {
    ((IRenderable)this).x = x;
    ((IRenderable)this).y = y;

    columnWidth = width;
    columnHeight = height;
    // this.words = words;
    columnByElements = GenerateColumn(words, columnWidth, columnHeight, wordLength, wordAmount);
    // MapPosToElements(columnByElements);
    GenerateHintData(columnWidth, hintAmount, resetAttemptsHintAmount);
    PlaceRandomSymbols();
    //TODO: there is no sense in all those functions to return value, they can just modify global variables 
  }





  /// <summary>
  /// Use for hints
  /// </summary>
  /// <param name="startingIndex">index of an element in columnByElements</param>
  /// <returns></returns>
  public List<char> GetCharsOf(MasterElement el)
  {
    List<char> res = new List<char>();
    res.Add(el.value);
    res = res.Concat(((MasterElement)el).slaveElements.Select(x => x.value)).ToList();
    return res;
  }


  // public ExecutionCode CheckInput()
  // {
  //   // int selectedIndex = posToElement[selectorPos];
  //   // string selectedItem = columnByElements[selectedIndex];
  //   Element selectedElement = columnByElements[selectorPos];
  //   if (selectedElement.elementType == ElementType.Word)//is this right word//selectedElement.GetType() == typeof(Word) && ((Word)selectedElement).word == rightWord
  //   {
  //     string word = selectedElement.GetType() == typeof(Word) ? ((Word)selectedElement).word : ((Symbol)selectedElement).belongsToWord.word;//.((Word)masterElement).word;
  //     if (word == rightWord)//is it a wrong word
  //     {
  //       return ExecutionCode.CorrectWord;
  //     }
  //     else
  //     {
  //       return ExecutionCode.Mistake;
  //     }
  //   }
  //   if (selectedElement.elementType == ElementType.Hint)//is this a hint
  //   {
  //     if (((Hint)selectedElement).hintType == HintType.Dud)
  //     {
  //       return ExecutionCode.HintDuds;
  //     }
  //     else
  //     {
  //       return ExecutionCode.HintLife;
  //     }
  //   }
  //   else//then it is a symbol
  //   {
  //     return ExecutionCode.WrongInput;
  //   }
  // }


  // public string GenerateLog(ExecutionCode executionCode, int selectedPos)
  // {
  //   return "Logs are temporary disabled";
  //   // string el = columnByElements[posToElement[selectedPos]];
  //   // string res = "";
  //   // if (executionCode == ExecutionCode.Mistake)
  //   // {
  //   //   if (rightWord == null)
  //   //     throw new NullReferenceException();
  //   //   res = $">{el}\n>Entry Denied\n>Likeness={CheckForLikeness(el, rightWord)}";
  //   // }
  //   // if (executionCode == ExecutionCode.CorrectWord)
  //   // {
  //   //   res = $">{el}\n>Exact match\n>Please Wait\n>while system\n>is accepted";
  //   // }
  //   // if (executionCode == ExecutionCode.HintDuds)
  //   // {
  //   //   char[] hint = GetCharsOf(posToElement[selectedPos], indexToHintPos[posToElement[selectedPos]]).ToArray();
  //   //   res = $">{string.Join("", hint)}\n>Dud removed";
  //   // }
  //   // if (executionCode == ExecutionCode.HintLife)
  //   // {
  //   //   res = $"ATTEMPTS RESTORED";
  //   // }
  //   // if (executionCode == ExecutionCode.HintLife || executionCode == ExecutionCode.HintDuds)
  //   // {
  //   //   indexToHintPos.Remove(posToElement[selectedPos]);
  //   //   indexToHintType.Remove(posToElement[selectedPos]);
  //   // }
  //   // return res;
  // }


  public void RemoveHint(Coordinates coordinates)
  {
    if (columnByElements[coordinates.x, coordinates.y] is Hint)
    {

      columnByElements[coordinates.x, coordinates.y] = new Symbol(columnByElements[coordinates.x, coordinates.y].value, coordinates);
      return;
    }
    throw new Exception("Tried to remove an element that is not a hint with RemoveHint() function");
  }



  public void RemoveDud(string exclusion)//!public temporarily for Game class dud removal
  {
    Word[] DudWords = columnByElements.OfType<Word>().Where(x => x.GetType() == typeof(Word) && x.word != exclusion).ToArray();
    if (DudWords.Length < 1)
    {
      return;
    }
    int randomIndex = rnd.Next(0, DudWords.Length);
    foreach (Element element in DudWords[randomIndex].slaveElements)
    {
      columnByElements[element.coordinates.x, element.coordinates.y] = new Symbol('.', element.coordinates);
    }
    columnByElements[DudWords[randomIndex].coordinates.x, DudWords[randomIndex].coordinates.y] = new Symbol('.', DudWords[randomIndex].coordinates);
  }

  private Element[,] GenerateColumn(string[] words, int width = 12, int height = 16, int wordLength = 4, int wordAmount = 6)
  {
    // int length = (height * width);
    Element[,] column = new Element[width, height];
    int wordI = 0;
    HashSet<Coordinates> randomWordPos = new HashSet<Coordinates>();
    for (int i = 0; i < words.Length; i++)
    {
      int rndY = rnd.Next(height / wordAmount * i, height / wordAmount * (i + 1));
      int rndX = rnd.Next(rndY == height / wordAmount * (i + 1) - 1 ? width - 1 - wordLength : width);
      Coordinates pos = new Coordinates(rndX, rndY);// rnd.Next((length / wordAmount) * i, (length / wordAmount) * (i + 1) - wordLength + 1);
      if (randomWordPos.Contains(pos))
      {
        i--;
        continue;
      }
      randomWordPos.Add(pos);
    }
    for (int y = 0; y < height; y++)
    {
      for (int x = 0; x < width; x++)
      {

        if (randomWordPos.Contains(new Coordinates(x, y)))
        {
          column[x, y] = new Word(words[wordI][0], new Coordinates(x, y), words[wordI]);
          Word masterElement = (Word)column[x, y];
          for (int c = 1; c < words[wordI].Length; c++)
          {
            x++;
            if (x == width && y < height - 1)
            {
              x = 0;
              y++;
            }
            column[x, y] = new Symbol(words[wordI][c], new Coordinates(x, y), masterElement);
            masterElement.AddSlaveElement(column[x, y]);
          }
          wordI++;
        }
        else
        {
          column[x, y] = new Symbol(' ', new Coordinates(x, y));
        }
      }
    }
    return column;
  }



  private void GenerateHintData(int rowWidth, int hintAmount, int resetAttemptsHintAmount)
  {
    for (int i = 0; i < hintAmount; i++)//!Possible infinite loops
    {
      List<Element> test = columnByElements.Cast<Element>().ToList();
      test.ForEach(x => Console.Write(x));
      List<Element> posPullForRandomHints = test.Where(x => x.elementType == ElementType.Symbol && (x.coordinates.x + 1) != rowWidth - 1).ToList();
      int pos = rnd.Next(0, posPullForRandomHints.Count() / hintAmount + posPullForRandomHints.Count() / hintAmount * i);
      posPullForRandomHints[pos].value = '\ufffd';
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

    int spawnedHints = 0;
    for (int y = 0; y < columnHeight && spawnedHints < hintAmount; y++)
    {
      for (int x = 0; x < columnWidth && spawnedHints < hintAmount; x++)
      {
        Element el = columnByElements[x, y];
        if (el.value != '\ufffd')
        {
          continue;
        }
        List<int> PossibleClosingPos = new List<int>();
        for (int j = 1; rowWidth > x + j; j++)
        {
          if (columnByElements[x + j, y].value != '\ufffd' && columnByElements[x + j, y].elementType == ElementType.Symbol)
          {
            PossibleClosingPos.Add(x + j);
            if (Constants.Parentheses.Count(p => p == columnByElements[x + j, y].value) > 0)
            {
              break;
            }
          }
        }
        if (PossibleClosingPos.Count == 0)//If no possible hints, create new hint start later in list
        {
          if (y == columnHeight - 1)
          {
            continue;
          }
          Coordinates NewPos = new Coordinates(rnd.Next(0, rowWidth), rnd.Next(y + 1, columnHeight));// rnd.Next(x + 1, columnByElements.Length);
          columnByElements[x, y].value = ' ';
          columnByElements[NewPos.x, NewPos.y].value = '\ufffd';
        }
        else
        {
          int endingPos = PossibleClosingPos[rnd.Next(0, PossibleClosingPos.Count)];
          if (columnByElements[endingPos, y].value == ' ')
          {
            char randomPar = Constants.Parentheses[rnd.Next(0, Constants.Parentheses.Length)];
            columnByElements[x, y] = new Hint(Constants.GetOppositeParentheses[randomPar], new Coordinates(x, y), resetAttemptHintsOrder.Contains(hintNumber) ? HintType.Attempt : HintType.Dud);
            for (int c = x + 1; c < endingPos; c++)
            {
              ((MasterElement)columnByElements[x, y]).AddSlaveElement(columnByElements[c, y]);
            }
            columnByElements[endingPos, y] = new Symbol(randomPar, new Coordinates(endingPos, y));//.value = randomPar;
            ((MasterElement)columnByElements[x, y]).AddSlaveElement(columnByElements[endingPos, y]);
          }
          else
          {
            columnByElements[x, y] = new Hint(Constants.GetOppositeParentheses[columnByElements[endingPos, y].value], new Coordinates(x, y), resetAttemptHintsOrder.Contains(hintNumber) ? HintType.Attempt : HintType.Dud);
          }
          spawnedHints++;
          hintNumber++;
        }
      }
    }
  }
  private void PlaceRandomSymbols()
  {
    for (int y = 0; y < columnHeight; y++)
    {
      for (int x = 0; x < columnWidth; x++)
      {
        if (columnByElements[x, y].value == ' ' || columnByElements[x, y].value == '\ufffd')
        {
          columnByElements[x, y].value = Constants.symbols[rnd.Next(0, Constants.symbols.Length)];
        }
      }
    }
  }




  public Element GetElement(Coordinates coordinates)
  {
    Element element = columnByElements[coordinates.x, coordinates.y];
    if (element.elementType == ElementType.Word && element is Symbol)
    {
      return ((Symbol)element).belongsToWord;
    }
    return element;
  }
  // public string GetElementAsString(int pos)
  // {
  //   string res = columnByElements[pos].value.ToString();
  //   if (columnByElements[pos] is MasterElement)
  //   {
  //     foreach (Element el in ((MasterElement)columnByElements[pos]).slaveElements)
  //     {
  //       res += el.value;
  //     }
  //   }
  //   return res;
  // }


  //IRenderable interface implementation
  List<List<RenderData>> IRenderable.GetRenderData()
  {
    List<List<RenderData>> res = new List<List<RenderData>>();
    // int x = 0;
    // int y = 0;
    // int jump;
    int saveBrushCount = 0;
    for (int y = 0; y < columnHeight; y++)
    {
      res.Add(new List<RenderData>());
      for (int x = 0; x < columnWidth; x++)
      {
        // Coordinates coords = new Coordinates(x, y);
        Element element = columnByElements[x, y];
        if (element is Word && ((element as Word).slaveElements.Count(el => el.coordinates == cursorPos) > 0 || element.coordinates == cursorPos) && isColumnSelected)
        {
          saveBrushCount = (element as Word).word.Length;
        }
        if (element is Hint && element.coordinates == cursorPos && isColumnSelected)
        {
          saveBrushCount = (element as Hint).slaveElements.Count();
        }
        if (element.coordinates == cursorPos && isColumnSelected)
        {
          res.Last().Add(new RenderData(element.value, CharacterState.selectedAsChar));
          if (saveBrushCount != 0)
          {
            saveBrushCount--;
          }
        }
        else if (saveBrushCount > 0 && isColumnSelected)
        {
          res.Last().Add(new RenderData(element.value, CharacterState.selectedAsElement));
          saveBrushCount--;
        }
        else
        {
          res.Last().Add(new RenderData(element.value, CharacterState.notSelected));
        }
      }
    }
    isColumnSelected = false;//expire "selection"
    return res;
  }
  public void PlaceCursorForFrame(Coordinates pos)
  {
    cursorPos = pos;
    isColumnSelected = true;
  }

  int IRenderable.x { get; set; }
  int IRenderable.y { get; set; }
}