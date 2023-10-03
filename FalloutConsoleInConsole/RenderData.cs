class RenderData
{
  public char character { get; private set; } = '\u259E';
  // public ConsoleColor backgroundColor = ConsoleColor.Magenta;
  // public ConsoleColor characterColor = ConsoleColor.Black;
  public CharacterState state { get; private set; } = CharacterState.notSelected;
  public RenderData(char character = '\u259E', CharacterState state = CharacterState.notSelected)
  {
    this.character = character;
    this.state = state;
  }
}