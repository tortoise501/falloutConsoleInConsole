using System;
using System.Linq;
using System.Reflection.PortableExecutable;
//test
class Program
{
  static void Main(string[] args)
  {
    Column column = new Column(9, 16, 4, 4);
    if (!column.init())
    {
      return;
    }
    while (true)
    {
      column.Render();
      column.HandleInput();
    }
  }


}