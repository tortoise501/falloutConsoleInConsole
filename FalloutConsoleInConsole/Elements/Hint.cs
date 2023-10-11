public class Hint : MasterElement
{
  public HintType hintType { get; private set; }
  public Hint(char value, Coordinates coordinates, HintType hintType, List<Element>? slaveElements = null) : base(value, coordinates, slaveElements)
  {
    this.hintType = hintType;
    elementType = ElementType.Hint;
  }
}