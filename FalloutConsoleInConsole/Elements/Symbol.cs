public class Symbol : Element
{
  public Word? belongsToWord { get; private set; }
  public Symbol(char value, int index, Word? masterElement = null) : base(value, index)
  {
    this.belongsToWord = masterElement;
    elementType = ElementType.Symbol;
  }
}