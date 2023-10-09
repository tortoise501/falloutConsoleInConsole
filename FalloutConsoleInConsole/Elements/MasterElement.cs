public class MasterElement : Element
{
  public List<Element> slaveElements { get; protected set; }
  public int length { get; protected set; }
  public MasterElement(char value, int index, List<Element>? slaveElements = null) : base(value, index)
  {
    if (slaveElements == null)
    {
      slaveElements = new List<Element>();
    }
    this.slaveElements = slaveElements;
    length = slaveElements.Count();
  }
  public void AddSlaveElement(Element element)
  {
    slaveElements.Add(element);
    element.elementType = elementType == ElementType.Word ? ElementType.Word : element.elementType;
    length++;
  }
}