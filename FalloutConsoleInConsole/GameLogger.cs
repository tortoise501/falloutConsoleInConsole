public class GameLogger : IRenderable
{
  Queue<string> Logs = new Queue<string>();
  int maxLength;

  public GameLogger(int maxLength, int x = 0, int y = 0)
  {
    this.maxLength = maxLength;
    this.x = x;
    this.y = y;
  }

  public void AddGameLogs(string str)
  {
    string[] logs = str.Split('\n');//.Reverse().ToArray();//!Change later in log generation
    foreach (string log in logs)
    {
      Logs.Enqueue(log);
    }
    while (Logs.Count() > maxLength)
    {
      Logs.Dequeue();
    }
  }

  public int GetPositionX()
  {
    return x;
  }

  public int GetPositionY()
  {
    return y;
  }

  List<List<RenderData>> IRenderable.GetRenderData()
  {
    List<List<RenderData>> res = new List<List<RenderData>>();
    foreach (string log in Logs)
    {
      res.Add(new List<RenderData>());
      foreach (char c in log)
      {
        res.Last().Add(new RenderData(c, ConsoleColor.Black, ConsoleColor.Green));
      }
    }
    return res;
  }
  int x = 0;
  int y = 0;
}