
using System.Security.Cryptography.X509Certificates;

public class Attempts : IRenderable
{
  int maxAttempts;
  public int attemptsLeft { get; private set; }
  public Attempts(int maxAttempts, int x = 0, int y = 0)
  {
    this.maxAttempts = maxAttempts;
    attemptsLeft = maxAttempts;
    this.x = x;
    this.y = y;
  }

  public int GetPositionX()
  {
    return x;
    // throw new NotImplementedException();
  }

  public int GetPositionY()
  {
    return y;
    // throw new NotImplementedException();
  }

  List<List<RenderData>> IRenderable.GetRenderData()
  {
    List<List<RenderData>> res = new List<List<RenderData>>();
    res.Add(new List<RenderData>());
    res[0].Add(new RenderData('[', ConsoleColor.Black, ConsoleColor.Green));
    for (int i = 0; i < maxAttempts; i++)
    {
      if (i < attemptsLeft)
      {
        res[0].Add(new RenderData('\u25A0', ConsoleColor.Black, ConsoleColor.Green));
      }
      else
      {
        res[0].Add(new RenderData(' ', ConsoleColor.Black, ConsoleColor.Green));
      }
    }
    res[0].Add(new RenderData(']', ConsoleColor.Black, ConsoleColor.Green));
    return res;
  }
  public bool LooseAttemptAndCheckForLoose()
  {
    attemptsLeft--;
    if (attemptsLeft <= 0)
    {
      return true;
    }
    return false;
  }
  int x;
  int y;
}