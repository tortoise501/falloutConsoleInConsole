using System;
using System.Linq;
using System.Reflection.PortableExecutable;
//test
class Program
{
  const int COLUMN_WIDTH = 9;
  const int COLUMN_HEIGHT = 16;
  const int WORD_AMOUNT = 8;
  const int WORD_LENGTH = 6;
  static int xCursorPosition = 0;
  static int yCursorPosition = 0;
  static int selectedColumn = 0;

  static int columnAmount = 2;//!for tests
  static void Main(string[] args)
  {




    ConsoleGameRenderer renderer = new ConsoleGameRenderer();
    Console.Clear();
    Column[] columns = new Column[]{
      new Column(COLUMN_WIDTH, COLUMN_HEIGHT, WORD_LENGTH, WORD_AMOUNT),
      new Column(COLUMN_WIDTH, COLUMN_HEIGHT, WORD_LENGTH, WORD_AMOUNT)
    };

    foreach (Column column in columns)
    {
      renderer.AddObjectToRender(column);
    }
    Console.CursorVisible = false;
    // Console.Clear();
    // column.RenderHitPoints();
    // column.Render(false);
    while (true)
    {
      renderer.UpdateColumnRenderData(2, 2, 9);
      renderer.Render();
      HandleInput();
      columns[selectedColumn].SelectElement(xCursorPosition + yCursorPosition * COLUMN_WIDTH);
      // column.HandleInput();
      // Console.Clear();
      // column.RenderHitPoints();
      // column.Render(false);
      // column.RenderGameLogs();
    }
  }
  static void HandleInput()
  {
    ConsoleKey key = Console.ReadKey().Key;
    switch (key)
    {
      case ConsoleKey.LeftArrow:
        {
          xCursorPosition--;
          if (xCursorPosition < 0)
          {
            xCursorPosition = COLUMN_WIDTH - 1;
            selectedColumn = Math.Max(0, selectedColumn - 1);
          }
          break;
        }
      case ConsoleKey.RightArrow:
        {
          xCursorPosition++;
          if (xCursorPosition > COLUMN_WIDTH - 1)
          {
            xCursorPosition = 0;
            selectedColumn = Math.Min(columnAmount - 1, selectedColumn + 1);
          }
          break;
        }
      case ConsoleKey.UpArrow:
        {
          yCursorPosition = Math.Max(0, yCursorPosition - 1);
          break;
        }
      case ConsoleKey.DownArrow:
        {
          yCursorPosition = Math.Min(COLUMN_HEIGHT - 1, yCursorPosition + 1);
          break;
        }
    }
  }


}