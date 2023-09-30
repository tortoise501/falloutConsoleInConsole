using System;
using System.Linq;
using System.Reflection.PortableExecutable;
//test
class Program
{
  static void Main(string[] args)
  {
    ConsoleGameRenderer renderer = new ConsoleGameRenderer();
    Console.Clear();
    Column column = new Column(9, 16, 4, 8);
    renderer.AddObjectToRender(column);
    if (!column.init())
    {
      return;
    }
    // Console.Clear();
    // column.RenderHitPoints();
    // column.Render(false);
    while (true)
    {
      renderer.UpdateRenderData();
      renderer.Render();
      column.HandleInput();
      // Console.Clear();
      // column.RenderHitPoints();
      // column.Render(false);
      // column.RenderGameLogs();
      if (column.gameState != GameState.InProgress)
      {
        break;
      }
    }
  }


}