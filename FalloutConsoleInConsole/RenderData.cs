class RenderData
{
  public char character = '\u259E';
  public ConsoleColor backgroundColor = ConsoleColor.Magenta;
  public ConsoleColor characterColor = ConsoleColor.Black;
  public RenderData(char character = '\u259E', ConsoleColor backgroundColor = ConsoleColor.Magenta, ConsoleColor characterColor = ConsoleColor.Black)
  {
    this.character = character;
    this.backgroundColor = backgroundColor;
    this.characterColor = characterColor;
  }
}