public class Symbol : Element
{
  public Word? belongsToWord { get; private set; }
  public Symbol(char value, Coordinates coordinates, Word? masterElement = null) : base(value, coordinates)
  {
    this.belongsToWord = masterElement;
    elementType = ElementType.Symbol;
  }
}