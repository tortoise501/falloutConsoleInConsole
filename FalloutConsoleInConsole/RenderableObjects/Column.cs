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

  Element[] columnByElements = new Element[0];
  readonly int columnWidth;
  readonly int columnHeight;



  public string[] words { get; private set; } = new string[0];//!is public temporarily for dud validation in Game class 
  // Dictionary<int, int> posToElement;
  // Dictionary<int, int> indexToHintPos;
  // Dictionary<int, HintType> indexToHintType = new Dictionary<int, HintType>();
  // Dictionary<int, string> indexToWord = new Dictionary<int, string>();

  //!probably deprecated
  int selectorPos = 0;
  bool isColumnSelected = false;
  public Column(int width, int height, int wordLength, int wordAmount, string[] words, int y = 0, int x = 0, int hintAmount = 4, int resetAttemptsHintAmount = 0)
  {
    ((IRenderable)this).x = x;
    ((IRenderable)this).y = y;

    columnWidth = width;
    columnHeight = height;
    this.words = words;
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


  public void RemoveHint(int index)
  {
    if (columnByElements[index] is Hint)
    {

      columnByElements[index] = new Symbol(columnByElements[index].value, index);
      return;
    }
    throw new Exception("Tried to remove an element that is not a hint with RemoveHint() function");
  }



  public void RemoveDud(int selectedPos, string exclusion)//!public temporarily for Game class dud removal
  {
    Word[] DudWords = columnByElements.OfType<Word>().Where(x => x.GetType() == typeof(Word) && x.word != exclusion).ToArray();
    if (DudWords.Length < 1)
    {
      return;
    }
    int randomIndex = rnd.Next(0, DudWords.Length);
    foreach (Element element in DudWords[randomIndex].slaveElements)
    {
      columnByElements[element.index] = new Symbol('.', element.index);
    }
    columnByElements[DudWords[randomIndex].index] = new Symbol('.', DudWords[randomIndex].index);
  }

  private Element[] GenerateColumn(string[] words, int width = 12, int height = 16, int wordLength = 4, int wordAmount = 6)
  {
    int length = (height * width);
    Element[] column = new Element[length];
    int wordI = 0;
    HashSet<int> randomWordPos = new HashSet<int>();
    for (int i = 0; i < words.Length; i++)
    {
      int pos = rnd.Next((length / wordAmount) * i, (length / wordAmount) * (i + 1) - wordLength + 1);
      if (randomWordPos.Contains(pos))
      {
        i--;
        continue;
      }
      randomWordPos.Add(pos);
    }


    for (int i = 0; i < length;)
    {
      if (randomWordPos.Contains(i))
      {
        column[i] = new Word(words[wordI][0], i, words[wordI]);
        Word masterElement = (Word)column[i];
        i++;
        for (int c = 1; c < words[wordI].Length; c++)
        {
          column[i] = new Symbol(words[wordI][c], i, masterElement);
          masterElement.AddSlaveElement(column[i]);
          i++;
        }
        wordI++;
      }
      else
      {
        column[i] = new Symbol(' ', i);
        i++;
      }
    }
    return column;
  }



  private void GenerateHintData(int rowWidth, int hintAmount, int resetAttemptsHintAmount)
  {
    for (int i = 0; i < hintAmount; i++)//!Possible infinite loops
    {
      List<Element> posPullForRandomHints = columnByElements.Where(x => x.elementType == ElementType.Symbol && (x.index + 1) % rowWidth != 0).ToList();
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
    for (int i = 0; i < columnByElements.Length && spawnedHints < hintAmount; i++)
    {
      Element el = columnByElements[i];
      if (el.value != '\ufffd')
      {
        continue;
      }
      List<int> PossibleClosingPos = new List<int>();
      for (int j = 1; rowWidth > ((i + 1) % rowWidth) + j; j++)
      {
        if (columnByElements[i + j].value != '\ufffd' && columnByElements[i + j].elementType == ElementType.Symbol)
        {
          PossibleClosingPos.Add(i + j);
          if (Constants.Parentheses.Count(x => x == columnByElements[i + j].value) > 0)
          {
            break;
          }
        }
      }
      if (PossibleClosingPos.Count == 0)//If no possible hints, create new hint start later in list
      {
        int NewPos = rnd.Next(i + 1, columnByElements.Length);
        columnByElements[i].value = ' ';
        columnByElements[NewPos].value = '\ufffd';
      }
      else
      {
        int endingPos = PossibleClosingPos[rnd.Next(0, PossibleClosingPos.Count)];
        if (columnByElements[endingPos].value == ' ')
        {
          char randomPar = Constants.Parentheses[rnd.Next(0, Constants.Parentheses.Length)];
          columnByElements[i] = new Hint(Constants.GetOppositeParentheses[randomPar], i, resetAttemptHintsOrder.Contains(hintNumber) ? HintType.Attempt : HintType.Dud);
          for (int x = i + 1; x < endingPos; x++)
          {
            ((MasterElement)columnByElements[i]).AddSlaveElement(columnByElements[x]);
          }
          columnByElements[endingPos] = new Symbol(randomPar, endingPos);//.value = randomPar;
          ((MasterElement)columnByElements[i]).AddSlaveElement(columnByElements[endingPos]);
        }
        else
        {
          columnByElements[i] = new Hint(Constants.GetOppositeParentheses[columnByElements[endingPos].value], i, resetAttemptHintsOrder.Contains(hintNumber) ? HintType.Attempt : HintType.Dud);
        }
        spawnedHints++;
        hintNumber++;
      }
    }
  }
  private void PlaceRandomSymbols()
  {
    for (int i = 0; i < columnByElements.Length; i++)
    {
      if (columnByElements[i].value == ' ' || columnByElements[i].value == '\ufffd')
      {
        columnByElements[i].value = Constants.symbols[rnd.Next(0, Constants.symbols.Length)];
      }
    }
  }




  public Element GetElement(int pos)
  {
    Element element = columnByElements[pos];
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
    int x = 0;
    int y = 0;
    int jump;
    res.Add(new List<RenderData>());
    for (int i = 0; i < columnByElements.Length; i += jump)
    {
      // jump = 1;//TODO:use private function GetCharsOf();

      List<Element> charsToRender = new List<Element>();
      if (columnByElements[i].elementType == ElementType.Symbol)
      {
        charsToRender.Add(columnByElements[i]);
      }
      else
      {
        charsToRender.Add(columnByElements[i]);
        charsToRender = charsToRender.Concat(((MasterElement)columnByElements[i]).slaveElements).ToList();
      }
      jump = charsToRender.Count();
      foreach (Element el in charsToRender)
      {
        if (el.index == selectorPos && isColumnSelected)
        {
          res.Last().Add(new RenderData(el.value, CharacterState.selectedAsChar));
        }
        else if
        (
          (
            charsToRender[0].index == selectorPos
            ||
            (charsToRender.Count(x => x.elementType == ElementType.Word) == charsToRender.Count() && charsToRender.Count(x => x.index == selectorPos) > 0)
          ) && isColumnSelected
        )
        {
          res.Last().Add(new RenderData(el.value, CharacterState.selectedAsElement));
        }
        else
        {
          res.Last().Add(new RenderData(el.value, CharacterState.notSelected));
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
  public void PlaceCursorForFrame(int pos)
  {
    selectorPos = pos;
    isColumnSelected = true;
  }

  int IRenderable.x { get; set; }
  int IRenderable.y { get; set; }
}