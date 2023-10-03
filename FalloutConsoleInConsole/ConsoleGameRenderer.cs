using System.Text;

class ConsoleGameRenderer
{
  const int ConsoleWidth = 60;
  const int ConsoleHeight = 20;
  private RenderData[,] renderData = new RenderData[ConsoleHeight, ConsoleWidth];
  private List<IRenderable> objectsToRender = new List<IRenderable>();
  public void UpdateColumnRenderData()
  {
    for (int i = 0; i < objectsToRender.Count(); i++)
    {
      InsertRenderData(objectsToRender[i], objectsToRender[i].GetPositionX(), objectsToRender[i].GetPositionY());//!19 is temporal for length of column
    }
  }
  public void InsertRenderData(IRenderable objectRenderI, int xOffset = 0, int yOffset = 0)
  {
    List<List<RenderData>> objectRenderData = objectRenderI.GetRenderData();
    for (int y = 0; y < objectRenderData.Count(); y++)
    {
      for (int x = 0; x < objectRenderData[y].Count(); x++)
      {
        if (y + yOffset < renderData.GetLength(0) && x + xOffset < renderData.GetLength(1))
        {
          renderData[y + yOffset, x + xOffset] = objectRenderData[y][x];
        }
      }
    }
  }

  /// <summary>
  /// Deprecated
  /// </summary>
  public void Render()
  {
    Console.Clear();
    for (int y = 0; y < renderData.GetLength(0); y++)
    {
      for (int x = 0; x < renderData.GetLength(1); x++)
      {
        if (renderData[y, x] == null)
        {
          Console.BackgroundColor = ConsoleColor.Black;
          Console.ForegroundColor = ConsoleColor.Black;
          Console.Write(" ");
          continue;
        }
        RenderData character = renderData[y, x];
        Console.BackgroundColor = Constants.CharStateToBackgroundColor[character.state];
        Console.ForegroundColor = Constants.CharStateToBackgroundColor[character.state];
        Console.Write(character.character);
      }
      Console.WriteLine();
    }
  }



  public void FastRender()
  {
    Console.Clear();
    for (int y = 0; y < renderData.GetLength(0); y++)
    {
      StringBuilder sb = new StringBuilder();
      for (int x = 0; x < renderData.GetLength(1); x++)
      {
        if (renderData[y, x] == null)
        {
          sb.Append(" ");
          continue;
        }
        RenderData character = renderData[y, x];
        if (character.state != CharacterState.notSelected)
        {

          if (sb.Capacity != 0)
          {
            Console.BackgroundColor = Constants.CharStateToBackgroundColor[CharacterState.notSelected];
            Console.ForegroundColor = Constants.CharStateToCharColor[CharacterState.notSelected];
            Console.Write(sb);
            sb.Clear();
          }
          Console.BackgroundColor = Constants.CharStateToBackgroundColor[character.state];
          Console.ForegroundColor = Constants.CharStateToBackgroundColor[character.state];
          Console.Write(character.character);
        }
        else
        {
          sb.Append(character.character);
        }

      }
      if (sb.Length != 0)
      {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(sb);
      }
      Console.WriteLine();
    }
    for (int y = 0; y < renderData.GetLength(0); y++)
    {
      for (int x = 0; x < renderData.GetLength(1); x++)
      {
        renderData[y, x] = null;
      }
    }
  }
  public void AddObjectToRender(IRenderable objectToRender)
  {
    objectsToRender.Add(objectToRender);
  }
}