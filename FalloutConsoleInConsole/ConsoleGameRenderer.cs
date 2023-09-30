class ConsoleGameRenderer
{
  const int ConsoleWidth = 30;
  const int ConsoleHeight = 20;
  private RenderData[,] renderData = new RenderData[ConsoleHeight, ConsoleWidth];
  private List<IRenderable> objectsToRender = new List<IRenderable>();
  public void UpdateColumnRenderData(int upPadding, int paddingBetweenColumns, int ColumnWidth)
  {
    for (int i = 0; i < objectsToRender.Count(); i++)
    {
      InsertRenderData(objectsToRender[i], i * (ColumnWidth + paddingBetweenColumns), upPadding);//!19 is temporal for length of column
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
        Console.BackgroundColor = character.backgroundColor;
        Console.ForegroundColor = character.characterColor;
        Console.Write(character.character);
      }
      Console.WriteLine();
    }
  }
  public void AddObjectToRender(IRenderable objectToRender)
  {
    objectsToRender.Add(objectToRender);
  }
}