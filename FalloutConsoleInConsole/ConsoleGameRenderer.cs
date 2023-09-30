class ConsoleGameRenderer
{
  const int ConsoleWidth = 60;
  const int ConsoleHeight = 30;
  private RenderData[,] renderData = new RenderData[ConsoleHeight, ConsoleWidth];
  private List<IRenderable> objectsToRender = new List<IRenderable>();
  public void UpdateRenderData()
  {
    foreach (IRenderable objectToRender in objectsToRender)
    {
      InsertRenderData(objectToRender);
    }
  }
  public void InsertRenderData(IRenderable objectRenderI)
  {
    int xOffest = 1;//!FOR TESTING
    int yOffset = 1;//!FOR TESTING
    List<List<RenderData>> objectRenderData = objectRenderI.GetRenderData();
    for (int y = 0; y < objectRenderData.Count(); y++)
    {
      for (int x = 0; x < objectRenderData[y].Count(); x++)
      {
        if (y < renderData.GetLength(0) && x < renderData.GetLength(1))
        {
          renderData[y, x] = objectRenderData[y][x];
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
          break;
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