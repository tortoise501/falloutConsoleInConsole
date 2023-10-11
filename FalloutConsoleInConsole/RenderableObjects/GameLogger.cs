public class GameLogger : IRenderable
{
  Queue<string> Logs = new Queue<string>();
  int maxLength;
  string selectedElement = "";

  public GameLogger(int maxLength, int x = 0, int y = 0)
  {
    this.maxLength = maxLength;
    ((IRenderable)this).x = x;
    ((IRenderable)this).y = y;
  }
  public void SetSelectedElement(string element)
  {
    selectedElement = ">" + element;
  }

  public void AddGameLogs(string str)
  {
    string[] logs = str.Split('\n');
    foreach (string log in logs)
    {
      Logs.Enqueue(log);
    }
    while (Logs.Count() > maxLength - 2)
    {
      Logs.Dequeue();
    }
  }
  //IRender interface implementation

  List<List<RenderData>> IRenderable.GetRenderData()
  {
    List<List<RenderData>> res = new List<List<RenderData>>();
    int i = 0;
    foreach (string log in Logs)
    {
      i++;
      res.Add(new List<RenderData>());
      foreach (char c in log)
      {
        res.Last().Add(new RenderData(c, CharacterState.notSelected));
      }
    }
    while (i < maxLength - 1)
    {
      i++;
      res.Add(new List<RenderData>());
    }
    res.Add(selectedElement.Select(x => new RenderData(x)).ToList());
    return res;
  }
  int IRenderable.x { get; set; }
  int IRenderable.y { get; set; }
}