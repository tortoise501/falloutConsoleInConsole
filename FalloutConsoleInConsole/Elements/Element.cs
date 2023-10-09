public abstract class Element
{
  public ElementType elementType { get; set; }
  public char value { get; set; }
  public int index { get; protected set; }
  public Element(char value, int index)
  {
    this.value = value;
    this.index = index;
  }
}