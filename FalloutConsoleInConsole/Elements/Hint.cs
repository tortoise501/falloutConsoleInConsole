public class Hint : MasterElement
{
  public HintType hintType { get; private set; }
  public Hint(char value, int index, HintType hintType, List<Element>? slaveElements = null) : base(value, index, slaveElements)
  {
    this.hintType = hintType;
    elementType = ElementType.Hint;
  }
}