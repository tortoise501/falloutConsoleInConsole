class RenderData
{
  public char character { get; private set; } = '\u259E';
  public CharacterState state { get; private set; } = CharacterState.notSelected;
  public RenderData(char character = '\u259E', CharacterState state = CharacterState.notSelected)
  {
    this.character = character;
    this.state = state;
  }
}