using System;
using System.Linq;
using System.Reflection.PortableExecutable;
//test
class Program
{
  static void Main(string[] args)
  {
    Console.Clear();
    Column column = new Column(9, 16, 4, 4);
    if (!column.init())
    {
      return;
    }
    column.RenderHitPoints();
    column.Render(false);
    while (true)
    {
      column.HandleInput();
      Console.Clear();
      column.RenderHitPoints();
      column.Render(false);
      if (column.gameState != GameState.InProgress)
      {
        break;
      }
    }
    if (column.gameState == GameState.Lost)
    {
      Console.Write("\n\n\n You lost :(");
    }
    else
    {
      Console.Write("\n\n\n You won :)");
    }
  }


}