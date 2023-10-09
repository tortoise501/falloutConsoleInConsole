public class Hint : MasterElement
{
  public HintType hintType { get; private set; }
  public Hint(char value, int index, int startPos, HintType hintType, List<Element> slaveElements) : base(value, index, startPos, slaveElements)
  {
    this.hintType = hintType;
    elementType = ElementType.Hint;
  }
}