public abstract class Element
{
  public ElementType elementType { get; protected set; }
  public char value { get; protected set; }
  public int index { get; protected set; }
  public int startPos { get; protected set; }
  public Element(char value, int index, int startPos)
  {
    this.value = value;
    this.index = index;
    this.startPos = startPos;
  }
}