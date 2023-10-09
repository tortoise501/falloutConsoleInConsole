public class MasterElement : Element
{
  public List<Element> slaveElements { get; protected set; }
  public int length { get; protected set; }
  public MasterElement(char value, int index, int startPos, List<Element> slaveElements) : base(value, index, startPos)
  {
    this.slaveElements = slaveElements;
    length = slaveElements.Count();
  }
}