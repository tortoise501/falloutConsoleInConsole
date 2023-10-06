
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
    string textToRender = $"{attemptsLeft} Attempt(s) Left: ";
    for (int i = 0; i < attemptsLeft; i++)
    {
      textToRender += "\u25A0 ";
    }
    return new List<List<RenderData>>() { textToRender.Select(x => new RenderData(x, CharacterState.notSelected)).ToList() };
    // List<List<RenderData>> res = new List<List<RenderData>>();
    // res.Add(new List<RenderData>());
    // res[0].Add(new RenderData('[', CharacterState.notSelected));
    // for (int i = 0; i < maxAttempts; i++)
    // {
    //   if (i < attemptsLeft)
    //   {
    //     res[0].Add(new RenderData('\u25A0', CharacterState.notSelected));
    //   }
    //   else
    //   {
    //     res[0].Add(new RenderData(' ', CharacterState.notSelected));
    //   }
    // }
    // res[0].Add(new RenderData(']', CharacterState.notSelected));
    // return res;
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
  public void ResetAttempts()
  {
    attemptsLeft = maxAttempts;
  }
  int x;
  int y;
}