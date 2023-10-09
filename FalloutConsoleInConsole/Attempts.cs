
using System.Security.Cryptography.X509Certificates;

public class Attempts : IRenderable
{
  int maxAttempts;
  public int attemptsLeft { get; private set; }
  public Attempts(int maxAttempts, int x = 0, int y = 0)
  {
    this.maxAttempts = maxAttempts;
    attemptsLeft = maxAttempts;
    ((IRenderable)this).x = x;
    ((IRenderable)this).y = y;
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


  //IRender Interface implementation 

  int IRenderable.x { get; set; }
  int IRenderable.y { get; set; }

  List<List<RenderData>> IRenderable.GetRenderData()
  {
    string textToRender = $"{attemptsLeft} Attempt(s) Left: ";
    for (int i = 0; i < attemptsLeft; i++)
    {
      textToRender += "\u25A0 ";
    }
    return new List<List<RenderData>>() { textToRender.Select(x => new RenderData(x, CharacterState.notSelected)).ToList() };
  }
}