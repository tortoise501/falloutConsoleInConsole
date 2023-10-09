public class Symbol : Element
{
  public MasterElement masterElement { get; private set; }
  public Symbol(char value, int index, int startPos, MasterElement masterElement) : base(value, index, startPos)
  {
    this.masterElement = masterElement;
    elementType = ElementType.Symbol;
  }
}